using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.PurchaseViews
{
    public sealed partial class AddStockView : Page
    {
        public AddStockView()
        {
            this.InitializeComponent();
            using AppDbContext db = new();

            // Populate the product categories dropdown
            productCategoryInput.ItemsSource = db.ProductsCategories.ToList();
            productCategoryInput.DisplayMemberPath = "Name";

            // Populate the brands dropdown
            brandInput.ItemsSource = db.Brands.ToList();
            brandInput.DisplayMemberPath = "Name";
        }

        public void BackToOverviewButton_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StockView));
        }

        public void AddProductButton_click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                // Input validation
                string? selectedBrandName = null;
                ProductsCategory selectedCategory;
                Brand selectedBrand = null;

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
                if (brandInput.SelectedItem == null)
                {
                    ErrorMessage.Text = "Brand is required";
                    return;
                }
                else
                {
                    selectedBrand = (Brand)brandInput.SelectedItem;
                }
                if (string.IsNullOrWhiteSpace(priceInput.Text))
                {
                    ErrorMessage.Text = "Price is required";
                    return;
                }

                if (!decimal.TryParse(priceInput.Text, out decimal price) || price <= 0)
                {
                    ErrorMessage.Text = "Price must be a valid positive number";
                    return;
                }

                if (string.IsNullOrWhiteSpace(stockInput.Text))
                {
                    ErrorMessage.Text = "Stock is required";
                    return;
                }

                if (!int.TryParse(stockInput.Text, out int stock) || stock < 0)
                {
                    ErrorMessage.Text = "Stock must be a valid non-negative integer";
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

                // Add product to the database
                db.Products.Add(new Product
                {
                    Name = nameInput.Text,
                    Description = descInput.Text,
                    BrandId = selectedBrand.Id, // Use the selected Brand's Id
                    Price = decimal.Parse(priceInput.Text),
                    Stock = int.Parse(stockInput.Text),
                    ProductsCategoryId = selectedCategory.Id
                });

                // Save changes to the database
                db.SaveChanges();

                // Reset input fields
                nameInput.Text = string.Empty;
                descInput.Text = string.Empty;
                brandInput.SelectedItem = null;
                priceInput.Text = string.Empty;
                stockInput.Text = string.Empty;
                productCategoryInput.SelectedItem = null;

                // Navigate back to the StockView
                Frame.Navigate(typeof(StockView));
            }
        }
    }
}
