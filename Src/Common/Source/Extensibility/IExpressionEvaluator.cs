using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Interface for evaluator, used to mock it if needed.
	/// </summary>
	internal interface IExpressionEvaluator
	{
		/// <summary>
		/// Evaluates the specified expression.
		/// </summary>
		string Evaluate(string expression);
	}
}
