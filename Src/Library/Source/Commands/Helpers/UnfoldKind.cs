
namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
	/// <summary>
	/// Specifies the way the template is being unfolded.
	/// </summary>
	public enum UnfoldKind
	{
		/// <summary>
		/// The unfold operation was initiated by the user by 
		/// unfolding a template.
		/// </summary>
		FromTemplate,

		/// <summary>
		/// The unfold operation was initiated by the automation 
		/// extension as part of handling an event or being 
		/// directly executed.
		/// </summary>
		FromAutomation,

		/// <summary>
		/// No current unfold operation is happening.
		/// </summary>
		None,
	}
}
