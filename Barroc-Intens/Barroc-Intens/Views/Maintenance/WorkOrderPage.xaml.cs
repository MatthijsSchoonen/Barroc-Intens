using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Barroc_Intens.Views.Maintenance
{
    public sealed partial class WorkOrderPage : Page
    {
        private ObservableCollection<Barroc_Intens.Data.WorkOrder> WorkOrderCollection = new();
        private ObservableCollection<Product> ProductCollection = new();
        private ObservableCollection<WorkOrderMat> WorkOrderMatsCollection = new();
        private Product SelectedProduct = null;

        public WorkOrderPage()
        {
            this.InitializeComponent();
            using (AppDbContext dbContext = new())
            {
                List<Barroc_Intens.Data.WorkOrder> retrievedWorkOrders = dbContext.WorkOrders.ToList();
                foreach (Barroc_Intens.Data.WorkOrder workOrder in retrievedWorkOrders)
                {
                    WorkOrderCollection.Add(workOrder);
                }
                GvWorkOrders.ItemsSource = WorkOrderCollection;

                List<Product> retrievedProducts = dbContext.Products.ToList();
                foreach(Product prod in retrievedProducts)
                {
                    ProductCollection.Add(prod);
                }
                FSelectProduct.ItemsSource = ProductCollection;
                GvSelectedProducts.ItemsSource = WorkOrderMatsCollection;
            }
        }

        private void OnAddPanelToggleClick(object sender, RoutedEventArgs e)
        {
            AddPanel.Visibility = Visibility.Visible;
        }

        private void OnGvSelectedProductsClick(object sender, ItemClickEventArgs e)
        {

        }


        private void OnFSelectProductSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cast sender to ComboBox
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                // Retrieve the selected product
                Product selectedProduct = comboBox.SelectedItem as Product;

                if (selectedProduct != null)
                {
                    // Work with the selected product
                    // For example, display its details or use it in your logic
                    string productName = selectedProduct.Name;
                    string productDescription = selectedProduct.Description;
                    SelectedProduct = selectedProduct;

                    // Example: Log product info
                    Debug.WriteLine($"Selected Product: {productName} - {productDescription}");
                    AmountPanel.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Debug.WriteLine("WorkOrderPage: OnFSelectProductSelectionChanged: No product selected OR ComboBox retrieval issue");
            }
        }
        private void OnAddProductClick(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct != null) { 
                int productAmount = int.Parse(FProductAmount.Text);
                WorkOrderMat newWorkOrderMat = new();
                SelectedProduct.Stock -= productAmount;
                newWorkOrderMat.Product = SelectedProduct;
                newWorkOrderMat.ProductAmount = productAmount;
                foreach (WorkOrderMat mat in WorkOrderMatsCollection)
                {
                    if (mat.ProductId == SelectedProduct.Id)
                    {
                        mat.Product.Stock -= productAmount;
                    }
                }
                WorkOrderMatsCollection.Add(newWorkOrderMat);
                Debug.WriteLine(WorkOrderMatsCollection.Count);


            }
            else
            {
                Debug.WriteLine("WorkOrderPage: OnAddProductClick: No product selected OR Text is not filled in");

            }
        }

    }
}
