using System;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Store
{
	/// <summary>
	/// Customizations for the InstanceBase class.
	/// </summary>
	public partial class InstanceBase : IInstanceBase
	{
		private string name;

		/// <summary>
		/// Occurs when the runtime element is being deleted from the state.
		/// </summary>
		public event EventHandler Deleting = (sender, args) => { };

		/// <summary>
		/// Occurs when the runtime element has been deleted from the state.
		/// </summary>
		public event EventHandler Deleted = (sender, args) => { };

		/// <summary>
		/// Gets the parent element of this instance, if any.
		/// </summary>
		public IInstanceBase Parent
		{
			get
			{
				var mel = this as ModelElement;
				if (mel == null || mel.Store == null)
				{
					return null;
				}

				return DomainRelationshipInfo.FindEmbeddingElement(mel) as IInstanceBase;
			}
		}

		/// <summary>
		/// Gets the root pattern ancestor for this instance. Note that for a pattern, 
		/// this may be an ancestor pattern if it has been instantiated as an 
		/// extension point.
		/// </summary>
		/// <remarks>The returned value may be null if the element is not rooted in any pattern.</remarks>
		public IProduct Root
		{
			get
			{
				IInstanceBase current = this;
				while (current != null && !IsRootProduct(current))
				{
					current = current.Parent;
				}

				return current as IProduct;
			}
		}

		private static bool IsRootProduct(IInstanceBase current)
		{
			return current is IProduct && current.Parent == null;
		}

		/// <summary>
		/// Returns whether the state containing this instance is currently 
		/// in a transaction or not.
		/// </summary>
		public bool InTransaction
		{
			get { return this.Store.TransactionActive; }
		}

		/// <summary>
		/// Returns whether the state containing this instance is currently 
		/// in a serialization transaction or not.
		/// </summary>
		public bool IsSerializing
		{
			get { return this.Store.InSerializationTransaction; }
		}

		/// <summary>
		/// Begins the transaction.
		/// </summary>
		public ITransaction BeginTransaction()
		{
			return new ModelingTransaction(this.Store.TransactionManager.BeginTransaction());
		}

		/// <summary>
		/// Begins the transaction.
		/// </summary>
		public ITransaction BeginTransaction(string name)
		{
			return new ModelingTransaction(this.Store.TransactionManager.BeginTransaction(name));
		}

        /// <summary>
        /// Raised when element is being deleted.
        /// </summary>
		protected override void OnDeleting()
		{
			base.OnDeleting();
			this.Deleting(this, EventArgs.Empty);
		}

        /// <summary>
        /// Raised when element has been deleted.
        /// </summary>
		protected override void OnDeleted()
		{
			base.OnDeleted();
			this.Deleted(this, EventArgs.Empty);
		}

		private string GetDefinitionNameValue()
		{
			return this.Info != null ? this.Info.Name : this.name;
		}

		private void SetDefinitionNameValue(string value)
		{
			this.name = value;
		}
	}
}