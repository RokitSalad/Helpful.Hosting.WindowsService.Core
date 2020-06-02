using System;
using Helpful.Logging.Standard;
using Topshelf;

namespace Helpful.Hosting.WindowsService.Core
{
    public class HostRunner : HostRunner<DefaultStartup>
    {
        public HostRunner(string serviceName, params string[] urls) : base(serviceName, urls)
        {
            
        }
    }

    public class HostRunner<TStartup> where TStartup : class
    {
        private readonly string _serviceName;
        private readonly string[] _urls;

        public HostRunner(string serviceName, params string[] urls)
        {
            _serviceName = serviceName;
            _urls = urls;
        }

        public TopshelfExitCode RunWebService()
        {
            return Run(new BasicWebService<TStartup>(_urls));
        }

        public TopshelfExitCode RunCompoundService(Action<object> singleRun, object state, int scheduleMilliseconds)
        {
            return Run(new TimerService<TStartup>(singleRun, state, scheduleMilliseconds, _urls));
        }

        public TopshelfExitCode Run(BasicWebService<TStartup> service)
        {
            try
            {
                ConfigureLogger.StandardSetup();
                return HostFactory.Run
                (
                    x =>
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
                        x.RunAsLocalSystem();
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
