using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.Shell;

namespace NuPattern.Modeling
{
    [Export(typeof(IUriReferenceProvider))]
    internal class ModelBusUriReferenceProvider : IUriReferenceProvider<ModelBusReference>
    {
        private const string Scheme = "modelbus";
        private IModelBus modelBus;

        [ImportingConstructor]
        public ModelBusUriReferenceProvider([Import(typeof(SVsServiceProvider), AllowDefault = true)] IServiceProvider serviceProviderForModelBus)
            : this((IModelBus)serviceProviderForModelBus.GetService(typeof(SModelBus)))
        {
        }

        public ModelBusUriReferenceProvider(IModelBus modelBus)
        {
            Guard.NotNull(() => modelBus, modelBus);

            this.modelBus = modelBus;
        }

        public string UriScheme
        {
            get { return Scheme; }
        }

        public Uri CreateUri(ModelBusReference instance)
        {
            // Append our "progression" authority to get away with the normalization of 
            // the adapter id that would happen otherwise.
            var serialized = ModelBusReference.Serialize(instance);
            serialized = serialized.Replace(@"://", @"://progression/");

            return new Uri(serialized);
        }

        public void Open(ModelBusReference instance)
        {
            //this.modelingService.OpenModel<object>(instance);

            try
            {
                ModelBusAdapter adapter = this.modelBus.CreateAdapter(instance);
                var view = adapter.GetDefaultView();
                view.SetSelection(instance);
                view.Open();
                view.Show();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format(
                        CultureInfo.CurrentCulture,
                        Properties.Resources.ModelBusUriReferenceProvider_CantCreateAdapterFromReference,
                        instance.ToString()), ex);
            }
        }

        public ModelBusReference ResolveUri(Uri uri)
        {
            // Standard MBR serialization format is: modelbus://adapterId/modelId/elementId/viewId
            // viewId contains a file path that we need to fix 'cause the 
            // Uri will normalize the slashes to forward ones, but the MBR
            // needs them as originally created from the file path.
            var sourceIdentifier = uri.ToString();

            // Escaped QIds have single quotes surrounding the string
            //sourceIdentifier = sourceIdentifier.Substring(1, sourceIdentifier.Length - 2);

            var sourceUri = new Uri(sourceIdentifier);
            var busBaseUri = sourceUri.GetLeftPart(UriPartial.Authority);
            // Can't use sourceUri.PathAndQuery as it contains escaped whitespaces
            // which won't match the expected serialized MBR.
            var sourcePath = sourceIdentifier.Substring(busBaseUri.Length);
            // First three segments are: [/][adapterId/][modelId/][elementId/]
            var modelAndElementIds = String.Join("", sourceUri.Segments.Skip(1).Take(3).ToArray());

            var viewId = sourcePath.Substring(modelAndElementIds.Length + 1);
            // Fix view path. Should be no-op if viewId is empty.
            var viewPath = viewId.Replace('/', '\\');

            var serializedReference = sourceUri.GetLeftPart(UriPartial.Scheme) + modelAndElementIds + viewPath;

            return ModelBusReference.Deserialize(serializedReference, modelBus, new ReferenceContext());
        }
    }
}
