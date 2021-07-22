using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Consul;
using DwFramework.Core;

namespace DwFramework.Web.Consul
{
    public static class ConsulExtension
    {
        public class Config
        {
            public class Service
            {
                public string Name { get; set; }
                public int Port { get; set; }
            }

            public string Host { get; set; }
            public string ServiceHost { get; set; }
            public int HealthCheckPort { get; set; }
            public string HealthCheckPath { get; set; } = "/consul/healthcheck";
            public Service[] Services { get; set; }
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="lifetime"></param>
        /// <param name="configuration"></param>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration, string configKey = null)
        {
            var config = configuration.ParseConfiguration<Config>(configKey);
            foreach (var item in config.Services)
            {
                var client = new ConsulClient(c =>
                {
                    c.Address = new Uri(config.Host);
                });
                var registration = new AgentServiceRegistration()
                {
                    ID = $"{item.Name}-{config.ServiceHost}:{item.Port}",
                    Name = item.Name,
                    Address = config.ServiceHost,
                    Port = item.Port,
                    Check = new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                        Interval = TimeSpan.FromSeconds(10),
                        Timeout = TimeSpan.FromSeconds(5),
                        HTTP = $"http://{config.ServiceHost}:{config.HealthCheckPort}{config.HealthCheckPath}"
                    }
                };
                client.Agent.ServiceRegister(registration).Wait();
                lifetime.ApplicationStopping.Register(() =>
                {
                    client.Agent.ServiceDeregister(registration.ID).Wait();
                });
            }
            app.Map(config.HealthCheckPath, s => s.Run(context => context.Response.WriteAsync("ok")));
            return app;
        }

        /// <summary>
        /// 执行Consul操作
        /// </summary>
        /// <param name="consulHost"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        private static void Execute(string consulHost, Action<ConsulClient> action, string token = null)
        {
            action(new ConsulClient(c =>
            {
                c.Address = new Uri(consulHost);
                c.Token = token;
            }));
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="consulHost"></param>
        /// <param name="serviceNames"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static AgentService[] GetServices(string consulHost, string[] serviceNames = null, string token = null)
        {
            AgentService[] services = null;
            Execute(consulHost, client =>
            {
                services = client.Agent.Services().Result.Response.Values.ToArray();
                if (serviceNames != null) services = services.Where(item => serviceNames.Contains(item.Service)).ToArray();
            }, token);
            return services;
        }
    }
}
