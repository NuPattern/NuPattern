using System;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Marks a member as hidden for code generation purposes or other.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class HiddenAttribute : Attribute
	{
	}
}