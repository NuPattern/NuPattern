using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Store.Properties;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
	/// <summary>
	/// A ProductElement.
	/// </summary>
	[ValidationState(ValidationState.Enabled)]
	public partial class ProductElement : IProductElement, IDataErrorInfo
	{
		/// <summary>
		/// Validates the validation bindings of the element.
		/// </summary>
		/// <param name="context"></param>
		[ValidationMethod(ValidationCategories.Custom, CustomCategory = ValidationConstants.RuntimeValidationCategory)]
		internal void ValidateValidationBindings(ValidationContext context)
		{
			try
			{
				if (this.ValidationBindings != null)
				{
					foreach (var binding in this.ValidationBindings)
					{
						if (binding.Evaluate(this.BindingContext))
						{
							var results = binding.Value.Validate();
							if (results != null) // null result is considered Success
							{
								foreach (var result in results)
								{
									context.LogError(result.ErrorMessage,
										Resources.ProductElement_ValidationFailedErrorCode, this);
								}
							}
						}
						else
						{
							context.LogError(Resources.ProductElement_BindingEvaluationFailed,
								Resources.ProductElement_BindingEvaluationErrorCode, this);
						}
					}
				}
			}
			catch (Exception ex)
			{
				tracer.TraceError(
					ex,
					Resources.ValidationMethodFailed_Error,
					Reflector<ProductElement>.GetMethod(n => n.ValidateValidationBindings(context)).Name);

				context.LogError(
					String.Format(CultureInfo.CurrentCulture, Resources.ProductElement_BindingExceptionDescription, ex.Message), string.Empty, this);
			}
		}

		/// <summary>
		/// Validates the typed interface layer data annotations.
		/// </summary>
		[ValidationMethod(ValidationCategories.Custom, CustomCategory = ValidationConstants.RuntimeValidationCategory)]
		internal void ValidateInterfaceLayer(ValidationContext dslContext)
		{
			var layer = ToolkitInterfaceLayer.GetInterfaceLayer(this);
			if (layer != null)
			{
				var dataContext = new System.ComponentModel.DataAnnotations.ValidationContext(layer, this.Store, null);
				var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

				System.ComponentModel.DataAnnotations.Validator.TryValidateObject(layer, dataContext, results, true);

				if (results.Any())
				{
					var properties = TypeDescriptor.GetProperties(layer);
					var typeName = this.Info != null ? 
						// If we have an info, first check that it differs from the instance name
						// to avoid duplicating the same string
						this.InstanceName != this.Info.DisplayName ?
							this.InstanceName +	" (" + this.Info.DisplayName + ")" :
							// If they are equal, just use the instance name.
							this.InstanceName
						: this.InstanceName;

					foreach (var result in results)
					{
						// Replace member names with display names in message and member names collection.
						var members = result.MemberNames
							.Select(name => new
							{
								Name = name,
								DisplayName = properties[name].DisplayName,
								FormattedName =  typeName + " '" + properties[name].DisplayName + "'",
							});

						var message = result.ErrorMessage;

						foreach (var member in members)
						{
							if (message.IndexOf(member.Name) != -1)
								message = message.Replace(member.Name, member.FormattedName);
							else if (message.IndexOf(member.DisplayName) != -1)
								message = message.Replace(member.DisplayName, member.FormattedName);
						}

						dslContext.LogError(message,
							Resources.ProductElement_ValidationFailedErrorCode, this);
					}
				}
			}
		}

		/// <summary>
		/// Validates that the instance name is of legal form.
		/// </summary>
		[ValidationMethod(ValidationCategories.Custom, CustomCategory = ValidationConstants.RuntimeValidationCategory)]
		internal void ValidateInstanceNameIsLegal(ValidationContext context)
		{
			try
			{
				if (!IsValidInstanceName(this.InstanceName))
				{
					context.LogError(
						string.Format(CultureInfo.CurrentCulture, Resources.ProductElement_InstanceNameNotValid, this.InstanceName),
						Resources.ProductElement_InstanceNameNotValidCode, this);
				}
			}
			catch (Exception ex)
			{
				tracer.TraceError(
					ex,
					Resources.ValidationMethodFailed_Error,
					Reflector<ProductElement>.GetMethod(n => n.ValidateInstanceNameIsLegal(context)).Name);

				throw;
			}
		}

		/// <summary>
		/// Determines whether the given value is a valid instance name.
		/// </summary>
		private static bool IsValidInstanceName(string value)
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

		/// <summary>
		/// Gets the validation error
		/// </summary>
		public virtual string Error
		{
			get
			{
				// Not used. If we want model-level validation we can validate on property change and state the list of errors. 
				// this[columnName} would then use this list instead of revalidating.
				return null;
			}
		}

		/// <summary>
		/// Gets the error text.
		/// </summary>
		public virtual string this[string columnName]
		{
			get
			{
				// Should we validate InstanceName? if (columnName == "InstanceName")
				// TODO: verify if there is a cheaper way of getting errors without the need to revalidate each time, because some validation is
				// already happening elsewhere (for example, errors are being shown in the VS' Error List).
				var property = this.Properties.Find(p => p.DefinitionName == columnName);
				if (property != null)
				{
					var validationController = new ValidationController();
					if (!validationController.ValidateCustom(property, ValidationConstants.RuntimeValidationCategory))
					{
						var errors = validationController.ErrorMessages.Select(e => e.Description).Where(d => d != null).ToList();
						if (errors.Count > 0)
						{
							return string.Join("\n", errors);
						}
					}
				}

				return null;
			}
		}
	}
}