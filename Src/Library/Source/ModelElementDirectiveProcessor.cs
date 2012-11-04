using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TextTemplating;

namespace Microsoft.VisualStudio.Patterning.Library
{
	/// <summary>
	/// A directive processor for the <see cref="DirectiveName"/> directive, which
	/// generates a new property on the template named <c>Element</c> typed to the
	/// value specified for the <see cref="TypeAttributeName"/> attribute.
	/// </summary>
	/// <remarks>
	/// This directive processor automatically adds the following namespaces for the
	/// template to use:
	/// <list type="bullet">
	/// 		<item>
	/// 			<description>System.Linq</description>
	/// 		</item>
	/// 		<item>
	/// 			<description>System.Collections.Generic</description>
	/// 		</item>
	/// 	</list>
	/// 	<para>
	/// The following assembly references are automatically added too for template compilation:
	/// <list type="bullet">
	/// 			<item>
	/// 				<description>System.Core.dll</description>
	/// 			</item>
	/// 			<item>
	/// 				<description>Microsoft.VisualStudio.Shell.Interop.dll</description>
	/// 			</item>
	/// 			<item>
	/// 				<description>Microsoft.VisualStudio.Modeling.Sdk.dll</description>
	/// 			</item>
	/// 			<item>
	/// 				<description>Microsoft.VisualStudio.Modeling.Sdk.Integration.dll</description>
	/// 			</item>
	/// 			<item>
	/// 				<description>Microsoft.VisualStudio.TextTemplating.Modeling.dll</description>
	/// 			</item>
	/// 		</list>
	/// 	</para>
	/// 	<para>
	/// 	Before transforming a template that uses this directive, the <see cref="KeyCallContextElementAssemblyPath"/> must 
	/// 	be set, and optionally the <see cref="KeyCallContextElementNamespace"/>.
	/// </para>
	/// </remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), ComVisible(false)]
	[Guid("F41C7005-1DA8-4D64-A1F4-21ED5BD956CD")]
	public class ModelElementDirectiveProcessor : DirectiveProcessor
	{
		/// <summary>
		/// Name of the directive processor, which equals <c>ModelElementProcessor</c>.
		/// </summary>
		public const string ProcessorName = "ModelElementProcessor";

		/// <summary>
		/// Name of the directive, which equals <c>ModelElement</c>.
		/// </summary>
		public const string DirectiveName = "ModelElement";

		/// <summary>
		/// Name of the required element type attribute, which equals <c>Type</c>.
		/// </summary>
		public const string TypeAttributeName = "Type";

		/// <summary>
		/// Optional key into the <see cref="CallContext"/> for retrieving the namespace of the 
		/// model element, to automatically import it in the template. Use <see cref="CallContext.LogicalSetData"/> 
		/// to set the value before invoking the template.
		/// </summary>
		public const string KeyCallContextElementNamespace = "ModelElementDirectiveProcessor.ElementNamespace";

		/// <summary>
		/// Required key into the <see cref="CallContext"/> for retrieving the path of the 
		/// assembly declaring the model element type, used for compiling the template. Use <see cref="CallContext.LogicalSetData"/> 
		/// to set the value before invoking the template.
		/// </summary>
		public const string KeyCallContextElementAssemblyPath = "ModelElementDirectiveProcessor.ElementAssemblyPath";

		private CodeDomProvider languageProvider;
		private StringWriter codeWriter;

		/// <summary>
		/// Get the code to contribute to the generated template processing class as
		/// a consequence of the most recent run.
		/// </summary>
		/// <returns>Class code for the run</returns>
		public override string GetClassCodeForProcessingRun()
		{
			return this.codeWriter.ToString();
		}

		/// <summary>
		/// Get any namespaces to import as a consequence of the most recent run.
		/// </summary>
		/// <returns>An array of imports, as text</returns>
		public override string[] GetImportsForProcessingRun()
		{
			var imports = new List<string>
			{
				typeof(System.Linq.IQueryable).Namespace,
				typeof(System.Collections.Generic.IEnumerable<>).Namespace,
                typeof(Microsoft.VisualStudio.Modeling.Integration.IModelBus).Namespace,
			};

			var elementNamespace = CallContext.LogicalGetData(KeyCallContextElementNamespace) as string;
			if (!string.IsNullOrEmpty(elementNamespace))
			{
				imports.Add(elementNamespace);
			}

			return imports.ToArray();
		}

