using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Appointments;

namespace Barroc_Intens.Views.Maintenance
{
    public sealed partial class WorkOrderPage : Page
    {
        private ObservableCollection<Barroc_Intens.Data.WorkOrder> WorkOrderCollection = new();
        private ObservableCollection<MaintenanceAppointment> MaintenanceAppointmentsCollection = new();
        private ObservableCollection<Product> ProductCollection = new();
        private ObservableCollection<WorkOrderMat> WorkOrderMatsCollection = new();
        private Product SelectedProduct = null;
        private MaintenanceAppointment SelectedAppointment = null;
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

                List<MaintenanceAppointment> retrievedMaintenanceAppointments = dbContext.MaintenanceAppointments.ToList();
                foreach(MaintenanceAppointment appointment in retrievedMaintenanceAppointments)
                {
                    MaintenanceAppointmentsCollection.Add(appointment);
                }

                FSelectAppointment.ItemsSource = MaintenanceAppointmentsCollection;
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
        private void OnFAppointmentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if(comboBox != null && comboBox.SelectedItem != null)
            {
                MaintenanceAppointment appointment = comboBox.SelectedItem as MaintenanceAppointment;
                if(appointment != null)
                {
                    SelectedAppointment = appointment;
                }
            }
            else
            {
                Debug.WriteLine("WorkOrderPage: OnFAppointmentSelectionChanged: No product selected OR ComboBox retrieval issue");
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
                WorkOrderMatsCollection.Add(newWorkOrderMat);
                foreach (WorkOrderMat mat in WorkOrderMatsCollection)
                {
                    if (mat.ProductId == SelectedProduct.Id)
                    {
                        mat.Product.Stock -= productAmount;
                        Debug.WriteLine("Dolor");
                        // Force UI update by removing and re-adding the item (UI would not update otherwise)
                        // In order to avoid this forced UI update, the Stock property and the Product class needs improvement
                        WorkOrderMatsCollection.Remove(mat);
                        WorkOrderMatsCollection.Add(mat);
                    }
                }
                
                Debug.WriteLine(WorkOrderMatsCollection.Count);


            }
            else
            {
                Debug.WriteLine("WorkOrderPage: OnAddProductClick: No product selected OR Text is not filled in");

            }
        }

        private async void OnSubmitClick(object sender, RoutedEventArgs e)
        {
            if (SelectedAppointment != null && WorkOrderMatsCollection.Count > 0)
            {
                try
                {
                    using (AppDbContext dbContext = new AppDbContext())
                    {
                        // Ensure the appointment exists in the context
                        var existingAppointment = dbContext.MaintenanceAppointments
                            .FirstOrDefault(a => a.Id == SelectedAppointment.Id);

                        if (existingAppointment == null)
                        {
                            throw new InvalidOperationException("Selected maintenance appointment not found in database.");
                        }

                        // Create a new WorkOrder
                        var newOrder = new Barroc_Intens.Data.WorkOrder
                        {
                            MaintenanceAppointment = existingAppointment
                        };

                        // Prepare WorkOrderMats
                        var orderMats = new List<WorkOrderMat>();
                        foreach (var workOrderMat in WorkOrderMatsCollection)
                        {
                            // Find the actual product in the database
                            var existingProduct = dbContext.Products
                                .FirstOrDefault(p => p.Id == workOrderMat.Product.Id);

                            if (existingProduct == null)
                            {
                                throw new InvalidOperationException($"Product with ID {workOrderMat.Product.Id} not found.");
                            }

                            // Create WorkOrderMat with existing entities
                            var orderMat = new WorkOrderMat
                            {
                                Product = existingProduct,
                                ProductAmount = workOrderMat.ProductAmount,
                                WorkOrder = newOrder
                            };

                            // Update product stock
                            if (existingProduct.Stock < workOrderMat.ProductAmount)
                            {
                                throw new InvalidOperationException($"Insufficient stock for product: {existingProduct.Name}");
                            }
                            existingProduct.Stock -= workOrderMat.ProductAmount;

                            orderMats.Add(orderMat);
                        }

                        // Add the work order and its materials
                        newOrder.WorkOrderMats = orderMats;
                        dbContext.WorkOrders.Add(newOrder);

                        // Save changes
                        await dbContext.SaveChangesAsync();

                        // Show confirmation dialog
                        ContentDialog confirmationDialog = new ContentDialog
                        {
                            Title = "Work Order Created",
                            Content = "The work order has been successfully created.",
                            CloseButtonText = "Ok",
                            XamlRoot = this.Content.XamlRoot
                        };
                        await confirmationDialog.ShowAsync();

                        // Reset UI and collections
                        WorkOrderMatsCollection.Clear();
                        SelectedAppointment = null;
                        SelectedProduct = null;
                        FSelectAppointment.SelectedItem = null;
                        FSelectProduct.SelectedItem = null;
                        FProductAmount.Text = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error creating work order: {ex.Message}");

                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = $"Failed to create work order: {ex.Message}",
                        CloseButtonText = "Ok",
                        XamlRoot = this.Content.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
            else
            {
                ContentDialog validationDialog = new ContentDialog
                {
                    Title = "Validation Error",
                    Content = "Please select a maintenance appointment and add at least one product.",
                    CloseButtonText = "Ok",
                    XamlRoot = this.Content.XamlRoot
                };
                await validationDialog.ShowAsync();
            }
        }

    }
}
