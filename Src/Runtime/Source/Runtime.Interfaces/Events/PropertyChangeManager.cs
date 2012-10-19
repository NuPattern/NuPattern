using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
    /// <summary>
    /// Manages weak references to <see cref="INotifyPropertyChanged"/> 
    /// subscribers and provides a friendly interface for <see cref="INotifyPropertyChanged"/> 
    /// implementors to expose typed event subscription without relying on 
    /// property string names.
    /// </summary>
    /// <remarks>
    /// This class never leaks references to subscribers. 
    /// </remarks>
    [CLSCompliant(true)]
    public class PropertyChangeManager
    {
        private List<Subscription> subscriptions = new List<Subscription>();
        private object source;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeManager"/> class.
        /// </summary>
        /// <param name="source">The property changed source.</param>
        public PropertyChangeManager(object source)
        {
            this.source = source;
        }

        /// <summary>
        /// Subscribes to changes in the property referenced in the given 
        /// <paramref name="propertyExpression"/> with the given 
        /// <paramref name="callbackAction"/> delegate.
        /// </summary>
        /// <param name="propertyExpression">A lambda expression that accesses a property, such as <c>x => x.Name</c>.</param>
        /// <param name="callbackAction">The callback action to invoke when the given property changes.</param>
        public IDisposable SubscribeChanged(Expression propertyExpression, Delegate callbackAction)
        {
            Guard.NotNull(() => propertyExpression, propertyExpression);
            Guard.NotNull(() => callbackAction, callbackAction);

            return this.AddSubscription(new Subscription
            {
                IsStatic = callbackAction.Target == null,
                PropertyName = GetPropertyName(propertyExpression),
                SubscriberReference = new WeakReference(callbackAction.Target),
                MethodCallback = callbackAction.Method
            });
        }

        /// <summary>
        /// Registers a regular event handler for change notification.
        /// </summary>
        public IDisposable AddHandler(PropertyChangedEventHandler handler)
        {
            Guard.NotNull(() => handler, handler);

            return this.AddSubscription(new Subscription
            {
                IsStatic = handler.Target == null,
                SubscriberReference = new WeakReference(handler.Target),
                MethodCallback = handler.Method
            });
        }

        /// <summary>
        /// Unregisters the given event handler from change notification.
        /// </summary>
        /// <param name="handler">The value.</param>
        public void RemoveHandler(PropertyChangedEventHandler handler)
        {
            this.CleanupSubscribers();

            this.subscriptions.RemoveAll(s => s.SubscriberReference.Target == handler.Target && s.MethodCallback == handler.Method);
        }

        /// <summary>
        /// Notifies subscribers that the given property has changed.
        /// </summary>
        /// <param name="propertyExpression">A lambda expression that accesses a property, such as <c>x => x.Name</c> 
        /// (where the type of x is <typeparamref name="TSource"/>).</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Provides aid to intellisense to determine the type of a lambda.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Provides aid to intellisense to determine the type of a lambda.")]
        public void NotifyChanged<TSource>(Expression<Func<TSource, object>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression);

            NotifyChanged(propertyName);
        }

        /// <summary>
        /// Notifies subscribers that the given property has changed.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Provides aid to intellisense to determine the type of a lambda.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Provides aid to intellisense to determine the type of a lambda.")]
        public void NotifyChanged(string propertyName)
        {
            this.CleanupSubscribers();

            foreach (var subscription in this.subscriptions.Where(s => s.PropertyName == propertyName && s.SubscriberReference.IsAlive).ToList())
            {
                try
                {
                    var target = subscription.SubscriberReference.Target;
                    if (target != null)
                        subscription.MethodCallback.Invoke(target, new object[] { this.source });
                }
                catch (TargetInvocationException tie)
                {
                    tie.InnerException.RethrowWithNoStackTraceLoss();
                }
            }

            // Call "old-style" handlers with the right signature.
            foreach (var subscription in this.subscriptions.Where(s => s.PropertyName == null && s.SubscriberReference.IsAlive).ToList())
            {
                try
                {
                    var target = subscription.SubscriberReference.Target;
                    if (target != null)
                        subscription.MethodCallback.Invoke(target, new object[] { this.source, new PropertyChangedEventArgs(propertyName) });
                }
                catch (TargetInvocationException tie)
                {
                    tie.InnerException.RethrowWithNoStackTraceLoss();
                }
            }
        }

        private static string GetPropertyName(Expression propertyExpression)
        {
            var lambda = propertyExpression as LambdaExpression;
            if (lambda == null)
            {
                throw new ArgumentException("Expression is not a lambda.", "propertyExpression");
            }

            MemberExpression memberExpr = null;

            // The Func<TTarget, object> we use returns an object, so first statement can be either 
            // a cast (if the field/property does not return an object) or the direct member access.
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                // The cast is an unary expression, where the operand is the 
                // actual member access expression.
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
            {
                throw new ArgumentException("Expression is not a property access.", "propertyExpression");
            }

            var propertyInfo = memberExpr.Member;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Expression is not a property access.", "propertyExpression");
            }

            return propertyInfo.Name;
        }

        private IDisposable AddSubscription(Subscription subscription)
        {
            this.CleanupSubscribers();

            this.subscriptions.Add(subscription);

            return new SubscriptionReference(this.subscriptions, subscription);
        }

        private void CleanupSubscribers()
        {
            this.subscriptions.RemoveAll(s => !s.IsStatic && !s.SubscriberReference.IsAlive);
        }

        /// <summary>
        /// Provides deterministic removal of a subscription without having to 
        /// create a separate class to hold the delegate reference. 
        /// Callers can simply keep the returned disposable from Subscribe 
        /// and use it to unsubscribe.
        /// </summary>
        private sealed class SubscriptionReference : IDisposable
        {
            private List<Subscription> subscriptions;
            private Subscription entry;

            /// <summary>
            /// Initializes a new instance of the <see cref="SubscriptionReference"/> class.
            /// </summary>
            public SubscriptionReference(List<Subscription> subscriptions, Subscription entry)
            {
                this.subscriptions = subscriptions;
                this.entry = entry;
            }

            /// <summary>
            /// Removes the subscription from the parent manager.
            /// </summary>
            public void Dispose()
            {
                this.subscriptions.Remove(this.entry);
            }
        }

        /// <summary>
        /// Represents an event subscription.
        /// </summary>
        private class Subscription
        {
            /// <summary>
            /// Gets or sets a value indicating whether this subscription is static.
            /// </summary>
            public bool IsStatic { get; set; }

            /// <summary>
            /// Gets or sets the name of the property.
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            /// Gets or sets the subscriber reference.
            /// </summary>
            public WeakReference SubscriberReference { get; set; }

            /// <summary>
            /// Gets or sets the method callback.
            /// </summary>
            public MethodInfo MethodCallback { get; set; }
        }
    }
}
