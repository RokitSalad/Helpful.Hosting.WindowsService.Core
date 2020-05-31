using System;
using Topshelf;

namespace Helpful.Hosting.WindowsService.Core
{
    public class HostRunner : HostRunner<DefaultStartup>
    {
        public HostRunner(string serviceName, int port) : base(serviceName, port)
        {
            
        }
    }

    public class HostRunner<TStartup> where TStartup : class
    {
        private readonly string _serviceName;
        private readonly int _port;

        public HostRunner(string serviceName, int port)
        {
            _serviceName = serviceName;
            _port = port;
        }

        public TopshelfExitCode Run()
        {
            try
            {
                return HostFactory.Run
                    (
                        x =>
                        {
                            x.Service<BasicWebService<TStartup>>
                                (
                                    s =>
                                    {
                                        s.ConstructUsing(svc => new BasicWebService<TStartup>($"http://*:{_port}"));
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
            catch (AggregateException)
            {
                return TopshelfExitCode.AbnormalExit;
            }
            catch (Exception)
            {
                return TopshelfExitCode.AbnormalExit;
            }
        }
    }
}
