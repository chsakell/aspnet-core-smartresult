using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNet.Core.SmartResult.Demo.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        [SmartResult(Default = typeof(IEnumerable<Customer>), Mobile = typeof(IEnumerable<MobileCustomer>))]
        public IEnumerable<Customer> Get()
        {
            var customer = new Customer
            {
                Name = "Christos",
                Age = 32,
                Address = "test"
            };

            List<Customer> customers = new List<Customer>();
            customers.Add(customer);

            return customers;
        }

        [HttpGet("{id}")]
        [SmartResult(Default = typeof(IEnumerable<Customer>), Mobile = typeof(MobileCustomer))]
        public MobileCustomer Get(int id)
        {
            var customer = new MobileCustomer
            {
                Name = "Christos",
                Age = 32
            };

            return customer;
        }


        // POST api/values
        [HttpPost]
        [SmartResult(Default = typeof(Customer), Mobile = typeof(MobileCustomer))]
        public IActionResult Post([FromBody]Customer value)
        {
            return Ok(value);
        }

    }

    public class Customer : MobileCustomer
    {
        public string Address { get; set; }
    }

    public class MobileCustomer
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
