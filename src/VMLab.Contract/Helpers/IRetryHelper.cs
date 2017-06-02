using System;

namespace VMLab.Contract.Helpers
{
    public interface IRetryHelper
    {
        void Retry(Action action, int attempts = 3);
    }
}