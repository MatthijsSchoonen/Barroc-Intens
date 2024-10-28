using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class Contract
    {
        public int Id { get; set; }
        public int CompanyId {  get; set; }
        public Company Company { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate {  get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<ContractProduct> ContractProducts { get; set; }
    }
}
