using Barroc_Intens.Data;
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
        ObservableCollection<Product> Products = new ObservableCollection<Product>();
        PurchaseOrder PurchaseOrder = new();
        Product SelectedProduct = new();
        public PurchasesView()
        {
            this.InitializeComponent();
            List<Product> productList = new();
            using (AppDbContext appDbContext = new())
            {
                productList = appDbContext.Products.ToList();
            }

            foreach (Product product in productList)
            {
                Products.Add(product);
            }

            FProductSelector.ItemsSource = Products;
            PurchaseOverviewLv.ItemsSource = PurchaseOrder.Products;
        }

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

        private void OnAddProductClick(object sender, RoutedEventArgs e) { 
            string fProdAmount = FProdAmount.Text;
            if (int.TryParse(fProdAmount, out int prodAmount))
            {
                SelectedProduct.Stock = prodAmount;
            }
            PurchaseOrder.Products.Add(SelectedProduct);
            Products.Remove(SelectedProduct);
            CreatePurchase.Visibility = Visibility.Collapsed;
            ProductTitle.Text = "";
            ProductDescription.Text = "";
            FProdAmount.Text = "";
            CalculateTotal();
            //PurchaseOverviewLv.ItemsSource = PurchaseOrder.Products;

        }

        private void PurchaseOverviewLv_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the clicked item from the event arguments
            Product selectedProduct = e.ClickedItem as Product;

            if (selectedProduct != null)
            {
                SelectedProduct = selectedProduct;
                UpdateInfoPanel.Visibility = Visibility.Visible;

                // Update the details in the info panel
                UpdateProductTitle.Text = SelectedProduct.Name;
                UpdateProductDescription.Text = SelectedProduct.Description;
            }
        }

        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            int amount = 0;
            string input = FUpdateProdAmount.Value.ToString();
            if (FUpdateProdAmount != null)
            {
                amount = int.Parse(input);
            }
            Product product = (Product)PurchaseOrder.Products.Where(p => p.Id == SelectedProduct.Id).FirstOrDefault();
            product.Stock = amount;
            UpdateInfoPanel.Visibility = Visibility.Collapsed;
            UpdateProductTitle.Text = "";
            UpdateProductDescription.Text = "";
            FUpdateProdAmount.Text = "";
            PurchaseOverviewLv.ItemsSource = null;
            PurchaseOverviewLv.ItemsSource = PurchaseOrder.Products;
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            int productCount = 0;
            decimal costs = 0;

            foreach(Product product in PurchaseOrder.Products)
            {
                productCount += product.Stock;
                costs += product.Price * product.Stock ?? 0 * product.Stock;
            }
            TotalProductCount.Text = productCount.ToString();
            CostsTb.Text = Math.Round(costs,2).ToString();
        }
    }
}
