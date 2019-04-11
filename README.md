# AspNet.Core.SmartResult

[![License](https://img.shields.io/github/license/chsakell/aspnet-core-smartresult.svg)](https://github.com/chsakell/aspnet-core-smartresult/blob/master/LICENSE) [![Build status](https://ci.appveyor.com/api/projects/status/i63utoec8j7wsjwy/branch/master?svg=true)](https://ci.appveyor.com/project/chsakell/aspnet-core-smartresult/branch/master) [![AppVeyor tests](https://img.shields.io/appveyor/tests/chsakell/aspnet-core-smartresult.svg)](https://ci.appveyor.com/project/chsakell/aspnet-core-smartresult)

Available as [NuGet package](https://www.nuget.org/packages/AspNet.Core.SmartResult/)

Result filter for ASP.NET Core MVC Controllers that can produce multiple types of results for the same action. Used in **multi-client** APIs that need to serve different types of results per device.

![AspNet.Core.SmartResult](https://chsakell.files.wordpress.com/2018/05/smart-result.png)

### Description

Most of the times mobile devices/browsers consume and render only partial data in contrast with those rendered by desktop browsers. This usually results to create different MVC actions or even applications that serve different content to different types of clients in order to reduce unused data tranfered over the wire. `SmartResult` filter can help you solve this problem by transforming the type of the action's result based on the device.

 `SmartResult` implements `IResultFilter` and `Attribute` so it can be placed as a filter attribute above ASP.NET Core MVC Controller actions. You declare the default returned type of the result *(which is for desktop)*, the returned type for mobile browsers and native apps as well. The filter upon `OnResultExecuting` event detects the type of the client and uses `AutoMapper` to transform the result if required.

### Examples of usage

In the following examples the `NativeCustomer` class is the base and more light weighted class that supposed to be consumed by native apps. `MobileCustomer` inherits it and adds a single property. `Customer` contains all available information for a single customer.

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

The action below will return type of `IEnumerable<Customer>` for desktop, type of `IEnumerable<MobileCustomer>` for mobile browsers and `IEnumerable<NativeCustomer>` based on configuration that follows.

```csharp

 // GET api/customers
[HttpGet]
[SmartResult]
public IActionResult Get()
{
    return repository.GetCustomers();
}

 ```

The configuration:

```csharp

List<ISmartResultProfile> profiles = new List<ISmartResultProfile>
{
    new SmartResultProfile<Customer, MobileCustomer, NativeCustomer>(new CustomerProfile())
};

// Use the minimum configuration
SmartResult.Configure(
    new SmartResultConfiguration(
        profiles
    )
);

```

Works with ActionResults and base types as well

 ```csharp

// GET api/customers/id
[HttpGet("{id}")]
[SmartResult]
public Customer Get(int id)
{
    return repository.GetCustomer(id);
}

```

The configuration is a generic class that accepts the types of classes for Desktop, Mobile and/or Native plus an `AutoMapper` **profile**. For example:

 ```csharp

List<ISmartResultProfile> profiles = new List<ISmartResultProfile>
{
    new SmartResultProfile<Customer, MobileCustomer, NativeCustomer>(new CustomerProfile())
};

// Use the minimum configuration
SmartResult.Configure(
    new SmartResultConfiguration(
        profiles
    )
);

```

1. `Customer` for desktop browsers
2. `MobileCustomer` for mobile browsers
3. `NativeCustomer` for native apps

The Desktop Generic type is mandatory but you can choose to support only Mobile or Native clients as follow:

```csharp

// Desktop & Mobile Only
List<ISmartResultProfile> profiles = new List<ISmartResultProfile>
{
    new SmartResultProfile<Customer, MobileCustomer>(new CustomerTestProfile(), Client.Mobile)
};

// Desktop & Native Only
List<ISmartResultProfile> profiles = new List<ISmartResultProfile>
{
    new SmartResultProfile<Customer, NativeCustomer>(new CustomerTestProfile(), Client.Native)
};

```

### How it works
When you apply the `SmartResult` filter on an Action, the filter will search for an `ISmartResultProfile` profile having as it's Desktop class type the base type of your Action's result *(e.g. if the action's result type is `IEnumerable<Customer>` or just `Customer` the filter will search a profile with desktop type of `Customer`)*. If it founds one then it will detect if the request comes from mobile or native device and manipulate the result respectively if needed.


### Installation

The package can be installed using one of the following ways:

* **Package Manager**:  `Install-Package AspNet.Core.SmartResult`
* **.NET CLI**: `dotnet add package AspNet.Core.SmartResult`
* **Paket CLI**: `paket add AspNet.Core.SmartResult`

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
    List<ISmartResultProfile> profiles = new List<ISmartResultProfile>
    {
        new SmartResultProfile<Customer, MobileCustomer, NativeCustomer>(new CustomerProfile())
    };

    // Use the minimum configuration
    SmartResult.Configure(
        new SmartResultConfiguration(
            profiles
        ));

    app.UseMvc();
}

```

The above configuration will use a default implementation for detecting mobile devices. `CustomerProfile` is an AutoMapper profile *(just for this demonstration)* and could look like this:

```csharp

public class CustomerProfile : Profile
{
    public SmartResultProfile()
    {
        CreateMap<Customer, MobileCustomer>();
        CreateMap<Customer, NativeCustomer>();
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

    List<ISmartResultProfile> profiles = new List<ISmartResultProfile>
    {
        new SmartResultProfile<Customer, MobileCustomer, NativeCustomer>(new CustomerProfile())
    };

    // Use the minimum configuration
    SmartResult.Configure(
        new SmartResultConfiguration(
            profiles,
            isMobile: MyCustomMobileDetection,
            isNative: MyCustomNativeDetection
        ));
        
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

#### Contributing to AspNet.Core.SmartResult
Feel free to contribute to this project by:

* Reporting a bug
* Discussing the current state of the code
* Submitting a fix
* Proposing new features

Pull requests are the best way to propose changes to the codebase so:

1. Fork the repo and create your branch from `master`
2. If you've added code that should be tested, add tests
3. If you've changed APIs, update the documentation
4. Ensure the test suite passes
5. Issue that pull request!

## License
Code released under the <a href="https://github.com/chsakell/aspnet-core-smartresult/blob/master/LICENSE" target="_blank"> MIT license</a>.
