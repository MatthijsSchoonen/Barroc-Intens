using Barroc_Intens.Dashboards;
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
using System.Net.Mime;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using MimeKit;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using System.Text;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Sales
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewCustomerPage : Page
    {
        public NewCustomerPage()
        {
            this.InitializeComponent();
            LoadCompaniesAsync();
        }

        private async Task LoadCompaniesAsync()
        {
            //connect to the database get the companies
            using (AppDbContext context = new AppDbContext())
            {
                var companies = await context.Companies.ToListAsync();
                CompanyComboBox.ItemsSource = companies;
                CompanyComboBox.DisplayMemberPath = "Name";
                CompanyComboBox.SelectedValuePath = "Id";
            }
        }
        private async  void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameInput.Text == "")
            {
                ErrorMessage.Text = "Name is required";
                return;
            }
            if (emailInput.Text == "")
            {
                ErrorMessage.Text = "Email is required";
                return;
            }

            var selectedCompany = CompanyComboBox.SelectedItem as Company;
            int? companyId = selectedCompany?.Id;

            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // Create a new user
                    User newUser = new User
                    {
                        Name = nameInput.Text,
                        DepartmentId = 1,
                        RoleId = 1,       
                        Email = emailInput.Text,
                        CompanyId = companyId,
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges(); // Save to generate the User ID

                    // Generate a random string for the password reset code
                    string randomCode = RandomString();

                    
                    PasswordReset passwordReset = new PasswordReset
                    {
                        UserId = newUser.Id, // Use the generated User ID
                        Code = randomCode,
                    };

                    db.PasswordResets.Add(passwordReset);                   
                    db.SaveChanges(); // Save the PasswordReset entry

                    await SendEmailAsync(emailInput.Text, randomCode);
                }

                // Navigate to the next page
                Frame.Navigate(typeof(SalesDash));
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"An error occurred: {ex.Message}";
            }
        }


        public string RandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringBuilder = new StringBuilder(10);

            for (int i = 0; i < 10; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }


            return stringBuilder.ToString();
        }

        private async Task SendEmailAsync(string recipientEmail, string ResetCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Barroc Intens", "officialbarrocintens@gmail.com"));
            message.To.Add(new MailboxAddress("", recipientEmail));
            //email Title
            message.Subject = "password reset " + ResetCode ;

            // Build email body
            var bodyBuilder = new BodyBuilder
            {
                TextBody = "Go to the ResetPassword Page to Reset your password\nThis is your password reset code: " + ResetCode
            };

           

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("officialbarrocintens@gmail.com", "ymlf npoq mhoo wiiq");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

       
    }
}