//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Graphics.Display;
//using System.Configuration;
//using System.Diagnostics;

//namespace Barroc_Intens.Data
//{
//    internal class AppDbContext : DbContext
//    {
//        public DbSet<Company> Companies { get; set; }
//        public DbSet<Contract> Contracts { get; set; }
//        public DbSet<InvoiceProduct> InvoiceProducts { get; set; }
//        public DbSet<Role> Roles { get; set; }
//        public DbSet<Brand> Brands { get; set; }
//        public DbSet<Department> Departments { get; set; }
//        public DbSet<Invoice> Invoices { get; set; }
//        public DbSet<MaintenanceAppointment> MaintenanceAppointments { get; set; }
//        public DbSet<MaintenanceAppointmentProduct> MaintenanceAppointmentProducts { get; set; }
//        public DbSet<Note> Notes { get; set; }
//        public DbSet<Product> Products { get; set; }
//        public DbSet<ProductsCategory> ProductsCategories { get; set; }
//        public DbSet<User> Users { get; set; }
//        public DbSet<PasswordReset> PasswordResets { get; set; }
//        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
//        public DbSet<PurchaseOrderStatus> PurchaseOrderStatuses { get; set; }
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            //Go to the App.config.example file and then follow Instructions

//            optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=;database=barrocintens", ServerVersion.Parse("8.0.30"));
//            base.OnConfiguring(optionsBuilder);
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            modelBuilder.Entity<User>()
//               .HasMany(u => u.Companies)
//               .WithMany(c => c.Users)
//               .UsingEntity<Note>();

//            modelBuilder.Entity<Contract>()
//                .HasMany(c => c.Products)
//                .WithMany(p => p.Contracts)
//                .UsingEntity<InvoiceProduct>();

//            modelBuilder.Entity<Role>().HasData(

//                new Role
//                {
//                    Id = 1,
//                    Name = "Customer"
//                },
//                new Role
//                {
//                    Id = 2,
//                    Name = "Employee"
//                },
//                new Role
//                {
//                    Id = 3,
//                    Name = "Admin"
//                }


//                );

//            modelBuilder.Entity<PurchaseOrderStatus>().HasData(

//                new PurchaseOrderStatus
//                {
//                    Id = 1,
//                    Name = "To Review"
//                },
//                new PurchaseOrderStatus
//                {
//                    Id = 2,
//                    Name = "In Review"
//                },
//                new PurchaseOrderStatus
//                {
//                    Id = 3,
//                    Name = "Approved"
//                }
//            );

//            modelBuilder.Entity<User>().HasData(

//new User
//{
//    Id = 1,
//    Name = "Customer",
//    Email = "Customer@gmail.com",
//    Password = SecureHasher.Hash("Customer"),
//    DepartmentId = 1,
//    RoleId = 1
//},

//                // Users for Department 2 (Finance)
//                new User
//                {
//                    Id = 6,
//                    Name = "FinanceAdmin",
//                    Email = "AdminFin@gmail.com",
//                    Password = SecureHasher.Hash("Finance"),
//                    DepartmentId = 2,
//                    RoleId = 3
//                },
//                new User
//                {
//                    Id = 7,
//                    Name = "Finance1",
//                    Email = "Fin1@gmail.com",
//                    Password = SecureHasher.Hash("Finance"),
//                    DepartmentId = 2,
//                    RoleId = 2
//                },
//                new User
//                {
//                    Id = 8,
//                    Name = "Finance2",
//                    Email = "Fin2@gmail.com",
//                    Password = SecureHasher.Hash("Finance"),
//                    DepartmentId = 2,
//                    RoleId = 2
//                },
//                new User
//                {
//                    Id = 9,
//                    Name = "Finance3",
//                    Email = "Fin3@gmail.com",
//                    Password = SecureHasher.Hash("Finance"),
//                    DepartmentId = 2,
//                    RoleId = 2
//                },
//                new User
//                {
//                    Id = 10,
//                    Name = "Finance4",
//                    Email = "Fin4@gmail.com",
//                    Password = SecureHasher.Hash("Finance"),
//                    DepartmentId = 2,
//                    RoleId = 2
//                },

