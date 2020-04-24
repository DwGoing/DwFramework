using System;
using System.Threading;

using DwFramework.Core;
using DwFramework.DataFlow;
using DwFramework.DataFlow.Extensions;

namespace _Test.DataFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceHost host = new ServiceHost();
                host.RegisterDataFlowService();
                var provider = host.Build();
                var service = provider.GetDataFlowService();
                service.CreateTaskQueue("test", new TaskHandler(), new ResultHandler());
                service.AddInput("test", 1);
                service.AddInput("test", 2);
                Thread.Sleep(1000);
                Console.WriteLine(service.GetResult("test"));
                while (true) Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class TaskHandler : ITaskHandler<int, int>
    {
        public int Invoke(int input)
        {
            return input * 10;
        }
    }

    public class ResultHandler : IResultHandler<int, int>
    {
        public int Invoke(int output, int result)
        {
            return output + result;
        }
    }
}
