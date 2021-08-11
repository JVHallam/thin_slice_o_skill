using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace entity_test
{

    public class CustomerContext : DbContext{
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options){
            var connectionString = Environment.GetEnvironmentVariable("connection_string");
            options.UseSqlServer(connectionString);
        }
    }

    public class Customer{
        [Required]
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var a = Environment.GetEnvironmentVariable("connection_string");
            Console.WriteLine("Hello World!");
            Console.WriteLine(a);
        }
    }
}
