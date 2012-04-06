using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
	[TestClass]
	public class CodeGenerationSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenInvokingEndInitTwice_ThenThrowsInvalidOperationOperation()
		{
			var codegen = new CodeGeneration();
			codegen.EndInit();

			Assert.Throws<InvalidOperationException>(() => codegen.EndInit());
		}

		[TestMethod]
		public void WhenInvokingBeginInitAfterEndInit_ThenThrowsInvalidOperationOperation()
		{
			var codegen = new CodeGeneration();
			codegen.EndInit();

			Assert.Throws<InvalidOperationException>(() => codegen.BeginInit());
		}

		[TestMethod]
		public void WhenSafeImportsAccessedBeforeEndInit_ThenThrowsInvalidOperationOperation()
		{
			var codegen = new CodeGeneration();

			Assert.Throws<InvalidOperationException>(() => codegen.SafeImports.Count());
		}

		[TestMethod]
		public void WhenAddUsedTypesInvokedAfterEndInit_ThenThrowsInvalidOperationOperation()
		{
			var codegen = new CodeGeneration();
			codegen.EndInit();

			Assert.Throws<InvalidOperationException>(() => codegen.AddUsedTypes(typeof(Foo)));
		}

		[TestMethod]
		public void WhenGetTypeNameInvokedBeforeEndInit_ThenThrowsInvalidOperationOperation()
		{
			var codegen = new CodeGeneration();

			Assert.Throws<InvalidOperationException>(() => codegen.GetTypeName(typeof(Foo)));
		}

		[TestMethod]
		public void WhenSimplifyingTypeMap_ThenNoNamespaceTypesAreValid()
		{
			var codegen = new CodeGeneration();

			codegen.TypeNameMap["Foo"] = "Foo";
			codegen.TypeNameMap["Bar"] = "Foo";

			codegen.EndInit();

			Assert.Equal("Foo", codegen.TypeNameMap["Foo"], "Foo does not have a namespace but it still is valid.");
			Assert.Equal("Bar", codegen.TypeNameMap["Bar"], "Bar does not have a namespace but it still is valid.");
		}

		[TestMethod]
		public void WhenSimplifyingGenericType_ThenAddsUsingsAndSimplifiesGenericParameterType()
		{
			var codegen = new CodeGeneration();

			codegen.AddType(typeof(IEnumerable<NonNestedType>));
			codegen.EndInit();

			Assert.Equal("IEnumerable<NonNestedType>", codegen.GetTypeName(typeof(IEnumerable<NonNestedType>)));
			Assert.True(codegen.SafeImports.Contains(typeof(IEnumerable<>).Namespace));
			Assert.True(codegen.SafeImports.Contains(typeof(NonNestedType).Namespace));
		}

		[TestMethod]
		public void WhenSimplifyingGenericTypeWithNestedTypeParameter_ThenRemovesPlusFromNestedTypeName()
		{
			var codegen = new CodeGeneration();

			codegen.AddType(typeof(IEnumerable<NestedType>));
			codegen.EndInit();

			Assert.Equal("IEnumerable<CodeGenerationSpec.NestedType>", codegen.GetTypeName(typeof(IEnumerable<NestedType>)));
			Assert.True(codegen.SafeImports.Contains(typeof(IEnumerable<>).Namespace));
			Assert.True(codegen.SafeImports.Contains(typeof(NestedType).Namespace));
			Assert.False(codegen.SafeImports.Contains(typeof(CodeGenerationSpec).FullName), "The nested type parent should not be mistaken for a namespace.");
		}

		[TestMethod]
		public void WhenSimplifyingGenericTypeWithCollidingParameter_ThenKeepsParameterFullName()
		{
			var codegen = new CodeGeneration();

			codegen.AddType(typeof(IEnumerable<StringConverter>));
			codegen.AddType(typeof(System.ComponentModel.StringConverter));
			codegen.EndInit();

            Assert.Equal("IEnumerable<Microsoft.VisualStudio.Patterning.Extensibility.UnitTests.StringConverter>", codegen.GetTypeName(typeof(IEnumerable<StringConverter>)));
			Assert.True(codegen.SafeImports.Contains(typeof(IEnumerable<>).Namespace));
			Assert.False(codegen.SafeImports.Contains(typeof(StringConverter).Namespace));
		}

		[TestMethod]
		public void WhenSimplifyingGenericAndNonGenericEnumerable_ThenAddsUsingForBoth()
		{
			var codegen = new CodeGeneration();

			codegen.AddType(typeof(IEnumerable<string>));
			codegen.AddType(typeof(IEnumerable));
			codegen.EndInit();

			Assert.Equal("IEnumerable<String>", codegen.GetTypeName(typeof(IEnumerable<string>)));
			Assert.Equal("IEnumerable", codegen.GetTypeName(typeof(IEnumerable)));
			Assert.True(codegen.SafeImports.Contains(typeof(IEnumerable<>).Namespace));
			Assert.True(codegen.SafeImports.Contains(typeof(IEnumerable).Namespace));
		}

		[TestMethod]
		public void WhenSimplifyingAllCoreLib_ThenAddsUsingForGenericsAndNonGenericEnumerable()
		{
			var codegen = new CodeGeneration();

			codegen.AddTypes(typeof(string).Assembly);
			codegen.AddType(typeof(IComparable<string>));
			codegen.EndInit();

			Assert.Equal("IComparable<String>", codegen.GetTypeName(typeof(IComparable<string>)));
			Assert.Equal("IComparable", codegen.GetTypeName(typeof(IComparable)));
			Assert.True(codegen.SafeImports.Contains(typeof(IComparable<>).Namespace));
		}

		[TestMethod]
		public void WhenAddingAssembly_ThenSafeUsingsDoNotContainGenerics()
		{
			var codegen = new CodeGeneration();

			codegen.AddType(typeof(IEnumerable<string>));
			codegen.AddType(typeof(IEnumerable));
			codegen.EndInit();

			Assert.False(codegen.SafeImports.Any(s => s.IndexOf('[') != -1));
		}

		[TestMethod]
		public void WhenSimplifyingMultipleGenerics_ThenSimplifiesAllParameters()
		{
			var codegen = new CodeGeneration();
			var type = typeof(IDictionary<IList<KeyValuePair<string, StringConverter>>, NestedType>);

			codegen.AddType(type);
			codegen.AddType(typeof(System.ComponentModel.StringConverter));
			codegen.EndInit();

            Assert.Equal("IDictionary<IList<KeyValuePair<String, Microsoft.VisualStudio.Patterning.Extensibility.UnitTests.StringConverter>>, CodeGenerationSpec.NestedType>", codegen.GetTypeName(type));
			Assert.True(codegen.SafeImports.Contains(typeof(IEnumerable<>).Namespace));
			Assert.True(codegen.SafeImports.Contains(typeof(CodeGenerationSpec).Namespace));
		}

		[TestMethod]
		public void WhenSimplifyingTypeMap_ThenOnlySimplifiesNonCollidingTypeNames()
		{
			var codegen = new CodeGeneration();

			codegen.TypeNameMap["Foo.A"] = "Foo.A";
			codegen.TypeNameMap["Foo.B"] = "Foo.B";
			codegen.TypeNameMap["Bar.A"] = "Bar.A";

			codegen.EndInit();

			Assert.Equal("Foo.A", codegen.TypeNameMap["Foo.A"], "A type name is duplicated on namespace Bar, so it cannot be used as a simple type name and its full name should be in the map instead.");
			Assert.Equal("B", codegen.TypeNameMap["Foo.B"], "B is unique in the dictionary, so it can be used as a simplified type name.");
			Assert.Equal("Bar.A", codegen.TypeNameMap["Bar.A"], "B type name is duplicated on namespace Foo, so it cannot be used as a simple type name and its full name should be in the map instead.");
		}

		[TestMethod]
		public void WhenSimplifyingTypeMap_ThenUniqueTypeNamesAreSimplified()
		{
			var codegen = new CodeGeneration();

			codegen.TypeNameMap["Foo.A"] = "Foo.A";
			codegen.TypeNameMap["Bar.B"] = "Foo.B";

			codegen.EndInit();

			Assert.Equal("A", codegen.TypeNameMap["Foo.A"], "A is unique in the dictionary, so it can be used as a simplified type name.");
			Assert.Equal("B", codegen.TypeNameMap["Bar.B"], "B is unique in the dictionary, so it can be used as a simplified type name.");
		}

		[TestMethod]
		public void WhenGettingSafeUsings_ThenOnlyGetsNamespacesFromSimplifiedTypeNames()
		{
			var codegen = new CodeGeneration();

			codegen.TypeNameMap["Foo.A"] = "Foo.A";
			codegen.TypeNameMap["Foo.B"] = "Foo.B";
			codegen.TypeNameMap["Bar.A"] = "Bar.A";
			codegen.TypeNameMap["Baz.C"] = "Baz.C";

			codegen.EndInit();

			Assert.True(codegen.SafeImports.Contains("Foo"));
			Assert.True(codegen.SafeImports.Contains("Baz"));
			Assert.False(codegen.SafeImports.Contains("Bar"));
		}

		[TestMethod]
		public void WhenBuildingTypeMap_ThenContainsCustomAttributeTypes()
		{
			var codegen = new CodeGeneration();

			codegen.AddUsedTypes(typeof(Foo));

			Assert.True(codegen.TypeNameMap.ContainsKey(typeof(TypeConverterAttribute).FullName));
			Assert.True(codegen.TypeNameMap.ContainsKey(typeof(BarAttribute).FullName));
			Assert.True(codegen.TypeNameMap.ContainsKey(typeof(BitConverter).FullName));
			Assert.True(codegen.TypeNameMap.ContainsKey(typeof(System.ComponentModel.StringConverter).FullName));
			Assert.True(codegen.TypeNameMap.ContainsKey(typeof(PlatformID).FullName));
		}

		[TestMethod]
		public void WhenSimplifyingAssemblyQualifiedName_ThenAddsUsingAndSimplifiesTypeName()
		{
			var codegen = new CodeGeneration();

			codegen.AddType("Foo.Bar, Foo");
			codegen.EndInit();

			Assert.Equal("Bar", codegen.GetTypeName("Foo.Bar, Foo"));
			Assert.True(codegen.SafeImports.Contains("Foo"));
		}

		public class Foo
		{
			[TypeConverter(typeof(System.ComponentModel.StringConverter))]
			public StringConverter Value { get; set; }

			[Bar("hello", true, null, typeof(UriTypeConverter), Platform = PlatformID.Win32NT, Port = 25, Name = null, TypeConverter = typeof(BitConverter))]
			public string StringValue { get; set; }
			public int IntValue { get; set; }
		}

		[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
		public sealed class BarAttribute : Attribute
		{
			public BarAttribute(string value, bool isbool, string nullValue, Type editorType)
			{
			}

			public Type TypeConverter { get; set; }
			public int Port { get; set; }
			public PlatformID Platform { get; set; }
			public string Name { get; set; }
		}

		public class NestedType { }
	}

	public class NonNestedType { }
	public class StringConverter { }
}
