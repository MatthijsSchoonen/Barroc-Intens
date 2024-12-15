using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Barroc_Intens.Data;

namespace Barroc_Intens.Views.Sales
{
    public sealed partial class AddCompany : Page
    {
        public AddCompany()
        {
            this.InitializeComponent();
        }

        private async void AddCompanyButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new Company object from the input fields
            var newCompany = new Company
            {
                Name = NameInput.Text,
                Phone = PhoneInput.Text,
                Address = AddressInput.Text,
                Zipcode = ZipcodeInput.Text,
                City = CityInput.Text,
                Country = CountryInput.Text,
                BkrCheckedAt = BkrCheckedAtInput.Date.DateTime,
                ContactName = string.IsNullOrWhiteSpace(ContactNameInput.Text) ? null : ContactNameInput.Text,
                ContactMail = string.IsNullOrWhiteSpace(ContactMailInput.Text) ? null : ContactMailInput.Text
            };

            // Save to database using AppDbContext
            using (var context = new AppDbContext())
            {
                context.Companies.Add(newCompany);
                await context.SaveChangesAsync();
            }

            // Provide feedback to the user
            var dialog = new ContentDialog
            {
                Title = "Success",
                Content = "Company added successfully!",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();

            //  clear fields after saving
            NameInput.Text = "";
            PhoneInput.Text = "";
            AddressInput.Text = "";
            ZipcodeInput.Text = "";
            CityInput.Text = "";
            CountryInput.Text = "";
            ContactNameInput.Text = "";
            ContactMailInput.Text = "";
        }
    }
}
