using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.CodeGen;

namespace NuPattern.Runtime.UnitTests.CodeGen
{
    [TestClass]
    public class ProductCodeGenerationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAnElementInfoWithAVariableProperty
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
                        Mocks.Of<IPropertyInfo>().First(p => p.Type == "System.Int32" && p.TypeConverterTypeName == "NuPattern.StringConverter, Microsoft"), 
                        Mocks.Of<IPropertyInfo>().First(p => p.Type == "System.Boolean" && p.EditorTypeName == "System.ComponentModel.UIEditor, System"), 
                        Mocks.Of<IPropertyInfo>().First(p => p.Type == "System.String" && p.EditorTypeName == "System.ComponentModel.Design.MultilineStringEditor, System.Design"), 
                    });

                this.element = mockElement.Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBuildingTypeMap_ThenContainsVariablePropertiesUsedTypes()
            {
                var codegen = new ProductCodeGeneration<IElementInfo, IElement>(this.element);
                codegen.EndInit();

                Assert.Equal("String", codegen.TypeNameMap["System.String"], "Unique type name should be in simple form.");
                Assert.Equal("UIEditor", codegen.TypeNameMap["System.ComponentModel.UIEditor, System"], "Unique type name should be in simple form.");
                Assert.Equal("System.ComponentModel.StringConverter", codegen.TypeNameMap["System.ComponentModel.StringConverter, System"], "Duplicated type name should exist in full form.");
                Assert.Equal("NuPattern.StringConverter", codegen.TypeNameMap["NuPattern.StringConverter, Microsoft"], "Duplicated type name should exist in full form.");
                Assert.Equal("MultilineStringEditor", codegen.TypeNameMap["System.ComponentModel.Design.MultilineStringEditor, System.Design"]);

                Assert.True(codegen.SafeImports.Contains("System"));
                Assert.True(codegen.SafeImports.Contains("System.ComponentModel"));
                Assert.True(codegen.SafeImports.Contains("System.ComponentModel.Design"));
                Assert.False(codegen.SafeImports.Contains("NuPattern"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBuildingTypeMap_ThenAlwaysContainsSystemDrawing()
            {
                var codegen = new ProductCodeGeneration<IElementInfo, IElement>(this.element);
                codegen.EndInit();

                Assert.Equal("UITypeEditor", codegen.TypeNameMap["System.Drawing.Design.UITypeEditor"], "Unique type name should be in simple form.");
                Assert.True(codegen.SafeImports.Contains("System.Drawing.Design"));
            }

            [TestMethod, TestCategory("Unit")]
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
