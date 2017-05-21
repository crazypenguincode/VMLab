using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMLab.Hypervisor.VMwareWorkstation.VIX
{
    public class VixException : Exception
    {
        public ulong ErrorCode { get; }

        public VixException(string message, ulong errorcode) : base(message)
        {
            ErrorCode = errorcode;
        }
    }
}
