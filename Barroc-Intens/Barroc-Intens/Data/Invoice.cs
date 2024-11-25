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
        public DateOnly Date { get; set; }
        public DateOnly? PaidAt { get; set; }
        public int ContractId { get; set; }
        public Contract Contract { get; set; }

        public int VAT { get; set; }

        public decimal ConnectionCost { get; set; } 
    }
}
