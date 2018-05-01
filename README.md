# AspNet.Core.SmartResult

 Result filter for ASP.NET Core MVC Controllers that can produce multiple types of results for the same action. Used in **multi-client** APIs that need to serve different types of results per device.

 ## Examples of usage

 ```csharp
 
 // GET api/customers
[HttpGet]
[SmartResult(Default = typeof(IEnumerable<Customer>), Mobile = typeof(IEnumerable<MobileCustomer>))]
public IEnumerable<Customer> Get()
{
    return repository.GetCustomers();
}

 ```

The snippet above declares that the default return type *(desktop)* is of type `IEnumerable<Customer>` and the return type for mobile browsers is `IEnumerable<MobileCustomer>`

The Filter also support different result types for native apps.

 ```csharp

[HttpGet]
[SmartResult(Default = typeof(IEnumerable<Customer>), Mobile = typeof(IEnumerable<MobileCustomer>))]
public IActionResult Get()
{
    return Ok(repository.GetCustomers());
}

```

The snippet above declares that the result type changes per device as follow:

1. `Customer` for desktop browsers
2. `MobileCustomer` for mobile browsers
3. `NativeCustomer` for native apps

## Installation