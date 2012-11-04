using System;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
	internal class StorePropertyBag : IDisposable
	{
		private bool ownedKey = false;
		private bool disposed = false;
		private Microsoft.VisualStudio.Modeling.Store store;
		private string propertyBagKey;

		public StorePropertyBag(Microsoft.VisualStudio.Modeling.Store store, string propertyBagKey, object propertyBagValue)
		{
			this.store = store;
			this.propertyBagKey = propertyBagKey;

			object value;
	
			if (!this.store.PropertyBag.TryGetValue(propertyBagKey, out value))
			{
				this.ownedKey = true;
				this.store.PropertyBag[this.propertyBagKey] = propertyBagValue;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.ownedKey)
					{
						this.store.PropertyBag.Remove(this.propertyBagKey);
					}
				}

				this.disposed = true;
			}
		}
	}
}