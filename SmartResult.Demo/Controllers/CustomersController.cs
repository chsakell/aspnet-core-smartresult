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
        private readonly IRepository _repository;

        public CustomersController(IRepository repository)
        {
            _repository = repository;
        }

        // GET api/customers
        [HttpGet]
        [SmartResult]
        public IActionResult Get()
        {
            return Ok(_repository.GetCustomers());
        }

        // GET api/customers/id
        [HttpGet("{id}")]
        [SmartResult]
        public Customer Get(int id)
        {
            return _repository.GetCustomer(id);
        }


        // POST api/values
        [HttpPost]
        [SmartResult]
        public IActionResult Post([FromBody]Customer value)
        {
            return Ok(value);
        }

    }
}
