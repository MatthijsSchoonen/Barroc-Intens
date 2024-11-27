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
            if (!ValidateInputs()) return; // Validate inputs first

            using var db = new AppDbContext();

            if (!TryCalculateInvoice(out decimal connectionCost, out int vatPercentage,
                                      out decimal vatAmount, out decimal total))
            {
                ShowError("Invalid values for connection costs or VAT percentage.");
                return;
            }

            var selectedCompany = CompanyComboBox.SelectedItem as Company;
            int companyId = selectedCompany?.Id ?? 0;

            // Create a new CustomInvoice
            var customInvoice = new CustomInvoice
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                CompanyId = companyId,
                ConnectionCost = connectionCost,
                VAT = vatPercentage,
            };

            db.CustomInvoices.Add(customInvoice);
            db.SaveChanges();

            // Add selected products to the invoice
            AddProductsToInvoice(db, customInvoice.Id);

            db.SaveChanges();

            // Display results and reset the form
            lblResult.Text = $"Aansluitkosten: �{connectionCost:F2}\n" +
                             $"BTW ({vatPercentage}%): �{vatAmount:F2}\n" +
                             $"Totaal: �{total:F2}";
            ResetForm();
        }


        // Try to calculate invoice totals
        private bool TryCalculateInvoice(out decimal connectionCost, out int vatPercentage,
                                   out decimal vatAmount, out decimal total)
        {
            connectionCost = vatAmount = total = 0;
            vatPercentage = 0;

            if (decimal.TryParse(txtAansluitkosten.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out connectionCost) &&
                int.TryParse(txtBtwPercentage.Text, out vatPercentage))
            {
                vatAmount = (connectionCost * vatPercentage) / 100;
                total = connectionCost + vatAmount;
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
    // Validate Connection Costs and VAT
    if (!decimal.TryParse(txtAansluitkosten.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out _) ||
        !int.TryParse(txtBtwPercentage.Text, out _))
    {
        ShowError("Please enter valid numbers for connection costs and VAT.");
        return false;
    }

    // Ensure a company is selected
    if (CompanyComboBox.SelectedIndex == -1)
    {
        ShowError("Please select a company.");
        return false;
    }

    // Ensure dates are selected and valid
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
