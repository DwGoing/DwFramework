using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

namespace _Test.Core
{
    public class I<T> where T : X
    {
        public string s { get; set; }
        public T data { get; set; }

        public I(T d)
        {
            data = d;
        }

        public void CCC()
        {
            data.CCC();
        }
    }

    public abstract class X
    {
        public abstract void CCC();
    }

    [Registerable]
    public class A : X
    {
        public override void CCC()
        {
            Console.WriteLine("A");
        }
    }

    [Registerable]
    public class B : X
    {
        public override void CCC()
        {
            Console.WriteLine("B");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop);
                host.RegisterGeneric(typeof(I<>));
                host.RegisterFromAssemblies();
                host.OnInitializing += p =>
                {
                    p.GetService<I<A>>().CCC();
                    p.GetService<I<B>>().CCC();
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
    }
}
