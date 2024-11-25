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

namespace Barroc_Intens.Maintenance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllMaintenanceAppointmentPage : Page
    {
        MainWindow mainWindow;
        public AllMaintenanceAppointmentPage()
        {
            this.InitializeComponent();
            AppDbContext db = new AppDbContext();
            maintenanceAppointmentListView.ItemsSource = db.MaintenanceAppointments.ToList();
            
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainWindow mainWindow = e.Parameter as MainWindow;
            //base.OnNavigatedTo(e);
            if (mainWindow != null)
            {
                this.mainWindow = mainWindow;
            }
        }

        private void maintenanceAppointmentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MaintenanceAppointment maintenanceAppointment = (MaintenanceAppointment)e.ClickedItem;
            Frame.Navigate(typeof(MaintenanceAppointment), maintenanceAppointment);
        }
    }
}
