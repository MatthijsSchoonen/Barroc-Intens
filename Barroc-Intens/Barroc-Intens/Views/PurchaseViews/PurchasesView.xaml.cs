using Barroc_Intens.Data;
using Barroc_Intens.PurchaseViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Views.PurchaseViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PurchasesView : Page
    {
        // Class properties (Inputs for forms, variables for calculations)
        ObservableCollection<Product> Products = new ObservableCollection<Product>();
        ObservableCollection<PurchaseOrderStatus> PurchaseOrderStatuses = new ObservableCollection<PurchaseOrderStatus>();
        ObservableCollection<PurchaseOrder> PurchaseOrders = new ObservableCollection<PurchaseOrder>();
       // PurchaseOrder PurchaseOrder = new();
        Product SelectedProduct = new();
        int ProductCount = 0;
        decimal Costs = 0;
        public PurchasesView()
        {
            this.InitializeComponent();
            
            // Retrieve Products
            List<Product> productList = new();
            using (AppDbContext appDbContext = new())
            {
                productList = appDbContext.Products.ToList();
                List<PurchaseOrderStatus> retrieved = appDbContext.PurchaseOrderStatuses.ToList();

                foreach (PurchaseOrderStatus status in retrieved)
                {
                    PurchaseOrderStatuses.Add(status);
                }
            }
            // Store products in ObservableCollection (so Updates once tyhis one does)
            foreach (Product product in productList)
            {
                Products.Add(product);
            }
            // Bind to UI
            FProductSelector.ItemsSource = Products;
            //PurchaseOverviewLv.ItemsSource = PurchaseOrder.Products;
            // Load last otrders
            LoadPurchaseOrderHistory();
            PurchaseOrderHistoryGridView.ItemsSource = PurchaseOrders;
            FStatusChange.ItemsSource = PurchaseOrderStatuses;
        }
        private void LoadPurchaseOrderHistory()
        {
            using (var dbContext = new AppDbContext())
            {
                List<PurchaseOrder> purchaseOrders = dbContext.PurchaseOrders
                    .Include(po => po.OrderStatus)
                    .OrderByDescending(po => po.OrderedAt)
                    .ToList();

                foreach(PurchaseOrder order in purchaseOrders)
                {
                    PurchaseOrders.Add(order);
                }
            }
        }
        // Shows up form
        private void MakePurchaseToggle_Click(object sender, RoutedEventArgs e)
        {
            if (CreatePurchase.Visibility == Visibility.Visible)
            {
                CreatePurchase.Visibility = Visibility.Collapsed;
                return;
            }
            CreatePurchase.Visibility = Visibility.Visible;
        }
        
        private void OnProductSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product selectedProduct = FProductSelector.SelectedItem as Product;

            if (selectedProduct != null) { 
                ProductTitle.Text = selectedProduct.Name.ToString();
                ProductDescription.Text = selectedProduct.Description.ToString();
                //PurchaseOrder.Products.Add(selectedProduct);
                SelectedProduct = selectedProduct;
                InfoPanel.Visibility = Visibility.Visible;
            }
        }

        private void OnAddProductClick(object sender, RoutedEventArgs e)
        {
            string fProdAmount = FProdAmount.Text;
            if (int.TryParse(fProdAmount, out int prodAmount))
            {
                //SelectedProduct.Stock = prodAmount;
            }

            PurchaseOrder order = new()
            {
                TotalProductAmount = prodAmount,
                TotalPrice = (decimal)(prodAmount * SelectedProduct.Price),
                OrderedAt = DateTime.Now,
                Products = new ObservableCollection<Product>(), // Initialiseer de lijst om producten toe te voegen
            };

            using (AppDbContext appDbContext = new AppDbContext())
            {
                // Zorg ervoor dat het product wordt opgehaald vanuit de database
                var existingProduct = appDbContext.Products.FirstOrDefault(p => p.Id == SelectedProduct.Id);

                if (existingProduct != null)
                {
                    // Voeg het bestaande product toe aan de bestelling
                    order.Products.Add(existingProduct);
                }
       

                // Bepaal de OrderStatus
                if (order.TotalProductAmount > 5000)
                {
                    order.OrderStatus = appDbContext.PurchaseOrderStatuses.FirstOrDefault(p => p.Id == 1);
                }
                else
                {
                    order.OrderStatus = appDbContext.PurchaseOrderStatuses.FirstOrDefault(p => p.Id == 3);
                }

                // Sla de bestelling op in de database
                PurchaseOrders.Add(order);
                appDbContext.PurchaseOrders.Add(order);
                appDbContext.SaveChanges();
            }

            // Reset form en herbereken totaal
            CreatePurchase.Visibility = Visibility.Collapsed;
            ProductTitle.Text = "";
            ProductDescription.Text = "";
            FProdAmount.Text = "";
            CalculateTotal();
            //LoadPurchaseOrderHistory();
        }





        private void PurchaseOverviewLv_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the clicked item from the event arguments
              PurchaseOrder purchaceOrder = e.ClickedItem as PurchaseOrder;
            if (purchaceOrder != null)
            {
                UpdateInfoPanel.Visibility = Visibility.Visible;

                // Update the details in the info panel
                UpdateProductTitle.Text = SelectedProduct.Name;
                UpdateProductDescription.Text = SelectedProduct.Description;
                FUpdateProdAmount.Text = purchaceOrder.TotalProductAmount.ToString();
                WriteToStock.Tag = purchaceOrder.Id;
                if(LocalStore.GetLoggedInUser().Department.Id == 7)
                {
                    TicketStatusUpdatePanel.Visibility = Visibility.Visible;
                }
                if(purchaceOrder.OrderStatus.Id == 3)
                {
                    WriteToStock.Visibility = Visibility.Visible;
                }
                UpdateProduct.Tag = purchaceOrder.Id;
            }
        }

        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)UpdateProduct.Tag;

            using (AppDbContext dbContext = new())
            {
                // Haal de bestaande PurchaseOrder op, inclusief de gekoppelde producten
                var order = dbContext.PurchaseOrders
                                     .Include(po => po.Products)
                                     .FirstOrDefault(po => po.Id == id);

                if (order != null)
                {
                    // Update de totale hoeveelheid en prijs
                    order.TotalProductAmount = int.Parse(FUpdateProdAmount.Text);
                    order.TotalPrice = (decimal)order.Products.Sum(p => p.Price * order.TotalProductAmount);

                    // Update de status van de bestelling
                    var selectedStatus = FStatusChange.SelectedItem as PurchaseOrderStatus;
                    if (selectedStatus != null)
                    {
                        order.OrderStatus = selectedStatus;
                    }
                    else
                    {
                        order.OrderStatus = dbContext.PurchaseOrderStatuses.Where(o => o.Id == 1).FirstOrDefault() as PurchaseOrderStatus;
                    }

                    // Zorg dat de context alle wijzigingen bijhoudt
                    dbContext.PurchaseOrders.Update(order);
                    dbContext.SaveChanges();

                    // Werk de UI bij: update in plaats van een nieuw item toe te voegen
                    var existingOrder = PurchaseOrders.FirstOrDefault(po => po.Id == id);
                    if (existingOrder != null)
                    {
                        // Update de bestaande order in de lijst
                        var index = PurchaseOrders.IndexOf(existingOrder);
                        PurchaseOrders[index] = order; // Dit triggert de UI-update
                    }
                }
            }

            // Sluit het info-paneel
            UpdateInfoPanel.Visibility = Visibility.Collapsed;
        }

        private void WriteToStock_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)WriteToStock.Tag; // Zorg dat dit de ID bevat van de juiste PurchaseOrder
            using (AppDbContext appDbContext = new())
            {
                // Haal de PurchaseOrder op inclusief gekoppelde producten
                PurchaseOrder order = appDbContext.PurchaseOrders
                                                  .Include(po => po.Products) // Include producten
                                                  .FirstOrDefault(po => po.Id == id);

                if (order != null)
                {
                    foreach (Product orderProduct in order.Products)
                    {
                        // Controleer of het product al bestaat in de Products-tabel
                        Product existingProduct = appDbContext.Products
                                                             .FirstOrDefault(p => p.Name == orderProduct.Name);

                        if (existingProduct != null)
                        {
                            // Update de voorraad van het bestaande product
                            existingProduct.Stock += order.TotalProductAmount;
                        }
                        else
                        {
                            // Voeg een nieuw product toe aan de database
                            var newProduct = new Product
                            {
                                Name = orderProduct.Name,
                                Description = orderProduct.Description,
                                Price = orderProduct.Price,
                                Stock = orderProduct.Stock
                            };
                            appDbContext.Products.Add(newProduct);
                        }
                    }

                    // Sla wijzigingen in de productvoorraad op
                    appDbContext.SaveChanges();

                    // Verwijder de afgehandelde PurchaseOrder
                    appDbContext.PurchaseOrders.Remove(order);
                    appDbContext.SaveChanges();
                }
            }

            // Sluit het info paneel en toon een bevestigingsdialoog
            UpdateInfoPanel.Visibility = Visibility.Collapsed;

            ContentDialog confirmationDialog = new ContentDialog
            {
                Title = "Success!",
                Content = "The order is successfully written to the Products and removed from the Purchase Orders.",
                CloseButtonText = "Open Stock Overview",
                XamlRoot = this.Content.XamlRoot
            };
            confirmationDialog.ShowAsync();
            //LoadPurchaseOrderHistory();
            // Navigeer naar de StockView-pagina
            Frame.Navigate(typeof(StockView));
        }


    }
}
