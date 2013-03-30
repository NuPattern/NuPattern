using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.CodeGen;

namespace NuPattern.Runtime.UnitTests.CodeGen
{
    [TestClass]
    public class ProductCodeGenerationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAnElementSchemaWithAVariableProperty
        {
            private IElementSchema element;

            [TestInitialize]
            public void Initialize()
            {
                var mockElement = new Mock<IElementSchema>();
                mockElement.Setup(x => x.Properties)
                    .Returns(new[] 
                    {
                        Mocks.Of<IPropertySchema>().First(p => p.Type == "System.String"), 
                        Mocks.Of<IPropertySchema>().First(p => p.Type == "System.Boolean" && p.TypeConverterTypeName == "System.ComponentModel.StringConverter, System"), 
                        Mocks.Of<IPropertySchema>().First(p => p.Type == "System.Int32" && p.TypeConverterTypeName == "NuPattern.StringConverter, Microsoft"), 
                        Mocks.Of<IPropertySchema>().First(p => p.Type == "System.Boolean" && p.EditorTypeName == "System.ComponentModel.UIEditor, System"), 
                    });

                this.element = mockElement.Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBuildingTypeMap_ThenContainsVariablePropertiesUsedTypes()
            {
                var codegen = new ProductCodeGeneration<IElementSchema, IElement>(this.element);
                codegen.EndInit();

                Assert.Equal("String", codegen.TypeNameMap["System.String"], "Unique type name should be in simple form.");
                Assert.Equal("UIEditor", codegen.TypeNameMap["System.ComponentModel.UIEditor, System"], "Unique type name should be in simple form.");
                Assert.Equal("System.ComponentModel.StringConverter", codegen.TypeNameMap["System.ComponentModel.StringConverter, System"], "Duplicated type name should exist in full form.");
                Assert.Equal("NuPattern.StringConverter", codegen.TypeNameMap["NuPattern.StringConverter, Microsoft"], "Duplicated type name should exist in full form.");

                Assert.True(codegen.SafeImports.Contains("System"));
                Assert.True(codegen.SafeImports.Contains("System.ComponentModel"));
                Assert.False(codegen.SafeImports.Contains("NuPattern"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBuildingTypeMap_ThenHasStandardValuesEditor()
            {
                var pattern = new Mock<IPatternSchema>();
                pattern.Setup(x => x.Properties)
                    .Returns(new[] 
                    {
                        Mocks.Of<IPropertySchema>().First(p => p.Type == "System.String"), 
                    });

                var codegen = new ProductCodeGeneration<IPatternSchema, IProduct>(pattern.Object);
                var typeName = typeof(StandardValuesEditor).FullName + ", " + typeof(StandardValuesEditor).Assembly.GetName().Name;
                codegen.AddType(typeName);
                codegen.EndInit();

                Assert.True(codegen.SafeImports.Contains(typeof(StandardValuesEditor).Namespace));
                Assert.Equal(typeof(StandardValuesEditor).Name, codegen.GetTypeName(typeName));
            }

        }
    }
}
