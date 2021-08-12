using System;
using api.Models;

namespace api.Services.Interfaces{
    public interface ICustomerService{
        int Save(Customer customer);
        Customer Retrieve(int customerId);
    }
}
