using Barroc_Intens.Dashboards;
using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Sales
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewCustomerPage : Page
    {
        public NewCustomerPage()
        {
            this.InitializeComponent();
            AppDbContext db = new AppDbContext();
            var companies = db.Companies.Where(c => string.IsNullOrEmpty(c.ContactMail)).ToList();
            var companyNames = companies.Select(c => c.Name).ToList();
            companyComboBox.ItemsSource = companyNames;
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameInput.Text == "")
            {
                ErrorMessage.Text = "Name is required";
            }
            if (emailInput.Text == "")
            {
                ErrorMessage.Text = "Email is required";
            }
            if (companyComboBox.SelectedItem == null)
            {
                ErrorMessage.Text = "Company is required";
            }
            AppDbContext db = new AppDbContext();
            Company currentCompany = db.Companies.Where(c => c.Name == companyComboBox.SelectedItem.ToString()).FirstOrDefault();
            db.Attach(currentCompany);
            currentCompany.ContactMail = emailInput.Text;
            currentCompany.ContactName = nameInput.Text;
            nameInput.Text = null;
            emailInput.Text = null;
            companyComboBox.SelectedItem = null;
            db.SaveChanges();
            Frame.Navigate(typeof(SalesDash));
        }
    }
}
