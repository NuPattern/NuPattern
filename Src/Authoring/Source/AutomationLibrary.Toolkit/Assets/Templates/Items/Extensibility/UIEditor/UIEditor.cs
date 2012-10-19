using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace $rootnamespace$
{
    /// <summary>
    /// A custom UI editor for displaying values.
    /// </summary>
    /// <remarks>See <see cref="UITypeEditor"/> for more details.</remarks>
    [DisplayName("$safeitemname$ Custom UI Editor")]
    [Category("General")]
    [Description("Edits a value with a custom UI designer.")]
    [CLSCompliant(false)]
    public class $safeitemname$ : UITypeEditor
    {
        /// <summary>
        /// Returns the editor style.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return base.GetEditStyle(context);
        }

        /// <summary>
        /// Gets a value determining whether the editor drop down is resizable.
        /// </summary>
        public override bool IsDropDownResizable
        {
            get
            {
                return base.IsDropDownResizable;
            }
        }

        /// <summary>
        /// Called to display the custom user interface for the current value.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return base.EditValue(context, provider, value);
        }
    }
}