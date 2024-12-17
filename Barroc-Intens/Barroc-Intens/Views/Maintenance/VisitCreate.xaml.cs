
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
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
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
        private async void FOnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous error messages
            ClearErrorMessages();

            // Validate form inputs
            if (!ValidateFormInputs())
            {
                return;
            }

            // Use the selected date and time directly without timezone conversions
            DateTime startDateTime = FVisitStart.Date.DateTime.Date + FVisitStartTime.Time;
            DateTime endDateTime = FVisitEnd.Date.DateTime.Date + FVisitEndTime.Time;

            using (AppDbContext dbContext = new AppDbContext())
            {
                MaintenanceAppointment newApp = new()
                {
                    Title = FTitle.Text,
                    Description = FDescription.Text,
                    StartTime = startDateTime,
                    EndTime = endDateTime,
                    Status = FStatus.SelectedIndex,
                    DateAdded = DateTime.Now, // Use local time for date added
                    User = FEmployee.SelectedItem as User,
                    Company = FCustomer.SelectedItem as Company
                };

                // Attach related entities
                dbContext.Attach(newApp.Company);
                dbContext.Attach(newApp.User);

                // Add selected compartments (products)
                foreach (Product product in selectedCompartments)
                {
                    dbContext.Attach(product);
                }

                // Save to database
                dbContext.MaintenanceAppointments.Add(newApp);
                await dbContext.SaveChangesAsync();

                // Show confirmation dialog
                ContentDialog confirmationDialog = new ContentDialog
                {
                    Title = "Successfully added",
                    Content = "The appointment has been successfully stored into the database. You will be redirected to the overview page.",
                    CloseButtonText = "Ok",
                    XamlRoot = this.Content.XamlRoot
                };
                await confirmationDialog.ShowAsync();

                Frame.Navigate(typeof(VisitOverview));
            }
        }

        private bool ValidateFormInputs()
        {
            bool isValid = true;

            // Customer validation
            if (FCustomer.SelectedItem == null)
            {
                CustomerError.Text = "Please select a customer.";
                isValid = false;
            }

            // Status validation
            if (FStatus.SelectedIndex == -1)
            {
                StatusError.Text = "Please select a status.";
                isValid = false;
            }

            // Date and time validation
            if (!FVisitStart.SelectedDate.HasValue || !FVisitEnd.SelectedDate.HasValue)
            {
                FormErrorBox.Text = "Please select start and end dates.";
                isValid = false;
            }
            else
            {
                DateTime startDateTime = FVisitStart.Date.DateTime.Date + FVisitStartTime.Time;
                DateTime endDateTime = FVisitEnd.Date.DateTime.Date + FVisitEndTime.Time;

                if (startDateTime > endDateTime)
                {
                    StartDateError.Text = "Start date and time cannot exceed end date and time.";
                    EndDateError.Text = "End date and time cannot be before start date and time.";
                    isValid = false;
                }
            }

            // Title validation
            if (string.IsNullOrWhiteSpace(FTitle.Text))
            {
                TitleError.Text = "Please enter a title.";
                isValid = false;
            }

            // Description validation
            if (string.IsNullOrWhiteSpace(FDescription.Text))
            {
                DescriptionError.Text = "Please enter a description.";
                isValid = false;
            }

            // Employee validation
            if (FEmployee.SelectedItem == null)
            {
                EmployeeError.Text = "Please select an employee.";
                isValid = false;
            }

            // Product of interest validation
            if (this.productOfInterest == null)
            {
                ProductOfInterestError.Text = "Please select a product of interest.";
                isValid = false;
            }

            // Compartments validation
            if (selectedCompartments.Count < 1)
            {
                CompartmentsError.Text = "Please select one or more parts.";
                isValid = false;
            }

            // Overall form error message
            if (!isValid)
            {
                FormErrorBox.Text = "One or more fields are not filled in properly. Please check the inputs.";
            }

            return isValid;
        }

        private void ClearErrorMessages()
        {
            CustomerError.Text = string.Empty;
            StatusError.Text = string.Empty;
            FormErrorBox.Text = string.Empty;
            StartDateError.Text = string.Empty;
            EndDateError.Text = string.Empty;
            TitleError.Text = string.Empty;
            DescriptionError.Text = string.Empty;
            EmployeeError.Text = string.Empty;
            ProductOfInterestError.Text = string.Empty;
            CompartmentsError.Text = string.Empty;
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
            if(!FVisitEnd.SelectedDate.HasValue)
            {
                FVisitEnd.SelectedDateChanged -= FVisitEnd_SelectedDateChanged;
                FVisitEnd.Date = FVisitStart.Date;
                FVisitEnd.SelectedDateChanged += FVisitEnd_SelectedDateChanged;
            }
        }

        private void FVisitStartTime_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            if(!FVisitEndTime.SelectedTime.HasValue)
            {
                FVisitEndTime.SelectedTimeChanged -= FVisitEndTime_SelectedTimeChanged;
                FVisitEndTime.Time = FVisitStartTime.Time.Add(new TimeSpan(0, 30, 0));
                FVisitEndTime.SelectedTimeChanged += FVisitEndTime_SelectedTimeChanged;
            }
        }

        private void FVisitEnd_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            if (!FVisitStart.SelectedDate.HasValue) {
                FVisitStart.SelectedDateChanged -= FVisitStart_SelectedDateChanged;
                FVisitStart.Date = FVisitEnd.Date;
                FVisitStart.SelectedDateChanged += FVisitStart_SelectedDateChanged;
            }

        }

        private void FVisitEndTime_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            if (!FVisitStartTime.SelectedTime.HasValue) {
                FVisitStartTime.SelectedTimeChanged -= FVisitStartTime_SelectedTimeChanged;
                FVisitStartTime.Time = FVisitEndTime.Time.Subtract(new TimeSpan(0, 30, 0));
                FVisitStartTime.SelectedTimeChanged += FVisitStartTime_SelectedTimeChanged;
            }
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
