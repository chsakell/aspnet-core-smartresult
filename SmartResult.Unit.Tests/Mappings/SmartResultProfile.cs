using AutoMapper;
using SmartResult.Unit.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartResult.Unit.Tests.Mappings
{
    public class SmartResultProfile : Profile
    {
        public SmartResultProfile()
        {
            CreateMap<Customer, MobileCustomer>();
            CreateMap<Customer, NativeCustomer>();
        }
    }
}
