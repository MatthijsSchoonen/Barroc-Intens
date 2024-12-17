using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace Barroc_Intens.Views.Maintenance
{
    public sealed partial class VisitDetails : Page
    {
        public MaintenanceAppointment SelectedAppointment { get; private set; }
        public Data.WorkOrder SelectedWorkOrder { get; set; }


        public VisitDetails()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SelectedAppointment = e.Parameter as MaintenanceAppointment;

            if (SelectedAppointment == null)
            {
                DetailsPanel.Visibility = Visibility.Collapsed;
                NotAvailableNotif.Visibility = Visibility.Visible;
            }
            else
            {
                using (AppDbContext dbContext = new AppDbContext()) {
                    // Query the WorkOrder associated with the selected appointment
                    Data.WorkOrder workOrder = dbContext.WorkOrders
                        .Where(w => w.MaintenanceAppointmentId == SelectedAppointment.Id)
                        .Include(w => w.WorkOrderMats)
                            .ThenInclude(wm => wm.Product) // Include related products, if necessary
                        .FirstOrDefault();

                    if (workOrder != null)
                    {
                        // Set the WorkOrder for binding in XAML
                        SelectedWorkOrder = workOrder;
                        DetailsPanel.Visibility = Visibility.Visible;
                        NotAvailableNotif.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        DetailsPanel.Visibility = Visibility.Collapsed;
                        NotAvailableNotif.Visibility = Visibility.Visible;
                    }
                }
   
            }
        }


        private void BtnOverviewNav_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VisitOverview));
        }
    }
}
