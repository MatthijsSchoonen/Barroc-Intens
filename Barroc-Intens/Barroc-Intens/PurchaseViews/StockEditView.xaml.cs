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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.PurchaseViews
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StockEditView : Window
    {
        private int currentProductId;
        public StockEditView(int productId)
        {
            this.InitializeComponent();

            this.currentProductId = productId;

            using AppDbContext db = new();

            Product product = db.Products.First(p => p.Id == productId);
            nameInput.Text = product.Name;
            descInput.Text = product.Description;
            brandInput.Text = product.Brand;
            priceInput.Text = product.Price.ToString();
            stockInput.Text = product.Stock.ToString();
            productCategoryInput.ItemsSource = db.ProductsCategories;
            productCategoryInput.DisplayMemberPath = "Name";
            productCategoryInput.SelectedItem = product.ProductsCategory;
        }

        public void BackToOverviewButton_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void EditProductButton_click(object sender, RoutedEventArgs e)
        {
            using AppDbContext db = new();
            string? brand;
            ProductsCategory selectedCategory;
            if (nameInput.Text == "")
            {
                ErrorMessage.Text = "Name is required";
                return;
            }
            if (descInput.Text == "")
            {
                ErrorMessage.Text = "Description is required";
                return;
            }
            if (brandInput.Text == "")
            {
                brand = null;
            }
            else
            {
                brand = brandInput.Text;
            }
            if (priceInput.Text == "")
            {
                ErrorMessage.Text = "Price is required";
                return;
            }
            if (stockInput.Text == "")
            {
                ErrorMessage.Text = "Stock is required";
                return;
            }
            if (productCategoryInput.SelectedItem == null)
            {
                ErrorMessage.Text = "Category is required";
                return;
            }
            else
            {
                selectedCategory = (ProductsCategory)productCategoryInput.SelectedItem;
            }
            var product = db.Products.First(p => p.Id == currentProductId);
            product.Name = nameInput.Text;
            product.Description = descInput.Text;
            product.Brand = brandInput.Text;
            product.ProductsCategory = (ProductsCategory)productCategoryInput.SelectedItem;
            db.SaveChanges();
            this.Close();
        }

        private void DeleteProductButton_click(object sender, RoutedEventArgs p)
        {
            using var db = new AppDbContext();
            {
                var product = db.Products.First(p => p.Id == currentProductId);
                db.Products.Remove(product);
                db.SaveChanges();
                this.Close();
            }
        }
    }
}
