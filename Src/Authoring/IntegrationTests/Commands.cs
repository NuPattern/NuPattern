using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Shell;

namespace NuPattern.Authoring.IntegrationTests
{
    [CLSCompliant(false)]
    public class Commands : CommandSet
    {
        public Commands(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public void Execute(CommandID id)
        {
            var command = this.GetMenuCommands().FirstOrDefault(menu => menu.CommandID == id);

            if (command == null)
            {
                throw new ArgumentException("Invalid command identifier.");
            }

            command.Invoke();
        }
    }
}
