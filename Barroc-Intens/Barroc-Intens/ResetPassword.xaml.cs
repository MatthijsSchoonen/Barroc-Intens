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
using Barroc_Intens.Data;
using Microsoft.UI.Xaml.Documents;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResetPassword : Page
    {
        private User currentUser; 

        public ResetPassword()
        {
            this.InitializeComponent();
        }

        private void CheckCredentials(object sender, RoutedEventArgs e)
        {
            if (email.Text == "" || resetCode.Text == "")
            {
                ErrorMessage.Text = "Please fill in all input fields";
                return;
            }

            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // Retrieve the user with the provided email
                    currentUser = db.Users.FirstOrDefault(u => u.Email == email.Text);

                    if (currentUser == null)
                    {
                        ErrorMessage.Text = "No user found with this email";
                        return;
                    }

                    // Check if there is a PasswordReset row with matching UserId and Code
                    var passwordResetEntry = db.PasswordResets.FirstOrDefault(pr => pr.UserId == currentUser.Id && pr.Code == resetCode.Text);

                    if (passwordResetEntry != null)
                    {

                        db.PasswordResets.Remove(passwordResetEntry);
                        db.SaveChanges();
                        ErrorMessage.Text = "";


                        userName.Text = currentUser.Name;
                    
                        resetCodeBlock.Visibility = Visibility.Collapsed;
                        ResetPasswordPanel.Visibility = Visibility.Visible;
                        email.Text = "";
                        resetCode.Text = "";

                    }
                    else
                    {
                        ErrorMessage.Text = "Invalid reset code.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"An error occurred: {ex.Message}";
            }
        }


     
           
        private void UpdatePass(object sender, RoutedEventArgs e)
        {
            if (newPassword.Text == "" || confirmPassword.Text == "")
            {
                ErrorMessage2.Text = "Please fill in all input fields";
                return;
            }

            if (newPassword.Text != confirmPassword.Text)
            {
                ErrorMessage2.Text = "Passwords do not match.";
                return;
            }

            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // Retrieve the current user from the database
                    var userToUpdate = db.Users.FirstOrDefault(u => u.Id == currentUser.Id);

                    if (userToUpdate != null)
                    {
                        // Update the user's password
                        userToUpdate.Password =  SecureHasher.Hash(newPassword.Text); 
                        db.SaveChanges();

                        ErrorMessage2.Text = "";
                        ErrorMessage.Text = "password updated";

                        resetCodeBlock.Visibility = Visibility.Visible;
                        ResetPasswordPanel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ErrorMessage2.Text = "User not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"An error occurred: {ex.Message}";
            }
        }
    }

}
