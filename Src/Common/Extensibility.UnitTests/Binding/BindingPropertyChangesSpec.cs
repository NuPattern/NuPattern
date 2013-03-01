using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility.Binding;

namespace NuPattern.Extensibility.UnitTests.Binding
{
    [TestClass]
    public class BindingPropertyChangesSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenTypeIdChanged_ThenBindingRaisesChanged()
        {
            var binding = new BindingSettings();
            var raised = false;
            binding.PropertyChanged += (sender, args) => raised = true;

            binding.TypeId = "foo";

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyAdded_ThenBindingRaisesChanged()
        {
            var binding = new BindingSettings();
            var raised = false;
            binding.PropertyChanged += (sender, args) => raised = true;

            binding.AddProperty("", typeof(string));

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyNameChanged_ThenBindingRaisesChanged()
        {
            var binding = new BindingSettings();
            var raised = false;
            var property = binding.AddProperty("", typeof(string));

            binding.Properties.Add(property);

            binding.PropertyChanged += (sender, args) => raised = true;

            property.Name = "hello";

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyValueChanged_ThenBindingRaisesChanged()
        {
            var binding = new BindingSettings();
            var raised = false;
            var property = binding.AddProperty("", typeof(string));

            binding.PropertyChanged += (sender, args) => raised = true;

            property.Value = "hello";

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyValueProviderSet_ThenBindingRaisesChanged()
        {
            var binding = new BindingSettings();
            var raised = false;
            var property = binding.AddProperty("", typeof(string));

            binding.PropertyChanged += (sender, args) => raised = true;

            property.ValueProvider = new ValueProviderBindingSettings();

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenValueProviderTypeIdChanged_ThenBindingRaisesChanged()
        {
            var raised = false;
            var provider = new ValueProviderBindingSettings();
            var binding = new BindingSettings();
            var propBinding = binding.AddProperty("", typeof(string));
            propBinding.ValueProvider = provider;

            binding.PropertyChanged += (sender, args) => raised = true;

            provider.TypeId = "foo";

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenValueProviderPropertyAdded_ThenBindingRaisesChanged()
        {
            var raised = false;
            var provider = new ValueProviderBindingSettings();
            var binding = new BindingSettings();
            var propBinding = binding.AddProperty("", typeof(string));
            propBinding.ValueProvider = provider;

            binding.PropertyChanged += (sender, args) => raised = true;

            provider.AddProperty("", typeof(string));

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenValueProviderPropertyChanged_ThenBindingRaisesChanged()
        {
            var raised = false;
            var binding = new BindingSettings();
            var propBinding = binding.AddProperty("", typeof(string));
            propBinding.ValueProvider = new ValueProviderBindingSettings();
            var property = propBinding.ValueProvider.AddProperty("", typeof(string));

            binding.PropertyChanged += (sender, args) => raised = true;

            property.Value = "foo";

            Assert.True(raised);
        }
    }
}
