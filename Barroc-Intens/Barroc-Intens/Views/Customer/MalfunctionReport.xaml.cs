using Barroc_Intens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Barroc_Intens.Views.Customer
{
    public sealed partial class MalfunctionReport : Page
    {
        public List<Invoice> Invoices { get; set; }
        private AppDbContext db;

        public MalfunctionReport()
        {
            this.InitializeComponent();

            // Initialize database context
            db = new AppDbContext();

            // Load invoices
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            // Fetch invoices for the logged-in user's company
            int? companyId = User.LoggedInUser.CompanyId;

            if (companyId.HasValue)
            {
                Invoices = db.Invoices
                                     .Where(i => i.CompanyId == companyId.Value)
                                     .ToList();
                InvoiceSelector.ItemsSource = Invoices;
            }
            else
            {
                DisplayMessage("No company associated with the current user.");
            }
        }

        private void InvoiceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedInvoice = (Invoice)InvoiceSelector.SelectedItem;

            if (selectedInvoice != null)
            {
                // Fetch products for the selected invoice
                var products = db.InvoiceProducts
                          .Where(ip => ip.InvoiceId == selectedInvoice.Id)
                          .Include(ip => ip.Product) 
                          .ToList();

                ProductsListView.ItemsSource = products;

            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProducts = ProductsListView.SelectedItems.Cast<InvoiceProduct>().ToList();

            if (!selectedProducts.Any())
            {
                DisplayMessage("Please select at least one product.");
                return;
            }

            // Get the description from the TextBox
            string problemDescription = ProblemDescriptionTextBox.Text.Trim();

            if (string.IsNullOrEmpty(problemDescription))
            {
                DisplayMessage("Please provide a description of the problem.");
                return;
            }

            // Create a new maintenance appointment with the description
            var appointment = new MaintenanceAppointment
            {
                UserId = 12,
                CompanyId = User.LoggedInUser.CompanyId.Value,
                Description = problemDescription, 
                DateAdded = DateTime.Now,
                StartTime = DateTime.Now.AddDays(3),
                EndTime = DateTime.Now.AddHours(1).AddDays(3),
                Title = "Maintenance Appointment",
                MaintenanceAppointmentProducts = selectedProducts.Select(p => new MaintenanceAppointmentProduct
                {
                    ProductId = p.ProductId
                }).ToList()
            };

            // Save appointment
            db.MaintenanceAppointments.Add(appointment);
            db.SaveChanges();

            DisplayMessage("Maintenance appointment created successfully.");
        }

        private void DisplayMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Message",
                Content = message,
                CloseButtonText = "Ok",
                XamlRoot = this.XamlRoot
            };

            dialog.ShowAsync();
        }
    }
}
