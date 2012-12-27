
namespace NuPattern.Library
{
	/// <summary>
	/// The formats of a GUID in string form.
	/// </summary>
	public enum GuidFormat
	{
		/// <summary>
		/// In the form: 00000000000000000000000000000000 (N)
		/// </summary>
		JustDigits,
		/// <summary>
		/// In the form: 00000000-0000-0000-0000-000000000000 (D) 
		/// </summary>
		JustDigitsWithHyphens,
		/// <summary>
		/// In the form: {00000000-0000-0000-0000-000000000000} (B)
		/// </summary>
		DigitsHyphensCurlyBraces,
		/// <summary>
		/// In the form: (00000000-0000-0000-0000-000000000000) (P)
		/// </summary>
		DigitsHyphensRoundBraces,
		/// <summary>
		/// In the form: {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}} (X)
		/// </summary>
		Hexadecimal
	}
}
