using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Compiler helper class")]
public class Compiler
{
	private const string CompilerVersion = "CompilerVersion";
	private const string CompilerVersionValue = "v3.5";

	public Compiler()
	{
		this.ReferenceAssemblies = GetCommonReferenceAssemblies();
	}

	public IList<string> ReferenceAssemblies { get; private set; }

	[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code")]
	public CompilerResults CompileAssemblyFromCSharp(string csharpContent)
	{
		var providerOptions = new Dictionary<string, string>();

		providerOptions.Add(CompilerVersion, CompilerVersionValue);

		return this.CompileAssemblyFromSourceImpl(
			new string[] { csharpContent },
			null,
			new CSharpCodeProvider(providerOptions));
	}

	[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code")]
	public CompilerResults CompileAssemblyFromVB(string visualBasicContent)
	{
		var providerOptions = new Dictionary<string, string>();

		providerOptions.Add(CompilerVersion, CompilerVersionValue);

		return this.CompileAssemblyFromSourceImpl(
			new string[] { visualBasicContent },
			null,
			new VBCodeProvider(providerOptions));
	}

	private static IList<string> GetCommonReferenceAssemblies()
	{
		return new List<string>
		{ 
			"mscorlib.dll", 
			"System.dll", 
			"System.Core.dll",
			"System.Drawing.dll"
		};
	}

	private CompilerResults CompileAssemblyFromSourceImpl(
		IEnumerable<string> sourceContents,
		string outputAssemblyName,
		CodeDomProvider provider)
	{
		var parameters = new CompilerParameters();

		parameters.GenerateExecutable = false;
		parameters.GenerateInMemory = string.IsNullOrEmpty(outputAssemblyName);
		parameters.IncludeDebugInformation = false;
		parameters.TreatWarningsAsErrors = true;
		parameters.CompilerOptions = "/optimize";
		parameters.OutputAssembly = outputAssemblyName;

		if (this.ReferenceAssemblies.Count() > 0)
		{
			parameters.ReferencedAssemblies.AddRange(this.ReferenceAssemblies.ToArray());
		}

		return provider.CompileAssemblyFromSource(parameters, sourceContents.ToArray());
	}
}