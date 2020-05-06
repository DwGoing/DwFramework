using Microsoft.Extensions.Configuration;

namespace DwFramework.Core
{
    public interface IEnvironment
    {
        EnvironmentType GetEnvironmentType();
        IConfiguration GetConfiguration();
    }
}
