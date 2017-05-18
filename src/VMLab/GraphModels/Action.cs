using System;
using VMLab.Script.FluentInterface;

namespace VMLab.GraphModels
{
    public class Action
    {
        public string Name { get; set; }
        public Action<string[], ISession> OnAction { get; set; }
    }
}
