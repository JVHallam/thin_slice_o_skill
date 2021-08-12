using System;
using System.Linq;
using api.Models;
using api.Services.Interfaces;
using api.Data;

//I want to introduce entity here

namespace api.Services{
    public class CustomerService : ICustomerService{
        private readonly CustomerContext _customerContext;

        public CustomerService(CustomerContext customerContext){
            _customerContext = customerContext;
        }

        public int Save(Customer customer){
            _customerContext.Customers.Add( customer );
            _customerContext.SaveChanges();

            //how do i get access to the context here?
            return customer.Id;
        }

        public Customer Retrieve(int customerId){
            var customer = _customerContext.Customers.First(customer => customer.Id == customerId);
            return customer;
        }
    }
}
