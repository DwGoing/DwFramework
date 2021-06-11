using System;

namespace DwFramework.Core
{
    public sealed class NotFoundException : ExceptionBase
    {
        public readonly string FoundThings;

        public NotFoundException(string foundThings) : base(StatusCode.NotFound, $"无法找到\"{foundThings}\"")
        {
            FoundThings = foundThings;
        }
    }
}