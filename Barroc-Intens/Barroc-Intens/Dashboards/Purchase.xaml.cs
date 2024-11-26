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
using Barroc_Intens;
using Barroc_Intens.Data;
using Microsoft.EntityFrameworkCore;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Dashboards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
      public sealed partial class Purchase : Page
        {
            public Purchase()
            {
                this.InitializeComponent();
                LoadMaintenanceList();
            }

        private void LoadMaintenanceList()
        {
            using (var context = new AppDbContext())
            {
                // Haal alleen afspraken op waar een gebruiker aan is gekoppeld
                var workOrders = context.MaintenanceAppointments
                    .Include(w => w.Company)
                    .Include(w => w.User) // Voeg deze relatie toe als de gebruiker nodig is
                    .Where(w => w.UserId != null) // Filter voor niet-lege gebruikers
                    .ToList();

                PurchaseMessages.ItemsSource = workOrders;
            }
        }
    }
}
