using System;
using System.Security.Cryptography.X509Certificates;
using Helpful.Logging.Standard;
using Topshelf;

namespace Helpful.Hosting.WindowsService.Core
{
    public class HostRunner : HostRunner<DefaultStartup>
    {
        public HostRunner(string serviceName, params ListenerInfo[] listenerInfo) : base(serviceName, listenerInfo)
        {
            
        }
    }

    public class ListenerInfo
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        internal string Protocol => UseSsl ? "https" : "http";
        public StoreName SslCertStoreName { get; set; }
        public string SslCertSubject { get; set; }
        public bool AllowInvalidCert { get; set; }
    }

    public class HostRunner<TStartup> where TStartup : class
    {
        private readonly string _serviceName;
        private readonly ListenerInfo[] _listenerInfo;

        public HostRunner(string serviceName, params ListenerInfo[] listenerInfo)
        {
            _serviceName = serviceName;
            _listenerInfo = listenerInfo;
        }

        public TopshelfExitCode RunWebService()
        {
            return Run(new BasicWebService<TStartup>(_listenerInfo));
        }

        public TopshelfExitCode RunCompoundService(Action<object> singleRun, object state, int scheduleMilliseconds)
        {
            return Run(new TimerService<TStartup>(singleRun, state, scheduleMilliseconds, _listenerInfo));
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
