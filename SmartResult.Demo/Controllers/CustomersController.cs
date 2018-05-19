using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartResult.Demo;
using SmartResult.Demo.Models;

namespace AspNet.Core.SmartResult.Demo.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly IRepository repository;

        public CustomersController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET api/customers
        [HttpGet]
        [SmartResult(Default = typeof(IEnumerable<Customer>), Mobile = typeof(IEnumerable<MobileCustomer>), 
        Native = typeof(IEnumerable<NativeCustomer>))]
        public IActionResult Get()
        {
            return Ok(repository.GetCustomers());
        }

        // GET api/customers/id
        [HttpGet("{id}")]
        [SmartResult(Default = typeof(Customer), Mobile = typeof(MobileCustomer), Native = typeof(NativeCustomer))]
        public MobileCustomer Get(int id)
        {
            return repository.GetCustomer(id);
        }


        // POST api/values
        [HttpPost]
        [SmartResult(Default = typeof(Customer), Mobile = typeof(MobileCustomer))]
        public IActionResult Post([FromBody]Customer value)
        {
            return Ok(value);
        }

    }
}
