using System;
using System.Collections;
using System.ComponentModel;
using Microsoft.VisualStudio.Patterning.Extensibility.UI;

namespace $rootnamespace$
{
    /// <summary>
    /// A custom UI editor for displaying types in the current solution filtered to specific base classes or interfaces.
    /// </summary>
    /// <remarks>Change the value of the <see cref="EditorBaseTypeAttribute"/>to apply a filter on the displayed types in the solution.</remarks>
    [DisplayName("$safeitemname$ Custom Type Picker Editor")]
    [Category("General")]
    [Description("Edits the value with a type picker filtered for types found in the solution.")]
    [CLSCompliant(false)]
    [EditorBaseType(typeof(object))]
    public class $safeitemname$ : TypePicker
    {
    }
}