using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
namespace Barroc_Intens.Views.Maintenance
{
    public sealed partial class VisitDetails : Page
    {
        public MaintenanceAppointment SelectedAppointment { get; private set; }

        public VisitDetails()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SelectedAppointment = e.Parameter as MaintenanceAppointment;

            if (SelectedAppointment == null)
            {
                DetailsPanel.Visibility = Visibility.Collapsed;
                NotAvailableNotif.Visibility = Visibility.Visible;
            }
        }

        private void BtnOverviewNav_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VisitOverview));
        }
    }
}
