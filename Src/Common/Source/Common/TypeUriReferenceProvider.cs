using System;
using System.ComponentModel.Composition;

namespace NuPattern
{
    [Export(typeof(IUriReferenceProvider))]
    internal class TypeUriReferenceProvider : IUriReferenceProvider<Type>
    {
        public string UriScheme { get { return "type"; } }

        public Uri CreateUri(Type instance)
        {
            return new Uri(UriScheme + "://assembly-qualified/" + instance.AssemblyQualifiedName);
        }

        public Type ResolveUri(Uri uri)
        {
            return Type.GetType(Uri.UnescapeDataString(uri.PathAndQuery.Substring(1)));
        }

        public void Open(Type instance)
        {
            throw new NotImplementedException();
        }
    }
}
