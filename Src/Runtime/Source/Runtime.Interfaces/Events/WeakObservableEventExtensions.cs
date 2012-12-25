using System;
using System.Diagnostics;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provides a set of static methods for subscribing delegates to observables in a weak 
	/// fashion (avoids managed memory leaks). 
	/// </summary>
	[DebuggerStepThrough]
	public static class WeakObservableEventExtensions
	{
		/// <summary>
		/// Subscribes a value handler to an observable sequence. 
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Intended API design.")]
		public static IDisposable Subscribe<TEventArgs>(this IObservable<IEvent<TEventArgs>> source, Action<IEvent<TEventArgs>> onNext)
			where TEventArgs : EventArgs
		{
			Guard.NotNull(() => source, source);
			Guard.NotNull(() => onNext, onNext);

			return source.Subscribe<TEventArgs>(onNext, null, null);
		}

		/// <summary>
		/// Subscribes a value handler and an exception handler to an observable sequence. 
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Intended API design.")]
		public static IDisposable Subscribe<TEventArgs>(this IObservable<IEvent<TEventArgs>> source, Action<IEvent<TEventArgs>> onNext, Action<Exception> onError)
			where TEventArgs : EventArgs
		{
			Guard.NotNull(() => source, source);
			Guard.NotNull(() => onNext, onNext);

			return source.Subscribe<TEventArgs>(onNext, onError, null);
		}

		/// <summary>
		/// Subscribes a value handler and a completion handler to an observable sequence. 
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Intended API design.")]
		public static IDisposable Subscribe<TEventArgs>(this IObservable<IEvent<TEventArgs>> source, Action<IEvent<TEventArgs>> onNext, Action onCompleted)
			where TEventArgs : EventArgs
		{
			Guard.NotNull(() => source, source);
			Guard.NotNull(() => onNext, onNext);

			return source.Subscribe<TEventArgs>(onNext, null, onCompleted);
		}

		/// <summary>
		/// Subscribes a value handler, an exception handler, and a completion handler to an observable sequence. 
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Intended API design.")]
		public static IDisposable Subscribe<TEventArgs>(this IObservable<IEvent<TEventArgs>> source, Action<IEvent<TEventArgs>> onNext, Action<Exception> onError, Action onCompleted)
			where TEventArgs : EventArgs
		{
			Guard.NotNull(() => source, source);
			Guard.NotNull(() => onNext, onNext);

			return source.Subscribe(WeakObserverFactory<TEventArgs>.CreateObserver(onNext, onError, onCompleted));
		}

		/// <summary>
		/// Provides a hook for tests to replace the factory for the observer.
		/// </summary>
		internal static class WeakObserverFactory<TEventArgs>
			where TEventArgs : EventArgs
		{
			/// <summary>
			/// Initializes static members of the <see cref="WeakObserverFactory{TEventArgs}"/> class.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "This is the only way to initialize it??")]
			static WeakObserverFactory()
			{
				CreateObserver = (onNext, onError, onCompleted) =>
					new WeakObserverEvent<TEventArgs>(onNext, onError, onCompleted);
			}

			/// <summary>
			/// Factory for tests.
			/// </summary>
			internal delegate IObserver<IEvent<TEventArgs>> WeakObserverFactoryHandler(
				Action<IEvent<TEventArgs>> onNext,
				Action<Exception> onError,
				Action onCompleted);

			/// <summary>
			/// Gets or sets the default factory that creates an instance of the actual implementation from <see cref="WeakObservable"/>.
			/// </summary>
			public static WeakObserverFactoryHandler CreateObserver { get; internal set; }
		}
	}
}
