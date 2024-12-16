using Barroc_Intens.Data;
using iText.Kernel.Pdf;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Windows.Storage;
using Windows.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Navigation;
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
            InitializePage();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            InitializePage();
        }
        private void InitializePage()
        {
            using (AppDbContext dbContext = new())
            {
                List<Barroc_Intens.Data.WorkOrder> retrievedWorkOrders = dbContext.WorkOrders.Include(wo => wo.MaintenanceAppointment).Include(wo => wo.WorkOrderMats).ToList();
                foreach (Barroc_Intens.Data.WorkOrder workOrder in retrievedWorkOrders)
                {
                    WorkOrderCollection.Add(workOrder);
                }

                List<Product> retrievedProducts = dbContext.Products.ToList();
                foreach (Product prod in retrievedProducts)
                {
                    ProductCollection.Add(prod);
                }

                List<MaintenanceAppointment> retrievedMaintenanceAppointments = dbContext.MaintenanceAppointments.Include(p => p.Company).ToList();
                foreach (MaintenanceAppointment appointment in retrievedMaintenanceAppointments)
                {
                    MaintenanceAppointmentsCollection.Add(appointment);
                }
                GvWorkOrders.ItemsSource = WorkOrderCollection;
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
                        SelectedProduct = null;
                        FSelectAppointment.SelectedItem = null;
                        FSelectProduct.SelectedItem = null;
                        WorkOrderCollection.Add(newOrder);
                        FProductAmount.Text = string.Empty;
                        await GenerateAndSendWorkOrderPDF(newOrder);
                        dbContext.Entry(SelectedAppointment).Reference(a => a.Company).Load();
                        Frame.Navigate(typeof(VisitDetails), SelectedAppointment);
                        AmountPanel.Visibility = Visibility.Collapsed;
                        AddPanel.Visibility = Visibility.Collapsed;
                        SelectedAppointment = null;
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

        private async Task GenerateAndSendWorkOrderPDF(Data.WorkOrder workOrder)
        {
            try
            {
                // 1. PDF generation
                string fileName = $"WorkOrder_{workOrder.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string pdfPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, fileName);

                using (PdfWriter writer = new PdfWriter(pdfPath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    // Titel
                    document.Add(new Paragraph("Werk Order")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(20));

                    // Work Order Details
                    document.Add(new Paragraph($"Work Order ID: {workOrder.Id}")
                        .SetFontSize(12));

                    document.Add(new Paragraph($"Maintenance Appointment: {workOrder.MaintenanceAppointment.Title}")
                        .SetFontSize(12));

                    // Producten tabel
                    Table table = new Table(3).UseAllAvailableWidth();
                    table.AddHeaderCell("Product");
                    table.AddHeaderCell("Beschrijving");
                    table.AddHeaderCell("Aantal");

                    foreach (var mat in workOrder.WorkOrderMats)
                    {
                        table.AddCell(mat.Product.Name);
                        table.AddCell(mat.Product.Description);
                        table.AddCell(mat.ProductAmount.ToString());
                    }

                    document.Add(table);
                }

                // 2. E-mail sending
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Barroc Intens", "officialbarrocintens@gmail.com"));
                message.To.Add(new MailboxAddress("Ontvanger", "officialbarrocintens@gmail.com"));
                message.Subject = $"Werk Order {workOrder.Id}";

                var builder = new BodyBuilder();
                builder.TextBody = $"Bijgevoegd vindt u de details van Werk Order {workOrder.Id}";
                builder.Attachments.Add(pdfPath);

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    // Let op: vervang deze instellingen met je eigen SMTP-configuratie
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("officialbarrocintens@gmail.com", "ymlf npoq mhoo wiiq");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                // Optioneel: PDF verwijderen na verzending
                File.Delete(pdfPath);

                // Bevestigingsdialoog
                ContentDialog confirmationDialog = new ContentDialog
                {
                    Title = "PDF Verzonden",
                    Content = "De werk order is succesvol gegenereerd en per e-mail verzonden.",
                    CloseButtonText = "Ok",
                    XamlRoot = this.Content.XamlRoot
                };
                await confirmationDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Foutafhandeling
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Fout",
                    Content = $"Er is een fout opgetreden: {ex.Message}",
                    CloseButtonText = "Ok",
                    XamlRoot = this.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }


    }
}
