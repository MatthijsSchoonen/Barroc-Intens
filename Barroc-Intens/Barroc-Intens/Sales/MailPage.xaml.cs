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

        private void LoadCompanies()
        {
            // Fetch companies from the database
            var companies = _dbContext.Companies.ToList();

            // Bind company names to the ComboBox
            CompaniesComboBox.ItemsSource = companies;
            CompaniesComboBox.DisplayMemberPath = "Name"; // Show company names
            CompaniesComboBox.SelectedValuePath = "Id";  // Use company ID as the value
        }

        private void OnSelectButtonClick(object sender, RoutedEventArgs e)
        {
            // Get the selected company
            var selectedCompany = CompaniesComboBox.SelectedItem as Company;
            if (selectedCompany != null)
            {
                // Fetch contracts and include ContractProducts and Products
                var contracts = _dbContext.Contracts
                                          .Where(c => c.CompanyId == selectedCompany.Id)
                                          .Select(c => new
                                          {
                                              c.Id,
                                              c.StartDate,
                                              c.EndDate,
                                              ContractProducts = c.ContractProducts.Select(cp => new
                                              {
                                                  cp.Product.Name
                                              }).ToList()
                                          })
                                          .ToList();

                // Bind contracts to the ListView
                ContractsListView.ItemsSource = contracts;
            }
            else
            {
                // Handle no selection
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please select a company before proceeding.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                _ = dialog.ShowAsync();
            }
        }
        private async void OnSendMailClick(object sender, RoutedEventArgs e)
        {
            var emailAddress = EmailTextBox.Text;

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please enter a valid email address.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
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
                ContentDialog successDialog = new ContentDialog
                {
                    Title = "Email Sent",
                    Content = "The email with the contract details has been sent successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to send email. Error: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
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
            }
        }

        private void GenerateContractPdf(string filePath)
        {
            using (var writer = new PdfWriter(filePath))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
                // Add a title
                document.Add(new Paragraph("Contract Details")
                    .SetFontSize(18));

                // Get the selected contract from the ListView
                var selectedContract = ContractsListView.SelectedItem;
                if (selectedContract == null)
                {
                    throw new InvalidOperationException("No contract selected.");
                }

                dynamic contract = selectedContract; // Use dynamic for anonymous type

                // Add contract details to the PDF
                document.Add(new Paragraph($"Contract ID: {contract.Id}")
                    .SetFontSize(14));
                document.Add(new Paragraph($"Start Date: {contract.StartDate}"));
                document.Add(new Paragraph($"End Date: {contract.EndDate}"));
                document.Add(new Paragraph("Products:"));

                // Add products associated with the contract
                foreach (var product in contract.ContractProducts)
                {
                    document.Add(new Paragraph($"- {product.Name}"));
                }
            }
        }
    }



}

