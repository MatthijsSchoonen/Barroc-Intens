using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Barroc_Intens.Data;
using System.Linq;

namespace Barroc_Intens
{
    public sealed partial class InvoicePage : Page
    {
        // ObservableCollection to track selected products
        public ObservableCollection<string> SelectedProducts { get; set; }

        public InvoicePage()
        {
            this.InitializeComponent();
            LoadCompaniesAsync();
            SelectedProducts = new ObservableCollection<string>();  // Initialize the collection
            ProductListBox.ItemsSource = SelectedProducts;  // Bind the ListBox to the ObservableCollection
        }

        private async Task LoadCompaniesAsync()
        {
            using (AppDbContext context = new AppDbContext())
            {
                var companies = await context.Companies.ToListAsync();
                CompanyComboBox.ItemsSource = companies;
                CompanyComboBox.DisplayMemberPath = "Name";
                CompanyComboBox.SelectedValuePath = "Id";
            }
            using (AppDbContext context = new AppDbContext())
            {
                var products = await context.Products.ToListAsync();
                ProductComboBox.ItemsSource = products;
                ProductComboBox.DisplayMemberPath = "Name";
                ProductComboBox.SelectedValuePath = "Id";
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {

            var selectedProduct = ProductComboBox.SelectedItem as Product;
            if (selectedProduct != null && !SelectedProducts.Contains(selectedProduct.Name))
            {
                SelectedProducts.Add(selectedProduct.Name);
            }
        }

        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.DataContext as string;

            if (product != null)
            {
                SelectedProducts.Remove(product);
            }
        }

        private void GenereerFactuur_Click(object sender, RoutedEventArgs e)
        {
            // Step 1: Validate user inputs
            if (!ValidateInputs())
            {
                return;
            }

            using (AppDbContext db = new AppDbContext())
            {
                string klantNaam = txtNaam.Text;
                string klantAdres = txtAdres.Text;

                if (decimal.TryParse(txtAansluitkosten.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal aansluitkosten) &&
                    decimal.TryParse(txtBtwPercentage.Text, out decimal btwPercentage))
                {
                    decimal btwBedrag = (aansluitkosten * btwPercentage) / 100;
                    decimal totaal = aansluitkosten + btwBedrag;

                    lblResult.Text = $"Factuur voor: {klantNaam}\n" +
                                     $"Adres: {klantAdres}\n\n" +
                                     $"Aansluitkosten: €{aansluitkosten:F2}\n" +
                                     $"BTW ({btwPercentage}%): €{btwBedrag:F2}\n" +
                                     $"Totaal: €{totaal:F2}";
                }
                else
                {
                    ShowError("Invalid values for connection costs or VAT percentage.");
                    return;
                }

                var selectedCompany = CompanyComboBox.SelectedItem as Company;
                int companyId = selectedCompany?.Id ?? 0;

                // Create a new invoice record
                var customInvoice = new CustomInvoice
                {
                    Date = DateTime.Now, // Use current time or pick a date
                    PaidAt = DateTime.Now, // Similarly, use the paid date
                    CompanyId = companyId
                };

                db.CustomInvoices.Add(customInvoice);
                db.SaveChanges();

                // Add each selected product to the invoice
                foreach (var productName in SelectedProducts)
                {
                    var product = db.Products.FirstOrDefault(p => p.Name == productName);
                    if (product != null)
                    {
                        db.CustomInvoiceProducts.Add(new CustomInvoiceProduct
                        {
                            ProductId = product.Id,
                            CustomInvoiceId = customInvoice.Id
                        });
                    }
                }

                db.SaveChanges();

                // Clear the input fields and reset selections
                ResetForm();
            }
        }

        // Validation method to check all required fields
        private bool ValidateInputs()
        {
            // Validate Customer Information
            if (string.IsNullOrWhiteSpace(txtNaam.Text) || string.IsNullOrWhiteSpace(txtAdres.Text))
            {
                ShowError("Please fill in the customer name and address.");
                return false;
            }

            // Validate Connection Costs and VAT
            if (!decimal.TryParse(txtAansluitkosten.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal aansluitkosten) ||
                !decimal.TryParse(txtBtwPercentage.Text, out decimal btwPercentage))
            {
                ShowError("Please enter valid numbers for connection costs and VAT.");
                return false;
            }

            // Ensure at least one product is selected
            if (SelectedProducts.Count == 0)
            {
                ShowError("Please add at least one product.");
                return false;
            }

            if (!startDatePicker.SelectedDate.HasValue || !endDatePicker.SelectedDate.HasValue)
            {
                ShowError("Please select both start and end dates.");
                return false;
            }

            // Ensure the end date is not before the start date
            if (endDatePicker.SelectedDate.Value < startDatePicker.SelectedDate.Value)
            {
                ShowError("The end date cannot be before the start date.");
                return false;
            }

            // Ensure a company is selected
            if (CompanyComboBox.SelectedIndex == -1)
            {
                ShowError("Please select a company.");
                return false;
            }

            return true;
        }

        // Show error message on the result label
        private void ShowError(string message)
        {
            lblResult.Text = $"Error: {message}";
        }

        // Reset form fields after invoice generation
        private void ResetForm()
        {
            txtNaam.Text = string.Empty;
            txtAdres.Text = string.Empty;
            txtAansluitkosten.Text = string.Empty;
            txtBtwPercentage.Text = "21";
            CompanyComboBox.SelectedIndex = -1;
            ProductComboBox.SelectedIndex = -1;
            startDatePicker.SelectedDate = null;
            endDatePicker.SelectedDate = null;
            SelectedProducts.Clear(); // Clear selected products list
        }
    }
}
