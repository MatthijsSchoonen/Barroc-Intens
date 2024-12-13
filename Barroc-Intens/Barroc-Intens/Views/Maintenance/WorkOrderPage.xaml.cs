using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Barroc_Intens.Views.Maintenance
{
    public sealed partial class WorkOrderPage : Page
    {
        private ObservableCollection<Barroc_Intens.Data.WorkOrder> WorkOrderCollection = new();

        public WorkOrderPage()
        {
            this.InitializeComponent();
            using (AppDbContext dbContext = new())
            {
                List<Barroc_Intens.Data.WorkOrder> retrievedWorkOrders = dbContext.WorkOrders.ToList();
                foreach (Barroc_Intens.Data.WorkOrder workOrder in retrievedWorkOrders)
                {
                    WorkOrderCollection.Add(workOrder);
                }
            }
        }

        private void OnAddPanelToggleClick(object sender, RoutedEventArgs e)
        {
            AddPanel.Visibility = Visibility.Visible;
        }
    }
}
