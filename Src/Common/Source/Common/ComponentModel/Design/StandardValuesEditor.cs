using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;

namespace NuPattern.ComponentModel.Design
{
    /// <summary>
    /// An editor for standard values
    /// </summary>
    public class StandardValuesEditor : UITypeEditor
    {
        /// <summary>
        /// Gets a value that indicates whether the drop down is resizable.
        /// </summary>
        public override bool IsDropDownResizable
        {
            get { return true; }
        }

        /// <summary>
        /// Edits the value.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var converter = context.PropertyDescriptor.Converter;

            if (!converter.GetStandardValuesSupported(context))
            {
                throw new InvalidOperationException("The converter does not support standard values");
            }

            var windowsFormsService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            var editorControl = new NuPattern.ComponentModel.UI.StandardValuesDropDown(windowsFormsService, converter.GetStandardValues(context));

            // If the user press "Esc" we should return the original value            
            // The result variable should be updated with the result of the drop down operation
            var result = true;
            windowsFormsService.DropDownControl(editorControl);

            if (result)
            {
                var selectedValue = editorControl.SelectedValue;

                if (selectedValue != null &&
                    selectedValue.GetType() != context.PropertyDescriptor.PropertyType &&
                    converter.CanConvertFrom(context, selectedValue.GetType()))
                {
                    selectedValue = converter.ConvertFrom(context, CultureInfo.CurrentCulture, selectedValue);
                }

                return selectedValue ?? value;
            }

            return value;
        }

        /// <summary>
        /// Gets the edit style of the editor.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}