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
            if (isLoggedIn)
            {
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                string searchTerm = selectedItem.Tag.ToString();
                SwitchPage("","",searchTerm);
                return;
            }
            ShowLoginPage();
        }

        // Navigation between pages.
        private void SwitchPage(string pageName="NotFound", string nameSpace = "Barroc_Intens",string completeTerm = "None")
        {
            
            Type pageType;
            if (completeTerm != "None") {
                pageType = Type.GetType(completeTerm);
            }
            else
            {
                pageType = Type.GetType($"{nameSpace}.{pageName}");
            }
            

            if (pageType != null)
            {
                contentFrame.Navigate(pageType);
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
                contentFrame.Navigate(typeof(Invoice));
                return;
            }
            if (completeTerm == "WorkOrder")
            {
                contentFrame.Navigate(typeof(WorkOrder));
                return;
            }

            if (completeTerm == "Mail")
            {
                contentFrame.Navigate(typeof(MailPage));
                return;
            }
            if (completeTerm == "logout")
            {
                contentFrame.Navigate(typeof(SignIn));
                nvMainNavBar.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftMinimal;
                this.loggedInUser = null;
                this.isLoggedIn = false;
                ShowLoginPage();
                return;
            }

            NotFound notFoundPage = new();
            contentFrame.Content = notFoundPage;
            return;
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