using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class ContractProduct
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public int ContractId { get; set; }
        public int ProductId { get; set; }
        public Contract Contract { get; set; }
        public Product Product { get; set; }
    }
}
