using Barroc_Intens.Dashboards;
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Maintenance
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewWorkOrderWindow : Window
    {
        private MaintenanceAppointment _currentAppointment;
        private ObservableCollection<Product> AllProducts = new ObservableCollection<Product>();
        private ObservableCollection<Product> AddedProducts = new ObservableCollection<Product>();
        private List<Product> Products = new List<Product>();
        public NewWorkOrderWindow(MaintenanceAppointment currentAppointment)
        {
            this.InitializeComponent();
            AppDbContext db = new AppDbContext();
            _currentAppointment = currentAppointment;

            Products = db.Products.ToList();
            foreach (Product product in Products)
            {
                if (product.ProductsCategoryId == 3)
                {
                    AllProducts.Add(product);
                }
            }

            addProductListView.ItemsSource = AllProducts;
            removeProductListView.ItemsSource = AddedProducts;
            Company company = db.Companies.FirstOrDefault(c => c.Id == _currentAppointment.CompanyId);

            nameCompanyInput.Text = company.Name;
            nameMechanicInput.Text = User.LoggedInUser.Name;
            descriptionInput.Text = currentAppointment.Description;
        }

        // Add a product to the work order
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            Product product = ((Button)sender).Tag as Product;
            Product productToRemove = Products.FirstOrDefault(p => p.Id == product.Id);
            if (productToRemove != null)
            {
                AllProducts.Remove(productToRemove);
            }
            AddedProducts.Add(product);
        }

        // Remove a product from the list of added products
        private void RemoveProductButton_Click(object sender, RoutedEventArgs e)
        {
            Product product = ((Button)sender).Tag as Product;
            Product productToRemove = Products.FirstOrDefault(p => p.Id == product.Id);
            if (productToRemove != null)
            {
                AddedProducts.Remove(productToRemove);
            }
            AllProducts.Add(product);
        }

        // Submit the work order and save it in the database
        private void SubmitWorkOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameMechanicInput.Text == "")
            {
                ErrorMessage.Text = "Mechanic name is required!";
            }
            if (nameCompanyInput.Text == "")
            {
                ErrorMessage.Text = "Company name is required!";
            }
            if (descriptionInput.Text == "")
            {
                ErrorMessage.Text = "Description is required!";
            }
            AppDbContext db = new AppDbContext();
            MaintenanceAppointment currentAppointment = db.MaintenanceAppointments.FirstOrDefault(m => m.Id == _currentAppointment.Id);
            currentAppointment.Status = 99;
            currentAppointment.EndTime = DateTime.Now;
            foreach (Product product in AddedProducts)
            {
                db.MaintenanceAppointmentProducts.Add(new MaintenanceAppointmentProduct
                {
                    MaintenanceAppointmentId = currentAppointment.Id,
                    ProductId = product.Id,
                });
            }
            db.SaveChanges();
            this.Close();
        }
    }
}

