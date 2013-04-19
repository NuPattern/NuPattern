using System.Collections.Generic;
using NuPattern.ComponentModel.Composition;

namespace NuPattern.Runtime.Guidance
{
    internal class MetadataEqualityComparer : IEqualityComparer<IComponentMetadata>
    {
        public bool Equals(IComponentMetadata x, IComponentMetadata y)
        {
            return object.ReferenceEquals(x, y) || object.Equals(x.Id, y.Id);
        }

        public int GetHashCode(IComponentMetadata obj)
        {
            if (obj.Id == null)
            {
                return obj.GetHashCode();
            }

            return obj.Id.GetHashCode();
        }
    }
}