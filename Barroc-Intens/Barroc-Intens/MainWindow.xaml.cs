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

namespace Barroc_Intens
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private bool isLoggedIn = false;
        private string loggedInUser;

        public MainWindow()
        {
            this.InitializeComponent();
            using (AppDbContext dbContext = new())
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }

            // Show login page initially
            ShowLoginPage();
        }

        private void ShowLoginPage()
        {
            var signInPage = new SignIn();
            signInPage.LoginSuccessful += OnLoginSuccessful;
            contentFrame.Content = signInPage;
        }

        private void OnLoginSuccessful(object sender, LoginData e)
        {
            isLoggedIn = true;
            loggedInUser = e.Username;
            // Use loggedInUser as needed
            TbNavDebug.Text = loggedInUser;
            contentFrame.Content = null; // Clear login page
        }

        private void NvSample_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (isLoggedIn)
            {
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                string pageName = selectedItem.Tag.ToString();
                Type pageType = Type.GetType($"Barroc_Intens.{pageName}");

                if (pageType != null)
                {
                    contentFrame.Navigate(pageType);
                }
            }
        }
    }
}