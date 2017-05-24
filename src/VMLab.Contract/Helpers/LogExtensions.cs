using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace VMLab.Contract.Helpers
{
    public static class LogExtensions
    {
        public static T LogWithObject<T>(this T obj, Action<T> logAction)
        {
            logAction(obj);
            return obj;
        }
    }
}
