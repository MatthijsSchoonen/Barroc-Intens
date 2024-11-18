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
        internal DayOfWeek DayOfTheWeek {  get; set; }
        internal int Month { get; set; }
        internal int Year { get; set; }
        internal ObservableCollection<MaintenanceAppointment> Appointments { get; set; }
    }
}
