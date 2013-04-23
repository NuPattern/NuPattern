using System;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Defines an attribute that is used to designate a DomainClass, DomainRole, DomainProperty as represeting a customizable setting.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	internal sealed class CustomizableDomainElementSettingAttribute : CustomizableSettingAttribute
	{
	}
}
