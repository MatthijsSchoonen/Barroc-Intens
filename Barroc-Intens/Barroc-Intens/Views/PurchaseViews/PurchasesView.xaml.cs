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
        }
    }
}
