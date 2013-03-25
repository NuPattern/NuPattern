using System;
using System.ComponentModel;
using System.Linq;
using NuPattern.ComponentModel;

namespace NuPattern.Library.Design
{
    /// <summary>
    /// CommandReference type description provider
    /// </summary>
    internal class CommandReferenceTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static TypeDescriptionProvider parent = TypeDescriptor.GetProvider(typeof(CommandReference));

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandReferenceTypeDescriptionProvider"/> class.
        /// </summary>
        public CommandReferenceTypeDescriptionProvider()
            : base(parent)
        {
        }

        /// <summary>
        /// Gets a custom type descriptor for the given type and object.
        /// </summary>
        /// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
        /// <param name="instance">An instance of the type. Can be null if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor"/>.</param>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.ICustomTypeDescriptor"/> that can provide metadata for the type.
        /// </returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new CommandReferenceTypeDescriptor(parent.GetTypeDescriptor(objectType, instance), instance as CommandReference);
        }

        private class CommandReferenceTypeDescriptor : CustomTypeDescriptor
        {
            private ICustomTypeDescriptor parent;

            public CommandReferenceTypeDescriptor(ICustomTypeDescriptor parent, CommandReference commandReference)
                : base(parent)
            {
                this.parent = parent;
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = this.parent.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

                properties.ReplaceDescriptor<CommandReference, Guid>(
                    x => x.CommandId,
                    d => new CommandReferencePropertyDescriptor(d));

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }
    }
}
