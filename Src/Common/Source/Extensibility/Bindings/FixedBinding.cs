using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Extensibility.Bindings
{
    /// <summary>
    /// Factory class for creating <see cref="FixedBinding{T}"/> 
    /// for concrete values.
    /// </summary>
    public static class FixedBinding
    {
        /// <summary>
        /// Creates a fixed binding for the specified value.
        /// </summary>
        public static FixedBinding<T> Create<T>(T value)
            where T : class
        {
            return new FixedBinding<T>(value);
        }
    }

    /// <summary>
    /// Implements an <see cref="IDynamicBinding{T}"/> that has a 
    /// fixed value provided at construction time.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class FixedBinding<T> : IDynamicBinding<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedBinding{T}"/> class.
        /// </summary>
        public FixedBinding(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Creates a dummy dynamic context.
        /// </summary>
        public IDynamicBindingContext CreateDynamicContext()
        {
            return new DummyDynamicContext();
        }

        /// <summary>
        /// Always returns <see langword="true"/>.
        /// </summary>
        public bool Evaluate(IDynamicBindingContext context)
        {
            return true;
        }

        /// <summary>
        /// Always returns <see langword="true"/>.
        /// </summary>
        public bool Evaluate()
        {
            return true;
        }

        /// <summary>
        /// Gets the evaluation results, which is always an empty list of <see cref="BindingResult"/>.
        /// </summary>
        public IEnumerable<BindingResult> EvaluationResults { get { return Enumerable.Empty<BindingResult>(); } }

        /// <summary>
        /// Gets whether the binding has errors, which always returns <see langword="false"/>.
        /// </summary>
        public bool HasErrors { get { return false; } }

        /// <summary>
        /// Gets or sets the optional user message.
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Gets the binding value.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// A dummy dynamic context that is never used.
        /// </summary>
        private class DummyDynamicContext : IDynamicBindingContext
        {
            public DummyDynamicContext()
            {
                this.CompositionService = new DummyCompositionService();
            }

            public ICompositionService CompositionService { get; set; }

            public void AddExport<TExport>(TExport instance) where TExport : class
            {
            }

            public void AddExport<TExport>(TExport instance, string contractName) where TExport : class
            {
            }

            public void Dispose()
            {
            }

            private class DummyCompositionService : ICompositionService
            {
                public void SatisfyImportsOnce(System.ComponentModel.Composition.Primitives.ComposablePart part)
                {
                }
            }
        }
    }
}