		/// <summary>
		/// Get any references to pass to the compiler as a consequence of the most recent run
		/// </summary>
		/// <returns>An array of fully qualified names for the assemblies needed</returns>
		public override string[] GetReferencesForProcessingRun()
		{
			var references = new List<string> 
			{
				// System.Core.dll
				typeof(System.Linq.IQueryable).Assembly.ManifestModule.FullyQualifiedName, 
				// Microsoft.VisualStudio.Shell.Interop.dll
				typeof(Microsoft.VisualStudio.Shell.Interop.IVsHierarchy).Assembly.ManifestModule.FullyQualifiedName, 
				// Microsoft.VisualStudio.Modeling.Sdk.dll
				typeof(Microsoft.VisualStudio.Modeling.ModelElement).Assembly.ManifestModule.FullyQualifiedName, 
				// Microsoft.VisualStudio.Modeling.Sdk.Integration
				typeof(Microsoft.VisualStudio.Modeling.Integration.IModelBus).Assembly.ManifestModule.FullyQualifiedName, 
				// Microsoft.VisualStudio.TextTemplating.Modeling.dll
				typeof(Microsoft.VisualStudio.TextTemplating.Modeling.ModelBusEnabledTextTransformation).Assembly.ManifestModule.FullyQualifiedName, 
				// This assembly.
				this.GetType().Assembly.ManifestModule.FullyQualifiedName,
			};

			var elementAssembly = CallContext.LogicalGetData(KeyCallContextElementAssemblyPath) as string;

			if (string.IsNullOrEmpty(elementAssembly))
			{
				throw new DirectiveProcessorException(Resources.ModelElementDirectiveProcessor_KeyCallContextElementAssemblyPathRequired);
			}
			else if (!File.Exists(elementAssembly))
			{
				throw new DirectiveProcessorException(Resources.ModelElementDirectiveProcessor_ElementAssemblyPathNotFound,
					new FileNotFoundException(Resources.ModelElementDirectiveProcessor_ElementAssemblyPathNotFound, elementAssembly));
			}

			if (!references.Contains(elementAssembly))
			{
				references.Add(elementAssembly);
			}

			return references.ToArray();
		}

		/// <summary>
		/// Does this DirectiveProcessor support the given directive
		/// </summary>
		/// <param name="directiveName">The name of the directive to check for</param>
		/// <returns>True if the directive is supported, false if it's not</returns>
		public override bool IsDirectiveSupported(string directiveName)
		{
			return String.Equals(directiveName, DirectiveName, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Process a directive from a template file
		/// </summary>
		/// <param name="directiveName">The name of the directive to be processed</param>
		/// <param name="arguments">An optional dictionary of values to be used in the directive</param>
		public override void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
		{
			Guard.NotNull(() => arguments, arguments);
			if (!arguments.ContainsKey(TypeAttributeName))
			{
				throw new DirectiveProcessorException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.ModelElementDirectiveProcessor_TypeAttributeRequired,
					ProcessorName,
					TypeAttributeName));
			}

			string elementType = arguments[TypeAttributeName];

			// Introduces the typed property Element.
			// public new {ElementType} Element { ... }
			var property = new CodeMemberProperty();
			property.Name = "Element";
			property.Type = new CodeTypeReference(elementType);
			property.Attributes = MemberAttributes.Public | MemberAttributes.New | MemberAttributes.Final;

			// get { return ({ElementType})base.Element; }
			property.GetStatements.Add(
				new CodeMethodReturnStatement(
					new CodeCastExpression(property.Type,
						new CodePropertyReferenceExpression(
							new CodeBaseReferenceExpression(), "Element"))));

			var opt = new CodeGeneratorOptions { BracingStyle = "C" };

			this.languageProvider.GenerateCodeFromMember(property, this.codeWriter, opt);
		}

		/// <summary>
		///  Begin a round of directive processing
		/// </summary>
		/// <param name="languageProvider">A Language Provider used to generate code</param>
		/// <param name="templateContents">The contents of the template file, as string</param>
		/// <param name="errors">collection to report processing errors in</param>
		public override void StartProcessingRun(CodeDomProvider languageProvider, string templateContents, CompilerErrorCollection errors)
		{
			base.StartProcessingRun(languageProvider, templateContents, errors);

			this.languageProvider = languageProvider;
			this.codeWriter = new StringWriter(CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Finish a round of directive processing. The code that this Directiveprocessor contributes to the GeneratedTextTemplating class
		/// </summary>
		public override void FinishProcessingRun()
		{
		}

		/// <summary>
		/// Get the code to contribute to the body of the initialize method of the generated
		/// template processing class as a consequence of the most recent run.  This
		/// code will run after the base class' Initialize method
		/// </summary>
		/// <returns>The code to contribute</returns>
		public override string GetPostInitializationCodeForProcessingRun()
		{
			return null;
		}

		/// <summary>
		/// Get the code to contribute to the body of the initialize method of the generated
		/// template processing class as a consequence of the most recent run.  This
		/// code will run before the base class' Initialize method
		/// </summary>
		/// <returns>The code to contribute</returns>
		public override string GetPreInitializationCodeForProcessingRun()
		{
			return null;
		}
	}
}
