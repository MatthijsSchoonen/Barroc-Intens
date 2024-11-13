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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddStockView : Page
    {
        public AddStockView()
        {
            this.InitializeComponent();
            using AppDbContext db = new();
            productCategoryInput.ItemsSource = db.ProductsCategories.ToList();
            productCategoryInput.DisplayMemberPath = "Name";
        }

        public void BackToOverviewButton_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockView));
            //navigate back to stockview
        }

        public void AddProductButton_click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                //input validatie
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

                
                db.Products.Add(new Product
                {
                    Name = nameInput.Text,
                    Description = descInput.Text,
                    Brand = brand,
                    Price = decimal.Parse(priceInput.Text),
                    Stock = int.Parse(stockInput.Text),
                    ProductsCategoryId = selectedCategory.Id
                });
                //add product to the database
                db.SaveChanges();
                nameInput.Text = string.Empty;
                descInput.Text = string.Empty;
                brandInput.Text = string.Empty;
                priceInput.Text = string.Empty;
                stockInput.Text = string.Empty;
                productCategoryInput.SelectedItem = -1;
            }
            Frame.Navigate(typeof(StockView));
        }
    }
}
