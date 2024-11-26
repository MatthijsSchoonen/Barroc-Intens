using Barroc_Intens.Dashboards;
using Barroc_Intens.Data;
using Microsoft.EntityFrameworkCore;
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
            // Fetch companies from the database
            var companies = db.Companies.ToList();

            // Bind company names to the ComboBox
            companyComboBox.ItemsSource = companies;
            companyComboBox.DisplayMemberPath = "Name"; // Show company names
            companyComboBox.SelectedValuePath = "Id";  // Use company ID as the value
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
            var currentCompany = companyComboBox.SelectedItem as Company;
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