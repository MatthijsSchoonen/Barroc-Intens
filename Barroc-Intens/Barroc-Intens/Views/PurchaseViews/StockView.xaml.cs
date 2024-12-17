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
using System.Collections.ObjectModel;
using System.Linq;

namespace Barroc_Intens.PurchaseViews
{
    public sealed partial class StockView : Page
    {
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Product> FilteredProducts { get; set; } = new ObservableCollection<Product>();

        public StockView()
        {
            this.InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var db = new AppDbContext())
            {
                // Include the related Brand data
                var products = db.Products.Include(p => p.Brand).ToList();

                Products.Clear();
                FilteredProducts.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                    FilteredProducts.Add(product);
                }
            }
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string brandFilter = BrandFilterTextBox.Text?.ToLower() ?? string.Empty;
            string descriptionFilter = DescriptionFilterTextBox.Text?.ToLower() ?? string.Empty;
            bool hideOutOfStock = HideOutOfStockToggle.IsOn;

            // Clear and repopulate FilteredProducts based on the filter criteria
            FilteredProducts.Clear();
            foreach (var product in Products)
            {
                bool matchesBrand = string.IsNullOrEmpty(brandFilter) ||
                                    (product.Brand != null && product.Brand.Name.ToLower().Contains(brandFilter));
                bool matchesDescription = string.IsNullOrEmpty(descriptionFilter) ||
                                          (product.Description != null && product.Description.ToLower().Contains(descriptionFilter));
                bool matchesStock = !hideOutOfStock || product.Stock > 0;

                if (matchesBrand && matchesDescription && matchesStock)
                {
                    FilteredProducts.Add(product);
                }
            }
        }

        private void HideOutOfStockToggle_Toggled(object sender, RoutedEventArgs e)
        {
            ApplyFilters(); // Reapply filters whenever the toggle state changes
        }

        private void AddProductFormButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddStockView));
        }

        private void StockSearchingView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedItem = (Product)e.ClickedItem;
            var window = new StockEditView(selectedItem.Id);
            window.Closed += StockEditWindow_Closed;
            window.Activate();
        }

        private void StockEditWindow_Closed(object sender, WindowEventArgs args)
        {
            Refresh();
        }

        public void Refresh()
        {
            LoadProducts();
            ApplyFilters();  // Reapply filters to update FilteredProducts after refreshing the data
        }
    }
}