﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

using Grpc.Core;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace DwFramework.Rpc
{
    public class RpcService : BaseService
    {
        public class Config
        {
            public string ContentRoot { get; set; }
            public Dictionary<string, string> Listen { get; set; }
        }

        private readonly Config _config;
        private readonly Server _server;

        /// <summary>
        /// 构造函数
        /// </summary>
        public RpcService()
        {
            _config = ServiceHost.Environment.GetConfiguration("ServiceHost").GetConfig<Config>("Rpc");
            _server = new Server();
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public Task OpenServiceAsync()
        {
            return TaskManager.CreateTask(() =>
            {
                if (_config.Listen == null || _config.Listen.Count <= 0) throw new Exception("缺少Listen配置");
                string listen = "";
                // 监听地址及端口
                if (_config.Listen.ContainsKey("http"))
                {
                    string[] ipAndPort = _config.Listen["http"].Split(":");
                    var ip = string.IsNullOrEmpty(ipAndPort[0]) ? "0.0.0.0" : ipAndPort[0];
                    var port = int.Parse(ipAndPort[1]);
                    _server.Ports.Add(new ServerPort(ip, port, ServerCredentials.Insecure));
                    listen += $"http://{ip}:{port}";
                }
                if (_config.Listen.ContainsKey("https"))
                {
                    string[] addrAndCert = _config.Listen["https"].Split(";");
                    string[] ipAndPort = addrAndCert[0].Split(":");
                    var ip = string.IsNullOrEmpty(ipAndPort[0]) ? "0.0.0.0" : ipAndPort[0];
                    var port = int.Parse(ipAndPort[1]);
                    string[] certPaths = addrAndCert[1].Split(",");
                    var rootPath = $"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}";
                    var rootCert = File.ReadAllText($"{rootPath}{certPaths[0]}");
                    var certChain = File.ReadAllText($"{rootPath}{certPaths[1]}");
                    var privateKey = File.ReadAllText($"{rootPath}{certPaths[2]}");
                    var serverCredentials = new SslServerCredentials(new[] { new KeyCertificatePair(certChain, privateKey) }, rootCert, false);
                    _server.Ports.Add(new ServerPort(ip, port, serverCredentials));
                    if (!string.IsNullOrEmpty(listen)) listen += ",";
                    listen += $"https://{ip}:{port}";
                }
                RegisterFuncFromAssemblies();
                _server.Start();
                Console.WriteLine($"Rpc服务已开启 => 监听地址:{listen}");
            });
        }

        /// <summary>
        /// 从程序集中注册Rpc函数
        /// </summary>
        private void RegisterFuncFromAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var item in types)
                {
                    var attr = item.GetCustomAttribute<RpcAttribute>();
                    if (attr == null) continue;
                    var service = ServiceHost.Provider.GetService(item);
                    if (service == null) continue;
                    var method = attr.ImplementType.GetMethod("BindService", new Type[] { item.BaseType });
                    _server.Services.Add((ServerServiceDefinition)method.Invoke(null, new[] { service }));
                }
            }
        }
    }
}