using System;
using System.Diagnostics;
using System.Reflection;

namespace NuPattern
{
    /// <summary>
    /// Implements a generic observer that works with lambdas. Only the <c>>onNext</c> is 
    /// required as it's the implementation of the actual event handler.
    /// </summary>
    [DebuggerStepThrough]
    internal class WeakObserverEvent<TEventArgs> : IObserver<IEvent<TEventArgs>>
    {
        private Tuple<WeakReference, MethodInfo> onNext;
        private Tuple<WeakReference, MethodInfo> onError;
        private Tuple<WeakReference, MethodInfo> onCompleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakObserverEvent{TEventArgs}"/> class.
        /// </summary>
        public WeakObserverEvent(Action<IEvent<TEventArgs>> onNext, Action<Exception> onError, Action onCompleted)
        {
            Guard.NotNull(() => onNext, onNext);

            this.onNext = Tuple.Create(GetReferenceToTarget(onNext.Target), onNext.Method);
            this.onError = (onError == null) ? null : Tuple.Create(GetReferenceToTarget(onError.Target), onError.Method);
            this.onCompleted = (onCompleted == null) ? null : Tuple.Create(GetReferenceToTarget(onCompleted.Target), onCompleted.Method);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        /// Returns <see langword="true"/> if this instance is alive; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsAlive
        {
            get
            {
                return this.onNext.Item1.IsAlive;
            }
        }

        /// <summary>
        /// Called when the event source is done raising events.
        /// </summary>
        public void OnCompleted()
        {
            if (this.onCompleted != null && this.onCompleted.Item1.IsAlive)
            {
                RethrowOnException(() => this.onCompleted.Item2.Invoke(this.onCompleted.Item1.Target, null));
            }
        }

        /// <summary>
        /// Called when the event source throws an error.
        /// </summary>
        public void OnError(Exception error)
        {
            if (this.onError != null && this.onError.Item1.IsAlive)
            {
                RethrowOnException(() => this.onError.Item2.Invoke(this.onError.Item1.Target, new object[] { error }));
            }
        }

        /// <summary>
        /// Called when a new event is available from the underlying source..
        /// </summary>
        public void OnNext(IEvent<TEventArgs> value)
        {
            if (this.IsAlive)
            {
                RethrowOnException(() => this.onNext.Item2.Invoke(this.onNext.Item1.Target, new object[] { value }));
            }
        }

        private static WeakReference GetReferenceToTarget(object target)
        {
            if (target == null)
            {
                return new StaticWeakReference();
            }

            return new WeakReference(target);
        }

        private static void RethrowOnException(Action action)
        {
            try
            {
                action();
            }
            catch (TargetInvocationException tie)
            {
                tie.InnerException.RethrowWithNoStackTraceLoss();
            }
        }
    }
}
