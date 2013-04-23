using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace NuPattern.ComponentModel.Design
{
    /// <summary>
    /// Generic multiple values editor
    /// </summary>
    public class CollectionDropDownEditor<TValue> : UITypeEditor
    {
        private bool userAccept;

        /// <summary>
        /// Gets a value indicating whether drop-down editors should be resizable by the user.
        /// </summary>
        /// <value></value>
        /// <returns>true if drop-down editors are resizable; otherwise, false. </returns>
        public override bool IsDropDownResizable
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => provider, provider);

            if (value != null && !typeof(ICollection<TValue>).IsAssignableFrom(value.GetType()))
            {
                throw new InvalidOperationException(Properties.Resources.CollectionDropDownEditor_ValueCollectionException);
            }

            if (!context.PropertyDescriptor.Converter.GetStandardValuesSupported(context))
            {
                throw new InvalidOperationException(Properties.Resources.CollectionDropDownEditor_ConverterException);
            }

            using (var editorControl = new CollectionDropDown<TValue>(context, (ICollection<TValue>)value))
            {
                this.userAccept = true;

                var windowsFormsEditorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

                if (windowsFormsEditorService != null)
                {
                    editorControl.OnCancel += (s, e) =>
                    {
                        windowsFormsEditorService.CloseDropDown();
                        userAccept = false;
                    };

                    editorControl.OnAccept += (s, e) =>
                    {
                        windowsFormsEditorService.CloseDropDown();
                    };

                    try
                    {
                        windowsFormsEditorService.DropDownControl(editorControl);
                    }
                    catch (Win32Exception)
                    {
                    }
                    finally
                    {
                        if (userAccept)
                        {
                            value = editorControl.CheckedValues;
                        }
                    }
                }
            }

            return value;
        }
    }

    /// <summary>
    /// Value item wrapper for multi values drop down
    /// </summary>
    public class ItemValue<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemValue&lt;TValue&gt;"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="value">The value.</param>
        public ItemValue(string description, string displayText, TValue value)
        {
            this.Description = description ?? string.Empty;
            this.DisplayText = displayText ?? string.Empty;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>The display text.</value>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public TValue Value { get; set; }
    }
}