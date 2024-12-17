using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class Product
    {
        public int Id { get; set; }
        public int? ProductsCategoryId { get; set; }
        public ProductsCategory ProductsCategory { get; set; }
        public int? BrandId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
      

        public decimal? Price { get; set; }
        private int stock;
        public int Stock
        {
            get => stock;
            set
            {
                if (stock != value)
                {
                    stock = value;
                    OnPropertyChanged(nameof(Stock));
                }
            }
        }



        public ICollection<MaintenanceAppointment> ProductMaintenanceAppointments { get; set; }
        public ICollection<MaintenanceAppointment> MaintenanceAppointments { get; set; }
        public ICollection<MaintenanceAppointmentProduct> MaintenanceAppointmentProducts { get; set; }
        public ICollection<Contract> Contracts { get; set; }
        public ICollection<Brand> Brands { get; set; }
        public ICollection<InvoiceProduct> ContractProducts { get; set; }

        public ObservableCollection<PurchaseOrder> PurchaseOrders { get; set; } = new ObservableCollection<PurchaseOrder>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
