using System;
using System.ComponentModel;
using System.Linq;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Custom <see cref="PropertyDescriptor"/> that delegates calls to the inner descriptor, buts allows overriding of its attributes.
    /// </summary>
    public class DelegatingPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor innerDescriptor;
        private object owningComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingPropertyDescriptor"/> class with optional 
        /// custom attributes to modify the property behavior.
        /// </summary>
        /// <param name="innerDescriptor">The inner descriptor.</param>
        /// <param name="overriddenAttributes">The optional custom attributes.</param>
        public DelegatingPropertyDescriptor(PropertyDescriptor innerDescriptor, params Attribute[] overriddenAttributes)
            : this(null, innerDescriptor, overriddenAttributes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingPropertyDescriptor"/> class with optional 
        /// custom attributes to modify the property behavior.
        /// </summary>
        /// <param name="owningComponent">The owner component of the descriptor.</param>
        /// <param name="innerDescriptor">The inner descriptor.</param>
        /// <param name="overriddenAttributes">The optional custom attributes.</param>
        public DelegatingPropertyDescriptor(object owningComponent, PropertyDescriptor innerDescriptor, params Attribute[] overriddenAttributes)
            : base(innerDescriptor, overriddenAttributes)
        {
            Guard.NotNull(() => innerDescriptor, innerDescriptor);

            this.innerDescriptor = innerDescriptor;
            this.owningComponent = owningComponent;
        }

        /// <summary>
        /// Gets the wrapped descriptor.
        /// </summary>
        protected PropertyDescriptor InnerDescriptor
        {
            get { return this.innerDescriptor; }
        }

        /// <summary>
        /// When overidden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.</returns>
        public override Type ComponentType
        {
            get { return this.innerDescriptor.ComponentType; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the property is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get
            {
                var attribute = this.Attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
                return attribute != null ? attribute.IsReadOnly : this.innerDescriptor.IsReadOnly;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is browsable.
        /// </summary>
        /// <value></value>
        /// <returns>true if the member is browsable; otherwise, false.</returns>
        public override bool IsBrowsable
        {
            get
            {
                var attribute = this.Attributes.OfType<BrowsableAttribute>().FirstOrDefault();
                return attribute != null ? attribute.Browsable : this.innerDescriptor.IsBrowsable;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the description of this property.
        /// </summary>
        public override string Description
        {
            get { return this.Attributes.OfType<DescriptionAttribute>().Select(x => x.Description).FirstOrDefault() ?? this.innerDescriptor.Description; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the display name of this property.
        /// </summary>
        public override string DisplayName
        {
            get { return this.Attributes.OfType<DisplayNameAttribute>().Select(x => x.DisplayName).FirstOrDefault() ?? this.innerDescriptor.DisplayName; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the display name of this property.
        /// </summary>
        public override string Category
        {
            get { return this.Attributes.OfType<CategoryAttribute>().Select(x => x.Category).FirstOrDefault() ?? this.innerDescriptor.Category; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the property.</returns>
        public override Type PropertyType
        {
            get { return this.innerDescriptor.PropertyType; }
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns>Returns <see langword="true"/> if resetting the component changes its value; <see langword="false"/> otherwise.</returns>
        public override bool CanResetValue(object component)
        {
            Guard.NotNull(() => component, component);

            var owner = this.GetInvocationTarget(component.GetType(), component);
            return this.innerDescriptor.CanResetValue(owner);
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            Guard.NotNull(() => component, component);

            var owner = this.GetInvocationTarget(component.GetType(), component);
            return this.innerDescriptor.GetValue(owner);
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        public override void ResetValue(object component)
        {
            Guard.NotNull(() => component, component);

            var owner = this.GetInvocationTarget(component.GetType(), component);
            this.innerDescriptor.ResetValue(owner);
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            Guard.NotNull(() => component, component);

            var owner = this.GetInvocationTarget(component.GetType(), component);
            this.innerDescriptor.SetValue(owner, value);
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns>Returns <see langword="true"/> if the property should be persisted; <see langword="false"/> otherwise.</returns>
        public override bool ShouldSerializeValue(object component)
        {
            Guard.NotNull(() => component, component);

            var owner = this.GetInvocationTarget(component.GetType(), component);
            return this.innerDescriptor.ShouldSerializeValue(owner);
        }

        /// <summary>
        /// When overridden in a derived class, return the editor.
        /// </summary>
        /// <param name="editorBaseType"></param>
        public override object GetEditor(Type editorBaseType)
        {
            var editorAttribute = this.Attributes.OfType<EditorAttribute>().FirstOrDefault();
            if (editorAttribute != null)
            {
                return base.GetEditor(editorBaseType);
            }

            return this.innerDescriptor.GetEditor(editorBaseType);
        }

        /// <summary>
        /// When overridden in a derived class, gets the owner of the property.
        /// </summary>
        protected override object GetInvocationTarget(Type type, object instance)
        {
            return this.owningComponent != null ? this.owningComponent : base.GetInvocationTarget(type, instance);
        }
    }
}