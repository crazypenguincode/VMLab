using System.Collections.Generic;
using VMLab.Contract;

namespace VMLab.CommandHandler.Import
{
    public class ImportTemplateHandler : IParamHandler
    {
        private readonly IVMBuilder _builder;

        public ImportTemplateHandler(IVMBuilder builder)
        {
            _builder = builder;
        }

        public string Group => "template";

        public bool CanHandle(string[] args, IEnumerable<IParamHandler> handlers)
        {
            if (args.Length < 2)
                return false;

            return args[0].ToLower() == "import";
        }

        public void Handle(string[] args)
        {
            _builder.ImportTemplate(args[1]);
        }
    }
}
