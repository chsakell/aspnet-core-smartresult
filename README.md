# AspNet.Core.SmartResult

 Result filter for ASP.NET Core MVC Controllers that can produce multiple types of results for the same action. Used in **multi-client** APIs that need to serve different types of results per device.

![AspNet.Core.SmartResult](https://chsakell.files.wordpress.com/2018/05/smart-result.png)

### Description

 Most of the times mobile devices/browsers consume and render only partial data in contrast with those rendered by desktop browsers. This usually results to create different MVC actions or even applications that serve different content to different types of clients in order to reduce unused data tranfered over the wire. `SmartResult` filter can help you solve this problem by transforming the type of the action's result based on the device.

 `SmartResult` implements `IResultFilter` and `Attribute` so it can be placed as a filter attribute above ASP.NET Core MVC Controller actions. You declare the default returned type of the result *(which is for desktop)*, the returned type for mobile browsers and native apps as well. The filter upon `OnResultExecuting` event detects the type of the client and uses `AutoMapper` to transform the result if required.

### Examples of usage

In the following examples the `NativeCustomer` class is the base and more light weighted class that suppoted to be consumed by native apps. `MobileCustomer` inherits it and adds a single property. `Customer` contains all available information for a single customer.

```csharp
public class Customer : MobileCustomer
{
    public string PhoneNumber { get; set; }
    public int Age { get; set; }
}

public class MobileCustomer : NativeCustomer
{
    public string Address { get; set; }
}

public class NativeCustomer
{
    public int Id { get; set; }
    public string Name { get; set; }
}

```

The snippet below declares that the default return type *(desktop)* is of type `IEnumerable<Customer>` and the return type for mobile browsers is `IEnumerable<MobileCustomer>`

```csharp

 // GET api/customers
[HttpGet]
[SmartResult(Default = typeof(IEnumerable<Customer>), Mobile = typeof(IEnumerable<MobileCustomer>))]
public IEnumerable<Customer> Get()
{
    return repository.GetCustomers();
}

 ```

You can also define different result type for native apps.

 ```csharp

// GET api/customers/id
[HttpGet("{id}")]
[SmartResult(Default = typeof(Customer), Mobile = typeof(MobileCustomer), Native = typeof(NativeCustomer))]
public MobileCustomer Get(int id)
{
    return repository.GetCustomer(id);
}

```

The snippet above declares that the result type changes per device as follow:

1. `Customer` for desktop browsers
2. `MobileCustomer` for mobile browsers
3. `NativeCustomer` for native apps

### Installation

The package is currently in alpha version and can be installed using one of the following ways:

* **Package Manager**:  `Install-Package AspNet.Core.SmartResult -Version 1.0.0-alpha2`
* **.NET CLI**: `dotnet add package AspNet.Core.SmartResult --version 1.0.0-alpha2`
* **Paket CLI**: `paket add AspNet.Core.SmartResult --version 1.0.0-alpha2`

### Configuration

The transformation of the result types is based on [AutoMapper](https://github.com/AutoMapper/AutoMapper) profiles and functions that determine if the `HttpRequest` comes from a mobile or a native device. The minimum requirement is to create the `AutoMapper` profiles and pass the list to a `SmartResultConfiguration` object using the `SmartResult.Configure` method. Place that code inside the `Startup` class before the `app.UseMvc()` call as follow:

#### Minimum Configuration

 ```csharp

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // Add a list of AutoMapper profiles to be used by SmartResult
    List<Profile> profiles = new List<Profile> { new SmartResultProfile() };

    // Use the minimum configuration
    SmartResult.Configure(new SmartResultConfiguration(profiles));

    app.UseMvc();
}

```

The above minumum configuration will use a default implementation for detecting mobile devices. It will also use the same implementation for detecting native devices. `SmartResultProfile` is an AutoMapper profile *(just for this demonstration)* and could look like this:

```csharp

public class SmartResultProfile : Profile
{
    public SmartResultProfile()
    {
        CreateMap<Customer, MobileCustomer>();
        CreateMap<MobileCustomer, Customer>();
    }
}

```

Use your own AutoMapper profiles and pass them to `SmartResultConfiguration` constructor.

#### Advance Configuration

You can override how mobile and native devices detection takes place by providing your own implementations. For example you may want to detect native devices based on a custom header sent by them. The `SmartResultConfiguration` accepts the following two optional delegates:

```csharp

delegate bool IsMobile(HttpContext context);
delegate bool IsNative(HttpContext context);

```

Hence, an advanced configuration would look like this:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    // Add a list of AutoMapper profiles to be used by SmartResult
    List<Profile> profiles = new List<Profile> { new SmartResultProfile() };

    // Use the minimum configuration
    SmartResult.Configure(
        new SmartResultConfiguration(
            profiles,
            isMobile: MyCustomMobileDetection,
            isNative: MyCustomNativeDetection
        )
    );

    app.UseMvc();
}

private bool MyCustomMobileDetection(HttpContext request)
{
    // Place your custom logic here for detecting Mobile browsers
    return true;
}

private bool MyCustomNativeDetection(HttpContext request)
{
    // Place your custom logic here for detecting Native devices
    return true;
}

```

## License
Code released under the <a href="https://github.com/chsakell/aspnet-core-smartresult/blob/master/LICENSE" target="_blank"> MIT license</a>.