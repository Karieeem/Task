using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Task.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CustomerProduct> CustomersProducts { get; set; }

        public System.Data.Entity.DbSet<Task.Models.Customer> Customers { get; set; }

        //public System.Data.Entity.DbSet<Task.Models.Product> Products { get; set; }
    }
}