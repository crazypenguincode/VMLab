using System;

namespace VMLab.Contract.Helpers
{
    public class RetryHelper : IRetryHelper
    {
        public void Retry(Action action, Func<Exception, bool> onError, int attempts = 3)
        {
            while (attempts > 0)
            {
                attempts--;

                try
                {
                    action();
                }
                catch(Exception e)
                {
                    if (attempts < 1)
                        throw;

                    if (!onError(e))
                        throw;
                }
            }
        }
    }
}
