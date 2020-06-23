using System;

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
                host.InitService(provider =>
                {
                    var service = provider.GetDataFlowService();
                    var key = service.CreateTaskQueue(new TaskHandler(), new ResultHandler());
                    service.AddTaskStartHandler<int>(key, (input) => Console.WriteLine($"------ Start ------\nInput=>{input}"));
                    service.AddTaskEndHandler<int, int, int>(key, (input, output, result) => Console.WriteLine($"------ End ------\nInput=>{input}\nOutput=>{output}\nResult=>{result}"));
                    service.AddInput(key, 1);
                    service.AddInput(key, 2);
                    service.AddInput(key, 3);
                    service.AddInput(key, 4);
                    service.AddInput(key, 5);
                    service.AddInput(key, 6);
                    service.AddInput(key, 7);
                    service.AddInput(key, 8);
                    service.AddInput(key, 9);
                    service.AddInput(key, 10);
                    service.GetResult(key);
                });
                host.Run();
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