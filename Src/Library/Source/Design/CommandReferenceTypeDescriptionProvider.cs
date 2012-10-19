using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Library.Design
{
    /// <summary>
    /// CommandReference type description provider
    /// </summary>
    public class CommandReferenceTypeDescriptionProvider : TypeDescriptionProvider
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
                    d => new CommandIdPropertyDescriptor(d));

                return new PropertyDescriptorCollection(properties.ToArray());
            }

            private class CommandIdPropertyDescriptor : PropertyDescriptor
            {
                public CommandIdPropertyDescriptor(PropertyDescriptor descriptor)
                    : base(descriptor.Name, descriptor.Attributes.Cast<Attribute>().ToArray())
                {
                }

                public override bool CanResetValue(object component)
                {
                    return false;
                }

                public override Type ComponentType
                {
                    get { return typeof(CommandReference); }
                }

                public override object GetValue(object component)
                {
                    var commandReference = component as CommandReference;
                    var settings = commandReference.CommandSettings.Owner.AutomationSettings;

                    return (from cs in settings
                            let setting = cs.As<ICommandSettings>()
                            where setting != null && setting.Id == commandReference.CommandId
                            select cs.Name)
                            .SingleOrDefault();
                }

                public override bool IsReadOnly
                {
                    get { return false; }
                }

                public override Type PropertyType
                {
                    get { return typeof(Guid); }
                }

                public override void ResetValue(object component)
                {
                }

                public override TypeConverter Converter
                {
                    get
                    {
                        return new CommandReferenceConverter();
                    }
                }

                public override void SetValue(object component, object value)
                {
                    var commandReference = component as CommandReference;
                    var settings = commandReference.CommandSettings.Owner.AutomationSettings;

                    var id = (from cs in settings
                             where cs.Name.Equals((string)value)
                             let setting = cs.As<ICommandSettings>()
                             where setting != null
                             select setting.Id)
                              .SingleOrDefault();

                    commandReference.CommandId = id;
                }

                public override bool ShouldSerializeValue(object component)
                {
                    return false;
                }
            }
        }
    }
}