//                // Users for Department 3 (Maintenance)
//                new User
//                {
//                    Id = 11,
//                    Name = "MaintenanceAdmin",
//                    Email = "AdminMaint@gmail.com",
//                    Password = SecureHasher.Hash("Maintenance"),
//                    DepartmentId = 3,
//                    RoleId = 3
//                },
//                new User
//                {
//                    Id = 12,
//                    Name = "Maintenance1",
//                    Email = "Maint1@gmail.com",
//                    Password = SecureHasher.Hash("Maintenance"),
//                    DepartmentId = 3,
//                    RoleId = 2
//                },
//                new User
//                {
//                    Id = 13,
//                    Name = "Maintenance2",
//                    Email = "Maint2@gmail.com",
//                    Password = SecureHasher.Hash("Maintenance"),
//                    DepartmentId = 3,
//                    RoleId = 2
//                },
//                new User
//                {
//                    Id = 14,
//                    Name = "Maintenance3",
//                    Email = "Maint3@gmail.com",
//                    Password = SecureHasher.Hash("Maintenance"),
//                    DepartmentId = 3,
//                    RoleId = 2
//                },
//                new User
//                {
//                    Id = 15,
//                    Name = "Maintenance4",
//                    Email = "Maint4@gmail.com",
//                    Password = SecureHasher.Hash("Maintenance"),
//                    DepartmentId = 3,
//                    RoleId = 2
//                },

//                // Users for Department 5(Purchase)
//                new User
//                {
//                    Id = 21,
//                    Name = "PurchaseAdmin",
//                    Email = "AdminPurch@gmail.com",
//                    Password = SecureHasher.Hash("Purchase"),
//                    DepartmentId = 7,
//                    RoleId = 3
//                },
//                new User
//                {
//                    Id = 22,
//                    Name = "Purchase1",
//                    Email = "Purch1@gmail.com",
//                    Password = SecureHasher.Hash("Purchase"),
//                    DepartmentId = 5,
//                    RoleId = 2
//                },
//                new User
//                {
//                    Id = 23,
//                    Name = "Purchase2",
//                    Email = "Purch2@gmail.com",
//                    Password = SecureHasher.Hash("Purchase"),
//                    DepartmentId = 5,
//                    RoleId = 2
//                },
//                new User
//                {
//                    Id = 24,
//                    Name = "Purchase3",
//                    Email = "Purch3@gmail.com",
//                    Password = SecureHasher.Hash("Purchase"),
//                    DepartmentId = 5,
//                    RoleId = 2
//                },
//                new User
//                {
//                    Id = 25,
//                    Name = "Purchase4",
//                    Email = "Purch4@gmail.com",
//                    Password = SecureHasher.Hash("Purchase"),
//                    DepartmentId = 5,
//                    RoleId = 2
//                }


//            );

