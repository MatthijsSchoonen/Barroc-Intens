using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Barroc_Intens.Data;
using Microsoft.EntityFrameworkCore;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace Barroc_Intens
{
    public partial class WorkOrder : Page
    {
        public WorkOrder()
        {
            InitializeComponent();
            LoadWorkOrders();
        }

        private void LoadWorkOrders()
        {
            using (var context = new AppDbContext())
            {
                var workOrders = context.MaintenanceAppointments.Include(w => w.Company).ToList();

                WorkOrdersListView.ItemsSource = workOrders;
            }
        }
    }
}