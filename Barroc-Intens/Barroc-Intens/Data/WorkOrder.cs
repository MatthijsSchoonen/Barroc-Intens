using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class WorkOrder
    {
        public int Id { get; set; }
        public int MaintenanceAppointmentId { get; set; }
        public MaintenanceAppointment MaintenanceAppointment { get; set; }
        public List<WorkOrderMat> WorkOrderMats { get; set; } = new List<WorkOrderMat>();

    }
}
