using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using Barroc_Intens.Data;
using System;

namespace Barroc_Intens.Views.Finance
{
    public sealed partial class EditContract : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private Contract _contract;

        public EditContract()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int contractId = (int)e.Parameter;
            _contract = _context.Contracts.Find(contractId);

            if (_contract != null)
            {
                CompanyIdInput.Text = _contract.CompanyId.ToString();
                StartDateInput.Text = _contract.StartDate.ToString();
                EndDateInput.Text = _contract.EndDate.ToString();
                BillingTypeInput.Text = _contract.BillingType;
                BkrCheckPassedInput.IsChecked = _contract.BkrCheckPassed;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            _contract.CompanyId = int.Parse(CompanyIdInput.Text);
            _contract.StartDate = DateOnly.Parse(StartDateInput.Text);
            _contract.EndDate = DateOnly.Parse(EndDateInput.Text);
            _contract.BillingType = BillingTypeInput.Text;
            _contract.BkrCheckPassed = BkrCheckPassedInput.IsChecked == true;

            _context.SaveChanges();
            Frame.GoBack();
        }

        private void DeleteContract_Click(object sender, RoutedEventArgs e)
        {
            var invoices = _context.Invoices.Where(i => i.ContractId == _contract.Id).ToList();
            _context.Invoices.RemoveRange(invoices);

            // Delete the contract
            _context.Contracts.Remove(_contract);
            _context.SaveChanges();
            Frame.GoBack();
        }
    }
}
