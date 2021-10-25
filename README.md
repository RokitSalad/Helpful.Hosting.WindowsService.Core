# Helpful.Hosting.WorkerService
Base package for Core 3.x and DotNet 5+ services

## Background
In the beginning, there were Windows Services. These were unforgiving creatures which required the developer to write an installer, they were littered with calls to Thread.Sleep to give developers time to attach a debugger. Reliability was rare, standards were low, and debugging was a pain. Then came [Topshelf](http://topshelf-project.com/ 'Topshelf') and the world of Windows Services became a brighter place, full of optimism and F5 debugging. With Windows Service development now looking more like developing for the commandline, even the great monster IIS felt the impact, as developers revelled in the consistent results and simple API. Many developement teams built wrappers around Topshelf to allow very easy setup of new services based on their favourite standards, and to help new developers get started without having to know too much. Then DotNet started to catch up, and the [Worker Service](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers 'Worker Services in .NET') stepped forward with an API reminiscent of Topshelf, as if someone somewhere had been watching and learning from Topshelf's success. With the Worker Service came a way to easily build and debug applications which could then be deployed in many different ways, with just native, DotNet libraries. The future was here, Topshelf had its feet up in front of a log fire, smoking a pipe, enjoying retirement, but things still weren't quite finished. Spinning up a Worker Service still requires some thought and some wider understanding of the API, if the right approach is to be taken - we still need some wrappers which make some reasonable assumptions and provide some simple, yet powerful, 'get started quick' functionality. To this end, I decided to extend my Helpful.Hosting.WindowsService.Core library to include Worker Service packages targeting both Windows Services and Linux Systemd. I hope someone somewhere finds this helpful.

## Why?
If you want a very quick way to get started writing either a Windows Service or a Systemd service in .NET Core 3.x or DotNet 5+, this library is for you. Services spun up with this library get the following features out of the box:
* Serilog logging from the [Helpful.Logging.Standard](https://github.com/RokitSalad/Helpful.Logging.Standard) library (added as a dependency).
* Swagger endpoints for any exposed API's added using Controller Actions, exposed at /swagger.
* A basic healthcheck endpoint exposed at /healthcheck (the endpoint appears in the swagger definitions).

Benefits of this library over other Host wrappers are:
* Well defined mechanism for declaring IOC bindings.
* Simple mechanism for setting options in the web app builder.
* A full range of choices for implementation, from very simple, to very flexible.

## Overview
The original Helpful.Hosting.WindowsService.Core is still in this repo, and deployed to Nuget as normal. See [the original readme](https://github.com/RokitSalad/Helpful.Hosting.WindowsService.Core/blob/master/src/Helpful.Hosting.WindowsService.Core/README.md 'README.md') for the original doco. Much of the API has remained the same in this update.

There are 5 sample projects in this repository, which show different ways to use this library. The quick start sections, below, are based on each.

## Quick Start - for a Web API running as a Windows Service
1. Create a commandline project for .NET Core 3.x or DotNet 5 (or above).
2. Add a NuGet reference to **Helpful.Hosting.WorkerService.Windows**.
3. Modify your Program.cs with the following (either as a top level statement, or as the content of Main):
```csharp
HostFactory.RunApiWorker(new RunApiWorkerParams
{
    Args = args,
    ListenerInfo = new[]
    {
        new ListenerInfo
        {
            Port = 8150
        }
    }
});
```
4. There is no 4 - that's it!

If you hit F5, you'll now have a webservice listening on port 8150 for non-TLS traffic. There is already a health check endpoint exposed at http://localhost:8150/healthcheck. To add more controllers, just implement Microsoft.AspNetCore.Mvc.ControllerBase. For example:
```csharp
[ApiController]
[Route("[controller]")]
public class DayOfTheWeekController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return DateTime.Now.DayOfWeek.ToString();
    }
}
```
Controllers are found automatically and exposed via however many ListenerInfo objects were injected to the RunApiWorker method.

## Quick Start - for a Linux Systemd running a simple looped process with no Web API

1. Create a commandline project for .NET Core 3.x or DotNet 5 (or above).
2. Add a NuGet reference to **Helpful.Hosting.WorkerService.Systemd**.
3. Modify your Program.cs with the following (either as a top level statement, or as the content of Main):
```csharp
HostFactory.RunBackgroundTaskWorker(new RunBackgroundTaskWorkerParams
{
    Args = args,
    WorkerProcess = async (cancellationToken) =>
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine(DateTime.Now);
            await Task.Delay(4000);
        }
    }
});
```
4. There is no 4 - that's it!

If you hit F5, you will just get the loop running with no endpoints exposed on any ports. There won't even be the health check. Any controllers you add to the project will be ignored.

## Quick Start - for a Linux Systemd running a simple looped process and exposing a Web API

1. Create a commandline project for .NET Core 3.x or DotNet 5 (or above).
2. Add a NuGet reference to **Helpful.Hosting.WorkerService.Systemd**.
3. Modify your Program.cs with the following (either as a top level statement, or as the content of Main):
```csharp
HostFactory.RunCompoundWorker(new RunCompoundWorkerParams
{
    Args = args,
    WorkerProcess = async (cancellationToken) =>
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine(DateTime.Now);
            await Task.Delay(4000);
        }
    },
    ListenerInfo = new []{
        new ListenerInfo
        {
            Port = 8151
        }
    } 
});
```
4. There is no 4 - that's it!

If you hit F5 you will see the current date and time written to the console in a loop. The healthcheck is also exposed at http://localhost:8151/healthcheck. You can add your own controllers to this service as well - it will happily run the background process and respond to HTTP requests.

## Quick Start - for more advanced usage

1. Create a commandline project for .NET Core 3.x or DotNet 5 (or above).
2. Add a NuGet reference to **Helpful.Hosting.WorkerService.Windows**.
3. Add a custom worker class to your project. Something like this:
```csharp
public class CustomWorker : CustomWorkerBase
{
    private readonly IDayOfTheWeekService _dayOfTheWeekService;

    public CustomWorker(IDayOfTheWeekService dayOfTheWeekService, Action<IApplicationBuilder> webAppBuilderDelegate,
        Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, 
        params ListenerInfo[] listenerInfo)
    : base(webAppBuilderDelegate, iocDelegate, listenerInfo)
    {
        _dayOfTheWeekService = dayOfTheWeekService;
    }

    protected override async Task CustomWorkerLogic(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"The day is {_dayOfTheWeekService.GetDayOfTheWeek()}");
            Console.WriteLine($"The time is {DateTime.Now.ToLongTimeString()}");
            Console.WriteLine(" --- ");
            await Task.Delay(4000);
        }
    }
}
```
4. Modify your Program.cs with the following (either as a top level statement, or as the content of Main):
```csharp
HostFactory.RunCustomWorker<CustomWorker>(new RunCustomWorkerParams{
    Args = args,
    IocDelegate = (hostContext, webHostContext, collection) =>
    {
        collection.AddScoped<IDayOfTheWeekService, DayOfTheWeekService>();
    },
    ListenerInfo = new []
    {
        new ListenerInfo
        {
            Port = 8152
        }
    }
});
```
Both the call to HostFactory.RunCustomWorker<> and the code in the CustomWorker itself, reference the DayOfTheWeekService. This could be any dependency in your project which you need to inject. The DayOfTheWeekService in the samples is one example:
```csharp
public class DayOfTheWeekService : IDayOfTheWeekService
{
    public string GetDayOfTheWeek()
    {
        return DateTime.Now.DayOfWeek.ToString();
    }
}
```
By declaring IOC bindings in the IocDelegate, you make your dependencies available to your controllers and to any CustomWorkers. The limitation of the first two 'quick start' methods, is that you don't get IOC injection, as you're simply defining a function as the background worker.

>To target Linux, reference the **Helpful.Hosting.WorkerService.Systemd** NuGet package, for Windows, use the **Hosting.WorkerService.Windows** package. Other than the choice of package, there is no difference in the way the library is used.

## Using HostFactory
The different methods available on the HostFactory are:
```csharp
HostFactory.RunApiWorker(RunApiWorkerParams runApiWorkerParams)
```
```csharp
HostFactory.RunApiWorker<TWorker>(RunApiWorkerParams runApiWorkerParams) where TWorker : class, IHostedService
```
```csharp
HostFactory.RunBackgroundTaskWorker(RunBackgroundTaskWorkerParams runBackgroundTaskWorkerParams) 
```
```csharp
HostFactory.RunBackgroundTaskWorker<TWorker>(RunBackgroundTaskWorkerParams runBackgroundTaskWorkerParams) where TWorker : class, IHostedService
```
```csharp
HostFactory.RunCompoundWorker(RunCompoundWorkerParams runCompoundWorkerParams)
```
```csharp
HostFactory.RunCompoundWorker<TWorker>(RunCompoundWorkerParams runCompoundWorkerParams) where TWorker : class, IHostedService
```
```csharp
HostFactory.RunCustomWorker<TWorker>(RunCustomWorkerParams runCustomWorkerParams) where TWorker : class, IHostedService
```
For each method (other than **RunCustomWorker<>()**) a default implementation is provided for **TWorker** which will work fine for most scenarios. If you think you want to provide your own, then I recommend considering the **RunCustomWorker<>()** option, as that gives you far more flexibility. The other generic methods end up being quite restrictive, as they're there predominantly to support the non-generic options.

Each option has a params class associated with it. These params provide the following options:
### Args
```csharp
public string[] Args { get; set; } = { };
```
The **Args** property is there so you can pass any commandline arguments into the host builder.
### IocDelegate
```csharp
public Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> IocDelegate { get; set; } = (hostContext, webHostContext, services) => { };
```
The **IocDelegate** property allows you to declare your ioc bindings. You have two contexts, **hostContext** and **webHostContext**. When the library is resolving IOC in the context of the background worker, the **hostContext** will be used. When the library is resolving IOC in the context of the web API, the **webHostContext** will be used. In either case, the unused context is null.
### WebAppBuilderDelegate
```csharp
public Action<IApplicationBuilder> WebAppBuilderDelegate { get; set; } = app => { };
```
The **WebAppBuilderDelegate** is there to allow you to add your own customisations into the web app builder.
### LogLevel
```csharp
public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
```
[Helpful.Logging.Standard](https://github.com/RokitSalad/Helpful.Logging.Standard) is a dependency, and is configured automatically for Serilog logging. The level of logging for the entire service is set using the **LogLevel** property.
### ListenerInfo
```csharp
public ListenerInfo[] ListenerInfo { get; set; }
```
The **ListenerInfo** property defines any endpoints which you want the API to listen on. It looks like this:
```csharp
 public class ListenerInfo
{
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public bool UseTls { get; set; }
    public StoreName SslCertStoreName { get; set; }
    public string SslCertSubject { get; set; }
    public bool AllowInvalidCert { get; set; }
}
```
Most of these properties should be pretty self explanatory, but to be clear, if **UseTls** is false, the certificate properties can be left null. You can add as many of these as you like, allowing you to listen on multiple ports, IP addresses, and with different certificates for TLS.
### WorkerProcess
```csharp
public Func<CancellationToken, Task> WorkerProcess { get; set; }
```
The **WorkerProcess** is a **Task** which will be ran in the background, when choosing either a compound service or a background process, as your service model. It will be awaited by the default **TWorker** implementations, so it should normally be a loop of some kind, otherwise your service will run for a short time and then stop. The **CancellationToken** is triggered when the service is stopping.
## What's in the future?
It's likely that I'll deprecate the Topshelf elements of this library at some point, to simplify the codebase. The packages will remain in NuGet.org and I'll create a branch with the source code so it's available if anyone wants it.

I haven't done much in CI with the Linux package - I'm likely going to have it spin up a Linux container in either Azure or AWS and deploy the **DemoWorkerDocker** project there to test, and to provide a sample on how to do that.

I may break things down a little, so there's an additional package delivering just a Worker Service, which can be pushed to Azure or used however you like.