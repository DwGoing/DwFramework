using System;

namespace DwFramework.DataFlow
{
    public interface ITaskHandler<TInput, TOutput>
    {
        TOutput Invoke(TInput input);
    }
}
