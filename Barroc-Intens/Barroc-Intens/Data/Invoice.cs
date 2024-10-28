using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    internal class Invoice
    {
        public int Id { get; set; }
        public DateOnly Date {  get; set; }
        public DateOnly PaidAt { get; set; }
        public int ContractId { get; set; }
        public Contract Contract { get; set; }
    }
}
