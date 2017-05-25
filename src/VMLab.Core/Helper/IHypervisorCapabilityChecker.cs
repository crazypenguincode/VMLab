using System;
using VMLab.GraphModels;

namespace VMLab
{
    public interface IHypervisorCapabilityChecker
    {
        Tuple<bool, string> CheckTemplate(Template template);
    }
}
