﻿using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Consul;

using DwFramework.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace DwFramework.WebAPI.Plugins
{
    public static class ConsulManager
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
            public string HealthCheckPath { get; set; }
            public Service[] Services { get; set; }
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration, IHostApplicationLifetime lifetime)
        {
            var config = configuration.GetConfig<Config>("Consul");
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
            app.Map(config.HealthCheckPath, s =>
            {
                s.Run(context => context.Response.WriteAsync("ok"));
            });
            return app;
        }
    }
}
