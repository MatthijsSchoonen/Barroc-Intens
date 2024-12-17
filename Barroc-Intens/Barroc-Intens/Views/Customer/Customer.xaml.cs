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
using Barroc_Intens.Data;
using System.Diagnostics.Contracts;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Dashboards
{
    public partial class Customer : Page
    {
        public Customer()
        {
            InitializeComponent();
            LoadContracts();
        }

        // Context voor Pomelo
        private AppDbContext _context;

        public void LoadContracts()
        {
            using (var context = new AppDbContext())
            {
                var leaseContracts = context.Contracts.Include(w => w.Company).ToList();

                ContractListBox.ItemsSource = leaseContracts;
            }
        }


        private void OnContractSelected(object sender, RoutedEventArgs e)
        {
            if (ContractListBox.SelectedItem != null)
            {
                int contractId = (int)ContractListBox.SelectedItem;

                
                var contract = _context.Contracts
                    .Include(c => c.Company)
                    .Include(c => c.Products)
                    .FirstOrDefault(c => c.Id == contractId);

            
            }
        }
    }
}

