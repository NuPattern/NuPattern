using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.TextTemplating.Modeling;

namespace Microsoft.VisualStudio.Patterning.Library
{
	/// <summary>
	/// Base class for text transformation templates that receive the element via the session context.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Optionally, the T4 session can also pass the target name to generate using the <see cref="KeyTargetName"/> key.
	/// </para>
	/// </remarks>
	public abstract class ModelElementTextTransformation : ModelBusEnabledTextTransformation
	{
		/// <summary>
		/// Key in the session properties that contains the serialized model bus reference 
		/// of the element to apply the transformation to.
		/// </summary>
		public const string KeyModelBusReference = "ModelElementTextTransformation.ModelBusReference";

		/// <summary>
		/// Optional key in the session properties that contains the target name to generate.
		/// </summary>
		public const string KeyTargetName = "ModelElementTextTransformation.TargetName";

		private object element;
		private ConcurrentDictionary<string, string> sessionProperties;

		/// <summary>
		/// Disposes the current element.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			this.element = null;
		}

		/// <summary>
		/// Gets the resolved element from the serialized reference.
		/// </summary>
		protected object Element
		{
			get { return this.element ?? (this.element = ResolveElement(this.ModelBusReference)); }
		}

		/// <summary>
		/// Gets the model bus reference serialized in the context.
		/// </summary>
		protected string ModelBusReference
		{
			get { return this.SessionProperties.GetOrAdd(KeyModelBusReference, string.Empty); }
		}

		/// <summary>
		/// Gets the name of the target class or code to generate.
		/// </summary>
		protected string TargetName
		{
			get { return this.SessionProperties.GetOrAdd(KeyTargetName, string.Empty); }
		}

		private ConcurrentDictionary<string, string> SessionProperties
		{
			get
			{
				//return this.sessionProperties ?? (this.sessionProperties =
				//    new ConcurrentDictionary<string, string>(this.Session
				//        .Where(pair => pair.Value is string)
				//        .Select(pair => new KeyValuePair<string, string>(pair.Key, (string)pair.Value))));

				// TODO: make this go via the actual template session, instead of hardcoded callcontext.
				if (this.sessionProperties == null)
				{
					this.sessionProperties = new ConcurrentDictionary<string, string>(new KeyValuePair<string, string>[]
					{
						new KeyValuePair<string, string>(KeyModelBusReference, (string)CallContext.LogicalGetData(KeyModelBusReference)), 
						new KeyValuePair<string, string>(KeyTargetName, (string)CallContext.LogicalGetData(KeyTargetName)), 
					});
				}

				return this.sessionProperties;
			}
		}

		/// <summary>
		/// Given an identifier for an Element, returns the actual object represented
		/// by this reference.
		/// </summary>
		/// <param name="reference">Unique identifier for a particular element</param>
		/// <returns>Instance of the resolved element object</returns>
		protected virtual object ResolveElement(string reference)
		{
			var modelBusReference = this.ModelBus.DeserializeReference(reference, null);
			var manager = this.ModelBus.GetAdapterManager(modelBusReference.LogicalAdapterId);
			var adapter = (ModelingAdapter)manager.CreateAdapter(modelBusReference);
			return adapter.ResolveElementReference(modelBusReference);
		}
	}
}