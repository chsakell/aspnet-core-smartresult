using SmartResult.Unit.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartResult.Unit.Tests
{
    public class Repository : IRepository
    {
        static List<Customer> Customers = new List<Customer>
        {
            new Customer() { Id = 1,  Name = "Christos", Address = "Athens", Age = 32, PhoneNumber = "123456789"},
            new Customer() { Id = 2, Name = "John", Address = "Paris", Age = 44, PhoneNumber = "253525252"},
            new Customer() { Id = 3, Name = "Mary", Address = "Milano", Age = 35, PhoneNumber = "425225252"}
        };
        //static Repository
        public Customer GetCustomer(int id)
        {
            return Customers.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return Customers;
        }
    }

    public interface IRepository
    {
        IEnumerable<Customer> GetCustomers();
        Customer GetCustomer(int id);
    }
}
