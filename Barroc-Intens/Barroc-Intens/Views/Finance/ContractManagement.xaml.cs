using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Barroc_Intens.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Barroc_Intens.Views.Finance
{
    public sealed partial class ContractManagement : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        public ObservableCollection<Contract> Contracts { get; set; } = new ObservableCollection<Contract>();

        public ContractManagement()
        {
            this.InitializeComponent();
            LoadContracts();
            LoadCompaniesAsync();
        }

        private async Task LoadCompaniesAsync()
        {
            //connect to the database get the companies
            using (AppDbContext context = new AppDbContext())
            {
                var companies = await context.Companies.ToListAsync();
                CompanyComboBox.ItemsSource = companies;
                CompanyComboBox.DisplayMemberPath = "Name";
                CompanyComboBox.SelectedValuePath = "Id";
            }
        }

        private void LoadContracts()
        {
            var contracts = _context.Contracts.ToList();
            Contracts.Clear();
            foreach (var contract in contracts)
                Contracts.Add(contract);
        }

        private void AddContract_Click(object sender, RoutedEventArgs e)
        {
            var selectedCompany = CompanyComboBox.SelectedItem as Company;
            int? companyId = selectedCompany?.Id;

            // Example: Gather inputs from text fields
            var newContract = new Contract
            {
                CompanyId = (int)companyId,
                StartDate = DateOnly.FromDateTime(StartDateInput.Date.DateTime),
                EndDate = DateOnly.FromDateTime(EndDateInput.Date.DateTime),
                BillingType = BillingTypeInput.Text,
                BkrCheckPassed = BkrCheckPassedInput.IsChecked == true
            };

            _context.Contracts.Add(newContract);
            _context.SaveChanges();
            LoadContracts();
            ClearInputs();
        }

       private void ClearInputs()
        {
         

            StartDateInput.Date = DateTime.Now;
            EndDateInput.Date = DateTime.Now;

            BillingTypeInput.Text = string.Empty;
            BkrCheckPassedInput.IsChecked = false;
        }


        private void EditContract_Click(object sender, RoutedEventArgs e)
        {
            if (ContractsListView.SelectedItem is Contract selectedContract)
            {
                Frame.Navigate(typeof(EditContract), selectedContract.Id);
            }
        }
    }
}
