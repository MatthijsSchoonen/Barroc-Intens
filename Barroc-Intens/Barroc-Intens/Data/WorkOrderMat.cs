using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    internal class WorkOrderMat
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public WorkOrder WorkOrder { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
