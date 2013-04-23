using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;

namespace NuPattern.Presentation
{
    /// <summary>
    /// Represents an editor control that abstract the way to edit a property containing a <c>TypeConverter</c> or <c>TypeUIEditor</c>.
    /// </summary>
    [ContentProperty(@"Value")]
    [DefaultProperty("Value")]
    [TemplatePart(Name = SelectorPart, Type = typeof(ComboBox))]
    [TemplatePart(Name = EditorPickerPart, Type = typeof(ButtonBase))]
    [TemplatePart(Name = TextHostPart, Type = typeof(TextBox))]
    public class ValueEditor : Control
    {
        private const string SelectorPart = "PART_Selector";
        private const string EditorPickerPart = "PART_EditorPicker";
        private const string TextHostPart = "PART_TextHost";

        private static readonly DependencyPropertyKey EditorTypePropertyKey = DependencyProperty.RegisterReadOnly(
            "EditorType",
            typeof(EditorType),
            typeof(ValueEditor),
            new FrameworkPropertyMetadata(EditorType.Default, FrameworkPropertyMetadataOptions.AffectsRender));

        // \o/ I need to get the PropertyDescriptor of the type
        private static Func<BindingExpression, object> SourceItemGetter = CreateGetAccessor(
            typeof(BindingExpression).GetProperty("SourceItem", BindingFlags.NonPublic | BindingFlags.Instance));

        /// <summary>
        /// Identifies the <see cref="EditorType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EditorTypeProperty = EditorTypePropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(object),
            typeof(ValueEditor),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private ComboBox selector;
        private ButtonBase editorPicker;
        private TypeDescriptorContext context;
        private UITypeEditor typeEditor;
        private TextBox textHost;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ValueEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValueEditor), new FrameworkPropertyMetadata(typeof(ValueEditor)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEditor"/> class.
        /// </summary>
        public ValueEditor()
        {
            this.SetValue(ValueProperty, EditorType.Default);
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets the type of the editor.
        /// </summary>
        /// <value>The type of the editor.</value>
        [Bindable(true)]
        [Browsable(false)]
        public EditorType EditorType
        {
            get { return (EditorType)this.GetValue(EditorTypeProperty); }
            private set { this.SetValue(EditorTypePropertyKey, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ValueEditor"/> is value.
        /// </summary>
        /// <value><c>true</c> if value; otherwise, <c>false</c>.</value>
        [Bindable(true)]
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public object Value
        {
            get { return (object)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.editorPicker != null)
            {
                this.editorPicker.Click -= this.OnEditorPickerClick;
            }

            switch (this.EditorType)
            {
                case EditorType.TypeEditor:
                    this.editorPicker = (ButtonBase)this.GetTemplateChild(EditorPickerPart);
                    this.editorPicker.Click += this.OnEditorPickerClick;
                    this.textHost = (TextBox)this.GetTemplateChild(TextHostPart);
                    this.textHost.IsReadOnly = this.context.PropertyDescriptor.Converter == null || !this.context.PropertyDescriptor.Converter.CanConvertFrom(this.context, typeof(string));
                    break;

                case EditorType.StandardValues:
                    this.selector = (ComboBox)this.GetTemplateChild(SelectorPart);
                    this.selector.ItemsSource = this.context.PropertyDescriptor.Converter.GetStandardValues(this.context);
                    this.selector.IsEditable = !this.context.PropertyDescriptor.Converter.GetStandardValuesExclusive(this.context);
                    break;
            }
        }

        private static Func<BindingExpression, object> CreateGetAccessor(PropertyInfo propertyInfo)
        {
            var instance = System.Linq.Expressions.Expression.Parameter(propertyInfo.DeclaringType, @"i");
            var property = System.Linq.Expressions.Expression.Property(instance, propertyInfo);
            return (Func<BindingExpression, object>)System.Linq.Expressions.Expression.Lambda(property, instance).Compile();
        }

        private static IServiceProvider GetServiceProvider(DependencyObject reference)
        {
            while (reference != null)
            {
                var serviceProvider = reference as IServiceProvider;
                if (serviceProvider != null)
                {
                    return serviceProvider;
                }

                reference = LogicalTreeHelper.GetParent(reference);
            }

            return null;
        }

        private TypeDescriptorContext GetTypeDescriptorContext(DependencyProperty property)
        {
            var expression = BindingOperations.GetBindingExpression(this, property);
            if (expression != null)
            {
                var sourceItem = SourceItemGetter(expression);
                if (sourceItem != null)
                {
                    // TODO take in account indexers and traversing ("\" instead of ".")
                    var propertyName = expression.ParentBinding.Path.Path.Split('.').LastOrDefault();

                    if (propertyName != null)
                    {
                        var descriptor = TypeDescriptor.GetProperties(sourceItem)
                            .Cast<PropertyDescriptor>()
                            .FirstOrDefault(d => d.Name.Equals(propertyName, StringComparison.Ordinal));

                        return new TypeDescriptorContext(descriptor, sourceItem, GetServiceProvider(this));
                    }
                }
            }

            return null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SetupControl();
        }

        private void OnEditorPickerClick(object sender, RoutedEventArgs e)
        {
            var value = this.typeEditor.EditValue(this.context, this.context.ServiceProvider, this.Value);
            if (this.context.PropertyDescriptor.Converter.CanConvertTo(this.context, this.context.PropertyDescriptor.PropertyType))
            {
                this.Value = this.context.PropertyDescriptor.Converter.ConvertTo(
                    this.context,
                    CultureInfo.CurrentCulture,
                    value,
                    this.context.PropertyDescriptor.PropertyType);
            }
            else
            {
                this.Value = value;
            }
        }

        private void SetupControl()
        {
            var editorType = EditorType.Default;

            this.context = GetTypeDescriptorContext(ValueProperty);
            if (this.context != null)
            {
                this.typeEditor = (UITypeEditor)this.context.PropertyDescriptor.GetEditor(typeof(UITypeEditor));
                if (this.typeEditor != null && this.typeEditor.GetEditStyle(null) != UITypeEditorEditStyle.DropDown)
                {
                    editorType = EditorType.TypeEditor;
                }
                else if (this.context.PropertyDescriptor.Converter.GetStandardValuesSupported())
                {
                    editorType = EditorType.StandardValues;
                }
            }

            this.SetValue(EditorTypePropertyKey, editorType);
        }

        private class TypeDescriptorContext : ITypeDescriptorContext
        {
            internal TypeDescriptorContext(PropertyDescriptor descriptor, object instance, IServiceProvider serviceProvider)
            {
                this.Instance = instance;
                this.PropertyDescriptor = descriptor;
                this.ServiceProvider = serviceProvider;
            }

            public IContainer Container
            {
                get { return null; }
            }

            public object Instance { get; private set; }

            public PropertyDescriptor PropertyDescriptor { get; private set; }

            public IServiceProvider ServiceProvider { get; private set; }

            public void OnComponentChanged()
            {
            }

            public bool OnComponentChanging()
            {
                return true;
            }

            public object GetService(Type serviceType)
            {
                if (this.ServiceProvider != null)
                {
                    return this.ServiceProvider.GetService(serviceType);
                }

                return null;
            }
        }
    }
}