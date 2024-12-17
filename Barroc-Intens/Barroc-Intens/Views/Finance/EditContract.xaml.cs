using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using Barroc_Intens.Data;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Barroc_Intens.Views.Finance
{
    public sealed partial class EditContract : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private Contract _contract;

        public EditContract()
        {
            this.InitializeComponent();
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int contractId = (int)e.Parameter;
            _contract = _context.Contracts.Find(contractId);

            if (_contract != null)
            {
                CompanyComboBox.SelectedValue = _contract.CompanyId;

                // Convert DateOnly to DateTimeOffset for DatePicker
                StartDateInput.Date = new DateTimeOffset(_contract.StartDate.ToDateTime(TimeOnly.MinValue));
                EndDateInput.Date = new DateTimeOffset(_contract.EndDate.ToDateTime(TimeOnly.MinValue));

                BillingTypeInput.Text = _contract.BillingType;
                BkrCheckPassedInput.IsChecked = _contract.BkrCheckPassed;
            }
        }


        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {

                var selectedCompany = CompanyComboBox.SelectedItem as Company;
                int? companyId = selectedCompany?.Id;
                _contract.CompanyId = (int)companyId;

                // Convert DateTimeOffset to DateOnly
                _contract.StartDate = DateOnly.FromDateTime(StartDateInput.Date.DateTime);
                _contract.EndDate = DateOnly.FromDateTime(EndDateInput.Date.DateTime);

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
