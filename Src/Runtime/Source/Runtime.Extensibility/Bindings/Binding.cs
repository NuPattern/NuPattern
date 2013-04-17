using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NuPattern.ComponentModel.Composition;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Composition;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.Diagnostics;
using NuPattern.Runtime.Validation;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// Represents a dynamic binding
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Binding<T> : IBinding<T> where T : class
    {
        private Dictionary<string, BindingResult> evaluationResults = new Dictionary<string, BindingResult>();
        private ITraceSource tracer;
        private IFeatureCompositionService featureComposition;
        private PropertyBinding[] propertyBindings;
        private Lazy<T, IFeatureComponentMetadata> lazyValue;
        private string userMessage;
        private string componentTypeId;

        /// <summary>
        /// Creates a new instance of the <see cref="Binding{T}"/> class.
        /// </summary>
        /// <param name="featureComposition"></param>
        /// <param name="componentTypeId"></param>
        /// <param name="propertyBindings"></param>
        public Binding(IFeatureCompositionService featureComposition, string componentTypeId, params PropertyBinding[] propertyBindings)
        {
            Guard.NotNull(() => featureComposition, featureComposition);
            Guard.NotNullOrEmpty(() => componentTypeId, componentTypeId);

            this.componentTypeId = componentTypeId;
            this.featureComposition = featureComposition;
            this.propertyBindings = propertyBindings;
            this.InitializeTracer();

            ResolveComponentTypeId();

            featureComposition.SatisfyImportsOnce(this);
        }

        private void ResolveComponentTypeId()
        {
            this.lazyValue = featureComposition.GetExports<T, IFeatureComponentMetadata>()
                        .FromFeaturesCatalog()
                        .FirstOrDefault(component => component.Metadata.Id == componentTypeId);
        }

        /// <summary>
        /// Gets the evaluated results
        /// </summary>
        public IEnumerable<BindingResult> EvaluationResults
        {
            get { return this.evaluationResults.Values; }
        }

        /// <summary>
        /// Gets the <see cref="IFeatureCompositionService"/> allowing access to MEF.
        /// </summary>
        protected IFeatureCompositionService FeatureComposition
        {
            get { return this.featureComposition; }
        }

        /// <summary>
        /// Gets a value that indicates whether the binding has errors.
        /// </summary>
        public bool HasErrors { get; protected set; }

        /// <summary>
        /// Gets the user readable message about the binding.
        /// </summary>
        public string UserMessage
        {
            get
            {
                if (this.userMessage == null && this.lazyValue != null)
                {
                    this.userMessage = this.lazyValue.Metadata.DisplayName;
                }

                return this.userMessage;
            }
            set
            {
                this.userMessage = value;
            }
        }

        /// <summary>
        /// Gets teh value of the binding.
        /// </summary>
        public T Value
        {
            get { return this.lazyValue == null ? null : this.lazyValue.Value; }
        }

        /// <summary>
        /// Gets the component type identifier.
        /// </summary>
        public string ComponentTypeId { get { return this.componentTypeId; } }

        /// <summary>
        /// Evaluates this binding.
        /// </summary>
        /// <returns><see langword="true"/>, if the evaluation of the binding succeded; otherwise <see langword="false"/>.</returns>
        public virtual bool Evaluate()
        {
            if (FeatureManagerSettings.VerboseBindingTracing)
            {
                tracer.TraceVerbose("Evaluating binding {0}",
                                    this.lazyValue == null ? this.ToString() : this.lazyValue.Metadata.Id, this);
            }
            this.evaluationResults.Clear();

            if (this.lazyValue == null)
            {
                // Re-evaluate component from MEF as new available exports 
                // might have made it available whereas previously it was not.
                ResolveComponentTypeId();
            }

            bool evaluationResult;
            if (this.Value == null)
            {
                evaluationResult = false;

                var bindingResult = new BindingResult("this");
                bindingResult.Errors.Add(string.Format(CultureInfo.CurrentCulture,
                    "Could not resolve component with id '{0}' to an actual instance of type {1}.",
                    this.componentTypeId, typeof(T).Name));

                this.evaluationResults.Add("this", bindingResult);
            }
            else
            {
                var initializable = this.Value as ISupportInitialize;
                var editable = this.Value as IEditableObject;
                if (initializable != null)
                    initializable.BeginInit();
                if (editable != null)
                    editable.BeginEdit();

                featureComposition.SatisfyImportsOnce(this.Value);
                evaluationResult = this.EvaluateProperties();

                if (evaluationResult)
                {
                    evaluationResult = this.Validate();
                    if (evaluationResult)
                    {
                        if (editable != null)
                            editable.EndEdit();
                        if (initializable != null)
                            initializable.EndInit();
                    }
                    else
                    {
                        if (editable != null)
                            editable.CancelEdit();
                    }
                }
                else
                {
                    // We don't call EndInit as we didn't end.
                    if (editable != null)
                        editable.CancelEdit();
                }
            }

            this.HasErrors = !evaluationResult;

            if (FeatureManagerSettings.VerboseBindingTracing)
            {
                tracer.TraceData(TraceEventType.Verbose, 0, this);
            }

            return evaluationResult;
        }

        /// <summary>
        /// Creates a validator for the binding, which derived classes 
        /// can change from the default <see cref="DataAnnotationsValidator"/>.
        /// </summary>
        protected virtual IValidator GetValidator()
        {
            return new DataAnnotationsValidator();
        }

        /// <summary>
        /// Evaluates properties and returns false if any property failed to 
        /// evaluate.
        /// </summary>
        private bool EvaluateProperties()
        {
            if (!this.propertyBindings.Any())
                return true;

            var result = true;
            var properties = TypeDescriptor.GetProperties(this.Value);

            foreach (var propertyBinding in this.propertyBindings)
            {
                var bindResult = new BindingResult(propertyBinding.PropertyName);
                try
                {
                    propertyBinding.SetValue(this.Value);
                    bindResult.Value = properties[propertyBinding.PropertyName].GetValue(this.Value);
                }
                catch (Exception e)
                {
                    bindResult.Errors.Add(e.Message);
                    result = false;
                }

                var valueProviderBinding = propertyBinding as ValueProviderPropertyBinding;
                if (valueProviderBinding != null)
                {
                    bindResult.InnerResults.AddRange(valueProviderBinding.Binding.EvaluationResults);
                }

                this.evaluationResults.Add(propertyBinding.PropertyName, bindResult);
            }

            return result;
        }

        private void InitializeTracer()
        {
            var featureManager = featureComposition.GetExportedValueOrDefault<IFeatureManager>();
            if (featureManager != null)
            {
                var featureRegistration = featureManager.FindFeature(this.GetType());
                if (featureRegistration != null)
                {
                    tracer = FeatureTracer.GetSourceFor<Binding<T>>(featureRegistration.FeatureId);
                }
            }

            if (tracer == null)
                tracer = Tracer.GetSourceFor<Binding<T>>();
        }

        private bool Validate()
        {
            var isValid = true;
            foreach (var validationResult in this.GetValidator().Validate(this.Value))
            {
                BindingResult bindResult;
                if (!this.evaluationResults.TryGetValue(validationResult.PropertyName, out bindResult))
                {
                    bindResult = new BindingResult(validationResult.PropertyName);
                    this.evaluationResults.Add(validationResult.PropertyName, bindResult);
                }

                bindResult.Errors.Add(validationResult.ErrorMessage);
                isValid = false;
            }

            return isValid;
        }
    }
}