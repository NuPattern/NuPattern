using System;
using System.ComponentModel;
using System.Linq;
using NuPattern.Library.Automation;

namespace NuPattern.Library.Design
{
    /// <summary>
    /// A descriptor for a <see cref="CommandReference"/>
    /// </summary>
    internal class CommandReferencePropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// CReates a new instance of the <see cref="CommandReferencePropertyDescriptor"/> class.
        /// </summary>
        /// <param name="descriptor"></param>
        public CommandReferencePropertyDescriptor(PropertyDescriptor descriptor)
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
