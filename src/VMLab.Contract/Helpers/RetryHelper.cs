using System;

namespace VMLab.Contract.Helpers
{
    public class RetryHelper : IRetryHelper
    {
        public void Retry(Action action, int attempts = 3)
        {
            while (attempts > 0)
            {
                attempts--;

                try
                {
                    action();
                }
                catch
                {
                    if (attempts < 1)
                        throw;
                }
            }
        }
    }
}