//            modelBuilder.Entity<Company>().HasData(
//                new Company
//                {
//                    Id = 1,
//                    Name = "Bol.com",
//                    Phone = "030 310 4999",
//                    Address = "Papendorpseweg",
//                    Zipcode = "3528 BJ",
//                    City = "Utrecht",
//                    Country = "Nederland",
//                    BkrCheckedAt = DateTime.Now,
//                    ContactName = "Klanten Service",
//                    ContactMail = "klantenservice@bol.com"
//                },
//                new Company
//                {
//                    Id = 2,
//                    Name = "TechHub Solutions",
//                    Phone = "0123 456 789",
//                    Address = "Innovation Avenue",
//                    Zipcode = "1234 AB",
//                    City = "TechCity",
//                    Country = "Nederland",
//                    BkrCheckedAt = DateTime.Now,
//                    ContactName = "Tech Support",
//                    ContactMail = "support@techhub.com"
//                },
//                new Company
//                {
//                    Id = 3,
//                    Name = "GreenEco Industries",
//                    Phone = "9876 543 210",
//                    Address = "Sustainable Street",
//                    Zipcode = "5678 CD",
//                    City = "EcoVille",
//                    Country = "Nederland",
//                    BkrCheckedAt = DateTime.Now,
//                    ContactName = "Customer Care",
//                    ContactMail = "info@greeneco.com"
//                },
//                new Company
//                {
//                    Id = 4,
//                    Name = "FashionTrends Boutique",
//                    Phone = "4567 890 123",
//                    Address = "Chic Boulevard",
//                    Zipcode = "7890 EF",
//                    City = "StyleCity",
//                    Country = "Nederland",
//                    BkrCheckedAt = DateTime.Now,
//                    ContactName = "Fashion Advisor",
//                    ContactMail = "info@fashiontrends.com"
//                },
//                new Company
//                {
//                    Id = 5,
//                    Name = "Foodie Delights Catering",
//                    Phone = "6789 012 345",
//                    Address = "Gourmet Lane",
//                    Zipcode = "0123 GH",
//                    City = "Culinary Town",
//                    Country = "Nederland",
//                    BkrCheckedAt = DateTime.Now,
//                    ContactName = "Event Coordinator",
//                    ContactMail = "events@foodiedelights.com"
//                }
//            );

//            modelBuilder.Entity<MaintenanceAppointment>().HasData(
//                new MaintenanceAppointment
//                {
//                    Id = 1,
//                    Description = "Water slang kwijt",
//                    DateAdded = new DateTime(2024, 01, 01, 9, 15, 0),
//                    StartTime = new DateTime(2024, 01, 01, 10, 0, 0),
//                    EndTime = new DateTime(2024, 08, 01, 01, 23, 0),
//                    CompanyId = 1,
//                    Status = 1,
//                },
//                new MaintenanceAppointment
//                {
//                    Id = 2,
//                    Description = "Gebroken koffie machine",
//                    DateAdded = new DateTime(2024, 01, 01, 9, 15, 0),
//                    StartTime = new DateTime(2024, 01, 01, 10, 0, 0),
//                    EndTime = new DateTime(2024, 08, 01, 01, 23, 0),
//                    CompanyId = 1,
//                    Status = 1,
//                    UserId = 12
//                },
//                new MaintenanceAppointment
//                {
//                    Id = 3,
//                    Description = "Koffie machien schoon maken",
//                    DateAdded = new DateTime(2023, 08, 02, 13, 23, 0),
//                    StartTime = new DateTime(2023, 08, 02, 13, 23, 0),
//                    EndTime = new DateTime(2023, 12, 09, 03, 05, 0),
//                    CompanyId = 1,
//                    Status = 1,
//                    UserId = 12
//                },
//                new MaintenanceAppointment
//                {
//                    Id = 4,
//                    Description = "Knop werkt niet",
//                    DateAdded = new DateTime(2023, 05, 04, 9, 34, 0),
//                    StartTime = new DateTime(2023, 08, 02, 13, 23, 0),
//                    EndTime = new DateTime(2023, 12, 09, 03, 05, 0),
//                    CompanyId = 1,
//                    Status = 1,
//                    UserId = null
//                },
//                new MaintenanceAppointment
//                {
//                    Id = 5,
//                    Description = "Melk container lekt",
//                    DateAdded = new DateTime(2023, 07, 09, 13, 2, 0),
//                    StartTime = new DateTime(2023, 08, 02, 13, 23, 0),
//                    EndTime = new DateTime(2023, 12, 09, 03, 05, 0),
//                    CompanyId = 1,
//                    Status = 1,
//                    UserId = null
//                },
//                new MaintenanceAppointment
//                {
//                    Id = 6,
//                    Description = "Schoonmaak machine werkt niet",
//                    DateAdded = new DateTime(2023, 08, 02, 11, 22, 0),
//                    StartTime = new DateTime(2023, 08, 02, 13, 23, 0),
//                    EndTime = new DateTime(2023, 12, 09, 03, 05, 0),
//                    CompanyId = 1,
//                    Status = 1,
//                    UserId = null
//                },
//                new MaintenanceAppointment
//                {
//                    Id = 7,
//                    Description = "Cleaning machine not working again",
//                    DateAdded = new DateTime(2023, 08, 02, 11, 22, 0),
//                    StartTime = new DateTime(2023, 08, 02, 13, 23, 0),
//                    EndTime = new DateTime(2023, 12, 09, 03, 05, 0),
//                    CompanyId = 1,
//                    Status = 99,
//                    UserId = null
//                },
//                new MaintenanceAppointment
//                {
//                    Id = 8,
//                    Description = "Cleaning machine not working again",
//                    DateAdded = new DateTime(2023, 08, 02, 11, 22, 0),
//                    StartTime = new DateTime(2023, 08, 02, 13, 23, 0),
//                    EndTime = new DateTime(2023, 12, 09, 03, 05, 0),
//                    CompanyId = 1,
//                    Status = 99,
//                    UserId = null
//                },
//                new MaintenanceAppointment
//                {
//                    Id = 9,
//                    Description = "Cleaning machine not working again",
//                    DateAdded = new DateTime(2023, 08, 02, 11, 22, 0),
//                    StartTime = new DateTime(2023, 08, 02, 13, 23, 0),
//                    EndTime = new DateTime(2023, 12, 09, 03, 05, 0),
//                    CompanyId = 1,
//                    Status = 99,
//                    UserId = null
//                }
//            );

