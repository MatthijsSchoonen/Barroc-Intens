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
using System.Diagnostics;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.PurchaseViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StockView : Page
    {
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Product> FilteredProducts { get; set; } = new ObservableCollection<Product>();

        public StockView()
        {
            this.InitializeComponent();

            // Load products from the database
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var db = new AppDbContext())
            {
                // Populate Products and FilteredProducts with data from the database
                var products = db.Products.ToList();
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

            // Clear and repopulate FilteredProducts based on the filter criteria
            FilteredProducts.Clear();
            //foreach (var product in Products)
            //{
            //    bool matchesBrand = string.IsNullOrEmpty(brandFilter) ||
            //                        (product.Brand != null && product.Brand.ToLower().Contains(brandFilter));
            //    bool matchesDescription = string.IsNullOrEmpty(descriptionFilter) ||
            //                              (product.Description != null && product.Description.ToLower().Contains(descriptionFilter));

            //    if (matchesBrand && matchesDescription)
            //    {
            //        FilteredProducts.Add(product);
            //    }
            //}
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
