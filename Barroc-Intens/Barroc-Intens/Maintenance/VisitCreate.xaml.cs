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
        private ObservableCollection<Product> products = [];
        private ObservableCollection<Product> compartments = [];
        private ObservableCollection<Product> selectedCompartments = [];
        private User loggedInUser = new();
        private Product productOfInterest;
        public VisitCreate(User loggedInUser)
        {
            this.InitializeComponent();
            this.loggedInUser = loggedInUser;
            // Retrieve items and add them to ObservableCollection
            using (AppDbContext dbContext = new AppDbContext()) {
                // Only retrieve maintenance employees, but all companies
                List<User> retrievedUsers = dbContext.Users.Where(u => u.DepartmentId == 3).ToList();
                List<Company> retrievedCompanies = dbContext.Companies.ToList();
                List<Product> retrievedProducts = dbContext.Products.ToList();
                // Did not find a way to properly set elements in ObservableCollections, so use foreach loops.
                foreach (User user in retrievedUsers)
                {
                    maintenanceEmployees.Add(user);
                }
                foreach (Company company in retrievedCompanies)
                {
                    companies.Add(company);
                }
                foreach (Product p in retrievedProducts)
                {
                    products.Add(p);
                    compartments.Add(p);
                }
            }
            // Set ObservableCollections as Itemsources for UI elements.
            FEmployee.ItemsSource = maintenanceEmployees;
            FCustomer.ItemsSource = companies;
            FSelectedEmployees.ItemsSource = selectedEmployees;
            FCompartments.ItemsSource = this.compartments;
            FProductOfInterest.ItemsSource = products;
        }

        // Validation and store into DB
        private void FOnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Check if start does not exceed end
            DateTimeOffset startDate = FVisitStart.Date;
            DateTimeOffset endDate = FVisitEnd.Date;
            string description = FDescription.Text;
            if (FCustomer.SelectedItem == null) {
                CustomerError.Text = "Please select a customer.";
            }
            Company customerCompany = FCustomer.SelectedItem as Company;
            if(FStatus.SelectedIndex == -1)
            {
                StatusError.Text = "Please select a status.";
                return;
            }
            int status = FStatus.SelectedIndex;
            if (startDate > endDate) {
                StartDateError.Text = "Start date cannot exceed end date, please check the input fields";
                return;
            }
            if (endDate < startDate) {
                EndDateError.Text = "End date cannot take place before the start";
                return;
            }

            if(this.productOfInterest == null)
            {
                ProductOfInterestError.Text = "Please select a product of interest (like a product where something broke or requires maintenance)";
                return;
            }
            
            if(selectedCompartments.Count < 1)
            {
                CompartmentsError.Text = "Please select one or more parts";
                return;
            }

            if(description.Length < 1)
            {
                DescriptionError.Text = "Please enter a description (provide more information about the visit, possibly some important notes)";
                return;
            }

            if (status < 3 && status > 0)
            {
                StatusError.Text = "Select a valid status.";
                return;
            }

            if (selectedEmployees.Count < 1) {
                EmployeeError.Text = "Select one or more employees.";
                return;
            }

            using (AppDbContext dbContext = new AppDbContext()) {
                MaintenanceAppointment newApp = new();
                newApp.Product = this.productOfInterest;
                newApp.User = this.loggedInUser;
                newApp.Company = customerCompany;
                newApp.Description = description;
                newApp.Status = status;
                newApp.DateAdded = DateTime.Now;
                newApp.StartTime = startDate.UtcDateTime;
                newApp.EndTime = endDate.UtcDateTime;
                newApp.Products = selectedCompartments;
                dbContext.MaintenanceAppointments.Add(newApp);
                dbContext.SaveChanges();

            }
        }

        // Remove employee from ComboBox source and display elsewhere in UI
        private void FEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if there's a valid selected item before proceeding
            if (FEmployee.SelectedItem is User selectedEmployee)
            {
                // Remove the selected employee from maintenanceEmployees
                maintenanceEmployees.Remove(selectedEmployee);

                // Add the selected employee to selectedEmployees
                selectedEmployees.Add(selectedEmployee);

                // Clear the selection to avoid re-triggering the selection change
                FEmployee.SelectedItem = null;
            }

        }

        // Removes employee from selectedEmployees and move it back to ComboBox
        private void FSelectedEmployees_ItemClick(object sender, ItemClickEventArgs e)
        {
            User selectedUser = (User)e.ClickedItem;
            selectedEmployees.Remove(selectedUser);
            maintenanceEmployees.Add(selectedUser);
        }

        private void FProductOfInterest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.productOfInterest = (Product)FProductOfInterest.SelectedItem;
        }

        private void FCompartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if there's a valid selected item before proceeding
            if (FCompartments.SelectedItem is Product selectedCompartment)
            {
                // Remove the selected employee from maintenanceEmployees
                compartments.Remove(selectedCompartment);

                // Add the selected employee to selectedEmployees
                selectedCompartments.Add(selectedCompartment);

                // Clear the selection to avoid re-triggering the selection change
                FCompartments.SelectedItem = null;
            }
        }
        private void FSelectedCompartments_ItemClick(object sender, ItemClickEventArgs e)
        {
            Product selectedProduct = e.ClickedItem as Product;
            selectedCompartments.Remove(selectedProduct);
            compartments.Add(selectedProduct);
        }
        private void FCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}
