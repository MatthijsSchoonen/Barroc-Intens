using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using Barroc_Intens.Data;
using System.Net.Mime;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.IO;
using MimeKit;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;

namespace Barroc_Intens.Sales
{
    public sealed partial class MailPage : Page
    {
        private readonly AppDbContext _dbContext;

        public MailPage()
        {
            this.InitializeComponent();

            // Initialize database context
            _dbContext = new AppDbContext();

            // Load companies from the database
            LoadCompanies();
        }

        // --- Company Loading Section ---
        private void LoadCompanies()
        {
            // Fetch companies from the database
            var companies = _dbContext.Companies.ToList();

            // Bind company names to the ComboBox
            CompaniesComboBox.ItemsSource = companies;
            CompaniesComboBox.DisplayMemberPath = "Name"; // Show company names
            CompaniesComboBox.SelectedValuePath = "Id";  // Use company ID as the value
        }

        // --- Contract Selection Section ---
        private void OnSelectButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedCompany = CompaniesComboBox.SelectedItem as Company;

            if (selectedCompany != null)
            {
                try
                {
                    // Fetch contracts and include related ContractProducts and Products
                    var contracts = _dbContext.Contracts
                        .Where(c => c.CompanyId == selectedCompany.Id)
                        .Include(c => c.ContractProducts) // Load related data
                        .ThenInclude(cp => cp.Product)
                        .ToList();

                    // Bind to ListView
                    ContractsListView.ItemsSource = contracts;
                }
                catch (Exception ex)
                {
                    ShowErrorDialog("Error", $"An error occurred: {ex.Message}");
                }
            }
            else
            {
                ShowErrorDialog("Error", "Please select a company before proceeding.");
            }
        }

        // --- Email Sending Section ---
        private async void OnSendMailClick(object sender, RoutedEventArgs e)
        {
            var emailAddress = EmailTextBox.Text;

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                await ShowDialog("Error", "Please enter a valid email address.");
                return;
            }

            try
            {
                // Generate PDF file
                string pdfFilePath = Path.Combine(Path.GetTempPath(), "ContractDetails.pdf");
                GenerateContractPdf(pdfFilePath);

                // Send the email with the generated PDF
                await SendEmailAsync(emailAddress, pdfFilePath);

                // Clean up the temporary PDF file
                File.Delete(pdfFilePath);

                // Show success dialog
                await ShowDialog("Email Sent", "The email with the contract details has been sent successfully.");
            }
            catch (Exception ex)
            {
                await ShowDialog("Error", $"Failed to send email. Error: {ex.Message}");
            }
        }

        private async Task SendEmailAsync(string recipientEmail, string pdfFilePath)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Barroc Intens", "officialbarrocintens@gmail.com"));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = "Contract Information";

            // Build email body
            var bodyBuilder = new BodyBuilder
            {
                TextBody = "Please find attached the contract details."
            };

            // Attach PDF file
            bodyBuilder.Attachments.Add(pdfFilePath);

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

        // --- PDF Generation Section ---
        private void GenerateContractPdf(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath); // Ensure no existing file conflicts
            }

            try
            {
                using (var writer = new PdfWriter(filePath))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf))
                {
                    // Add a title
                    document.Add(new Paragraph("Contract Details").SetFontSize(18));

                    // Get the selected contract
                    var selectedContract = ContractsListView.SelectedItem;
                    if (selectedContract == null)
                    {
                        throw new InvalidOperationException("No contract selected.");
                    }

                    dynamic contract = selectedContract; // Anonymous type
                    document.Add(new Paragraph($"Contract ID: {contract.Id}").SetFontSize(14));
                    document.Add(new Paragraph($"Start Date: {contract.StartDate}"));
                    document.Add(new Paragraph($"End Date: {contract.EndDate}"));
                    document.Add(new Paragraph("Products:"));

                    foreach (var product in contract.ContractProducts)
                    {
                        document.Add(new Paragraph($"- {product.Product.Name}")); // Access Product.Name correctly
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                throw;
            }
        }

        // --- Helper Methods ---
        private async Task ShowDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private void ShowErrorDialog(string title, string content)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            _ = errorDialog.ShowAsync();
        }
    }
}
