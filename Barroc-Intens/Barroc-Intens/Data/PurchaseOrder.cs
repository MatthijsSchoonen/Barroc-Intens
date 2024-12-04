using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    internal class PurchaseOrder
    {
        public int Id { get; set; }
        public ICollection<Product> Products { get; set; }
        public DateTime OrderedAt { get; set; }
        public int TotalPrice { get; set; }
        public int StatusId { get; set; }
        public PurchaseOrderStatus Status { get; set; }

    }
}
