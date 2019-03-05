using AspNet.Core.SmartResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using SmartResult.Unit.Tests.Mappings;
using SmartResult.Unit.Tests.Models;
using System.Collections.Generic;
using Xunit;

namespace SmartResult.Unit.Tests
{
    public class MobileTests
    {
        private readonly IRepository _repository;
        public MobileTests()
        {
            _repository = new Repository();
        }

        [Fact]
        public void Should_Return_Mobile_Result()
        {
            List<SmartResultProfile> profiles = new List<SmartResultProfile>
            {
                new SmartResultProfile(new CustomerTestProfile(),
                    typeof(Customer),
                    typeof(MobileCustomer),
                    typeof(NativeCustomer))
            };

            AspNet.Core.SmartResult.SmartResult.Configure(
                new AspNet.Core.SmartResult.SmartResultConfiguration(
                    profiles
                )
            );

            var filter = new AspNet.Core.SmartResult.SmartResult();

            // Mock out the context to run the action filter.
            var request = new Mock<HttpRequest>();
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add("User-Agent", Constants.AndroidMobileBrowser);
            request.SetupGet<IHeaderDictionary>(r => r.Headers).Returns(requestHeaders);
            var response = new Mock<HttpResponse>();
            response.SetupGet<IHeaderDictionary>(r => r.Headers).Returns(new HeaderDictionary());

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Request).Returns(request.Object);
            httpContext.SetupGet(c => c.Response).Returns(response.Object);

            var modelState = new ModelStateDictionary();

            var actionContext = new ActionContext(
                httpContext.Object,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object,
                modelState
            );

            var resultExecutedContext = new ResultExecutingContext(actionContext,
            new List<IFilterMetadata>(),
            new OkObjectResult(_repository.GetCustomers()),
            new Mock<Controller>().Object);

            filter.OnResultExecuting(resultExecutedContext);
            var resultHeaderType = resultExecutedContext.HttpContext.Response.Headers["Result-Type"].ToString();
            var result = resultExecutedContext.Result;

            // Assert
            Assert.IsAssignableFrom<IEnumerable<MobileCustomer>>((result as ObjectResult).Value);
            Assert.Equal("Mobile", resultHeaderType);
        }

        [Fact]
        public void Should_Return_Default_Result_When_Mobile_Not_Defined()
        {
            List<SmartResultProfile> profiles = new List<SmartResultProfile>
            {
                new SmartResultProfile(new CustomerTestProfile(),
                    typeof(Customer),
                    native:typeof(NativeCustomer))
            };

            AspNet.Core.SmartResult.SmartResult.Configure(
                new SmartResultConfiguration(
                    profiles
                )
            );

            var filter = new AspNet.Core.SmartResult.SmartResult();

            // Mock out the context to run the action filter.
            var request = new Mock<HttpRequest>();
            var requestHeaders = new HeaderDictionary();
            requestHeaders.Add("User-Agent", Constants.AndroidMobileBrowser);
            request.SetupGet<IHeaderDictionary>(r => r.Headers).Returns(requestHeaders);
            var response = new Mock<HttpResponse>();
            response.SetupGet<IHeaderDictionary>(r => r.Headers).Returns(new HeaderDictionary());

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Request).Returns(request.Object);
            httpContext.SetupGet(c => c.Response).Returns(response.Object);

            var modelState = new ModelStateDictionary();

            var actionContext = new ActionContext(
                httpContext.Object,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object,
                modelState
            );

            var resultExecutedContext = new ResultExecutingContext(actionContext,
            new List<IFilterMetadata>(),
            new OkObjectResult(_repository.GetCustomers()),
            new Mock<Controller>().Object);

            filter.OnResultExecuting(resultExecutedContext);
            var resultHeaderType = resultExecutedContext.HttpContext.Response.Headers["Result-Type"].ToString();
            var result = resultExecutedContext.Result;

            // Assert
            Assert.IsAssignableFrom<IEnumerable<Customer>>((result as ObjectResult).Value);
            Assert.Equal("Default", resultHeaderType);
        }
    }
}
