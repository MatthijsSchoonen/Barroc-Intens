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

namespace Barroc_Intens
{

    public sealed partial class SignIn : UserControl
    {
        internal event EventHandler<User> LoginSuccessful;

        public SignIn()
        {
            this.InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            User user = new();
            using(AppDbContext dbContext = new())
            {
                user = dbContext.Users.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());
            }

            if (user != null) {
                if (SecureHasher.Verify(password, user.Password))
                {
                    LoginSuccessful?.Invoke(this, user);
                }
                ErrorMessage.Text = "Incorrect password";
                return;
            }
            ErrorMessage.Text = "User not found";
        }
    }
}
