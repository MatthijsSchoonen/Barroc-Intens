using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Barroc_Intens.Data;
using System.Collections.ObjectModel;
using Barroc_Intens.Dashboards;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Maintenance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VisitCreate : Page
    {
        private ObservableCollection<User> maintenanceEmployees = [];
        private ObservableCollection<User> selectedEmployees = [];
        private ObservableCollection<Company> companies = [];
        
        public VisitCreate()
        {
            this.InitializeComponent();
            // Retrieve items and add them to ObservableCollection
            using (AppDbContext dbContext = new AppDbContext()) {
                // Only retrieve maintenance employees, but all companies
                List<User> retrievedUsers = dbContext.Users.Where(u => u.DepartmentId == 3).ToList();
                List<Company> retrievedCompanies = dbContext.Companies.ToList();

                // Did not find a way to properly set elements in ObservableCollections, so use foreach loops.
                foreach (User user in retrievedUsers)
                {
                    maintenanceEmployees.Add(user);
                }
                foreach (Company company in retrievedCompanies)
                {
                    companies.Add(company);
                }
            }
            // Set ObservableCollections as Itemsources for UI elements.
            FEmployee.ItemsSource = maintenanceEmployees;
            FCustomer.ItemsSource = companies;

        }

        // Validation and store into DB
        private void FOnSubmit_Click(object sender, RoutedEventArgs e)
        {

        }

        // Remove employee from ComboBox source and display elsewhere in UI
        private void FEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User selectedEmployee = (User)FEmployee.SelectedItem;
            maintenanceEmployees.Remove(selectedEmployee);
            selectedEmployees.Add(selectedEmployee);

        }

        // Removes employee from selectedEmployees and move it back to ComboBox
        private void FSelectedEmployees_ItemClick(object sender, ItemClickEventArgs e)
        {
            User selectedUser = (User)e.ClickedItem;
            selectedEmployees.Remove(selectedUser);
            maintenanceEmployees.Add(selectedUser);
        }
    }
}
