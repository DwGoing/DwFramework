using System;
namespace DwFramework.Core
{
    public enum EnvironmentType
    {
        UNKNOW = 0,
        Develop = 1,
        Produce = 2
    }

    public interface IRunEnvironment
    {

    }

    public class RunEnvironment : IRunEnvironment
    {
        public EnvironmentType EnvironmentType { get; private set; }

        public RunEnvironment(EnvironmentType environmentType)
        {
            EnvironmentType = environmentType;
        }
    }
}
