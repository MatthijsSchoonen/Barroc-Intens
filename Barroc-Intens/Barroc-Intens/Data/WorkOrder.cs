using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    internal class WorkOrder
    {
        public int Id { get; set; }
        public int MaintenanceAppointmentId { get; set; }
        public MaintenanceAppointment MaintenanceAppointment { get; set; }
    }
}
