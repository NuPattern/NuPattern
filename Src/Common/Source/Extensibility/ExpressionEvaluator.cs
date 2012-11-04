using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	// NOTE: this evaluator now has pattern-toolkit knowledge, so it will 
	// never be removed from VSPAT. Maybe it should be renamed to PatternExpressionEvaluator or something.

	/// <summary>
	/// Evaluates expression against a target.
	/// For example: {Prop1.Prop2}Foo{Prop3}.cs
	/// </summary>
	public class ExpressionEvaluator : IExpressionEvaluator
	{
		static Regex regex = new Regex(@"\{(?<Expression>[^\}]+)\}", RegexOptions.Singleline | RegexOptions.Compiled);
		private IPropertyEvaluator propertyEvaluator;

		internal static Func<object, IExpressionEvaluator> DefaultFactory { get; set; }

		/// <summary>
		/// Initializes for any instance of the <see cref="ExpressionEvaluator"/> class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static ExpressionEvaluator()
		{
			DefaultFactory = target => new ExpressionEvaluator(target, new PropertyEvaluator());
		}

		internal object Target { get; private set; }

		/// <summary>
		/// Creates a new instance of the <see cref="ExpressionEvaluator"/> class.
		/// </summary>
		public ExpressionEvaluator(object target, IPropertyEvaluator propertyEvaluator)
		{
			Guard.NotNull(() => target, target);
			Guard.NotNull(() => propertyEvaluator, propertyEvaluator);

			this.Target = target;
			this.propertyEvaluator = propertyEvaluator;
		}

		/// <summary>
		/// Evaluates the expression on  the the target object.
		/// </summary>
		public static string Evaluate(object target, string expression)
		{
			return DefaultFactory(target).Evaluate(expression);
		}

		/// <summary>
		/// Evaluates the expression as a regular expression.
		/// </summary>
		public string Evaluate(string expression)
		{
			if (string.IsNullOrEmpty(expression))
				return expression;

			return regex.Replace(expression, new MatchEvaluator(Evaluate));
		}

		private string Evaluate(Match match)
		{
			var expression = match.Groups["Expression"].Value;
			var target = this.Target;

			foreach (var propertyName in expression.Split('.'))
			{
				target = this.propertyEvaluator.Evaluate(target, propertyName);
			}

			return target != null ? target.ToString() : string.Empty;
		}
	}
}