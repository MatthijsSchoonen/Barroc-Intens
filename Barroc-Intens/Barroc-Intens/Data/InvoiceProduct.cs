using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class InvoiceProduct
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
