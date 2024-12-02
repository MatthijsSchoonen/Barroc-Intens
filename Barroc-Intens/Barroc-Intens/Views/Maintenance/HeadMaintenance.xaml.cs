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
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Dashboards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HeadMaintenance : Page
    {
        public HeadMaintenance()
        {
            this.InitializeComponent();
            LoadMaintenanceList();
        }

        private void LoadMaintenanceList()
        {
            using (var context = new AppDbContext())
            {
                var workOrders = context.MaintenanceAppointments.Include(w => w.Company).ToList();

                MaintenanceList.ItemsSource = workOrders;
            }
        }
    }

}