using System;
using VMLab.GraphModels;

namespace VMLab.Hypervisor.VMwareWorkstation.VMX
{
    public class GuestOSTranslator : IGuestOSTranslator
    {       
        public string FromVMLabGuestOS(GuestOS os, Arch arch)
        {
            var result = "";

            switch (os)
            {
                case GuestOS.Nano:
                    result = "windows9srv";
                    break;
                case GuestOS.Windows2016:
                    result = "windows9srv";
                    break;
                case GuestOS.Windows2016Core:
                    result = "windows9srv";
                    break;
                case GuestOS.Windows2012R2:
                    result = "windows8srv";
                    break;
                case GuestOS.Windows2012R2Core:
                    result = "windows8srv";
                    break;
                case GuestOS.Windows2012:
                    result = "windows8srv";
                    break;
                case GuestOS.Windows2012Core:
                    result = "windows8srv";
                    break;
                case GuestOS.Windows2008R2:
                    result = "windows7srv";
                    break;
                case GuestOS.Windows2008R2Core:
                    result = "windows7srv";
                    break;
                case GuestOS.Windows10:
                    result = "windows9";
                    break;
                case GuestOS.Windows81:
                    result = "windows8";
                    break;
                case GuestOS.Windows8:
                    result = "windows8";
                    break;
                case GuestOS.Windows7:
                    result = "windows7";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(os), os, null);
            }

            if (arch == Arch.X64)
                result += "-64";

            return result;
        }
    }
}