//            modelBuilder.Entity<Department>().HasData(
//                new Department
//                {
//                    Id = 1,
//                    Type = "Customer"
//                },
//                new Department
//                {
//                    Id = 2,
//                    Type = "Finance"
//                },
//                new Department
//                {
//                    Id = 3,
//                    Type = "Maintenance"
//                },
//                new Department
//                {
//                    Id = 4,
//                    Type = "Sales"
//                },
//                new Department
//                {
//                    Id = 5,
//                    Type = "Purchase"
//                },
//                new Department
//                {
//                    Id = 6,
//                    Type = "HeadMaintenance"
//                },
//                new Department
//                {
//                    Id = 7,
//                    Type = "HeadPurchase"
//                }
//            );

//            modelBuilder.Entity<Note>().HasData(
//                new Note
//                {
//                    Id = 1,
//                    Content = "Test",
//                    Date = DateTime.Now,
//                    CompanyId = 1,
//                    UserId = 12,
//                }
//            );

//            modelBuilder.Entity<ProductsCategory>().HasData(
//                new ProductsCategory
//                {
//                    Id = 1,
//                    Name = "Automaten",
//                    IsEmployeeOnly = false,
//                },
//                new ProductsCategory
//                {
//                    Id = 2,
//                    Name = "Koffiebonen",
//                    IsEmployeeOnly = false,
//                },
//                new ProductsCategory
//                {
//                    Id = 3,
//                    Name = "Onderdelen",
//                    IsEmployeeOnly = false,
//                }
//            );

//            modelBuilder.Entity<MaintenanceAppointmentProduct>().HasData(
//                new MaintenanceAppointmentProduct
//                {
//                    Id = 1,
//                    MaintenanceAppointmentId = 7,
//                    ProductId = 12,
//                },
//                new MaintenanceAppointmentProduct
//                {
//                    Id = 2,
//                    MaintenanceAppointmentId = 7,
//                    ProductId = 13,
//                },
//                new MaintenanceAppointmentProduct
//                {
//                    Id = 3,
//                    MaintenanceAppointmentId = 7,
//                    ProductId = 14,
//                },
//                new MaintenanceAppointmentProduct
//                {
//                    Id = 4,
//                    MaintenanceAppointmentId = 8,
//                    ProductId = 12,
//                },
//                new MaintenanceAppointmentProduct
//                {
//                    Id = 5,
//                    MaintenanceAppointmentId = 8,
//                    ProductId = 15,
//                },
//                new MaintenanceAppointmentProduct
//                {
//                    Id = 6,
//                    MaintenanceAppointmentId = 9,
//                    ProductId = 12,
//                },
//                new MaintenanceAppointmentProduct
//                {
//                    Id = 7,
//                    MaintenanceAppointmentId = 9,
//                    ProductId = 17,
//                }
//            );
//            modelBuilder.Entity<Brand>().HasData(
//                new Brand
//                {
//                    Id = 1,
//                    Name = "Barroc",
//                }
//                );




