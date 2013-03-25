using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility.Bindings;
using NuPattern.Extensibility.Bindings.Design;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    internal class DefaultValuePropertyDescriptor : PropertyDescriptor
    {
        private static ITraceSource tracer = Tracer.GetSourceFor<DefaultValuePropertyDescriptor>();
        private PropertySchema schema;
        private Type propertyType;

        public DefaultValuePropertyDescriptor(string propertyName, PropertySchema schema, Type propertyType, Attribute[] attrs)
            : base(propertyName, attrs.Add(new DefaultValueAttribute(propertyType.GetDefaultValueString())).ToArray())
        {
            this.schema = schema;
            this.propertyType = TypeDescriptor.GetProvider(propertyType).GetRuntimeType(propertyType);
        }

        public override bool CanResetValue(object component)
        {
            return this.schema.DefaultValue.IsConfigured();
        }

        public override Type ComponentType
        {
            get { return typeof(PropertySchema); }
        }

        public override bool SupportsChangeEvents
        {
            get { return true; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(DesignProperty); }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override object GetValue(object component)
        {
            //TODO: Display instructional text to user when unconfigured.
            //i.e. !propertySettings.IsConfigured => "(Expand to modify)"

            // We know that we're always dealing with a concrete type implementation as it's not 
            // externally set-able and we always instantiate a PropertyBindingSettings.
            return new DesignProperty(this.schema.DefaultValue, this.propertyType, this.AttributeArray);
        }

        public override void SetValue(object component, object value)
        {
            //Ignore setting value at this level
            tracer.TraceVerbose(
                Resources.DefaultValueDescriptor_TraceSetValue, this.schema.Owner.Name, this.Name);
        }

        public override void ResetValue(object component)
        {
            // Reset both Value and ValueProvider
            tracer.TraceVerbose(
                Resources.DefaultValueDescriptor_TraceResetValue, this.schema.Owner.Name, this.Name);

            this.schema.DefaultValue.Reset();
        }
    }
}
