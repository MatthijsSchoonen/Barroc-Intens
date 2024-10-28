using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public int Stock {  get; set; }

        [InverseProperty(nameof(MaintenanceAppointment.Product))]
        public ICollection<MaintenanceAppointment> ProductMaintenanceAppointments { get; set; }
        public ICollection<MaintenanceAppointment> MaintenanceAppointments { get; set; }
        public ICollection<MaintenanceAppointmentProduct> MaintenanceAppointmentProducts { get; set; }
        public ICollection<CustomInvoice> CustomInvoices { get; set; }
        public ICollection<CustomInvoiceProduct> CustomInvoiceProducts { get; set; }
        public ICollection<Contract> Contracts { get; set; }
        public ICollection<ContractProduct> ContractProducts { get; set; }
        public int ProductsCategoryId { get; set; }
        public ProductsCategory ProductsCategory { get; set; }
    }
}
