using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides an item property.
    /// </summary>
    [DisplayNameResource(@"ItemPropertyValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ItemPropertyValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_VisualStudio", typeof(Resources))]
    [CLSCompliant(false)]
    public class ItemPropertyValueProvider : ValueProvider
    {
        private static readonly ITracer tracer = Tracer.Get<ItemPropertyValueProvider>();

        /// <summary>
        /// The element that owns this value provider.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// The URI service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IUriReferenceService UriService { get; set; }

        /// <summary>
        /// The current solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution { get; set; }

        /// <summary>
        /// The project path.
        /// </summary>
        [DisplayNameResource(@"ItemPropertyValueProvider_ItemPath_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ItemPropertyValueProvider_ItemPath_Description", typeof(Resources))]
        public virtual string ItemPath { get; set; }

        /// <summary>
        /// Gets or sets the expression to evaluate.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"ItemPropertyValueProvider_Property_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ItemPropertyValueProvider_Property_Description", typeof(Resources))]
        public string Property { get; set; }

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.ItemPropertyValueProvider_TraceInitial, this.CurrentElement.InstanceName, this.ItemPath);

            var resolver = new PathResolver(this.CurrentElement, this.UriService, path: (!String.IsNullOrEmpty(this.ItemPath)) ? this.ItemPath : String.Empty);
            if (!resolver.TryResolve(i => i.Kind == ItemKind.Item))
            {
                tracer.Error(
                    Resources.VsProjectPropertyValueProvider_TraceNotResolved, this.ItemPath);

                return string.Empty;
            }

            var item = this.Solution.Find(resolver.Path).FirstOrDefault();

            if (item == null)
            {
                tracer.Warn(
                    Resources.VsProjectPropertyValueProvider_TraceNoItemFound, this.ItemPath, resolver.Path);

                return string.Empty;
            }
            else
            {
                if (item.Kind == ItemKind.Item)
                {
                    var propValue = GetPropertyValue((IItem)item);

                    tracer.Info(
                        Resources.ItemPropertyValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.ItemPath, propValue);

                    return propValue;
                }
                else
                {
                    tracer.Warn(
                        Resources.ItemPropertyValueProvider_TraceNotItem, this.ItemPath, resolver.Path, item.Kind);

                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Returns the value of the project property.
        /// </summary>
        private object GetPropertyValue(IItem item)
        {
            return item.Data[this.Property];
        }
    }
}