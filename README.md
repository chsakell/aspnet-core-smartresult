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

The package is currently in alpha version and can be installed using one of the following ways:

* **Package Manager**:  `Install-Package AspNet.Core.SmartResult -Version 1.0.0-alpha1`
* **.NET CLI**: `dotnet add package AspNet.Core.SmartResult --version 1.0.0-alpha1`
* **Paket CLI**: paket add AspNet.Core.SmartResult --version 1.0.0-alpha1

## Configuration

The transformation of the result types is based on [AutoMapper](https://github.com/AutoMapper/AutoMapper) profiles and functions that determine if the `HttpRequest` comes from a mobile or a native device. The minimum requirement is to create the `AutoMapper` profiles and pass the list to a `SmartResultConfiguration` object using the `SmartResult.ConfigureProfiles` method. Place that code inside the `Startup` class before the `app.UseMvc()` call as follow:

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
    SmartResult.ConfigureProfiles(new SmartResultConfiguration(profiles));

    app.UseMvc();
}

```

The above minumum configuration will use a default implementation for detecting mobile devices. It will also use the same implementation for detecting native devices.

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
    SmartResult.ConfigureProfiles(
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