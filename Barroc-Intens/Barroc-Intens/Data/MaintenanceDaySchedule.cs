using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    internal class MaintenanceDaySchedule
    {
        internal string DayOfTheWeek {  get; set; }
        internal string Month { get; set; }
        internal string Year { get; set; }
        internal ObservableCollection<MaintenanceAppointment> Appointments { get; set; }
    }
}
