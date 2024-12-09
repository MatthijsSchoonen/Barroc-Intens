using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    internal class PurchaseOrder
    {
        public int Id { get; set; }
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public DateTime OrderedAt { get; set; }
        public int TotalPrice { get; set; }
        public int StatusId { get; set; }
        public PurchaseOrderStatus Status { get; set; }

    }
}
