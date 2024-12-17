using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Barroc_Intens.Data;
using System.Linq;

namespace Barroc_Intens.Dashboards
{
    public partial class Customer : Page
    {
        private AppDbContext _context;

        public Customer()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadContracts();
        }

        public void LoadContracts()
        {
            // Load all contracts and include company information
            var leaseContracts = _context.Contracts
                                         .Include(c => c.Company)
                                         .ToList();

            ContractListBox.ItemsSource = leaseContracts;
        }

        private void OnContractSelected(object sender, SelectionChangedEventArgs e)
        {
            if (ContractListBox.SelectedItem is Contract selectedContract)
            {
                // Query invoices related to the selected contract
                var invoices = _context.Invoices
                                       .Include(i => i.Company)
                                       .Include(i => i.Contract)
                                       .Where(i => i.ContractId == selectedContract.Id)
                                       .ToList();

                // Bind invoices to the InvoiceListView
                InvoiceListView.ItemsSource = invoices;
            }
        }

        private void OnInvoiceSelected(object sender, SelectionChangedEventArgs e)
        {
            if (InvoiceListView.SelectedItem is Invoice selectedInvoice)
            {
                // Load the products associated with the selected invoice
                var invoiceProducts = _context.InvoiceProducts
                                              .Include(ip => ip.Product)
                                              .Where(ip => ip.InvoiceId == selectedInvoice.Id)
                                              .ToList();

                // Bind to the InvoiceProductsListView
                InvoiceProductsListView.ItemsSource = invoiceProducts;
            }
        }
    }
}