//            modelBuilder.Entity<Product>().HasData(
//                new Product
//                {
//                    Id = 1,
//                    Name = "Barroc Intens Italian Light",
//                    Description = "Een Koffie machine",
//                    BrandId = 1,
//                    Price = 499,
//                    Stock = 50,
//                    ProductsCategoryId = 1
//                },
//                new Product
//                {
//                    Id = 2,
//                    Name = "Barroc Intens Italian",
//                    Description = "Een Koffie machine",
//                    BrandId = 1,
//                    Price = 499,
//                    Stock = 50,
//                    ProductsCategoryId = 1
//                },
//                new Product
//                {
//                    Id = 3,
//                    Name = "Barroc Intens Italian Deluxe",
//                    Description = "Een Koffie machine",
//                    BrandId = 1,
//                    Price = 499,
//                    Stock = 50,
//                    ProductsCategoryId = 1
//                },
//                new Product
//                {
//                    Id = 4,
//                    Name = "Barroc Intens Italian Deluxe Special",
//                    Description = "Een Koffie machine",
//                    BrandId = 1,
//                    Price = 499,
//                    Stock = 50,
//                    ProductsCategoryId = 1
//                },
//                new Product
//                {
//                    Id = 5,
//                    Name = "Espresso Beneficio",
//                    Description = "Een toegankelijke en zachte koffie. Hij is afkomstig van de Finca El Limoncillo, een weelderige plantage aan de rand van het regenwoud in de Matagalpa regio in Nicaragua",
//                    Price = 21,
//                    Stock = 0,
//                    ProductsCategoryId = 2
//                },
//                new Product
//                {
//                    Id = 6,
//                    Name = "Rubber (10 mm)",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 7,
//                    Name = "Rubber (14 mm)",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 8,
//                    Name = "Slang",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 9,
//                    Name = "Voeding (elektra)",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 10,
//                    Name = "Ontkalker",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 11,
//                    Name = "Waterfilter",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 12,
//                    Name = "Reservoir sensor",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 13,
//                    Name = "Druppelstop",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 14,
//                    Name = "Electrische pomp",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 15,
//                    Name = "Tandwiel (110mm)",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 16,
//                    Name = "Tandwiel (70mm)",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 17,
//                    Name = "Maalmotor",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 18,
//                    Name = "Zeef",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 19,
//                    Name = "Reinigingstabletten",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 20,
//                    Name = "Reinigingsborsteltjes",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                },
//                new Product
//                {
//                    Id = 21,
//                    Name = "Ontkalkingspijp",
//                    Description = "Onderdeel van koffiemachine",
//                    Price = 1,
//                    Stock = 50,
//                    ProductsCategoryId = 3
//                }
//            );

//            modelBuilder.Entity<Contract>().HasData(
//                new Contract
//                {
//                    Id = 1,
//                    CompanyId = 1,
//                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
//                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
//                }
//            );

//            modelBuilder.Entity<InvoiceProduct>().HasData(
//                new InvoiceProduct
//                {
//                    Id = 1,
//                    ProductId = 1,
//                    Amount = 5,
//                }
//            );





//            modelBuilder.Entity<Invoice>().HasData(
//                new Invoice
//                {
//                    Id = 1,
//                    StartDate = DateTime.UtcNow,
//                    EndDate = DateTime.UtcNow,
//                    ContractId = 1,
//                    VAT = 21,
//                }
//            );
//        }
//    }
//}
