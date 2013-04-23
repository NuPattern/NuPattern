using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Composition;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// A binding that allows creating and populating a dynamic context for 
    /// binding evaluation.
    /// </summary>
    [CLSCompliant(false)]
    public class DynamicBinding<T> : Binding<T>, IDynamicBinding<T>
        where T : class
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<DynamicBinding<T>>();

        private INuPatternCompositionService originalComposition;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicBinding{T}"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller owns this instance.")]
        public DynamicBinding(INuPatternCompositionService composition, string componentTypeId, params PropertyBinding[] propertyBindings)
            : base(composition, componentTypeId, propertyBindings)
        {
            var delegatingComposition = this.CompositionService as DelegatingCompositionService;
            if (delegatingComposition != null)
            {
                this.originalComposition = delegatingComposition.CompositionService;
            }
        }

        /// <summary>
        /// Creates the context for providing dynamic values for binding evaluation.
        /// </summary>
        public IDynamicBindingContext CreateDynamicContext()
        {
            // This field it set at construction time just for delegating composition which 
            // is the only one that supports dynamic contexts.
            if (this.originalComposition == null)
            {
                throw new NotSupportedException(Resources.DynamicBinding_UnsupportedCompositionService);
            }

            return new CompositionServiceBindingContext(this.originalComposition);
        }

        /// <summary>
        /// Evaluates this instance.
        /// </summary>
        public override bool Evaluate()
        {
            // TODO this is overriden because Feature builder do not validate properties that have an import
            var evaluationResult = base.Evaluate() && this.EvaluateImports();
            this.HasErrors = !evaluationResult;
            return evaluationResult;
        }

        /// <summary>
        /// Evaluates the binding with the additional exports provided in the given context.
        /// </summary>
        public bool Evaluate(IDynamicBindingContext context)
        {
            Guard.NotNull(() => context, context);

            var compositionContext = context as CompositionServiceBindingContext;
            if (compositionContext == null)
            {
                throw new ArgumentException(Resources.BindingFactory_UnsupportedDynamicContext);
            }

            // Since we're the only ones that can create the valid binding context, 
            // and we're validating that the base FeatureComposition is a Delegating one 
            // at CreateDynamicContext time, we can simply cast here.
            var delegatingComposition = (DelegatingCompositionService)this.CompositionService;

            try
            {
                using (var compositionService = new ContainerCompositionServiceAdapter(compositionContext.Container))
                {
                    delegatingComposition.CompositionService = compositionService;
                    return this.Evaluate();
                }
            }
            finally
            {
                delegatingComposition.CompositionService = this.originalComposition;
            }
        }

        private bool EvaluateImports()
        {
            // TODO get rid of this if the bug in feature builder is fixed.
            var properties = TypeDescriptor.GetProperties(this.Value)
                .Cast<PropertyDescriptor>()
                .Where(x => x.Attributes.OfType<ImportAttribute>().Any());

            var evaluationResult = true;
            foreach (var property in properties)
            {
                var results = Validate(this.Value, property.Name);

                foreach (var result in results)
                {
                    tracer.TraceError(Resources.DynamicBinding_TraceFailedDynamicBinding, this.ComponentTypeId, property.Name, result.ErrorMessage);
                    evaluationResult = false;
                }
            }

            return evaluationResult;
        }

        private static IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> Validate(object component, string propertyName)
        {
            //TODO: Replace this method with proper method of Validator.Validate()
            Guard.NotNull(() => component, component);
            Guard.NotNull(() => propertyName, propertyName);

            return from descriptor in TypeDescriptor.GetProperties(component).Cast<PropertyDescriptor>()
                   where descriptor.Name.Equals(propertyName, StringComparison.Ordinal)
                   from validation in descriptor.Attributes.OfType<ValidationAttribute>()
                   where !validation.IsValid(descriptor.GetValue(component))
                   select new System.ComponentModel.DataAnnotations.ValidationResult(GetErrorMessage(validation, descriptor.DisplayName), new[] { descriptor.Name });
        }
        private static string GetErrorMessage(ValidationAttribute validation, string displayName)
        {
            return validation.FormatErrorMessage(displayName) ?? string.Format(Resources.Culture, Resources.DynamicBinding_ValidatorDefaultMessage, validation.GetType().Name);
        }
    }
}
