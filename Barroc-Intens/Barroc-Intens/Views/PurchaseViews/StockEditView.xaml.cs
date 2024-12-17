using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using System.Linq;

namespace Barroc_Intens.PurchaseViews
{
    public sealed partial class StockEditView : Window
    {
        private int currentProductId;
        public StockEditView(int productId)
        {
            this.InitializeComponent();

            this.currentProductId = productId;

            using AppDbContext db = new();
            // Fetch the product data to pre-fill the inputs
            Product product = db.Products.First(p => p.Id == productId);

            // Populate fields with product data
            nameInput.Text = product.Name;
            descInput.Text = product.Description;

            // Populate brand dropdown and select the correct brand
            brandInput.ItemsSource = db.Brands.ToList();
            brandInput.DisplayMemberPath = "Name";
            brandInput.SelectedItem = db.Brands.FirstOrDefault(b => b.Id == product.BrandId);

            // Populate category dropdown and select the correct category
            productCategoryInput.ItemsSource = db.ProductsCategories.ToList();
            productCategoryInput.DisplayMemberPath = "Name";
            productCategoryInput.SelectedItem = product.ProductsCategory;

            // Populate price and stock
            priceInput.Text = product.Price.ToString();
            stockInput.Text = product.Stock.ToString();
        }

        public void BackToOverviewButton_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void EditProductButton_click(object sender, RoutedEventArgs e)
        {
            using AppDbContext db = new();

            // Input validation
            if (string.IsNullOrWhiteSpace(nameInput.Text))
            {
                ErrorMessage.Text = "Name is required";
                return;
            }
            if (string.IsNullOrWhiteSpace(descInput.Text))
            {
                ErrorMessage.Text = "Description is required";
                return;
            }
            if (brandInput.SelectedItem == null)
            {
                ErrorMessage.Text = "Brand is required";
                return;
            }
            if (string.IsNullOrWhiteSpace(priceInput.Text))
            {
                ErrorMessage.Text = "Price is required";
                return;
            }
            if (string.IsNullOrWhiteSpace(stockInput.Text))
            {
                ErrorMessage.Text = "Stock is required";
                return;
            }
            if (productCategoryInput.SelectedItem == null)
            {
                ErrorMessage.Text = "Category is required";
                return;
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


            // Update the product in the database
            var product = db.Products.First(p => p.Id == currentProductId);
            product.Name = nameInput.Text;
            product.Description = descInput.Text;
            product.BrandId = ((Brand)brandInput.SelectedItem).Id; // Save selected brand
            product.Price = decimal.Parse(priceInput.Text);
            product.Stock = int.Parse(stockInput.Text);
            product.ProductsCategoryId = ((ProductsCategory)productCategoryInput.SelectedItem).Id;

            db.SaveChanges();
            this.Close();
        }

        private void DeleteProductButton_click(object sender, RoutedEventArgs e)
        {
            using AppDbContext db = new();
            var product = db.Products.First(p => p.Id == currentProductId);
            db.Products.Remove(product);
            db.SaveChanges();
            this.Close();
        }
    }
}
