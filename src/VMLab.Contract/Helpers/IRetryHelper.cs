using System;

namespace VMLab.Contract.Helpers
{
    public interface IRetryHelper
    {
        void Retry(Action action, Func<Exception, bool> onError, int attempts = 3);
    }
}