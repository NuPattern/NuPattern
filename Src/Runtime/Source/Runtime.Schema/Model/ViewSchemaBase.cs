using System.Globalization;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    ///  Defines a view in a model element.
    /// </summary>
    partial class ViewSchemaBase
    {
        private string GetCaptionValue()
        {
            return this.IsDefault ?
                string.Format(CultureInfo.CurrentUICulture, "{0} ({1})", this.Name, Properties.Resources.Default) : this.Name;
        }
    }
}