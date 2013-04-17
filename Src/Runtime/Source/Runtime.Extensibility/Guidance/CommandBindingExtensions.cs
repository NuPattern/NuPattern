using System;
using System.Collections.Generic;
using System.Linq;

namespace NuPattern.Runtime.Guidance
{
    internal static class CommandBindingExtensions
    {
        public static ICommandBinding FindByName(this IEnumerable<ICommandBinding> commands, string bindingName)
        {
            return commands.FirstOrDefault(binding => binding.Name.Equals(bindingName, StringComparison.OrdinalIgnoreCase));
        }
    }
}