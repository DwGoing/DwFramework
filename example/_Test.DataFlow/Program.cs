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
                host.OnInitializing += provider =>
                {
                    var service = provider.GetDataFlowService();
                    var key = service.CreateTaskQueue<int, int, int>(TaskHandler, ResultHandler);
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
                    Console.WriteLine(service.GetResult(key, out var id));
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static int TaskHandler(int value)
        {
            return value++;
        }

        private static int ResultHandler(int value)
        {
            return value++;
        }
    }
}