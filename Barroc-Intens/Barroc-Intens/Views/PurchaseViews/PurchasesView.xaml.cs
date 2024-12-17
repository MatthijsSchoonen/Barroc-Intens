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
using System.Diagnostics;
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
            }

            PurchaseOrder order = new()
            {
                TotalProductAmount = prodAmount,
                TotalPrice = (decimal)(prodAmount * SelectedProduct.Price),
                OrderedAt = DateTime.Now,
                Products = new ObservableCollection<Product>(),
            };

            using (AppDbContext appDbContext = new AppDbContext())
            {
                Product existingProduct = appDbContext.Products.FirstOrDefault(p => p.Id == SelectedProduct.Id);

                if (existingProduct != null)
                {
                    order.Products.Add(existingProduct);
                }
                // Deterine Order status
                if (order.TotalProductAmount > 5000)
                {
                    order.OrderStatus = appDbContext.PurchaseOrderStatuses.Where(p => p.Id == 1).FirstOrDefault();
                }
                else
                {
                    order.OrderStatus = appDbContext.PurchaseOrderStatuses.Where(p => p.Id == 3).FirstOrDefault();
                }

                // Store order locally and in DB
                PurchaseOrders.Add(order);
                appDbContext.PurchaseOrders.Add(order);
                appDbContext.SaveChanges();
            }
            // Reset form fields
            CreatePurchase.Visibility = Visibility.Collapsed;
            ProductTitle.Text = "";
            ProductDescription.Text = "";
            FProdAmount.Text = "";
        }

        private void PurchaseOverviewLv_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the clicked item from the event arguments
             PurchaseOrder purchaceOrder = e.ClickedItem as PurchaseOrder;
            Debug.WriteLine(purchaceOrder.ToString());
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
                // Eagerly load selected PurchaseOrder
                PurchaseOrder order = dbContext.PurchaseOrders
                                     .Include(po => po.Products)
                                     .FirstOrDefault(po => po.Id == id);

                if (order != null)
                {
                    // Update price and amount
                    order.TotalProductAmount = int.Parse(FUpdateProdAmount.Text);
                    order.TotalPrice = (decimal)order.Products.Sum(p => p.Price * order.TotalProductAmount);

                    // Update status if selected, set to status ID 1 otherwise.
                    PurchaseOrderStatus selectedStatus = FStatusChange.SelectedItem as PurchaseOrderStatus;
                    if (selectedStatus != null)
                    {
                        order.OrderStatus = selectedStatus;
                    }
                    else
                    {
                        order.OrderStatus = dbContext.PurchaseOrderStatuses.Where(o => o.Id == 1).FirstOrDefault() as PurchaseOrderStatus;
                    }

                    // DB update
                    dbContext.PurchaseOrders.Update(order);
                    dbContext.SaveChanges();

                    // UI update
                    PurchaseOrder existingOrder = PurchaseOrders.FirstOrDefault(po => po.Id == id);
                    if (existingOrder != null)
                    {
                        // Find order in list and replace it with updated variant
                        int index = PurchaseOrders.IndexOf(existingOrder);
                        PurchaseOrders[index] = order; // triggers UI update
                    }
                }
            }
            UpdateInfoPanel.Visibility = Visibility.Collapsed;
        }

        private void WriteToStock_Click(object sender, RoutedEventArgs e)
        {
            // Id & PurchaseOrder retrieval
            int id = (int)WriteToStock.Tag; 
            using (AppDbContext appDbContext = new())
            {
                // Eagerly load PurchaseOrder (also include product)
                PurchaseOrder order = appDbContext.PurchaseOrders
                                                  .Include(po => po.Products)
                                                  .FirstOrDefault(po => po.Id == id);

                if (order != null)
                {
                    foreach (Product orderProduct in order.Products)
                    {
                        // Checks if product exists in Products table
                        Product existingProduct = appDbContext.Products
                                                             .FirstOrDefault(p => p.Name == orderProduct.Name);

                        if (existingProduct != null)
                        {
                            // Add Order Value to Product Stock
                            existingProduct.Stock += order.TotalProductAmount;
                        }
                        else
                        {
                            // Add new product
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

                    // Remove processed order & save db changes
                    appDbContext.PurchaseOrders.Remove(order);
                    appDbContext.SaveChanges();
                }
            }

            // Confirmation
            UpdateInfoPanel.Visibility = Visibility.Collapsed;
            ContentDialog confirmationDialog = new ContentDialog
            {
                Title = "Success!",
                Content = "The order is successfully written to the Products and removed from the Purchase Orders.",
                CloseButtonText = "Open Stock Overview",
                XamlRoot = this.Content.XamlRoot
            };
            confirmationDialog.ShowAsync();
            Frame.Navigate(typeof(StockView));
        }
    }
}
