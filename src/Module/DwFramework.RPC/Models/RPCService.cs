using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Grpc.Core;
using Microsoft.Extensions.Logging;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace DwFramework.RPC
{
    public sealed class RPCService : ConfigableService
    {
        public sealed class Config
        {
            public string ContentRoot { get; init; }
            public Dictionary<string, string> Listen { get; init; }
        }

        private readonly ILogger<RPCService> _logger;
        private Config _config;
        private Server _server;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public RPCService(ILogger<RPCService> logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public void ReadConfig(string path = null, string key = null)
        {
            try
            {
                _config = ReadConfig<Config>(path, key);
                if (_config == null) throw new Exception("未读取到Rpc配置");
            }
            catch (Exception ex)
            {
                _ = _logger?.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            try
            {
                _server = new Server();
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
                if (_logger != null) await _logger?.LogInformationAsync($"RPC服务正在监听:{listen}");
            }
            catch (Exception ex)
            {
                _ = _logger?.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            await _server.KillAsync();
        }

        /// <summary>
        /// 添加RPC服务
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public RPCService AddService(object service)
        {
            _server.Services.Add(GetServerServiceDefinition(service));
            return this;
        }

        /// <summary>
        /// 获取gRPC服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceImpl"></param>
        /// <returns></returns>
        private ServerServiceDefinition GetServerServiceDefinition(object serviceImpl)
        {
            var type = serviceImpl.GetType();
            var baseType = type.BaseType;
            if (baseType.ReflectedType == null) throw new Exception("非gRPC实现");
            var method = baseType.ReflectedType.GetMethod("BindService", new Type[] { baseType });
            if (method == null) throw new Exception("非gRPC实现");
            return (ServerServiceDefinition)method.Invoke(null, new[] { serviceImpl });
        }

        /// <summary>
        /// 从程序集中注册Rpc函数
        /// </summary>
        private void RegisterFuncFromAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            assemblies.ForEach(assembly =>
            {
                var types = assembly.GetTypes();
                types.ForEach(type =>
                {
                    var attr = type.GetCustomAttribute<RPCAttribute>();
                    if (attr == null) return;
                    var service = ServiceHost.Provider.GetService(type);
                    if (service == null) return;
                    _server.Services.Add(GetServerServiceDefinition(service));
                });
            });
        }
    }
}