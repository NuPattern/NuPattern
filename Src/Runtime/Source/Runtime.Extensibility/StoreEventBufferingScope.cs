using System;
using System.Collections.Generic;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Scope for buffering runtime store events. Use when you need to delay the raising of the events.
    /// </summary>
    /// <example>
    /// using (new StoreEventBuggeringScope())
    /// {
    ///		//code that needs the events not to be raised
    /// }
    /// </example>
    public class StoreEventBufferingScope : IDisposable
    {
        private static readonly ITracer tracer = Tracer.Get<StoreEventBufferingScope>();
        private static Stack<StoreEventBufferingScope> scopes = new Stack<StoreEventBufferingScope>(new StoreEventBufferingScope[] { null });
        private static bool RaisingEvents { get; set; }

        private List<Action> events;
        private bool autoComplete;
        private bool completed;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreEventBufferingScope"/> class.
        /// </summary>
        public StoreEventBufferingScope(bool autoComplete = false)
        {
            this.events = new List<Action>();
            this.autoComplete = autoComplete;

            scopes.Push(this);

            tracer.Verbose(Resources.StoreEventBufferingScope_Initialized);
        }

        /// <summary>
        /// Gets a value indicating whether there is an active scope.
        /// </summary>
        public static bool IsActive
        {
            get { return StoreEventBufferingScope.Current != null; }
        }

        /// <summary>
        /// Gets the current scope.
        /// </summary>
        public static StoreEventBufferingScope Current
        {
            get { return scopes.Peek(); }
        }

        /// <summary>
        /// Gets whether the scope or any of its child scopes has been cancelled.
        /// </summary>
        public bool IsCanceled { get; private set; }

        /// <summary>
        /// Completes the scope by raising all buffered events in the same order they were originally captured.
        /// </summary>
        public void Complete()
        {
            this.completed = true;
            tracer.Verbose(Resources.StoreEventBufferingScope_MarkedCompleted);
        }

        /// <summary>
        /// Cancels this instance without raising any of the buffered events.
        /// </summary>
        public void Cancel()
        {
            completed = false;
            this.IsCanceled = true;
            tracer.Verbose(Resources.StoreEventBufferingScope_MarkedCanceled);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="StoreEventBufferingScope"/> class.
        /// </summary>
        ~StoreEventBufferingScope()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raises all buffered events if <see cref="Complete"/> was called. Otherwise, 
        /// clears all events. 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void AddEvent(Action raiseEvent)
        {
            if (this.IsCanceled)
            {
                tracer.Verbose(Resources.StoreEventBufferingScope_CancelledScopeIgnoringEvent);
            }
            else
            {
                this.events.Add(raiseEvent);
            }
        }

        /// <summary>
        /// If a scope is active, buffers the event. Otherwise, directly raises it.
        /// </summary>
        public static void RaiseOrBufferEvent(Action raiseEvent)
        {
            if (Current != null)
            {
                tracer.Verbose(Resources.StoreEventBufferingScope_BufferingEvent);
                Current.AddEvent(raiseEvent);
            }
            else
            {
                raiseEvent();
            }
        }

        /// <summary>
        /// If a scope is active or events are being raised, drops the event. Otherwise, directly raises it.
        /// </summary>
        public static void RaiseOrDropEvent(Action raiseEvent)
        {
            if (RaisingEvents || Current != null)
            {
                // Drop the event, no need to raise or buffer it.
            }
            else
            {
                raiseEvent();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                while (Current != this && Current != null)
                {
                    Current.IsCanceled = this.IsCanceled;
                    // This would automatically pop the nested scopes.
                    Current.Dispose();
                }

                // Pop myself now.
                scopes.Pop();

                if (this.IsCanceled)
                {
                    tracer.Verbose(Resources.StoreEventBufferingScope_CancelledScopeIgnoringEvent);

                    // If we have been cancelled, we don't waste time copying our events to the potential parent 
                    // scope as they will never be raised anyways.
                }
                else if (this.completed || this.autoComplete)
                {
                    if (Current != null)
                    {
                        tracer.Verbose(Resources.StoreEventBufferingScope_CompleteEnqueingOnParent);

                        // There's another parent scope. Append our events to the 
                        // end of its queue for it to raise them later when it is itself 
                        // disposed in turn.
                        this.events.ForEach(ev => Current.AddEvent(ev));
                    }
                    else
                    {
                        tracer.Verbose(Resources.StoreEventBufferingScope_RaisingEvents);

                        // We're the topmost scope, so we raise all buffered events now.
                        foreach (var raiseEvent in this.events)
                        {
                            RaisingEvents = true;
                            try
                            {
                                raiseEvent();
                            }
                            finally
                            {
                                RaisingEvents = false;
                            }
                        }
                    }
                }
                else
                {
                    tracer.Verbose(Resources.StoreEventBufferingScope_DisposingNonComplete);
                }

                // Propagate cancellation flag upwards.
                if (Current != null)
                    Current.IsCanceled = this.IsCanceled;

                this.disposed = true;
            }
        }
    }
}
