using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Barroc_Intens.Data;

namespace Barroc_Intens
{
    public sealed partial class InvoicePage : Page
    {
        // ObservableCollection to track selected products
        public ObservableCollection<string> SelectedProducts { get; set; }

        public InvoicePage()
        {
            this.InitializeComponent();
            InitializeData();
        }

        // Initialize data and set up bindings
        private async void InitializeData()
        {
            SelectedProducts = new ObservableCollection<string>();
            ProductListBox.ItemsSource = SelectedProducts; // Bind ListBox to ObservableCollection
            await LoadCompaniesAndProductsAsync();
        }

        // Load companies and products asynchronously
        private async Task LoadCompaniesAndProductsAsync()
        {
            using var context = new AppDbContext();

            // Load companies
            var companies = await context.Companies.ToListAsync();
            CompanyComboBox.ItemsSource = companies;
            CompanyComboBox.DisplayMemberPath = "Name";
            CompanyComboBox.SelectedValuePath = "Id";

            // Load products
            var products = await context.Products.ToListAsync();
            ProductComboBox.ItemsSource = products;
            ProductComboBox.DisplayMemberPath = "Name";
            ProductComboBox.SelectedValuePath = "Id";
        }

        // Add selected product to the list
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is Product selectedProduct &&
                !SelectedProducts.Contains(selectedProduct.Name))
            {
                SelectedProducts.Add(selectedProduct.Name);
            }
        }

        // Remove selected product from the list
        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is string product)
            {
                SelectedProducts.Remove(product);
            }
        }

        // Generate invoice
        private void GenereerFactuur_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return; // Step 1: Validate user inputs

            using var db = new AppDbContext();

            if (!TryCalculateInvoice(out decimal aansluitkosten, out decimal btwPercentage,
                                      out decimal btwBedrag, out decimal totaal))
            {
                ShowError("Invalid values for connection costs or VAT percentage.");
                return;
            }

            var selectedCompany = CompanyComboBox.SelectedItem as Company;
            int companyId = selectedCompany?.Id ?? 0;

            // Step 2: Create invoice record
            var customInvoice = new CustomInvoice
            {
                Date = DateTime.Now,
                PaidAt = DateTime.Now,
                CompanyId = companyId
            };

            db.CustomInvoices.Add(customInvoice);
            db.SaveChanges();

            // Step 3: Add selected products to invoice
            AddProductsToInvoice(db, customInvoice.Id);

            db.SaveChanges();

            // Step 4: Display results and reset the form
            lblResult.Text = $"Aansluitkosten: €{aansluitkosten:F2}\n" +
                             $"BTW ({btwPercentage}%): €{btwBedrag:F2}\n" +
                             $"Totaal: €{totaal:F2}";
            ResetForm();
        }

        // Try to calculate invoice totals
        private bool TryCalculateInvoice(out decimal aansluitkosten, out decimal btwPercentage,
                                         out decimal btwBedrag, out decimal totaal)
        {
            aansluitkosten = btwPercentage = btwBedrag = totaal = 0;

            if (decimal.TryParse(txtAansluitkosten.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out aansluitkosten) &&
                decimal.TryParse(txtBtwPercentage.Text, out btwPercentage))
            {
                btwBedrag = (aansluitkosten * btwPercentage) / 100;
                totaal = aansluitkosten + btwBedrag;
                return true;
            }

            return false;
        }

        // Add selected products to the invoice
        private void AddProductsToInvoice(AppDbContext db, int invoiceId)
        {
            foreach (var productName in SelectedProducts)
            {
                var product = db.Products.FirstOrDefault(p => p.Name == productName);
                if (product != null)
                {
                    db.CustomInvoiceProducts.Add(new CustomInvoiceProduct
                    {
                        ProductId = product.Id,
                        CustomInvoiceId = invoiceId
                    });
                }
            }
        }

        // Validate user inputs
        private bool ValidateInputs()
        {
            if (!decimal.TryParse(txtAansluitkosten.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out _) ||
                !decimal.TryParse(txtBtwPercentage.Text, out _))
            {
                ShowError("Please enter valid numbers for connection costs and VAT.");
                return false;
            }

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

            if (endDatePicker.SelectedDate.Value < startDatePicker.SelectedDate.Value)
            {
                ShowError("The end date cannot be before the start date.");
                return false;
            }

            if (CompanyComboBox.SelectedIndex == -1)
            {
                ShowError("Please select a company.");
                return false;
            }

            return true;
        }

        // Show error messages
        private void ShowError(string message)
        {
            lblResult.Text = $"Error: {message}";
        }

        // Reset form fields after invoice generation
        private void ResetForm()
        {
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
