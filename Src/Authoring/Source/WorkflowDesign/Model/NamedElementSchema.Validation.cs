using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Extensibility;

namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Customizations for the NamedElementSchema class.
	/// </summary>
	[ValidationState(ValidationState.Enabled)]
	public abstract partial class NamedElementSchema
	{
		internal static readonly string[] ReservedNames = new[] 
        { 
            Reflector<Runtime.IProductElement>.GetProperty(p => p.InstanceName).Name, 
        };

		/// <summary>
		/// Validates if an element has a valid name.
		/// </summary>
		[ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
		internal void ValidateNameIsLegal(ValidationContext context)
		{
			if (!DataFormats.IsValidCSharpIdentifier(this.Name))
			{
				context.LogError(
					string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_NamedElementNameNotValid, this.Name),
					Properties.Resources.Validate_NamedElementNameNotValidCode, this);
			}
		}

		/// <summary>
		/// Validates if an element has a valid display name.
		/// </summary>
		[ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
		internal void ValidateDisplayNameIsLegal(ValidationContext context)
		{
			if (!IsValidDisplayName(this.DisplayName))
			{
				context.LogError(
					string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_NamedElementDisplayNameNotValid, this.Name),
					Properties.Resources.Validate_NamedElementDisplayNameNotValidCode, this);
			}
		}

		/// <summary>
		/// Validates if an element has a reserved name.
		/// </summary>
		[ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
		internal void ValidateNameIsNotReserved(ValidationContext context)
		{
			if (ReservedNames.SingleOrDefault(name => name == this.Name) != null)
			{
				context.LogError(
					string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_NamedElementDisplayNameReserved, this.Name,
					string.Join(", ", ReservedNames)),
					Properties.Resources.Validate_NamedElementDisplayNameReservedCode, this);
			}
		}

		/// <summary>
		/// Determines whether the given value is a valid display name.
		/// </summary>
		private static bool IsValidDisplayName(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}

			if (value.Length > DataFormats.DesignTime.MaxPropertyLength)
			{
				return false;
			}

			return Regex.IsMatch(value, DataFormats.DesignTime.DisplayNameExpression);
		}
	}
}
