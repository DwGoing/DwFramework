using System;

namespace DwFramework.DataFlow
{
    public interface IResultHandler<TOutput, TResult>
    {
        TResult Invoke(TOutput output, TResult result);
    }
}
