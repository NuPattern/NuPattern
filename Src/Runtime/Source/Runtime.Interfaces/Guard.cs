using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using NuPattern.Runtime.Interfaces;

//// DO NOT CHANGE: this root namespace is on purpose one level up from the runtime, 
//// as it's used by the TracingSettings class to specify the root for all logging.
namespace NuPattern
{
    /// <summary>
    /// Validates common arguments.
    /// </summary>
    [DebuggerStepThrough]
    public static class Guard
    {
        /// <summary>
        /// Ensures the given <paramref name="value"/> is not null, otherwise throws <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to be validated.</typeparam>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        public static void NotNull<T>(Expression<Func<T>> reference, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(GetParameterName(reference), Resources.ArgumentCanNotBeNull);
            }
        }

        /// <summary>
        /// Ensures the given <paramref name="value"/> is not null, otherwise throws <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to be validated.</typeparam>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="messageOrFormat">Message to display if the value is null.</param>
        /// <param name="formatArgs">Optional arguments to format the <paramref name="messageOrFormat"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3")]
        public static void NotNull<T>(Expression<Func<T>> reference, T value, string messageOrFormat, params object[] formatArgs)
        {

            if (value == null)
            {
                if (!string.IsNullOrEmpty(messageOrFormat))
                {
                    if (formatArgs != null && formatArgs.Length != 0)
                        throw new ArgumentNullException(GetParameterName(reference), string.Format(CultureInfo.CurrentCulture,
                            messageOrFormat, formatArgs));
                    else
                        throw new ArgumentNullException(GetParameterName(reference), messageOrFormat);
                }
                else
                {
                    throw new ArgumentNullException(GetParameterName(reference), Resources.ArgumentCanNotBeNull);
                }
            }
        }

        /// <summary>
        /// Ensures the given string <paramref name="value"/> is not null or empty. Throws <see cref="ArgumentNullException"/>
        /// in the first case, or <see cref="ArgumentException"/> in the latter.
        /// </summary>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static void NotNullOrEmpty(Expression<Func<string>> reference, string value)
        {
            NotNull(reference, value);
            if (value.Length == 0)
            {
                throw new ArgumentOutOfRangeException(GetParameterName(reference), Resources.ArgumentCanNotBeEmpty);
            }
        }

        /// <summary>
        /// Ensures the given string <paramref name="value"/> is not null or empty. Throws <see cref="ArgumentNullException"/>
        /// in the first case, or <see cref="ArgumentException"/> in the latter.
        /// </summary>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="messageOrFormat">Message to display if the value is null.</param>
        /// <param name="formatArgs">Optional arguments to format the <paramref name="messageOrFormat"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static void NotNullOrEmpty(Expression<Func<string>> reference, string value, string messageOrFormat, params object[] formatArgs)
        {
            NotNull(reference, value, messageOrFormat, formatArgs);
            if (value.Length == 0)
            {
                if (!string.IsNullOrEmpty(messageOrFormat))
                {
                    if (formatArgs != null && formatArgs.Length != 0)
                        throw new ArgumentOutOfRangeException(GetParameterName(reference), string.Format(CultureInfo.CurrentCulture,
                            messageOrFormat, formatArgs));
                    else
                        throw new ArgumentOutOfRangeException(GetParameterName(reference), messageOrFormat);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(GetParameterName(reference), Resources.ArgumentCanNotBeEmpty);
                }
            }
        }

        /// <summary>
        /// Ensures that the value is of the given expected type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2")]
        public static void OfType<T>(Expression<Func<T>> reference, T value, Type expectedType)
        {
            if (value != null && !expectedType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentNullException(GetParameterName(reference), string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ArgumentTypeNotExpected,
                    expectedType.FullName));
            }
        }

        private static string GetParameterName<T>(Expression<T> reference)
        {
            return ((MemberExpression)reference.Body).Member.Name;
        }
    }
}