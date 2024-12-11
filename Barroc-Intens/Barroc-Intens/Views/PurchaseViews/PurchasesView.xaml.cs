using Barroc_Intens.Data;
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

        PurchaseOrder PurchaseOrder = new();
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
            PurchaseOrder.OrderedAt = DateTime.Now;
            // Load last otrders
            LoadPurchaseOrderHistory();
        }
        private void LoadPurchaseOrderHistory()
        {
            using (var dbContext = new AppDbContext())
            {
                var purchaseOrders = dbContext.PurchaseOrders
                    .Include(po => po.OrderStatus)
                    .OrderByDescending(po => po.OrderedAt)
                    .Take(10) // Limit to last 10 orders
                    .ToList();

                PurchaseOrderHistoryListView.ItemsSource = purchaseOrders;
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
                SelectedProduct.Stock = prodAmount;
            }

            // Create a new Product instance for the purchase order
            var newProduct = new Product
            {
                Name = SelectedProduct.Name,
                Description = SelectedProduct.Description,
                Price = SelectedProduct.Price,
                Stock = SelectedProduct.Stock,
            };

            PurchaseOrder.Products.Add(newProduct);
            Products.Remove(SelectedProduct);
            CreatePurchase.Visibility = Visibility.Collapsed;
            ProductTitle.Text = "";
            ProductDescription.Text = "";
            FProdAmount.Text = "";
            CalculateTotal();

            using (AppDbContext dbContext = new AppDbContext())
            {
                // Create a new PurchaseOrder instance for database insertion
                PurchaseOrder purchaseOrderToSave = new PurchaseOrder
                {
                    OrderedAt = DateTime.Now,
                    TotalProductAmount = PurchaseOrder.TotalProductAmount,
                    TotalPrice = PurchaseOrder.TotalPrice,
                    Products = PurchaseOrder.Products // Create a new list of products
                };

                PurchaseOrderStatus status;
                if (purchaseOrderToSave.TotalProductAmount > 5000)
                {
                    status = dbContext.PurchaseOrderStatuses.FirstOrDefault(p => p.Id == 1);
                }
                else
                {
                    status = dbContext.PurchaseOrderStatuses.FirstOrDefault(p => p.Id == 3);
                }

                purchaseOrderToSave.OrderStatus = status;

                dbContext.PurchaseOrders.Add(purchaseOrderToSave);
                dbContext.SaveChanges();

                // Reset the current PurchaseOrder
                PurchaseOrder = new PurchaseOrder();
                //PurchaseOverviewLv.ItemsSource = PurchaseOrder.Products;
                LoadPurchaseOrderHistory();
            }
            CreatePurchase.Visibility = Visibility.Collapsed;
        }


        private void PurchaseOverviewLv_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the clicked item from the event arguments
              PurchaseOrder purchaceOrder = e.ClickedItem as PurchaseOrder;
            if (purchaceOrder != null)
            {
                PurchaseOrder = purchaceOrder;
                UpdateInfoPanel.Visibility = Visibility.Visible;

                // Update the details in the info panel
                UpdateProductTitle.Text = SelectedProduct.Name;
                UpdateProductDescription.Text = SelectedProduct.Description;

                if(LocalStore.GetLoggedInUser().Department.Id == 7)
                {
                    TicketStatusUpdatePanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            PurchaseOrder purchaseOrder = sender as PurchaseOrder;
            if (purchaseOrder != null) {
                int amount = (int)FUpdateProdAmount.Value;
                using (AppDbContext dbContext = new())
                {
                    dbContext.PurchaseOrders.Update(purchaseOrder);
                    dbContext.SaveChanges();
                }
            }

            LoadPurchaseOrderHistory();
            UpdateInfoPanel.Visibility = Visibility.Collapsed;
        }

        private void CalculateTotal()
        {
            foreach (Product product in PurchaseOrder.Products)
            {
                ProductCount += product.Stock;
                Costs += product.Price * product.Stock ?? 0 * product.Stock;
            }
            TotalProductCount.Text = ProductCount.ToString();
            CostsTb.Text = Math.Round(Costs, 2).ToString();
            PurchaseOrder.TotalProductAmount = ProductCount;
            PurchaseOrder.TotalPrice = Costs;
        }
    }
}
