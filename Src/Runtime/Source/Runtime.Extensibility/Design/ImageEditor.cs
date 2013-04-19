using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Design
{
    /// <summary>
    /// Image editor thats allows to select an image file from the solution.
    /// </summary>
    public class ImageEditor : UITypeEditor
    {
        private IServiceProvider provider;

        internal IEnumerable<string> ImageFilePaths { get; set; }

        internal ImageFilterAttribute Filter { get; set; }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "provider", Justification = "NotApplicable"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "NotApplicable", MessageId = "2")]
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Guard.NotNull(() => provider, provider);

            this.provider = provider;

            var valueString = value as string;

            if (valueString != null)
            {
                var editorService = this.provider.GetService<IWindowsFormsEditorService>();

                if (editorService != null)
                {
                    this.SetContext(context);

                    using (var dialog = new ImageDialog(this, this.provider))
                    {
                        Uri @ref = null;

                        if (Uri.TryCreate(valueString, UriKind.Absolute, out @ref))
                        {
                            dialog.ImagePath = ResolveItemReference(@ref);
                        }
                        else
                        {
                            dialog.ImagePath = string.Empty;
                        }

                        if (editorService.ShowDialog(dialog) == DialogResult.OK)
                        {
                            valueString = dialog.ImagePath;

                            if (!string.IsNullOrEmpty(valueString))
                            {
                                value = valueString;
                            }
                        }
                    }
                }
            }

            return CreateItemReference(value);
        }

        internal static bool IsCursorFile(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) &&
                string.Equals(Path.GetExtension(fileName), ".cur", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsIconFile(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) &&
                string.Equals(Path.GetExtension(fileName), ".ico", StringComparison.OrdinalIgnoreCase);
        }

        private void SetContext(ITypeDescriptorContext context)
        {
            if (context != null && context.PropertyDescriptor != null)
            {
                this.Filter = context.PropertyDescriptor.Attributes[typeof(ImageFilterAttribute)] as ImageFilterAttribute;
            }
            else
            {
                this.Filter = null;
            }

            if (this.Filter == null)
            {
                this.Filter = new ImageFilterAttribute(ImageKind.Image);
            }

            this.ImageFilePaths = null;

            if (context != null && context.Instance != null)
            {
                ISolution solution = null;

                if (this.provider == null)
                {
                    solution = Package.GetGlobalService(typeof(ISolution)) as ISolution;
                }
                else
                {
                    solution = this.provider.GetService<ISolution>();
                }

                this.ImageFilePaths = solution.Items.Traverse(item => item.Items)
                    .Where(item => item.Kind == ItemKind.Item &&
                        this.Filter.Extensions.Any(ext => ext.Equals(Path.GetExtension(item.PhysicalPath), StringComparison.OrdinalIgnoreCase)))
                    .Select(item => item.PhysicalPath);
            }
        }

        private object CreateItemReference(object value)
        {
            if (value is string)
            {
                var solution = this.provider.GetService<ISolution>();
                var item = solution.Traverse()
                    .OfType<IItem>()
                    .FirstOrDefault(i => i.PhysicalPath.Equals((string)value, StringComparison.OrdinalIgnoreCase));

                if (item != null)
                {
                    var uriReferenceService = this.provider.GetService<IUriReferenceService>();

                    if (uriReferenceService.CanCreateUri<IItemContainer>(item))
                    {
                        return uriReferenceService.CreateUri<IItemContainer>(item).ToString();
                    }
                }
            }

            return value;
        }

        private string ResolveItemReference(Uri reference)
        {
            var uriReferenceService = this.provider.GetService<IUriReferenceService>();

            var item = uriReferenceService.ResolveUri<IItemContainer>(reference);
            if (item != null)
            {
                return item.PhysicalPath;
            }

            return string.Empty;
        }
    }
}