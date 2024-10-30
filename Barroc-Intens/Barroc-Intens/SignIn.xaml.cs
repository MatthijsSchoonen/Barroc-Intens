using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
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
using Microsoft.UI.Composition;
using System.Numerics;
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
                user = dbContext.Users.FirstOrDefault(u => u.Name.ToLower() == username.ToLower());
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

        //private void SignIn_Loaded(object sender, RoutedEventArgs e)
        //{
        //    AddDropShadow();
        //}

        //private void AddDropShadow()
        //{
        //    var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        //    var dropShadow = compositor.CreateDropShadow();
        //    dropShadow.Mask = null;
        //    dropShadow.Offset = new Vector3(90, 90, 0);
        //    dropShadow.BlurRadius = 10.0f;
        //    dropShadow.Color = Microsoft.UI.Colors.Black;

        //    var shadowVisual = compositor.CreateSpriteVisual();
        //    shadowVisual.Shadow = dropShadow;
        //    shadowVisual.Size = new Vector2((float)spSignInPanel.ActualWidth, (float)spSignInPanel.ActualHeight);

        //    ElementCompositionPreview.SetElementChildVisual(spSignInPanel, shadowVisual);
        //}

    }
}
