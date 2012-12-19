using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// The default serialization binder used when resolving and loading classes from type names.
    /// </summary>
    public class DefaultSerializationBinder : SerializationBinder
    {
        internal static readonly DefaultSerializationBinder Instance = new DefaultSerializationBinder();

        private readonly ThreadSafeStore<TypeNameKey, Type> _typeCache = new ThreadSafeStore<TypeNameKey, Type>(GetTypeFromTypeNameKey);

        private static Type GetTypeFromTypeNameKey(TypeNameKey typeNameKey)
        {
            string assemblyName = typeNameKey.AssemblyName;
            string typeName = typeNameKey.TypeName;

            if (assemblyName != null)
            {
                Assembly assembly;

#if !SILVERLIGHT && !PocketPC
                // look, I don't like using obsolete methods as much as you do but this is the only way
                // Assembly.Load won't check the GAC for a partial name
#pragma warning disable 618,612
                assembly = Assembly.LoadWithPartialName(assemblyName);
#pragma warning restore 618,612
#else
        assembly = Assembly.Load(assemblyName);
#endif

                if (assembly == null)
                    throw new JsonSerializationException("Could not load assembly '{0}'.".FormatWith(CultureInfo.InvariantCulture, assemblyName));

                Type type = assembly.GetType(typeName);
                if (type == null)
                    throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeName, assembly.FullName));

                return type;
            }
            else
            {
                return Type.GetType(typeName);
            }
        }

        internal struct TypeNameKey : IEquatable<TypeNameKey>
        {
            internal readonly string AssemblyName;
            internal readonly string TypeName;

            public TypeNameKey(string assemblyName, string typeName)
            {
                AssemblyName = assemblyName;
                TypeName = typeName;
            }

            public override int GetHashCode()
            {
                return ((AssemblyName != null) ? AssemblyName.GetHashCode() : 0) ^ ((TypeName != null) ? TypeName.GetHashCode() : 0);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is TypeNameKey))
                    return false;

                return Equals((TypeNameKey)obj);
            }

            public bool Equals(TypeNameKey other)
            {
                return (AssemblyName == other.AssemblyName && TypeName == other.TypeName);
            }
        }

        /// <summary>
        /// When overridden in a derived class, controls the binding of a serialized object to a type.
        /// </summary>
        /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly"/> name of the serialized object.</param>
        /// <param name="typeName">Specifies the <see cref="T:System.Type"/> name of the serialized object.</param>
        /// <returns>
        /// The type of the object the formatter creates a new instance of.
        /// </returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            return _typeCache.Get(new TypeNameKey(assemblyName, typeName));
        }

#if !(NET35 || NET20)
        /// <summary>
        /// When overridden in a derived class, controls the binding of a serialized object to a type.
        /// </summary>
        /// <param name="serializedType">The type of the object the formatter creates a new instance of.</param>
        /// <param name="assemblyName">Specifies the <see cref="T:System.Reflection.Assembly"/> name of the serialized object. </param>
        /// <param name="typeName">Specifies the <see cref="T:System.Type"/> name of the serialized object. </param>
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
#if !SILVERLIGHT
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.FullName;
#else
      assemblyName = null;
      typeName = serializedType.AssemblyQualifiedName;
#endif
        }
#endif
    }
}