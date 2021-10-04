# Helpful.Hosting.WindowsService.Core
Base package for dotnet core 3.x microservices

## Overview
This package uses [TopShelf](http://topshelf-project.com/) to give a developer fast, 
low code approach to getting a service running. There are different ways to override the default behaviour, allowing a developer to choose between:
* Just my standard configuration
* My configuration plus other options
* Fully implementing their own configuration.

## Quick Start
1. Create a dotnet core commandline project and install Helpful.Hosting.WindowsService.Core from Nuget.org.
2. Make your Program.cs look like this:
```c#
class Program
{
    static void Main(string[] args)
    {
        var runner = new HostRunner("DemoService_QuickStartApi", new ListenerInfo
            {
                Port = 8053
            });
            var exit = runner.RunWebService();
    }
}
```
3. There is no 3, that's it!

Hit F5, and you have a web api listening on port 5002. If you go to http://localhost:8053/healthcheck or 
http://localhost:8053/swagger you'll see that your service is running. Just like TopShelf, because the hostname is * the service
is listening publicly and on localhost.

If you want your service to do more than just respond to http requests, you will want to make this a 
compound service. Try this code:
```c#
class Program
{
    static void Main(string[] args)
    {
        var runner = new HostRunner("DemoService_BackgroundTask", new ListenerInfo
            {
                Port = 8052
            });
            var exit = runner.RunCompoundService(obj => Console.WriteLine($"The time is: {DateTime.Now:T}"), null, 5000);
    }
}
```
Now when you hit F5 you can still see the service has a healthcheck and swagger endpoints (https://localhost:8052/healthcheck)
(https://localhost:8052/swagger), but now you will also see output from the lambda in the command window, every 5 seconds.
Obviously, you can do much more with this than just print the time.

## Samples
There are sample projects included with the source code. The DemoService project shows how to use your own custom 
startup class.
```c#
class CustomStartup
{
    public IConfiguration Configuration { get; }

    public CustomStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        BasicConfiguration.ConfigureBasicServices(services);
        // Add your additional service setup code here
    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        BasicConfiguration.Configure(app, env);
        // Add your additional configuration code here
    }
}
```
and
```c#
class Program
{
    static void Main(string[] args)
    {
        var runner = new HostRunner<CustomStartup>("DemoService", new ListenerInfo
                {
                    Port = 8050
                },
                new ListenerInfo
                {
                    AllowInvalidCert = true,
                    Port = 8051,
                    SslCertStoreName = StoreName.My,
                    SslCertSubject = "CN=localhost", 
                    UseSsl = true
                });
            var exit = runner.RunWebService(LogEventLevel.Debug);
    }
}
```
The bits to take note of here are the calls to the two static setup methods:
```c#
BasicConfiguration.ConfigureBasicServices(services);
...
BasicConfiguration.Configure(app, env);
```
These allow you to still use the standard configuration which you'd get without a custom setup, and then add to it just
like you would in any other setup class.

Other things to note here are the use of a certificate for configuring SSL, and setting the log level to Debug (the default level is Information).

### Options
1. Use all the defaults - *hardly any coding of set up.*
2. Add your own setup class and add more to the defaults - *very small amount of additional setup depending on your requirements.*
3. Add your own setup class and fully implement your own configuration - *a little bit more work.*
4. Completely ignore my quick setup and implement your own HostFactory.Run configuration using TopShelf directly - *still a win as you get the other nice features of this package.*

## Build Status
[![Build Status](https://dev.azure.com/pete0159/Helpful.Libraries/_apis/build/status/RokitSalad.Helpful.Hosting.WindowsService.Core?branchName=master)](https://dev.azure.com/pete0159/Helpful.Libraries/_build/latest?definitionId=4&branchName=master)

## Configuration in More Detail
### Service account
Service credentials can be injected when running any of:
```c#
public class HostRunner<TStartup> where TStartup : class
{
    ...

    public TopshelfExitCode RunWebService(LogEventLevel logLevel = LogEventLevel.Information, Credentials credentials = null)
    {
        ...
    }

    public TopshelfExitCode RunCompoundService(Action<object> serviceAction, object state, int scheduleMilliseconds, LogEventLevel logLevel = LogEventLevel.Information, Credentials credentials = null)
    {
        ...
    }

    public TopshelfExitCode Run(BasicWebService<TStartup> service, LogEventLevel logLevel = LogEventLevel.Information, Credentials credentials = null)
    {
        ...
    }

    ...

}
```
The service will create Swagger and health check endpoints for you automatically, so the account the service is running under must have permissions to reserve TCP ports. For this reason, the default behaviour is to use the Local System account, as this is the only standard account available which will have access to do this. 

*Using the Local System account is not advised. This account has more access than an Administrator.*

### Log levels
By default, the logging level is set to Information. Serilog is the logging package used, so any level of logging available in Serilog is permitted. 
* Verbose
* Debug
* Information
* Warning
* Error
* Fatal

The log level can be injected when running the service in one of the HostRunner methods, above.

### HTTPS
The constructor of HostRunner allows the injection of a collection of ListenerInfo:
```c#
public HostRunner(string serviceName, params ListenerInfo[] listenerInfo)

...

/// <summary>
/// Defines an HTTP binding.
/// </summary>
public class ListenerInfo
{
    /// <summary>
    /// The IP address to listen on. Leave null to listen on all addresses.
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// The port to listen on.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Whether to use TLS.
    /// </summary>
    public bool UseTls { get; set; }

    /// <summary>
    /// The name of the store where the certificate is stored which should be used for this binding. Leave null if UseTls is false.
    /// </summary>
    public StoreName SslCertStoreName { get; set; }

    /// <summary>
    /// The subject of the certificate which should be used for this binding. Leave null if UseTls is false.
    /// </summary>
    public string SslCertSubject { get; set; }

    /// <summary>
    /// A flag to indicate whether to allow the located certificate to be used if it is invalid.
    /// </summary>
    public bool AllowInvalidCert { get; set; }
}
```
In order to respond to HTTPS calls, your service must have access to a certificate identifiable by the store name and subject. Adding multiple ListenerInfo's will result in multiple bindings.

### Helpful.Logging.Standard
[Helpful.Logging.Standard](https://github.com/RokitSalad/Helpful.Logging.Standard) is automatically referenced and configured for logging to the console and to a rolling text file. See the link for usage from your own code.

