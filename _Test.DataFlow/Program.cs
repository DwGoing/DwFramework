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
                var key = service.CreateTaskQueue(new TaskHandler(), new ResultHandler());
                service.AddTaskStartHandler<int>(key, (input) => Console.WriteLine($"Start1:{input}"));
                //service.AddTaskStartHandler<int>(key, (input) => Console.WriteLine($"Start2:{input}"));
                service.AddTaskEndHandler<int, int, int>(key, (input, output, result) => Console.WriteLine($"End1:{input} {output} {result}"));
                //service.AddTaskEndHandler<int, int, int>(key, (input, output, result) => Console.WriteLine($"End2:{input} {output} {result}"));
                service.AddInput(key, 1);
                service.AddInput(key, 2);
                service.GetResult(key);
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