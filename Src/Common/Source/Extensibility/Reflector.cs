using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Provides strong-typed reflection of the <typeparamref name="TTarget"/> 
	/// type.
	/// </summary>
	/// <typeparam name="TTarget">Type to reflect.</typeparam>
	public static class Reflector<TTarget>
	{
		/// <summary>
		/// Gets the method represented by the lambda expression.
		/// </summary>
		/// <param name="method">An expression that invokes a method.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
		/// <returns>The method info.</returns>
		public static MethodInfo GetMethod(Expression<Action<TTarget>> method)
		{
			return GetMethodInfo(method);
		}

		/// <summary>
		/// Gets the method represented by the lambda expression.
		/// </summary>
		/// <param name="method">An expression that invokes a method.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
		/// <returns>The method info.</returns>
		public static MethodInfo GetMethod<T1>(Expression<Action<TTarget, T1>> method)
		{
			return GetMethodInfo(method);
		}

		/// <summary>
		/// Gets the method represented by the lambda expression.
		/// </summary>
		/// <param name="method">An expression that invokes a method.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
		/// <returns>The method info.</returns>
		public static MethodInfo GetMethod<T1, T2>(Expression<Action<TTarget, T1, T2>> method)
		{
			return GetMethodInfo(method);
		}

		/// <summary>
		/// Gets the method represented by the lambda expression.
		/// </summary>
		/// <param name="method">An expression that invokes a method.</param>
		/// <typeparam name="T1">Type of the first argument.</typeparam>
		/// <typeparam name="T2">Type of the second argument.</typeparam>
		/// <typeparam name="T3">Type of the third argument.</typeparam>
		/// <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
		/// <returns>The method info.</returns>
		public static MethodInfo GetMethod<T1, T2, T3>(Expression<Action<TTarget, T1, T2, T3>> method)
		{
			return GetMethodInfo(method);
		}

		/// <summary>
		/// Gets the name of the property represented by the lambda expression.
		/// </summary>
		/// <param name="property">An expression that accesses a property.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="property"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property"/> is not a lambda expression or it does not represent a property access.</exception>
		/// <returns>The property info.</returns>
		public static string GetPropertyName<TResult>(Expression<Func<TTarget, TResult>> property)
		{
			return GetProperty(property).Name;
		}

		/// <summary>
		/// Gets the property represented by the lambda expression.
		/// </summary>
		/// <param name="property">An expression that accesses a property.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="property"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="property"/> is not a lambda expression or it does not represent a property access.</exception>
		/// <returns>The property info.</returns>
		public static PropertyInfo GetProperty<TResult>(Expression<Func<TTarget, TResult>> property)
		{
			PropertyInfo info = GetMemberInfo(property) as PropertyInfo;
			if (info == null)
			{
				throw new ArgumentException("Member is not a property");
			}

			return info;
		}

		/// <summary>
		/// Gets the field represented by the lambda expression.
		/// </summary>
		/// <param name="field">An expression that accesses a field.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="field"/> is null.</exception>
		/// <exception cref="ArgumentException">The <paramref name="field"/> is not a lambda expression or it does not represent a field access.</exception>
		/// <returns>The field info.</returns>
		public static FieldInfo GetField<TResult>(Expression<Func<TTarget, TResult>> field)
		{
			FieldInfo info = GetMemberInfo(field) as FieldInfo;
			if (info == null)
			{
				throw new ArgumentException("Member is not a field");
			}

			return info;
		}

		private static MethodInfo GetMethodInfo(LambdaExpression lambda)
		{
			Guard.NotNull(() => lambda, lambda);

			if (lambda.Body.NodeType != ExpressionType.Call)
			{
				throw new ArgumentException("Not a method call", "lambda");
			}

			return ((MethodCallExpression)lambda.Body).Method;
		}

		private static MemberInfo GetMemberInfo(LambdaExpression lambda)
		{
			Guard.NotNull(() => lambda, lambda);

			if (lambda.Body.NodeType == ExpressionType.MemberAccess)
			{
				return ((MemberExpression)lambda.Body).Member;
			}
			else
			{
				throw new ArgumentException("Not a member access", "lambda");
			}
		}
	}
}