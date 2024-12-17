using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Barroc_Intens.Data;

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
            // Example: Gather inputs from text fields
            var newContract = new Contract
            {
                CompanyId = int.Parse(CompanyIdInput.Text),
                StartDate = DateOnly.Parse(StartDateInput.Text),
                EndDate = DateOnly.Parse(EndDateInput.Text),
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
            CompanyIdInput.Text = string.Empty;
            StartDateInput.Text = string.Empty;
            EndDateInput.Text = string.Empty;
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
