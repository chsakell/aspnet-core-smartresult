using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartResult.Demo.Models;

namespace AspNet.Core.SmartResult.Demo.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, MobileCustomer>();
            CreateMap<Customer, NativeCustomer>();
        }
    }
}
