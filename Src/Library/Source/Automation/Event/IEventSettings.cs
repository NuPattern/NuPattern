﻿using System.Collections.Generic;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    partial interface IEventSettings
    {
        /// <summary>
        /// Gets the condition settings.
        /// </summary>
        IEnumerable<IBindingSettings> ConditionSettings { get; }
    }
}