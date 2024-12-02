using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class Invoice
    {
        public int Id { get; set; }
        public int? ContractId { get; set; }
        public int? CompanyId { get; set;}
        public Company Company { get; set; }
        public Contract Contract { get; set; }

        public decimal ConnectionCost { get; set; } // Add if missing
        public int VAT { get; set; } // Add if missing
        public DateTime StartDate { get; set; } // Add Start Date
        public DateTime EndDate { get; set; } // Add End Date
    }

}
