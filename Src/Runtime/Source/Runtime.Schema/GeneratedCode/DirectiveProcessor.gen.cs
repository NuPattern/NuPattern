﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace NuPattern.Runtime.Schema
{
	#region Using directives
	using DslModeling = global::Microsoft.VisualStudio.Modeling;
	using DslValidation = global::Microsoft.VisualStudio.Modeling.Validation;
	using VSTextTemplating = global::Microsoft.VisualStudio.TextTemplating;
	#endregion
	
	/// <summary>
	/// Double-derived template directive processor that provides PatternModel files
	/// The implementation is done in PatternModelDirectiveProcessorBase. This class
	/// exist so users can customize behavior easily.
	/// </summary>
	public sealed partial class PatternModelDirectiveProcessor : PatternModelDirectiveProcessorBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PatternModelDirectiveProcessor()
			: base()
		{
		}
	}
	
	/// <summary>
	/// Base implementation for template directive processor that provides PatternModel files
	/// </summary>
	public class PatternModelDirectiveProcessorBase : VSTextTemplating::RequiresProvidesDirectiveProcessor
	{
		/// <summary>
		/// The friendly name of this processor.
		/// </summary>
		public const string PatternModelDirectiveProcessorName = "PatternModelDirectiveProcessor";
	
		/// <summary>
		/// The name for the requires parameter that provides the filename of the model
		/// </summary>
		private const string requiresFileParameter = "FileName";
	
		/// <summary>
		/// The name for the requires parameter that provides the validation categories to run. 
		/// The value for this parameter defaults to String.Empty
		/// </summary>
		private const string requiresValidationParameter = "Validation";
	
		/// <summary>
		/// The name for the provides parameter whose value decides the name of the property 
		/// generated by this DirectiveProcessor. The property returns the root element of
		/// the model in the given file. 
		/// </summary>
		private const string providesModelParameter = "PatternModelSchema";
	
		/// <summary>
		/// The default value of the provides parameter above. It defaults to the name of
		/// the parameter itself.
		/// </summary>
		private const string defaultProvidesModelParameter = providesModelParameter;
	
		/// <summary>
		/// The directive name that is supported by this direcive processor. This defaults
		/// to the name of the model. 
		/// </summary>
		private const string supportedDirectiveName = "PatternModel";
		
		/// <summary>
		/// Flag to ensure that some code is only generated once regardless of how many times the processor is used.
		/// </summary>
		private bool oneTimeCodeGenerated;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public PatternModelDirectiveProcessorBase()
			: base()
		{
		}
	
		/// <summary>
		/// The friendly name of this processor.
		/// </summary>
		protected override string FriendlyName
		{
			get
			{
				return PatternModelDirectiveProcessorName;
			}
		}
	
		/// <summary>
		/// Check if the directive name is supported by this directive processor
		/// </summary>
		/// <param name="directiveName"></param>
		/// <returns></returns>
		public override bool IsDirectiveSupported(string directiveName)
		{
			return (global::System.StringComparer.OrdinalIgnoreCase.Compare(directiveName, supportedDirectiveName) == 0);
		}
	
		/// <summary>
		/// Override StartProcessingRun to reset the helpers flag.
		/// </summary>
		/// <param name="languageProvider"></param>
		/// <param name="templateContents"></param>
		/// <param name="errors"></param>
		public override void StartProcessingRun(global::System.CodeDom.Compiler.CodeDomProvider languageProvider, string templateContents, global::System.CodeDom.Compiler.CompilerErrorCollection errors)
		{
			this.oneTimeCodeGenerated = false;
			base.StartProcessingRun(languageProvider, templateContents, errors);
		}
	
	
		/// <summary>
		/// Override to initialize requires dictionary
		/// </summary>
		/// <param name="directiveName"></param>
		/// <param name="requiresDictionary"></param>
		protected override void InitializeRequiresDictionary(string directiveName, global::System.Collections.Generic.IDictionary<string, string> requiresDictionary)
		{
			if ((global::System.StringComparer.OrdinalIgnoreCase.Compare(directiveName, supportedDirectiveName) == 0))
			{
				requiresDictionary[requiresFileParameter] = null;
				requiresDictionary[requiresValidationParameter] = "";
			}
		}
	
		/// <summary>
		/// Override to initialize provides dictinoary
		/// </summary>
		/// <param name="directiveName"></param>
		/// <param name="providesDictionary"></param>
		protected override void InitializeProvidesDictionary(string directiveName, global::System.Collections.Generic.IDictionary<string, string> providesDictionary)
		{
			if ((global::System.StringComparer.OrdinalIgnoreCase.Compare(directiveName, supportedDirectiveName) == 0))
			{
				providesDictionary[providesModelParameter] = defaultProvidesModelParameter;
			}
		}
	
		/// <summary>
		/// Generate the code to access the model. Use to CodeDomProvider so we are language-agnostic
		/// </summary>
		/// <param name="directiveName"></param>
		/// <param name="codeBuffer"></param>
		/// <param name="languageProvider"></param>
		/// <param name="requiresArguments"></param>
		/// <param name="providesArguments"></param>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Generated code is not meant for normalization purpose.")]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
		protected override void GenerateTransformCode(string directiveName, global::System.Text.StringBuilder codeBuffer, global::System.CodeDom.Compiler.CodeDomProvider languageProvider, global::System.Collections.Generic.IDictionary<string, string> requiresArguments, global::System.Collections.Generic.IDictionary<string, string> providesArguments)
		{
			if ((global::System.StringComparer.OrdinalIgnoreCase.Compare(directiveName, supportedDirectiveName) == 0))
			{
				if (languageProvider != null)
				{
					// Create a field to store the model once it is loaded
					global::System.CodeDom.CodeMemberField rootElement = new global::System.CodeDom.CodeMemberField();
					rootElement.Name = providesArguments[providesModelParameter].ToLower(global::System.Globalization.CultureInfo.InvariantCulture) + "Value";
					rootElement.Type = new global::System.CodeDom.CodeTypeReference(typeof(global::NuPattern.Runtime.Schema.PatternModelSchema));
					rootElement.Attributes = global::System.CodeDom.MemberAttributes.Private;
	
					// Create a property for the Model that delay-loads the model
					global::System.CodeDom.CodeMemberProperty rootElementProperty = new global::System.CodeDom.CodeMemberProperty();
					rootElementProperty.Name = providesArguments[providesModelParameter];
					rootElementProperty.Type = new global::System.CodeDom.CodeTypeReference(typeof(global::NuPattern.Runtime.Schema.PatternModelSchema));
					rootElementProperty.Attributes = global::System.CodeDom.MemberAttributes.Private;
					rootElementProperty.HasSet = false;
					rootElementProperty.HasGet = true;
					rootElementProperty.GetStatements.Add(new global::System.CodeDom.CodeMethodReturnStatement(new global::System.CodeDom.CodeFieldReferenceExpression(new global::System.CodeDom.CodeThisReferenceExpression(), rootElement.Name)));
	
					// Create a helper method for resolving relative paths
					global::System.CodeDom.CodeMemberMethod resolver = null;
					if (!this.oneTimeCodeGenerated && global::System.IO.File.Exists(this.Host.TemplateFile))
					{
						resolver = new global::System.CodeDom.CodeMemberMethod();
						resolver.Name = "Convert" + "PatternModelSchema" + "RelativePathToTemplateRelativePath";
						resolver.ReturnType = new global::System.CodeDom.CodeTypeReference(typeof(string));
						resolver.Parameters.Add(new global::System.CodeDom.CodeParameterDeclarationExpression(typeof(string), "path"));
						resolver.Attributes = global::System.CodeDom.MemberAttributes.Public;
						global::System.CodeDom.CodeVariableDeclarationStatement declaration = new global::System.CodeDom.CodeVariableDeclarationStatement(typeof(string), "modelPath", new global::System.CodeDom.CodePrimitiveExpression((string)(requiresArguments[requiresFileParameter])));
						resolver.Statements.Add(declaration);
						declaration = new global::System.CodeDom.CodeVariableDeclarationStatement(typeof(string), "templatePath", new global::System.CodeDom.CodePrimitiveExpression((string)(this.Host.TemplateFile)));
						resolver.Statements.Add(declaration);
						global::System.CodeDom.CodeMethodReturnStatement returnStatement = new global::System.CodeDom.CodeMethodReturnStatement(
							new global::System.CodeDom.CodeMethodInvokeExpression(
								new global::System.CodeDom.CodeMethodReferenceExpression(new global::System.CodeDom.CodeTypeReferenceExpression("Microsoft.VisualStudio.TextTemplating.VSHost.ModelingTextTransformation"), "ConvertModelRelativePathToTemplateRelativePath"),
								new global::System.CodeDom.CodeVariableReferenceExpression("modelPath"),
								new global::System.CodeDom.CodeVariableReferenceExpression("templatePath"),
								new global::System.CodeDom.CodeVariableReferenceExpression("path")));
						resolver.Statements.Add(returnStatement);
					}
	
					// Generate the actual code using the CodeDomProvider
					global::System.CodeDom.Compiler.CodeGeneratorOptions options = new global::System.CodeDom.Compiler.CodeGeneratorOptions();
					options.BlankLinesBetweenMembers = true;
					options.IndentString = "    ";
					options.VerbatimOrder = true;
					options.BracingStyle = "C";
					using (global::System.IO.StringWriter writer = new global::System.IO.StringWriter(codeBuffer, global::System.Globalization.CultureInfo.InvariantCulture))
					{
						languageProvider.GenerateCodeFromMember(rootElement, writer, options);
						languageProvider.GenerateCodeFromMember(rootElementProperty, writer, options);
						if (resolver != null)
						{
							languageProvider.GenerateCodeFromMember(resolver, writer, options);
						}
					}
					this.oneTimeCodeGenerated = true;
				}
			}
		}
	
		/// <summary>
		/// Contribute additively to initialization code for the TextTransformation generated class.
		/// </summary>
		/// <remarks>
		/// This code will be added before the call to the base class.
		/// </remarks>
		/// <param name="directiveName"></param>
		/// <param name="codeBuffer"></param>
		/// <param name="languageProvider"></param>
		/// <param name="requiresArguments"></param>
		/// <param name="providesArguments"></param>
		protected override void GeneratePreInitializationCode(string directiveName, global::System.Text.StringBuilder codeBuffer, global::System.CodeDom.Compiler.CodeDomProvider languageProvider, global::System.Collections.Generic.IDictionary<string, string> requiresArguments, global::System.Collections.Generic.IDictionary<string, string> providesArguments)
		{
			if ((global::System.StringComparer.OrdinalIgnoreCase.Compare(directiveName, supportedDirectiveName) == 0))
			{
				if (languageProvider != null)
				{
					string[] domainModels = {
						"NuPattern.Runtime.Schema.PatternModelDomainModel",
					};
	
					global::System.CodeDom.Compiler.CodeGeneratorOptions options = new global::System.CodeDom.Compiler.CodeGeneratorOptions();
					options.BlankLinesBetweenMembers = true;
					options.IndentString = "    ";
					options.VerbatimOrder = true;
					options.BracingStyle = "C";
					using (global::System.IO.StringWriter writer = new global::System.IO.StringWriter(codeBuffer, global::System.Globalization.CultureInfo.InvariantCulture))
					{
						foreach (string domainModel in domainModels)
						{
							global::System.CodeDom.CodeExpressionStatement addModel = new global::System.CodeDom.CodeExpressionStatement(new global::System.CodeDom.CodeMethodInvokeExpression(new global::System.CodeDom.CodeMethodReferenceExpression(new global::System.CodeDom.CodeThisReferenceExpression(), "AddDomainModel"), new global::System.CodeDom.CodeTypeOfExpression(domainModel)));
							languageProvider.GenerateCodeFromStatement(addModel, writer, options);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Property that indicates whether this directive processor needs access to diagram data as well as
		/// model data.  Normally, this is not the case so the default value of this property is false.
		/// Derived classes may override this property to change the value.
		/// </summary>
		protected virtual bool LoadDiagramData
		{
	 		get
			{
				return false;
			}
		}
	
		/// <summary>
		/// Contribute additively to initialization code for the TextTransformation generated class.
		/// </summary>
		/// <remarks>
		/// This code will be added after the call to the base class.
		/// </remarks>
		/// <param name="directiveName"></param>
		/// <param name="codeBuffer"></param>
		/// <param name="languageProvider"></param>
		/// <param name="requiresArguments"></param>
		/// <param name="providesArguments"></param>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Generated code is not meant for normalization purpose.")]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
		protected override void GeneratePostInitializationCode(string directiveName, global::System.Text.StringBuilder codeBuffer, global::System.CodeDom.Compiler.CodeDomProvider languageProvider, global::System.Collections.Generic.IDictionary<string, string> requiresArguments, global::System.Collections.Generic.IDictionary<string, string> providesArguments)
		{
			if ((global::System.StringComparer.OrdinalIgnoreCase.Compare(directiveName, supportedDirectiveName) == 0))
			{
				// We check the encoding of the model file, and ask the host to use that
				// encoding to write the output. This is to make sure we use the correct encoding
				// if the model file contains international characters that could be used in 
				// the template output.
				string fileName = requiresArguments[requiresFileParameter];
				if (!string.IsNullOrEmpty(fileName) && global::System.IO.File.Exists(fileName))
				{
					Host.SetOutputEncoding(VSTextTemplating::EncodingHelper.GetEncoding(fileName), false);
				}
	
				if (languageProvider != null)
				{
					global::System.CodeDom.CodeMethodInvokeExpression invokeLoad;
					global::System.String transactionName = providesArguments[providesModelParameter].ToLower(global::System.Globalization.CultureInfo.InvariantCulture) + "Transaction";
					global::System.CodeDom.CodeVariableDeclarationStatement transactionDeclaration = new global::System.CodeDom.CodeVariableDeclarationStatement(new global::System.CodeDom.CodeTypeReference(typeof(DslModeling::Transaction)), transactionName, new global::System.CodeDom.CodePrimitiveExpression(null));
					global::System.CodeDom.CodeStatement finallyStatement = new global::System.CodeDom.CodeConditionStatement(new global::System.CodeDom.CodeBinaryOperatorExpression(new global::System.CodeDom.CodeVariableReferenceExpression(transactionName), global::System.CodeDom.CodeBinaryOperatorType.IdentityInequality, new global::System.CodeDom.CodePrimitiveExpression(null)), new global::System.CodeDom.CodeExpressionStatement(new global::System.CodeDom.CodeMethodInvokeExpression(new global::System.CodeDom.CodeVariableReferenceExpression(transactionName), "Dispose")));
					global::System.Collections.Generic.List<global::System.CodeDom.CodeStatement> txTryStatements = new global::System.Collections.Generic.List<global::System.CodeDom.CodeStatement>();
					global::System.CodeDom.CodeVariableDeclarationStatement serializationResultDeclaration = new global::System.CodeDom.CodeVariableDeclarationStatement(new global::System.CodeDom.CodeTypeReference(typeof(DslModeling::SerializationResult)), "serializationResult", new global::System.CodeDom.CodeObjectCreateExpression(new global::System.CodeDom.CodeTypeReference(typeof(DslModeling::SerializationResult))));
	
					global::System.CodeDom.CodeMethodInvokeExpression invokeEnableDiagramRules = null;
					global::System.CodeDom.CodeVariableDeclarationStatement diagramFileDeclaration = null;
					global::System.CodeDom.CodeAssignStatement diagramFileAssign = null;
					if(LoadDiagramData)
					{
						// generate code to enable diagram fixup rules and load the diagram if required.
						invokeEnableDiagramRules = new global::System.CodeDom.CodeMethodInvokeExpression(
							new global::System.CodeDom.CodeMethodReferenceExpression(new global::System.CodeDom.CodeTypeReferenceExpression("PatternModelDomainModel"), "EnableDiagramRules"), new global::System.CodeDom.CodePropertyReferenceExpression(new global::System.CodeDom.CodeThisReferenceExpression(), "Store"));
						diagramFileDeclaration = new global::System.CodeDom.CodeVariableDeclarationStatement(new global::System.CodeDom.CodeTypeReference("System.String"), "diagramFileName");
						diagramFileAssign = new global::System.CodeDom.CodeAssignStatement(new global::System.CodeDom.CodeVariableReferenceExpression("diagramFileName"), new global::System.CodeDom.CodeBinaryOperatorExpression(new global::System.CodeDom.CodePrimitiveExpression(requiresArguments[requiresFileParameter]), global::System.CodeDom.CodeBinaryOperatorType.Add, new global::System.CodeDom.CodePrimitiveExpression(".diagram")));
						invokeLoad = new global::System.CodeDom.CodeMethodInvokeExpression(
							new global::System.CodeDom.CodeMethodReferenceExpression(new global::System.CodeDom.CodeFieldReferenceExpression(new global::System.CodeDom.CodeTypeReferenceExpression(typeof(PatternModelSerializationHelper)), "Instance"), "LoadModelAndDiagram"), new global::System.CodeDom.CodeVariableReferenceExpression("serializationResult"), new global::System.CodeDom.CodePropertyReferenceExpression(new global::System.CodeDom.CodeThisReferenceExpression(), "Store"), new global::System.CodeDom.CodePrimitiveExpression(requiresArguments[requiresFileParameter]), new global::System.CodeDom.CodeVariableReferenceExpression("diagramFileName"), new global::System.CodeDom.CodePrimitiveExpression(null), new global::System.CodeDom.CodePrimitiveExpression(null), new global::System.CodeDom.CodePrimitiveExpression(null));
					}
					else
					{
						invokeLoad = new global::System.CodeDom.CodeMethodInvokeExpression(
							new global::System.CodeDom.CodeMethodReferenceExpression(new global::System.CodeDom.CodeFieldReferenceExpression(new global::System.CodeDom.CodeTypeReferenceExpression(typeof(PatternModelSerializationHelper)), "Instance"), "LoadModel"), new global::System.CodeDom.CodeVariableReferenceExpression("serializationResult"), new global::System.CodeDom.CodePropertyReferenceExpression(new global::System.CodeDom.CodeThisReferenceExpression(), "Store"), new global::System.CodeDom.CodePrimitiveExpression(requiresArguments[requiresFileParameter]), new global::System.CodeDom.CodePrimitiveExpression(null), new global::System.CodeDom.CodePrimitiveExpression(null), new global::System.CodeDom.CodePrimitiveExpression(null));
					}
	
					global::System.CodeDom.CodeAssignStatement loadAssign = new global::System.CodeDom.CodeAssignStatement(new global::System.CodeDom.CodeFieldReferenceExpression(new global::System.CodeDom.CodeThisReferenceExpression(), providesArguments[providesModelParameter].ToLower(global::System.Globalization.CultureInfo.InvariantCulture) + "Value"), invokeLoad);
					txTryStatements.Add(serializationResultDeclaration);
					txTryStatements.Add(new global::System.CodeDom.CodeAssignStatement(new global::System.CodeDom.CodeVariableReferenceExpression(transactionName), new global::System.CodeDom.CodeMethodInvokeExpression(new global::System.CodeDom.CodePropertyReferenceExpression(new global::System.CodeDom.CodePropertyReferenceExpression(new global::System.CodeDom.CodeThisReferenceExpression(), "Store"), "TransactionManager"), "BeginTransaction", new global::System.CodeDom.CodePrimitiveExpression("Load"), new global::System.CodeDom.CodePrimitiveExpression(true))));
					txTryStatements.Add(loadAssign);
					global::System.CodeDom.CodeConditionStatement serializationResultCheck = new global::System.CodeDom.CodeConditionStatement(
						new global::System.CodeDom.CodePropertyReferenceExpression(new global::System.CodeDom.CodeVariableReferenceExpression("serializationResult"), "Failed"),
						new global::System.CodeDom.CodeThrowExceptionStatement(new global::System.CodeDom.CodeObjectCreateExpression(new global::System.CodeDom.CodeTypeReference(typeof(DslModeling::SerializationException)), new global::System.CodeDom.CodeVariableReferenceExpression("serializationResult")))
					);
					txTryStatements.Add(serializationResultCheck);
					txTryStatements.Add(new global::System.CodeDom.CodeExpressionStatement(new global::System.CodeDom.CodeMethodInvokeExpression(new global::System.CodeDom.CodeVariableReferenceExpression(transactionName), "Commit")));
					global::System.CodeDom.CodeTryCatchFinallyStatement txTryStatement = new global::System.CodeDom.CodeTryCatchFinallyStatement(txTryStatements.ToArray(), new global::System.CodeDom.CodeCatchClause[] { }, new global::System.CodeDom.CodeStatement[] { finallyStatement });
	
	
					// We check if the user has requested validation to be performed. If so, we call
					// ValidateStore() on the base ModelingTextTransformation class.
					string validationCategories = requiresArguments[requiresValidationParameter];
					global::System.CodeDom.CodeMethodInvokeExpression validationInvoke = null;
					if (!string.IsNullOrEmpty(validationCategories))
					{
						validationInvoke = new global::System.CodeDom.CodeMethodInvokeExpression(new global::System.CodeDom.CodeThisReferenceExpression(), "ValidateStore", new global::System.CodeDom.CodePrimitiveExpression(validationCategories), new global::System.CodeDom.CodePropertyReferenceExpression(new global::System.CodeDom.CodeThisReferenceExpression(), "Errors"));
					}
	
					global::System.CodeDom.Compiler.CodeGeneratorOptions options = new global::System.CodeDom.Compiler.CodeGeneratorOptions();
					options.BlankLinesBetweenMembers = true;
					options.IndentString = "    ";
					options.VerbatimOrder = true;
					options.BracingStyle = "C";
					using (global::System.IO.StringWriter writer = new global::System.IO.StringWriter(codeBuffer, global::System.Globalization.CultureInfo.InvariantCulture))
					{
						if(invokeEnableDiagramRules != null)
						{
							languageProvider.GenerateCodeFromStatement(new global::System.CodeDom.CodeExpressionStatement(invokeEnableDiagramRules), writer, options);
						}
						
						if(diagramFileDeclaration != null)
						{
							languageProvider.GenerateCodeFromStatement(diagramFileDeclaration, writer, options);
							languageProvider.GenerateCodeFromStatement(diagramFileAssign, writer, options);
						}
						
						languageProvider.GenerateCodeFromStatement(transactionDeclaration, writer, options);
						languageProvider.GenerateCodeFromStatement(txTryStatement, writer, options);
						
						if (validationInvoke != null)
						{
							languageProvider.GenerateCodeFromStatement(new global::System.CodeDom.CodeExpressionStatement(validationInvoke), writer, options);
						}
					}
				}
			}
		}
	
		/// <summary>
		/// Process arguments
		/// </summary>
		/// <param name="directiveName"></param>
		/// <param name="requiresArguments"></param>
		/// <param name="providesArguments"></param>
		protected override void PostProcessArguments(string directiveName, global::System.Collections.Generic.IDictionary<string, string> requiresArguments, global::System.Collections.Generic.IDictionary<string, string> providesArguments)
		{
			if ((global::System.StringComparer.OrdinalIgnoreCase.Compare(directiveName, supportedDirectiveName) == 0))
			{
				// Give the host a chance to resolve the fileName
				requiresArguments[requiresFileParameter] = this.Host.ResolvePath(requiresArguments[requiresFileParameter]);
			}
		}
	
		/// <summary>
		/// Return namespace imports necessary for running template
		/// </summary>
		/// <returns></returns>
		public override string[] GetImportsForProcessingRun()
		{
			global::System.Collections.Generic.List<string> imports = new global::System.Collections.Generic.List<string>(base.GetImportsForProcessingRun());
	
			imports.Add("Microsoft.VisualStudio.Modeling");
			imports.Add("System.CodeDom.Compiler");
			imports.Add("NuPattern.Runtime.Schema");
	
			return imports.ToArray();
		}
	
		/// <summary>
		/// Get assembly references needed for running template
		/// </summary>
		/// <returns></returns>
		public override string[] GetReferencesForProcessingRun()
		{
			global::System.Collections.Generic.List<string> references = new global::System.Collections.Generic.List<string>(base.GetReferencesForProcessingRun());
			references.Add(this.GetType().Assembly.Location);
			references.Add(typeof(global::Microsoft.VisualStudio.Modeling.Diagrams.Diagram).Assembly.Location);
			references.Add(typeof(DslModeling::Store).Assembly.Location);
			references.Add("Microsoft.VisualStudio.TextTemplating.Modeling.11.0.dll");
			return references.ToArray();
		}
	}
}

