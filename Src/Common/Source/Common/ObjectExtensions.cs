using System;

namespace NuPattern
{
    /// <summary>
    /// Extensions to an <see cref="object"/>
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Invokes the provided initializers passing the instance and returning it 
        ///	afterwards.
        /// </summary>
        /// <typeparam name="T">Type of the object instance.</typeparam>
        /// <param name="instance">The instance to apply the initializers to.</param>
        /// <param name="initializers">One or more initializers to apply to the instance.</param>
        /// <returns>The <paramref name="instance"/>.</returns>
        /// <remarks>
        /// Allows to chain calls after an object has been constructed, so that 
        /// further invocations can be done in it without declaring intermediate variables.
        /// </remarks>
        /// <example>
        /// The following example shows how to invoke a factory method, initialize the 
        /// object and continue invoking methods on the original object.
        /// <code>
        /// model
        /// 		.CreatePackage()
        /// 			.CreatePackage()
        /// 				.With(
        /// 					p => p.CreateClass(),
        /// 					p => p.CreateClass(), 
        /// 					p => p.CreateInterface()
        /// 				)
        /// 			.CreatePackage()
        /// 				.With(
        /// 					p => p.CreateClass(),
        /// 					p => p.CreateClass()
        /// 				);
        /// </code>
        /// </example>
        public static T With<T>(this T instance, params Action<T>[] initializers)
        {
            foreach (var initializer in initializers)
            {
                initializer(instance);
            }

            return instance;
        }
    }
}
