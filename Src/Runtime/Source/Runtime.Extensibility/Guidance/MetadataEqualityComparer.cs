using System.Collections.Generic;
using NuPattern.ComponentModel.Composition;

namespace NuPattern.Runtime.Guidance
{
    internal class MetadataEqualityComparer : IEqualityComparer<IFeatureComponentMetadata>
    {
        public bool Equals(IFeatureComponentMetadata x, IFeatureComponentMetadata y)
        {
            return object.ReferenceEquals(x, y) || object.Equals(x.Id, y.Id);
        }

        public int GetHashCode(IFeatureComponentMetadata obj)
        {
            if (obj.Id == null)
            {
                return obj.GetHashCode();
            }

            return obj.Id.GetHashCode();
        }
    }
}