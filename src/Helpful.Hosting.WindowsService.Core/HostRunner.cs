using System;
using System.Security;
using Helpful.Hosting.Dto;
using Helpful.Logging.Standard;
using Serilog.Events;
using Topshelf;
using Credentials = Helpful.Hosting.Dto.Credentials;

namespace Helpful.Hosting.WindowsService.Core
{
    /// <summary>
    /// The <c>HostRunner</c> to use if you intend to use the default service intialisation.
    /// </summary>
    public class HostRunner : HostRunner<DefaultStartup>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceName">The name of the service. This will appear in the Services list.</param>
        /// <param name="listenerInfo">The details of what HTTP endpoints will be listening.</param>
        public HostRunner(string serviceName, params ListenerInfo[] listenerInfo) : base(serviceName, listenerInfo)
        {
            
        }
    }

    /// <summary>
    /// The <c>HostRunner</c> to use if you want to create your own <typeparamref name="TStartup"/> class.
    /// </summary>
    /// <typeparam name="TStartup">The <c>Startup</c> class which will be used to initialise your service.</typeparam>
    public class HostRunner<TStartup> where TStartup : class
    {
        private readonly string _serviceName;
        private readonly ListenerInfo[] _listenerInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceName">The name of the service. This will appear in the Services list.</param>
        /// <param name="listenerInfo">The details of what HTTP endpoints will be listening.</param>
        public HostRunner(string serviceName, params ListenerInfo[] listenerInfo)
        {
            _serviceName = serviceName;
            _listenerInfo = listenerInfo;
        }

        /// <summary>
        /// Run a web service, which could be an API or a web application.
        /// </summary>
        /// <param name="logLevel">The minimum logging level to use. Defaults to Information</param>
        /// <param name="credentials">Credentials to use for your service. Will default to using the Local System account as this has permissions to reserve TCP ports.</param>
        /// <returns><c>TopshelfExitCode</c></returns>
        public TopshelfExitCode RunWebService(LogEventLevel logLevel = LogEventLevel.Information, Credentials credentials = null)
        {
            return Run(new BasicWebService<TStartup>(_listenerInfo), logLevel, credentials);
        }

        /// <summary>
        /// Run a compound service, comprised of a web service and a background task.
        /// </summary>
        /// <param name="serviceAction">The service logic, executed repeatedly on a schedule determined by scheduleMilliseconds.</param>
        /// <param name="state">A state object which can be passed into the service and may persist state between runs.</param>
        /// <param name="scheduleMilliseconds">The number of milliseconds to wait after triggering the service logic before triggering it again.</param>
        /// <param name="logLevel">The minimum logging level to use. Defaults to Information.</param>
        /// <param name="credentials">Credentials to use for your service. Will default to using the Local System account as this has permissions to reserve TCP ports.</param>
        /// <returns><c>TopshelfExitCode</c></returns>
        public TopshelfExitCode RunCompoundService(Action<object> serviceAction, object state, int scheduleMilliseconds, LogEventLevel logLevel = LogEventLevel.Information, Credentials credentials = null)
        {
            return Run(new TimerService<TStartup>(serviceAction, state, scheduleMilliseconds, _listenerInfo), logLevel, credentials);
        }

        /// <summary>
        /// Run any kind of service dictated by <paramref name="service"/>.
        /// </summary>
        /// <param name="service">The service to run.</param>
        /// <param name="logLevel">The minimum logging level to use. Defaults to Information.</param>
        /// <param name="credentials">Credentials to use for your service. Will default to using the Local System account as this has permissions to reserve TCP ports.</param>
        /// <returns><c>TopshelfExitCode</c></returns>
        public TopshelfExitCode Run(BasicWebService<TStartup> service, LogEventLevel logLevel = LogEventLevel.Information, Credentials credentials = null)
        {
            try
            {
                ConfigureLogger.StandardSetup(logLevel: logLevel);
                return HostFactory.Run
                (
                    x =>
                    {
                        try
                        {
                            x.Service<BasicWebService<TStartup>>
                            (
                                s =>
                                {
                                    s.ConstructUsing(svc => service);
                                    s.WhenStarted((ts, hc) => ts.Start(hc));
                                    s.WhenStopped((ts, hc) => ts.Stop(hc));
                                }
                            );
                        
                            x.SetServiceName(_serviceName);
                            x.SetDisplayName(_serviceName);
                            x.SetDescription(_serviceName);
                            x.EnableShutdown();
                            if(credentials == null)
                            {
                                x.RunAsLocalSystem();
                            }
                            else
                            {
                                x.RunAs(credentials.Username, credentials.Password);
                            }

                        }
                        catch (AggregateException ae)
                        {
                            ae.Handle((e) =>
                            {
                                this.GetLogger().LogFatalWithContext(e, "Fatal exception while attempting to start service.");
                                return false;
                            });
                            throw;
                        }
                        catch (Exception e)
                        {
                            this.GetLogger().LogFatalWithContext(e, "Fatal exception while attempting to start service.");
                            throw;
                        }
                    }
                );
            }
            catch (HelpfulLoggingConfigurationException)
            {
                throw;
            }
            catch (AggregateException ae)
            {
                ae.Handle((e) =>
                {
                    this.GetLogger().LogFatalWithContext(e, "Fatal exception while attempting to start service.");
                    return true;
                });
                return TopshelfExitCode.AbnormalExit;
            }
            catch (Exception e)
            {
                this.GetLogger().LogFatalWithContext(e, "Fatal exception while attempting to start service.");
                return TopshelfExitCode.AbnormalExit;
            }
        }
    }
}
