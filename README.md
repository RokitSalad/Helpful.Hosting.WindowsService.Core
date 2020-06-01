# Helpful.Hosting.WindowsService.Core
Base package for dotnet core 3.x microservices

*This is still under development - expect significant changes in implementation until v1.1.*
## Overview
This package uses [TopShelf](http://topshelf-project.com/) to give a developer the fast, 
low code approach to getting a service running. There are different ways to override the default behaviour, 
allowing a developer to choose between:
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
        var runner = new HostRunner("DemoService_QuickStartApi", "http://*:5002");
        var exit = runner.RunWebService();
    }
}
```
3. There is no 3, that's it!

Hit F5, and you have a web api listening on port 5002. If you go to http://localhost:5002/healthcheck or 
http://localhost:5002/swagger you'll see that your service is running. Just like TopShelf, because the hostname is * the service
is listening publicly and on localhost.

If you want your service to do more than just respond to http requests, you will want to make this a 
compound service. Try this code:
```c#
class Program
{
    static void Main(string[] args)
    {
        var runner = new HostRunner("DemoService_BackgroundTask", "https://localhost:5003");
        var exit = runner.RunCompoundService(obj => Console.WriteLine($"The time is: {DateTime.Now:T}"), null, 5000);
    }
}
```
Now when you hit F5 you can still see the service has a healthcheck and swagger endpoints (https://localhost:5003/healthcheck)
(https://localhost:5003/swagger), but now you will also see output from the lambda in the command window, every 5 seconds.
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
        var runner = new HostRunner<CustomStartup>("DemoService", "http://*:5001", "https://*:5011");
        var exit = runner.RunWebService();
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

### Options
1. Use all the defaults - *hardly any coding of set up.*
2. Add your own setup class and add more to the defaults - *very small amount of additional setup depending on your requirements.*
3. Add your own setup class and fully implement your own configuration - *a little bit more work.*
4. Completely ignore my quick setup and implement your own HostFactory.Run configuration using TopShelf directly - *still a win as you get the other nice features of this package.*

## Build Status
[![Build Status](https://dev.azure.com/pete0159/Helpful.Libraries/_apis/build/status/RokitSalad.Helpful.Hosting.WindowsService.Core?branchName=master)](https://dev.azure.com/pete0159/Helpful.Libraries/_build/latest?definitionId=4&branchName=master)