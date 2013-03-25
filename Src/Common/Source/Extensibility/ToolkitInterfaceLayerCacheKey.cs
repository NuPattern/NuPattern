using System;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
    internal class ToolkitInterfaceLayerCacheKey : IEquatable<ToolkitInterfaceLayerCacheKey>
    {
        private const string KeyName = "ToolkitInterfaceLayerCacheKey";

        public ToolkitInterfaceLayerCacheKey(IInstanceBase runtimeElement, Guid definitionId)
        {
            this.Element = runtimeElement;
            this.DefinitionId = definitionId;
        }

        public IInstanceBase Element { get; private set; }

        public Guid DefinitionId { get; private set; }

        #region Equality

        public bool Equals(ToolkitInterfaceLayerCacheKey other)
        {
            return ToolkitInterfaceLayerCacheKey.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return ToolkitInterfaceLayerCacheKey.Equals(this, obj as ToolkitInterfaceLayerCacheKey);
        }

        public static bool Equals(ToolkitInterfaceLayerCacheKey obj1, ToolkitInterfaceLayerCacheKey obj2)
        {
            if (Object.Equals(null, obj1) ||
                Object.Equals(null, obj2) ||
                obj1.GetType() != obj2.GetType())
                return false;

            if (Object.ReferenceEquals(obj1, obj2))
                return true;

            return Object.Equals(obj1.Element, obj2.Element)
                && Object.Equals(obj1.DefinitionId, obj2.DefinitionId);
        }

        public override int GetHashCode()
        {
            var hash = this.Element.GetHashCode();
            hash = hash ^ this.DefinitionId.GetHashCode();
            hash = hash ^ KeyName.GetHashCode();

            return hash;
        }

        #endregion
    }
}
