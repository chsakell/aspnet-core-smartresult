using AutoMapper;
using SmartResult.Demo.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartResult.Demo.Mappings
{
    public class SmartResultProfile : Profile
    {
        public SmartResultProfile()
        {
            CreateMap<Customer, MobileCustomer>();
            CreateMap<MobileCustomer, Customer>();
        }
    }
}
