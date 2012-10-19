using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
	[TestClass]
	public class ProductCodeGenerationSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenAnElementSchemaWithAVariableProperty
		{
			private IElementInfo element;

			[TestInitialize]
			public void Initialize()
			{
				var mockElement = new Mock<IElementInfo>();
				mockElement.Setup(x => x.Properties)
					.Returns(new[] 
					{
						Mocks.Of<IPropertyInfo>().First(p => p.Type == "System.String"), 
						Mocks.Of<IPropertyInfo>().First(p => p.Type == "System.Boolean" && p.TypeConverterTypeName == "System.ComponentModel.StringConverter, System"), 
						Mocks.Of<IPropertyInfo>().First(p => p.Type == "System.Int32" && p.TypeConverterTypeName == "Microsoft.VisualStudio.Patterning.StringConverter, Microsoft"), 
						Mocks.Of<IPropertyInfo>().First(p => p.Type == "System.Boolean" && p.EditorTypeName == "System.ComponentModel.UIEditor, System"), 
					});

				this.element = mockElement.Object;
			}

			[TestMethod]
			public void WhenBuildingTypeMap_ThenContainsVariablePropertiesUsedTypes()
			{
				var codegen = new ProductCodeGeneration<IElementInfo, IElement>(this.element);
				codegen.EndInit();

				Assert.Equal("String", codegen.TypeNameMap["System.String"], "Unique type name should be in simple form.");
				Assert.Equal("UIEditor", codegen.TypeNameMap["System.ComponentModel.UIEditor, System"], "Unique type name should be in simple form.");
				Assert.Equal("System.ComponentModel.StringConverter", codegen.TypeNameMap["System.ComponentModel.StringConverter, System"], "Duplicated type name should exist in full form.");
				Assert.Equal("Microsoft.VisualStudio.Patterning.StringConverter", codegen.TypeNameMap["Microsoft.VisualStudio.Patterning.StringConverter, Microsoft"], "Duplicated type name should exist in full form.");

				Assert.True(codegen.SafeImports.Contains("System"));
				Assert.True(codegen.SafeImports.Contains("System.ComponentModel"));
				Assert.False(codegen.SafeImports.Contains("Microsoft.VisualStudio.Patterning"));
			}

			[TestMethod]
			public void WhenBuildingTypeMap_ThenHasStandardValuesEditor()
			{
				var pattern = new Mock<IPatternInfo>();
				pattern.Setup(x => x.Properties)
					.Returns(new[] 
					{
						Mocks.Of<IPropertyInfo>().First(p => p.Type == "System.String"), 
					});

				var codegen = new ProductCodeGeneration<IPatternInfo, IProduct>(pattern.Object);
				var typeName = typeof(StandardValuesEditor).FullName + ", " + typeof(StandardValuesEditor).Assembly.GetName().Name;
				codegen.AddType(typeName);
				codegen.EndInit();

				Assert.True(codegen.SafeImports.Contains(typeof(StandardValuesEditor).Namespace));
				Assert.Equal(typeof(StandardValuesEditor).Name, codegen.GetTypeName(typeName));
			}

		}
	}
}
