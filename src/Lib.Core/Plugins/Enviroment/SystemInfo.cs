using System;
using System.Runtime.InteropServices;

namespace DwFramework.Core.Plugins
{
    public class SystemInfo
    {
        public readonly string OSPlatform;
        public readonly string OSDescription;
        public readonly string FrameworkDescription;
        public readonly string SystemVersion;

        public SystemInfo()
        {
            var osPlatforms = new[] { System.Runtime.InteropServices.OSPlatform.Linux, System.Runtime.InteropServices.OSPlatform.OSX, System.Runtime.InteropServices.OSPlatform.Windows };
            foreach (var item in osPlatforms)
            {
                if (RuntimeInformation.IsOSPlatform(item))
                {
                    OSPlatform = item.ToString();
                    OSPlatform += $" {RuntimeInformation.OSArchitecture}";
                    break;
                }
            }
            if (OSPlatform == null) OSPlatform = "Unknow";
            OSDescription = RuntimeInformation.OSDescription;
            FrameworkDescription = RuntimeInformation.FrameworkDescription;
            SystemVersion = RuntimeEnvironment.GetSystemVersion();
        }
    }
}
