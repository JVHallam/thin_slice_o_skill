using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.Data{
    public class CustomerContext : DbContext{
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options){
            var connectionString = Environment.GetEnvironmentVariable("connection_string");
            options.UseSqlServer(connectionString);
        }
    }
}
