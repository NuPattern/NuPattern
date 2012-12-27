using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Binding
{
    /// <summary>
    /// Extensions for evaluating <see cref="IPropertyBindingSettings"/>.
    /// </summary>
    [CLSCompliant(false)]
    public static class PropertyBindingSettingsExtensions
    {
        /// <summary>
        /// Evaluates the specified property binding or throws if it cannot be evaluated.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static string Evaluate(this IPropertyBindingSettings settings, IBindingFactory bindingFactory, ITraceSource tracer, Action<IDynamicBindingContext> contextInitializer = null)
        {
            string result;

            if (settings.ValueProvider != null)
            {
                var binding = bindingFactory.CreateBinding<IValueProvider>(settings.ValueProvider);
                // Make the entire set of element interfaces available to VP bindings.
                // We add the owner with its full interfaces. And we add the IProperty as well.
                using (var context = binding.CreateDynamicContext())
                {
                    if (contextInitializer != null)
                        contextInitializer(context);

                    if (binding.Evaluate(context))
                    {
                        result = binding.Value.Evaluate() as string;
                    }
                    else
                    {
                        var failMessage = string.Format(CultureInfo.CurrentCulture,
                            Resources.ValueProviderBinding_FailedToEvaluate,
                            settings.Name,
                            settings.Name);

                        tracer.TraceData(TraceEventType.Error, 0, new DictionaryTraceRecord(
                            TraceEventType.Error,
                            typeof(PropertyBindingSettingsExtensions).FullName,
                            failMessage,
                            binding.EvaluationResults));

                        throw new InvalidOperationException(failMessage);
                    }
                }
            }
            else
            {
                result = settings.Value;
            }

            return result;
        }

        /// <summary>
        /// Determines whether the binding is configured with a value or value provider
        /// </summary>
        public static bool IsConfigured(this IPropertyBindingSettings settings)
        {
            return (settings.HasValue() || settings.HasValueProvider());
        }

        /// <summary>
        /// Determines whether the binding has a value configured
        /// </summary>
        public static bool HasValue(this IPropertyBindingSettings settings)
        {
            Guard.NotNull(() => settings, settings);

            return !string.IsNullOrEmpty(settings.Value);
        }

        /// <summary>
        /// Determines whether the binding has a value configured
        /// </summary>
        public static bool HasValueProvider(this IPropertyBindingSettings settings)
        {
            Guard.NotNull(() => settings, settings);

            return (settings.ValueProvider == null)
                ? false
                : !string.IsNullOrEmpty(settings.ValueProvider.TypeId);
        }

        /// <summary>
        /// Resets binding.
        /// </summary>
        public static void Reset(this IPropertyBindingSettings settings)
        {
            Guard.NotNull(() => settings, settings);

            settings.Value = BindingSettings.Empty;
            settings.ValueProvider = null;
        }
    }
}
