using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class Note
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; } //do not forget
        public int UserId { get; set; }
        public string Content {  get; set; }
        public DateTime Date { get; set; }
      
    }
}
