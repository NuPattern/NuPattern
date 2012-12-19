using System;
using System.Diagnostics;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provides a set of static methods for creating observables that don't hold 
	/// strong references to their subscribers.
	/// </summary>
	[DebuggerStepThrough]
	public static partial class WeakObservable
	{
		/// <summary>
		/// Returns an observable sequence that contains the values of the underlying .NET event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Intended API design.")]
		public static IObservable<IEvent<TEventArgs>> FromEvent<TEventArgs>(Action<EventHandler<TEventArgs>> addHandler, Action<EventHandler<TEventArgs>> removeHandler) where TEventArgs : EventArgs
		{
			Guard.NotNull(() => addHandler, addHandler);
			Guard.NotNull(() => removeHandler, removeHandler);

			return FromEvent<EventHandler<TEventArgs>, TEventArgs>(handler => handler, addHandler, removeHandler);
		}

		/// <summary>
		/// Returns an observable sequence that contains the values of the underlying .NET event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Intended API design.")]
		public static IObservable<IEvent<TEventArgs>> FromEvent<TDelegate, TEventArgs>(
			Func<EventHandler<TEventArgs>, TDelegate> delegateConversion,
			Action<TDelegate> addHandler,
			Action<TDelegate> removeHandler)
			where TEventArgs : EventArgs
		{
			Guard.NotNull(() => delegateConversion, delegateConversion);
			Guard.NotNull(() => addHandler, addHandler);
			Guard.NotNull(() => removeHandler, removeHandler);

			return WeakObservableFactory<TDelegate, TEventArgs>.CreateObservable(delegateConversion, addHandler, removeHandler);
		}

		/// <summary>
		/// Provides a hook for tests to replace the factory for the observer.
		/// </summary>
		internal static class WeakObservableFactory<TDelegate, TEventArgs>
			where TEventArgs : EventArgs
		{
			/// <summary>
			/// Initializes static members of the <see cref="WeakObservableFactory{TDelegate, TEventArgs}"/> class.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "This is the only way to initialize it??")]
			static WeakObservableFactory()
			{
				CreateObservable = (delegateConversion, addHandler, removeHandler) =>
					new WeakObservableEvent<TDelegate, TEventArgs>(delegateConversion, addHandler, removeHandler);
			}

			/// <summary>
			/// Factory for tests.
			/// </summary>
			internal delegate IObservable<IEvent<TEventArgs>> WeakObservableEventFactory(
				Func<EventHandler<TEventArgs>, TDelegate> delegateConversion,
				Action<TDelegate> addHandler,
				Action<TDelegate> removeHandler);

			/// <summary>
			/// Gets or sets the default factory that creates an instance of the actual implementation from <see cref="WeakObservable"/>.
			/// </summary>
			public static WeakObservableEventFactory CreateObservable { get; internal set; }
		}
	}
}
