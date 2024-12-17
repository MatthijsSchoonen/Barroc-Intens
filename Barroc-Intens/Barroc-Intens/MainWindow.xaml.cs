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
using Barroc_Intens.Dashboards;
using Barroc_Intens.PurchaseViews;
using Barroc_Intens.Sales;
using Barroc_Intens.Maintenance;
using System.Diagnostics;
using Barroc_Intens.Views.Sales;
using Barroc_Intens.Views.PurchaseViews;
using Barroc_Intens.Customer;
using Barroc_Intens.Views.Maintenance;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private bool isLoggedIn = false;
        private User loggedInUser;

        public MainWindow()
        {
            this.InitializeComponent();
            // Initialize Database (remove and add again withcvurrent setup, acceptable during development)
            using AppDbContext db = new AppDbContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            ShowLoginPage();
            LoadNav();
        }

        private void LoadNav()
        {
            // Reset visibility for all links
            PurchaseLinks.Visibility = Visibility.Collapsed;
            MaintenanceLinks.Visibility = Visibility.Collapsed;
            SalesLinks.Visibility = Visibility.Collapsed;
            FinanceLinks.Visibility = Visibility.Collapsed;
            SalesMailLink.Visibility = Visibility.Collapsed;
            FinanceWorkOrderLinks.Visibility = Visibility.Collapsed;
            NewCustomerLink.Visibility = Visibility.Collapsed;
            resetPassLink.Visibility = Visibility.Collapsed;
            NavItemBackToDashboard.Visibility = Visibility.Collapsed;
            AddCompanyLink.Visibility = Visibility.Collapsed;
            PurchasesLinks.Visibility = Visibility.Collapsed;
        
                CustomerStockViewLink.Visibility = Visibility.Visible;
            


            if (loggedInUser == null)
            {
                // Show login and hide logout if no user is logged in
                LoginLinks.Visibility = Visibility.Visible;
                LogoutLinks.Visibility = Visibility.Collapsed;
                resetPassLink.Visibility = Visibility.Visible;
                return;
            }

            // Hide login and show logout when a user is logged in
            LoginLinks.Visibility = Visibility.Collapsed;
            LogoutLinks.Visibility = Visibility.Visible;
            resetPassLink.Visibility = Visibility.Collapsed;
            NavItemBackToDashboard.Visibility = Visibility.Visible;

            // Show specific navigation items based on department type
            switch (loggedInUser.Department.Type)
            {
                case "Purchase":
                    PurchaseLinks.Visibility = Visibility.Visible;
                    PurchasesLinks.Visibility = Visibility.Visible;
                    break;
                case "HeadPurchase":
                    PurchaseLinks.Visibility = Visibility.Visible;
                    PurchasesLinks.Visibility = Visibility.Visible;
                    break;
                case "Maintenance":
                    MaintenanceLinks.Visibility = Visibility.Visible;
                    MaintenanceLink.Visibility = Visibility.Visible;
                    break;
                case "Sales":
                    SalesLinks.Visibility = Visibility.Visible;
                    SalesMailLink.Visibility = Visibility.Visible;
                    NewCustomerLink.Visibility = Visibility.Visible;
                    AddCompanyLink.Visibility= Visibility.Visible;
                    break;
                case "Finance":
                    FinanceLinks.Visibility = Visibility.Visible;
                    FinanceWorkOrderLinks.Visibility= Visibility.Visible;
                    break;
            }
        }


        private void ShowLoginPage()
        {
            nvMainNavBar.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
            SignIn signInPage = new();
            signInPage.LoginSuccessful += OnLoginSuccessful; // Bind event handler of SignIn to method.
            contentFrame.Content = signInPage;

        }

        // Things need to be done once user is logged in.
        private void OnLoginSuccessful(object sender, User user)
        {
            isLoggedIn = true;
            contentFrame.Content = null; // Clear login page
            nvMainNavBar.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
            Department userDepartment = user.Department;
            loggedInUser = user;
            NavItemBackToDashboard.Tag = $"Barroc_Intens.Dashboards.{loggedInUser.Department.Type}";
            SwitchPage(user.Department.Type, "Barroc_Intens.Dashboards");
        }

        // Gets executed once the selection is changed in the navmenu. 
        private void nvMainNavBar_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
         {
       
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                string searchTerm = selectedItem.Tag.ToString();
                SwitchPage("","",searchTerm);
                return;
            
            
        }

        // Navigation between pages.
        private void SwitchPage(string pageName="NotFound", string nameSpace = "Barroc_Intens",string completeTerm = "None")
        {
            LoadNav();
            Type pageType;

           
            if (completeTerm != "None") {
                pageType = Type.GetType(completeTerm);
            }
            else
            {
                pageType = Type.GetType($"{nameSpace}.{pageName}");
            }

            if (completeTerm == "CustomerStockView")
            {
                contentFrame.Navigate(typeof(CustomerStockView));
                return;
            }

            if (pageType != null)
            {
                contentFrame.Navigate(pageType);
                return;
            }
            if(completeTerm == "Barroc_Intens.Dashboards.Sales"  || pageName == "Sales")
            {
                contentFrame.Navigate(typeof(SalesDash));
                return;
            }
            if (completeTerm == "NewCustomer")
            {
                contentFrame.Navigate(typeof(NewCustomerPage));
                return;
            }
            if (completeTerm == "AddCompany")
            {
                ShowAddCompany();
                return;
            }
            if (completeTerm == "resetPassword")
            {
                contentFrame.Navigate(typeof(ResetPassword));
                return;
            }
            if(completeTerm == "StockView")
            {
                ShowStockView();
                return;
            }
            if(completeTerm == "NotePage")
            {
                ShowNoteView();
                return;
            }
         
            if (completeTerm == "Invoice")
            {
                contentFrame.Navigate(typeof(InvoicePage));
                return;
            }
            if (completeTerm == "WorkOrderPage")
            {
                contentFrame.Navigate(typeof(WorkOrderPage));
                return;
            }

            if (completeTerm == "Mail")
            {
                contentFrame.Navigate(typeof(MailPage));
                return;
            }
            if (completeTerm == "login")
            {
                ShowLoginPage();
                return;
            }

            if ( completeTerm == "VisitCreate")
            {
                //Debug.WriteLine("Logged in user's name: "+loggedInUser.Name);
                //Debug.WriteLine("ContentFrame: "+ contentFrame);
                //Debug.WriteLine("Result of TypeOf VisitCreate: " + typeof(VisitCreate));
                contentFrame.Navigate(typeof(VisitCreate), this.loggedInUser);
                return; 
            }
            if(completeTerm == "logout")
            {
                contentFrame.Navigate(typeof(SignIn));
                nvMainNavBar.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
                this.loggedInUser = null;
                this.isLoggedIn = false;
                LoadNav();
                ShowLoginPage();
                return;
            }
            
            if(completeTerm == "Purchases")
            {
                contentFrame.Navigate(typeof(PurchasesView));
                return;
            }

            NotFound notFoundPage = new();
            contentFrame.Content = notFoundPage;
            return;
        }
        public void ShowAddCompany()
        {
            contentFrame.Navigate(typeof(AddCompany), this);
        }
        public void ShowStockView()
        {
            contentFrame.Navigate(typeof(StockView));
        }

        public void ShowNoteView()
        {
            contentFrame.Navigate(typeof(NotePage));
        }
    }
}