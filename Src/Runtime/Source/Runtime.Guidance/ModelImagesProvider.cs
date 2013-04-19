using System;
using System.Drawing;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Utilities
{
    internal class ModelImagesProvider<TDomainModel> : IDisposable
        where TDomainModel : DomainModel
    {
        #region private fields
        private bool disposed = false;
        private Store store;
        private dynamic toolboxHelper;
        #endregion

        public ModelImagesProvider(dynamic toolboxHelper)
        {
            this.toolboxHelper = toolboxHelper;
            this.store = new Store(typeof(CoreDomainModel), typeof(TDomainModel));
        }

        internal Bitmap GetImage<T>()
        {
            return GetImage(typeof(T).Name);
        }

        internal Bitmap GetImage<T>(string preffix)
        {
            return GetImage<T>(preffix, "ToolboxItem");
        }

        internal Bitmap GetImage<T>(string preffix, string suffix)
        {
            return DoGetImage(preffix + typeof(T).Name + suffix);
        }

        internal Bitmap GetImage(string name)
        {
            return DoGetImage(typeof(TDomainModel).Namespace + "." + name + "ToolboxItem");
        }

        private Bitmap DoGetImage(string name)
        {
            using (var transaction = this.store.TransactionManager.BeginTransaction("Getting Toolbox Item"))
            {
                var toolboxItem = toolboxHelper.GetToolboxItem(name, store) as ModelingToolboxItem;
                if (toolboxItem == null || toolboxItem.Bitmap == null)
                {
                    return null;
                }

                toolboxItem.Bitmap.MakeTransparent(Color.Magenta);
                return toolboxItem.Bitmap;
            }
        }

        #region Dispose
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overridable Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this.store != null)
                    {
                        this.store.Dispose();
                        this.store = null;
                    }
                }
            }
            this.disposed = true;
        }
        #endregion

    }
}