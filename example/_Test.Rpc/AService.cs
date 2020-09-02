using System;
using System.Threading.Tasks;
using Grpc.Core;
using DwFramework.Rpc;

namespace _Test.Rpc
{
    [Rpc]
    public class AService : A.ABase
    {
        public override Task<Response> Do(Request request, ServerCallContext context)
        {
            Console.WriteLine(123);
            return Task.FromResult(new Response()
            {
                Message = request.Message
            });
        }
    }
}
