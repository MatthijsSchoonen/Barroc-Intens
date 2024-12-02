
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
using System.Diagnostics;
using Barroc_Intens.Views.Maintenance;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Maintenance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VisitCreate : Page
    {
        private ObservableCollection<User> maintenanceEmployees = new ObservableCollection<User>();
        //private ObservableCollection<User> selectedEmployees = new ObservableCollection<User>();
        private ObservableCollection<Company> companies = new ObservableCollection<Company>();
        private ObservableCollection<Product> products = new ObservableCollection<Product>();
        private ObservableCollection<Product> compartments = new ObservableCollection<Product>();
        private ObservableCollection<Product> selectedCompartments = new ObservableCollection<Product>();

        private User loggedInUser;
        private Product productOfInterest;

        public VisitCreate()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is User user)
            {
                loggedInUser = user;

                using (AppDbContext dbContext = new AppDbContext())
                {
                    List<User> retrievedUsers = dbContext.Users.Where(u => u.DepartmentId == 3).ToList();
                    List<Company> retrievedCompanies = dbContext.Companies.ToList();
                    List<Product> retrievedProducts = dbContext.Products.ToList();

                    foreach (User employee in retrievedUsers)
                    {
                        maintenanceEmployees.Add(employee);
                    }
                    foreach (Company company in retrievedCompanies)
                    {
                        companies.Add(company);
                    }
                    foreach (Product product in retrievedProducts)
                    {
                        products.Add(product);
                        compartments.Add(product);
                    }
                }

                FEmployee.ItemsSource = maintenanceEmployees;
                FCustomer.ItemsSource = companies;
                //FSelectedEmployees.ItemsSource = selectedEmployees;
                FCompartments.ItemsSource = compartments;
                FProductOfInterest.ItemsSource = products;
                FSelectedCompartments.ItemsSource = selectedCompartments;
            }
            else
            {
                Debug.WriteLine("Invalid navigation parameter.");
            }
        }

        // Validation and store into DB
        private void FOnSubmit_Click(object sender, RoutedEventArgs e)
        {

            // Check if start does not exceed end
            DateTimeOffset startDate = FVisitStart.Date;
            DateTimeOffset endDate = FVisitEnd.Date;
            Company customerCompany = FCustomer.SelectedItem as Company;
            TimeSpan startTime = FVisitStartTime.Time;
            TimeSpan endTime = FVisitEndTime.Time;
            string title = FTitle.Text;
            startDate += startTime;
            endDate += endTime;
            string description = FDescription.Text;
            int status = FStatus.SelectedIndex;
            bool isValid = true;

            Debug.WriteLine(startTime + " |" + TimeSpan.Zero);
            if (FCustomer.SelectedItem == null)
            {
                CustomerError.Text = "Please select a customer.";
            }
            if (FStatus.SelectedIndex == -1)
            {
                StatusError.Text = "Please select a status.";
                isValid = false;
            }


            if (!FVisitStart.SelectedDate.HasValue || FVisitStart.SelectedDate == DateTimeOffset.MinValue || startTime <= TimeSpan.Zero)
            {
                StartDateError.Text = "Please enter a start date and time";
                isValid = false;
            }
            if (!FVisitEnd.SelectedDate.HasValue || FVisitEnd.SelectedDate == DateTimeOffset.MinValue || endTime <= TimeSpan.Zero)
            {
                EndDateError.Text = "Please enter a end date and time";
                isValid = false;
            }
            if (startDate > endDate)
            {
                StartDateError.Text = "Start date cannot exceed end date, please check the input fields";
                isValid = false;
            }
            if (endDate < startDate)
            {
                EndDateError.Text = "End date cannot take place before the start";
                isValid = false;
            }

            if (this.productOfInterest == null)
            {
                ProductOfInterestError.Text = "Please select a product of interest (like a product where something broke or requires maintenance)";
                isValid = false;
            }

            if (selectedCompartments.Count < 1)
            {
                CompartmentsError.Text = "Please select one or more parts";
                isValid = false;
            }

            if (description.Length < 1)
            {
                DescriptionError.Text = "Please enter a description (provide more information about the visit, possibly some important notes)";
                isValid = false;
            }

            if (status > 3 || status < 0)
            {
                StatusError.Text = "Select a valid status.";
                isValid = false;
            }

            //if (selectedEmployees.Count < 1) {
            //    EmployeeError.Text = "Select one or more employees.";
            //    return;
            //}
            if (FEmployee.SelectedItem == null)
            {
                EmployeeError.Text = "Please select a user";
                isValid = false;
            }

            if (title.Length < 1)
            {
                TitleError.Text = "Please enter a description";
                isValid = false;
            }

            if (!isValid)
            {
                FormErrorBox.Text = "One or more fields are not filled in properly. Please make sure everything is correct.";
                return;
            }
            User attachedUser = FEmployee.SelectedItem as User;

            using (AppDbContext dbContext = new AppDbContext())
            {
                MaintenanceAppointment newApp = new();

                //newApp.User = null;
                //newApp.Products = null;
                //newApp.Company = null;
                //newApp.Product = null;
                dbContext.Attach(customerCompany);
                dbContext.Attach(attachedUser);
                dbContext.Attach(productOfInterest);
                foreach (Product product in selectedCompartments)
                {
                    dbContext.Attach(product);
                }

                newApp.Product = this.productOfInterest;
                newApp.User = attachedUser;
                newApp.Company = customerCompany;
                newApp.Description = description;
                newApp.Status = status;
                newApp.DateAdded = DateTime.Now;
                newApp.StartTime = startDate.UtcDateTime;
                newApp.EndTime = endDate.UtcDateTime;
                newApp.Products = selectedCompartments;
                newApp.Title = title;
                dbContext.MaintenanceAppointments.Add(newApp);
                //newApp.User = attachedUser;
                //newApp.Products = selectedCompartments.ToArray();
                Debug.WriteLine(newApp.ToString());
                dbContext.SaveChanges();

            }
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
            else
            {
                Debug.WriteLine("VisitCreate: No compartment detected in SelectionChanged: " + FCompartments.SelectedItem);
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

        private void TimeTestButton_Click(object sender, RoutedEventArgs e)
        {
            StartDateError.Text = FVisitStartTime.Time.ToString();
        }

        private void FVisitStart_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            FVisitEnd.SelectedDateChanged -= FVisitEnd_SelectedDateChanged;
            FVisitEnd.Date = FVisitStart.Date;
            FVisitEnd.SelectedDateChanged += FVisitEnd_SelectedDateChanged;
        }

        private void FVisitStartTime_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            FVisitEndTime.SelectedTimeChanged -= FVisitEndTime_SelectedTimeChanged;
            FVisitEndTime.Time = FVisitStartTime.Time.Add(new TimeSpan(0, 30, 0));
            FVisitEndTime.SelectedTimeChanged += FVisitEndTime_SelectedTimeChanged;

        }

        private void FVisitEnd_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            FVisitStart.SelectedDateChanged -= FVisitStart_SelectedDateChanged;
            FVisitStart.Date = FVisitEnd.Date;
            FVisitStart.SelectedDateChanged += FVisitStart_SelectedDateChanged;
        }

        private void FVisitEndTime_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            FVisitStartTime.SelectedTimeChanged -= FVisitStartTime_SelectedTimeChanged;
            FVisitStartTime.Time = FVisitEndTime.Time.Subtract(new TimeSpan(0, 30, 0));
            FVisitStartTime.SelectedTimeChanged += FVisitStartTime_SelectedTimeChanged;
        }

        private void NavToOverview_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VisitOverview));
        }

        // Remove employee from ComboBox source and display elsewhere in UI
        //private void FEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // Check if there's a valid selected item before proceeding
        //    if (FEmployee.SelectedItem is User selectedEmployee)
        //    {
        //        // Remove the selected employee from maintenanceEmployees
        //        maintenanceEmployees.Remove(selectedEmployee);

        //        // Add the selected employee to selectedEmployees
        //        selectedEmployees.Add(selectedEmployee);

        //        // Clear the selection to avoid re-triggering the selection change
        //        FEmployee.SelectedItem = null;
        //    }

        //}

        // Removes employee from selectedEmployees and move it back to ComboBox
        //private void FSelectedEmployees_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    User selectedUser = (User)e.ClickedItem;
        //    selectedEmployees.Remove(selectedUser);
        //    maintenanceEmployees.Add(selectedUser);
        //}
    }
}
