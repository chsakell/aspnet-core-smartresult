using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartResult.Unit.Tests.Models
{
    public class Customer : MobileCustomer
    {
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
    }
}
