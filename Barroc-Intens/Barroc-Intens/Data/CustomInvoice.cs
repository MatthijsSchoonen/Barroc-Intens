﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class CustomInvoice
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CompanyId {  get; set; }
        public decimal ConnectionCost { get; set; } 
        public int VAT { get; set; } 
        public Company Company { get; set; }
        public ICollection<Product> Products{ get; set; }
        public ICollection<CustomInvoiceProduct> CustomInvoiceProducts { get; set; }
    }
}