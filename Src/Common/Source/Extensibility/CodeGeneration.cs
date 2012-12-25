using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Base class with general-purpose methods useful for code generation.
	/// </summary>
	public class CodeGeneration : ISupportInitialize
	{
		/// <summary>
		/// Checks whether the type contains the &lt; and &gt; characters that denotes a C# generic type.
		/// </summary>
		private static readonly Func<string, bool> IsCSharpGenericType = type => type.IndexOf('<') != -1 || type.IndexOf('>') != -1;

		/// <summary>
		/// Checks whether the type contains the ` but also the generic type arguments with [[ and ]].
		/// </summary>
		private static readonly Func<string, bool> IsFullGenericType = type => Expressions.GenericsArityAndBracket.IsMatch(type);

		/// <summary>
		/// Checks whether the type is a nested type by finding the '+' symbol.
		/// </summary>
		private static readonly Func<string, bool> IsNestedType = type => type.IndexOf('+') != -1;

		private bool initialized = false;
		private List<string> safeImports = new List<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeGeneration"/> class.
		/// </summary>
		public CodeGeneration()
		{
			this.TypeNameMap = new Dictionary<string, string>();
		}

		/// <summary>
		/// Gets the type map built so far, where the keys are the full type names, 
		/// and the value is the type name to use in code generation.
		/// </summary>
		protected internal IDictionary<string, string> TypeNameMap { get; private set; }

		/// <summary>
		/// Gets the list of safe imports for type map in use.
		/// </summary>
		public IEnumerable<string> SafeImports
		{
			get { ThrowIfNotInitialized(); return safeImports; }
		}

		/// <summary>
		/// Signals the object that initialization is starting.
		/// </summary>
		public void BeginInit()
		{
			ThrowIfInitialized();
		}

		/// <summary>
		/// Completes initialization by processing the type name map and 
		/// searches for potential simplifications to type names.
		/// </summary>
		public void EndInit()
		{
			ThrowIfInitialized();

			this.BuildSafeTypeNames();
			this.BuildSafeImports();

			this.initialized = true;
		}

		/// <summary>
		/// Adds the given type to the type map if it wasn't added already.
		/// </summary>
		public void AddType(Type type)
		{
			Guard.NotNull(() => type, type);

			AddType(type.FullName);
		}

		/// <summary>
		/// Adds the given type to the type map if it wasn't added already.
		/// </summary>
		public void AddType(string typeFullName)
		{
			this.TypeNameMap[typeFullName] = typeFullName;
		}

		/// <summary>
		/// Adds all public types of the given assembly to the type map.
		/// </summary>
		public void AddTypes(Assembly assembly)
		{
			Guard.NotNull(() => assembly, assembly);

			AddTypes(assembly.GetTypes().Select(type => type.FullName));
		}

		/// <summary>
		/// Adds a batch of new types to the map if they haven't been added already.
		/// </summary>
		public void AddTypes(params string[] newTypes)
		{
			AddTypes((IEnumerable<string>)newTypes);
		}

		/// <summary>
		/// Adds a batch of new types to the map if they haven't been added already.
		/// </summary>
		public void AddTypes(IEnumerable<string> newTypes)
		{
			Guard.NotNull(() => newTypes, newTypes);

			ThrowIfInitialized();

			foreach (var typeName in newTypes)
			{
				AddType(typeName);
			}
		}

		/// <summary>
		/// Gets the name of the type that can be used in code generation, considering 
		/// the already determined <see cref="SafeImports"/>.
		/// </summary>
		public string GetTypeName(Type type)
		{
			Guard.NotNull(() => type, type);

			return GetTypeName(type.FullName);
		}

		/// <summary>
		/// Gets the name of the type that can be used in code generation, considering 
		/// the already determined <see cref="SafeImports"/>.
		/// </summary>
		public string GetTypeName(string fullName)
		{
			ThrowIfNotInitialized();

			Guard.NotNullOrEmpty(() => fullName, fullName);

			var typeName = fullName;
			if (!this.TypeNameMap.TryGetValue(typeName, out typeName))
				typeName = Expressions.ComaWithoutSpace.Replace(
					Expressions.GenericsArity.Replace(SanitizeGenerics(fullName), ""), ", ");

			return typeName;
		}

		/// <summary>
		/// Returns a valid C# representation of a typed argument that can be 
		/// used in a custom attribute for code generation.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "We want this method available on an instance.")]
		public string ToCSharpString(CustomAttributeTypedArgument argument)
		{
			Guard.NotNull(() => argument, argument);

			using (var provider = new Microsoft.CSharp.CSharpCodeProvider())
			using (var writer = new System.IO.StringWriter(CultureInfo.InvariantCulture))
			{
				if (argument.Value == null)
				{
					provider.GenerateCodeFromExpression(new System.CodeDom.CodePrimitiveExpression(null), writer, new System.CodeDom.Compiler.CodeGeneratorOptions());
					return writer.ToString();
				}

				if (argument.ArgumentType == typeof(string))
					return "\"" + argument.Value + "\"";

				if (argument.ArgumentType.IsEnum)
					return argument.ArgumentType.Name + "." + Enum.GetName(argument.ArgumentType, argument.Value);

				if (argument.ArgumentType == typeof(Type))
					provider.GenerateCodeFromExpression(new System.CodeDom.CodeTypeOfExpression(((Type)argument.Value).Name), writer, new System.CodeDom.Compiler.CodeGeneratorOptions());
				else
					provider.GenerateCodeFromExpression(new System.CodeDom.CodePrimitiveExpression(argument.Value), writer, new System.CodeDom.Compiler.CodeGeneratorOptions());

				return writer.ToString();
			}
		}

		/// <summary>
		/// Adds the types used in all properties and their attributes 
		/// for the given type, as well as the properties of all its 
		/// implemented interfaces (not annotated with the 
		/// <see cref="HiddenAttribute"/> attribute in both cases).
		/// </summary>
		internal void AddUsedTypes(Type type)
		{
			ThrowIfInitialized();

			Guard.NotNull(() => type, type);

			// Concat property types from the schema types itself that are not hidden.
			var properties = type.GetProperties().AsEnumerable();
			// As well as all their implemented interfaces
			properties = properties.Concat(type
				.GetInterfaces()
				.SelectMany(t => t.GetProperties()));
			// But only those that are not hidden.
			properties = properties
				.Where(property => !property.IsDefined(typeof(HiddenAttribute), true));

			var usedTypes = properties
				.Select(property => property.PropertyType.FullName);

			// Add all the namespaces from all custom attributes.
			usedTypes = usedTypes.Concat(properties
				.SelectMany(property => GetUsedTypes(property)));

			AddTypes(usedTypes.Distinct());
		}

		/// <summary>
		/// Searches the type map for potential simplifications to type names. 
		/// All type names that are unique across all used namespaces are 
		/// turned into their simple type name (without the namespace).
		/// </summary>
		protected virtual void BuildSafeTypeNames()
		{
			// Generics have special notation.
			foreach (var genericParameter in this.TypeNameMap.Keys
				.Where(IsFullGenericType)
				.Select(fullName => SanitizeGenerics(fullName))
				.SelectMany(sanitized => Expressions.GenericParameters.Matches(sanitized).Cast<Match>())
				.Select(parameterMatch => parameterMatch.Value)
				.ToList())
			{
				// Add all full name of generic parameters.
				// Note that this also includes the generic type itself.
				this.AddType(genericParameter);
			}

			// Build the list of simple type names from the dictionary for counting.
			// Add only non-generic first, as the actual generic parameters have 
			// already been added above
			var allSimpleTypeNames = this.TypeNameMap.Keys
				.Where(type => !IsFullGenericType(type))
				.Select(type => ToSimpleName(type))
				.ToList();

			foreach (var type in this.TypeNameMap.Keys.Where(name => !IsFullGenericType(name)).ToList())
			{
				var simpleTypeName = ToSimpleName(type);

				// Only make the value map for this entry the simple name if there's 
				// only one type with that simple name (no collisions)
				if (allSimpleTypeNames.Count(s => s == simpleTypeName) == 1)
					this.TypeNameMap[type] = simpleTypeName;
			}

			// Now do the replacement on the generic parameters
			foreach (var type in this.TypeNameMap.Keys.Where(IsFullGenericType).ToList())
			{
				var sanitized = SanitizeGenerics(this.TypeNameMap[type]);
				this.TypeNameMap[type] = Expressions.GenericParameters.Replace(sanitized, match => this.TypeNameMap[match.Value]);
			}

			// Remove the assembly on full type names that were not simplified and are not generics
			foreach (var pair in this.TypeNameMap.Where(t => !IsCSharpGenericType(t.Value) && t.Value.IndexOf(',') != -1).ToList())
			{
				this.TypeNameMap[pair.Key] = pair.Value.Substring(0, pair.Value.IndexOf(','));
			}

			// Finally remove the '+' from nested type names.
			foreach (var type in this.TypeNameMap.Keys.Where(IsNestedType).ToList())
			{
				this.TypeNameMap[type] = this.TypeNameMap[type].Replace('+', '.');
			}

			// Finally add whitespaces between generic parameters and remove the arity for a C# valid identifier
			foreach (var type in this.TypeNameMap.Keys.Where(IsFullGenericType).ToList())
			{
				this.TypeNameMap[type] = Expressions.GenericsArity.Replace(
					Expressions.ComaWithoutSpace.Replace(this.TypeNameMap[type], ", "), "");
			}
		}

		/// <summary>
		/// From the type map, finds those namespaces that can be safely imported ("using" in C#) 
		/// without causing type collisions among the type names in the map.
		/// </summary>
		protected virtual void BuildSafeImports()
		{
			Func<string, bool> hasNamespace = typeName =>
				typeName.TakeWhile(c => c != '<' && c != ',').Any(c => c == '.');

			Func<string, string> namespaceSelector = typeName =>
			{
				var simpleName = new string(typeName.TakeWhile(c => c != '<' && c != ',').ToArray());
				// The hasNamespace filter already ensures we do have a dot.
				return simpleName.Substring(0, simpleName.LastIndexOf('.'));
			};

			var fullNames = this.TypeNameMap.Keys
				.Where(type => !IsFullGenericType(type))
				.Where(hasNamespace)
				.Select(namespaceSelector)
				.GroupBy(ns => ns)
				.ToDictionary(group => group.Key, group => group.Count());

			var finalNames = this.TypeNameMap.Values
				.Where(type => !IsFullGenericType(type))
				.Where(hasNamespace)
				.Select(namespaceSelector)
				.GroupBy(ns => ns)
				.ToDictionary(group => group.Key, group => group.Count());

			foreach (var fullName in fullNames.OrderBy(i => i.Key))
			{
				if (!finalNames.ContainsKey(fullName.Key) ||
					fullName.Value > finalNames[fullName.Key])
				{
					this.safeImports.Add(fullName.Key);
				}
			}
		}

		/// <summary>
		/// Sanitizes the generics.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		protected virtual string SanitizeGenerics(string typeName)
		{
			// Remove full assembly names
			var fullName = Expressions.FullAssemblyName.Replace(typeName, string.Empty);
			// Remove [] from type names
			fullName = Expressions.TypeNameWithBrackets.Replace(fullName, "$1");
			// Replace [[ with <
			fullName = Expressions.DoubleOpenBrackets.Replace(fullName, "<");
			// Replace ]] with >
			fullName = Expressions.DoubleCloseBrackets.Replace(fullName, ">");
			// Replace [ with <
			fullName = Expressions.SingleOpenBracket.Replace(fullName, "<");
			// Replace ] with >
			fullName = Expressions.SingleCloseBracket.Replace(fullName, ">");

			return fullName;
		}

		private static string ToSimpleName(string type)
		{
			var simpleTypeName = type;
			if (simpleTypeName.IndexOf(',') != -1)
				simpleTypeName = simpleTypeName.Substring(0, simpleTypeName.IndexOf(','));

			simpleTypeName = simpleTypeName.Substring(simpleTypeName.LastIndexOf('.') + 1);

			return simpleTypeName;
		}

		/// <summary>
		/// Gets the used namespaces in the property, including the property type 
		/// and also all of its custom attributes and attribute values.
		/// </summary>
		private static IEnumerable<string> GetUsedTypes(PropertyInfo property)
		{
			Guard.NotNull(() => property, property);

			var usedTypes = new[] { property.PropertyType.FullName }.AsEnumerable();

			var customAttributes = property.GetCustomAttributesData();

			// Add the namespaces of all attribute types
			usedTypes = usedTypes.Concat(customAttributes
				.Select(data => data.Constructor.DeclaringType.FullName));

			// Add the namespaces of all the parameter of all constructors
			usedTypes = usedTypes.Concat(customAttributes
				.SelectMany(data => data.ConstructorArguments)
				.Select(arg => arg.ArgumentType.FullName));

			// Add the namespaces of all the named parameters
			usedTypes = usedTypes.Concat(customAttributes
				.SelectMany(data => data.NamedArguments)
				.Select(arg => arg.TypedValue.ArgumentType.FullName));

			// Add the namespaces of all constructor arguments that are typeof(..)
			usedTypes = usedTypes.Concat(customAttributes
				.SelectMany(data => data.ConstructorArguments)
				.Where(arg => arg.ArgumentType == typeof(Type))
				.Select(arg => ((Type)arg.Value).FullName));

			// Add the namespaces of all named arguments that are typeof(..)
			usedTypes = usedTypes.Concat(customAttributes
				.SelectMany(data => data.NamedArguments)
				.Where(arg => arg.TypedValue.ArgumentType == typeof(Type))
				.Select(arg => ((Type)arg.TypedValue.Value).FullName));

			return usedTypes;
		}

		private void ThrowIfInitialized()
		{
			if (initialized)
				throw new InvalidOperationException(Resources.CodeGeneration_InitializationFinished);
		}

		private void ThrowIfNotInitialized()
		{
			if (!initialized)
				throw new InvalidOperationException(Resources.CodeGeneration_InitializationPending);
		}

		private static class Expressions
		{
			/// <summary>
			/// Matches the generic type and its parameter type names with C# syntax: 
			/// for System.IEnumerable{System.Boolean}, matches System.IEnumerable and System.Boolean
			/// </summary>
			public static readonly Regex GenericParameters = new Regex(@"[^<,>]+?(?=(<|,|>|$))", RegexOptions.Compiled);

			/// <summary>
			/// Matches two identifiers that are separated by a coma but without a whitespace after the coma, such as Boolean,String.
			/// </summary>
			public static readonly Regex ComaWithoutSpace = new Regex(@"(?<=[^,]),(?=[^\s])", RegexOptions.Compiled);

			/// <summary>
			/// Matches the assembly part of an assembly qualified type name (i.e. , mscorlib, Version=..., Culture=..., PublicKeyToken=...).
			/// </summary>
			public static readonly Regex FullAssemblyName = new Regex(@",[^\[]+?,\s?Version.*?,\s?Culture.*?,\s?PublicKeyToken.*?(?=\])", RegexOptions.Compiled);

			/// <summary>
			/// Matches a type name that has square brackets surrounding it, like [System.Boolean].
			/// </summary>
			public static readonly Regex TypeNameWithBrackets = new Regex(@"\[([^\]\[]+?)\]", RegexOptions.Compiled);

			/// <summary>
			/// Matches [[
			/// </summary>
			public static readonly Regex DoubleOpenBrackets = new Regex(@"\[\[", RegexOptions.Compiled);

			/// <summary>
			/// Matches ]]
			/// </summary>
			public static readonly Regex DoubleCloseBrackets = new Regex(@"\]\]", RegexOptions.Compiled);

			/// <summary>
			/// Matches [
			/// </summary>
			public static readonly Regex SingleOpenBracket = new Regex(@"\[", RegexOptions.Compiled);

			/// <summary>
			/// Matches ]
			/// </summary>
			public static readonly Regex SingleCloseBracket = new Regex(@"\]", RegexOptions.Compiled);

			/// <summary>
			/// Matches the arity part of a generics type, like IEnumerable`1 (would match `1).
			/// </summary>
			public static readonly Regex GenericsArity = new Regex(@"`\d{1,}", RegexOptions.Compiled);

			/// <summary>
			/// Matches the arity part of a generics type, like IEnumerable`1[[System.Boolean]] (would match `1[).
			/// </summary>
			public static readonly Regex GenericsArityAndBracket = new Regex(@"`\d{1,}\[", RegexOptions.Compiled);
		}
	}
}
