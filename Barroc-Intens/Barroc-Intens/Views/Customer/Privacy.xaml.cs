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

namespace Barroc_Intens.Views.Customer
{
    public sealed partial class Privacy : Page
    {
        public Privacy()
        {
            this.InitializeComponent();
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            var currentUser = User.LoggedInUser;

            if (currentUser != null)
            {
                var userInfo = new List<User> { currentUser };
                UserInfoListView.ItemsSource = userInfo;
            }
            else
            {
                UserInfoListView.ItemsSource = null;
            }
        }

    }
}
