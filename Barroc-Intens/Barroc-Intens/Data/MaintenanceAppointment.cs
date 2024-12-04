using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class MaintenanceAppointment
    {
        public int Id { get; set; }

        public int? UserId { get; set; } = null;
        public User User { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Title { get; set; }

        public ICollection<MaintenanceAppointmentProduct> MaintenanceAppointmentProducts { get; set; }

        // New properties for formatted time strings
        public string StartTimeOnly => StartTime.ToString("HH:mm"); 
        public string EndTimeOnly => EndTime.ToString("HH:mm");
        public string TruncatedDescription => Description.Length > 100 ? Description.Substring(0, 100) + "..." : Description;
    }
}
