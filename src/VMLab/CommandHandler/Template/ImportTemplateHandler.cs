using System.Collections.Generic;
using VMLab.Contract;

namespace VMLab.CommandHandler.Import
{
    public class ImportTemplateHandler : BaseParamHandler
    {
        private readonly IVMBuilder _builder;

        public ImportTemplateHandler(IVMBuilder builder, IUsage usage) : base(usage)
        {
            _builder = builder;
        }

        public override string Group => "template";
        public override string[] Handles => new[] { "import", "i" };

        public override void OnHandle(string[] args)
        {
            _builder.ImportTemplate(args[1]);
        }

        public override string UsageDescription => "Imports a template into the VMLab template store!";
    }
}
