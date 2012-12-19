using System;
using System.Collections.Generic;
using System.Reflection;

namespace NuPattern.Runtime
{
	/// <summary>
	/// A <see cref="WeakObserverEvent{TEventArgs}"/>-aware implementation of 
	/// <see cref="IObservable{T}"/> for events that removes subscribers 
	/// that are not alive anymore.
	/// </summary>
	/// <typeparam name="TDelegate">The type of the delegate.</typeparam>
	/// <typeparam name="TEventArgs">The type of the event args.</typeparam>
	internal class WeakObservableEvent<TDelegate, TEventArgs> : IObservable<IEvent<TEventArgs>>
		where TEventArgs : EventArgs
	{
		private List<IObserver<IEvent<TEventArgs>>> observers = new List<IObserver<IEvent<TEventArgs>>>();
		private Func<EventHandler<TEventArgs>, TDelegate> delegateConversion;
		private Action<TDelegate> addHandler;
		private Action<TDelegate> removeHandler;

		/// <summary>
		/// Initializes a new instance of the <see cref="WeakObservableEvent{TDelegate, TEventArgs}"/> class.
		/// </summary>
		/// <param name="delegateConversion">The delegate conversion function to accomodate non-standard handlers.</param>
		/// <param name="addHandler">The handler to subscribe to the source event.</param>
		/// <param name="removeHandler">The handler to unsubscribe to the source event.</param>
		public WeakObservableEvent(
			Func<EventHandler<TEventArgs>, TDelegate> delegateConversion,
			Action<TDelegate> addHandler,
			Action<TDelegate> removeHandler)
		{
			Guard.NotNull(() => delegateConversion, delegateConversion);
			Guard.NotNull(() => addHandler, addHandler);
			Guard.NotNull(() => removeHandler, removeHandler);

			this.delegateConversion = delegateConversion;
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		/// <summary>
		/// Subscribes the specified observer with this event.
		/// </summary>
		public IDisposable Subscribe(IObserver<IEvent<TEventArgs>> observer)
		{
			if (this.observers.Count == 0)
			{
				this.addHandler(this.delegateConversion(this.OnRaisedTyped));
			}

			this.observers.Add(observer);

			return new AnonymousDisposable(() => this.OnUnsubscribe(observer));
		}

		private void OnUnsubscribe(IObserver<IEvent<TEventArgs>> observer)
		{
			this.observers.Remove(observer);
			if (this.observers.Count == 0)
			{
				this.removeHandler(this.delegateConversion(this.OnRaisedTyped));
			}
		}

		private void OnRaisedTyped(object sender, TEventArgs args)
		{
			// To collect items to remove as we attempt notify.
			var toRemove = new List<IObserver<IEvent<TEventArgs>>>();
			var @event = Event.Create(sender, args);

			foreach (var observer in this.observers.ToArray())
			{
				var weakSubscription = observer as WeakObserverEvent<TEventArgs>;

				if (weakSubscription != null && !weakSubscription.IsAlive)
				{
					toRemove.Add(weakSubscription);
				}
				else
				{
					try
					{
						observer.OnNext(@event);
					}
					catch (TargetInvocationException tie)
					{
						// TODO: what's the expected contract with regards to the OnError callback 
						// in the subscriber?
						tie.InnerException.RethrowWithNoStackTraceLoss();
					}
				}
			}

			// Release the weak subscribers whose target object has been GC'ed.
			foreach (var observer in toRemove)
			{
				this.OnUnsubscribe(observer);
			}
		}
	}
}
