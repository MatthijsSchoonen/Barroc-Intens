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

    public class LoginData
    {
        public string Username { get; set; }
        // Add more properties as needed
    }

    public sealed partial class SignIn : UserControl
    {
        public event EventHandler<LoginData> LoginSuccessful;

        public SignIn()
        {
            this.InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (username == "admin" && password == "password") // Example validation
            {
                User user = new();
                using(AppDbContext dbContext = new())
                {

                }
                var loginData = new LoginData
                {
                    Username = username,
                    // Add more data as needed
                };
                LoginSuccessful?.Invoke(this, loginData); // Raise event with data
            }
            else
            {
                ErrorMessage.Text = "Invalid credentials.";
            }
        }
    }
}
