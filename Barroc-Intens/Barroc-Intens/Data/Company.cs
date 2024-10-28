using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone {  get; set; }
        public string Address {  get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Country {  get; set; }
        public DateTime BkrCheckedAt { get; set; }
        public string? ContactName { get; set; } = null;
        public string? ContactMail { get; set; } = null;
        public ICollection<User> Users { get; set; }
        public ICollection<Note> Notes { get; set; }

        [InverseProperty(nameof(Contract.Company))]
        public ICollection<Contract> CompanyContracts { get; set; }

    }
}
