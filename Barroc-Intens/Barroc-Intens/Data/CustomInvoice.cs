using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    internal class CustomInvoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime PaidAt { get; set; }
        public int CompanyId { get; set; }
    }
}
