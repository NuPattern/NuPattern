using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Microsoft.CSharp;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.Modeling;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests
{
	[TestClass]
	public class ModelElementDirectiveProcessorSpec
	{
		internal static readonly IAssertion Assert = new Assertion();
		Dictionary<string, string> directiveAttributes;
		CodeDomProvider provider;

		public ModelElementDirectiveProcessorSpec()
		{
			this.directiveAttributes = new Dictionary<string, string>();
			this.directiveAttributes.Add("Type", "Foo.Bar");
			this.provider = new CSharpCodeProvider();
		}

		[TestMethod]
		public void WhenTypeAttributeIsNotFound_ThenThrows()
		{
			var processor = new ModelElementDirectiveProcessor();

			processor.StartProcessingRun(provider, null, new CompilerErrorCollection());
			Assert.Throws<DirectiveProcessorException>(() => processor.ProcessDirective("ModelElement", new Dictionary<string, string>()));
		}

		[TestMethod]
		public void WhenTypeAttributeIsFound_ThenElementPropertyIsGenerated()
		{
			var processor = new ModelElementDirectiveProcessor();

			processor.StartProcessingRun(provider, null, new CompilerErrorCollection());
			processor.ProcessDirective("ModelElement", directiveAttributes);
			var result = processor.GetClassCodeForProcessingRun();

			Assert.NotNull(result);
			Assert.True(result.Contains("public new Foo.Bar Element"));
			Assert.True(result.Contains("return ((Foo.Bar)(base.Element));"));
		}

		[TestMethod]
		public void WhenDirectiveNameIsNotModelElement_ThenDirectiveIsNotSupported()
		{
			var processor = new ModelElementDirectiveProcessor();

			Assert.False(processor.IsDirectiveSupported("Foo"));
		}

		[TestMethod]
		public void WhenDirectiveNameIsModeling_ThenDirectiveIsSupported()
		{
			var processor = new ModelElementDirectiveProcessor();

			Assert.True(processor.IsDirectiveSupported("ModelElement"));
		}

		[TestMethod]
		public void WhenGettingReferenceWithoutElementAssemblyPath_ThenThrows()
		{
			var processor = new ModelElementDirectiveProcessor();

			Assert.Throws<DirectiveProcessorException>(() => processor.GetReferencesForProcessingRun());
		}

		[TestMethod]
		public void WhenGettingReferenceWithEmptyElementAssemblyPath_ThenThrows()
		{
			var processor = new ModelElementDirectiveProcessor();
			CallContext.LogicalSetData(ModelElementDirectiveProcessor.KeyCallContextElementAssemblyPath, "");

			Assert.Throws<DirectiveProcessorException>(() => processor.GetReferencesForProcessingRun());
		}

		[TestMethod]
		public void WhenGettingReferenceWithNonExistingElementAssemblyPath_ThenThrows()
		{
			var processor = new ModelElementDirectiveProcessor();
			CallContext.LogicalSetData(ModelElementDirectiveProcessor.KeyCallContextElementAssemblyPath, "C:\\Temp\\" + Guid.NewGuid().ToString("X") + ".dll");

			Assert.Throws<DirectiveProcessorException>(() => processor.GetReferencesForProcessingRun());
		}

		[TestMethod]
		public void WhenGettingReference_ThenReturnsDefaultReferences()
		{
			var processor = new ModelElementDirectiveProcessor();
			var elementAssemblyPath = this.GetType().Assembly.ManifestModule.FullyQualifiedName;
			CallContext.LogicalSetData(ModelElementDirectiveProcessor.KeyCallContextElementAssemblyPath, elementAssemblyPath);

			var references = processor.GetReferencesForProcessingRun();

			Assert.True(references.Contains(typeof(IQueryable).Assembly.Location));
			Assert.True(references.Contains(typeof(IVsHierarchy).Assembly.Location));
			Assert.True(references.Contains(typeof(ModelElement).Assembly.Location));
			Assert.True(references.Contains(typeof(IModelBus).Assembly.Location));
			Assert.True(references.Contains(typeof(ModelBusEnabledTextTransformation).Assembly.Location));
		}

		[TestMethod]
		public void WhenGettingReferenceAndElementAssemblyPathWasDefined_ThenReturnsDefaultReferencesAndElementReference()
		{
			var processor = new ModelElementDirectiveProcessor();

			var elementAssemblyPath = Path.GetTempFileName();
			CallContext.LogicalSetData(ModelElementDirectiveProcessor.KeyCallContextElementAssemblyPath, elementAssemblyPath);
			var references = processor.GetReferencesForProcessingRun();

			Assert.True(references.Contains(elementAssemblyPath));
		}

		[TestMethod]
		public void WhenGettingImports_ThenReturnsDefaultImports()
		{
			var processor = new ModelElementDirectiveProcessor();

			var imports = processor.GetImportsForProcessingRun();

			Assert.True(imports.Contains(typeof(System.Linq.IQueryable).Namespace));
			Assert.True(imports.Contains(typeof(System.Collections.Generic.IEnumerable<>).Namespace));
			Assert.True(imports.Contains(typeof(Microsoft.VisualStudio.Modeling.Integration.IModelBus).Namespace));
		}

		[TestMethod]
		public void WhenGettingImportsAndElementNamespaceWasDefined_ThenReturnsDefaultImportsAndElementNamespace()
		{
			var processor = new ModelElementDirectiveProcessor();

			CallContext.LogicalSetData(ModelElementDirectiveProcessor.KeyCallContextElementNamespace, "FooNamespace");
			var imports = processor.GetImportsForProcessingRun();

			Assert.True(imports.Contains("FooNamespace"));
		}

	}
}
