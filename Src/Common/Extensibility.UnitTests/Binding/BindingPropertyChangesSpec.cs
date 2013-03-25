using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility.Bindings;

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

            binding.Properties.Add(new PropertyBindingSettings());

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyNameChanged_ThenBindingRaisesChanged()
        {
            var binding = new BindingSettings();
            var property = new PropertyBindingSettings();
            var raised = false;
            binding.Properties.Add(property);

            binding.PropertyChanged += (sender, args) => raised = true;

            property.Name = "hello";

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyValueChanged_ThenBindingRaisesChanged()
        {
            var binding = new BindingSettings();
            var property = new PropertyBindingSettings();
            var raised = false;
            binding.Properties.Add(property);

            binding.PropertyChanged += (sender, args) => raised = true;

            property.Value = "hello";

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyValueProviderSet_ThenBindingRaisesChanged()
        {
            var binding = new BindingSettings();
            var property = new PropertyBindingSettings();
            var raised = false;
            binding.Properties.Add(property);

            binding.PropertyChanged += (sender, args) => raised = true;

            property.ValueProvider = new ValueProviderBindingSettings();

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenValueProviderTypeIdChanged_ThenBindingRaisesChanged()
        {
            var raised = false;
            var provider = new ValueProviderBindingSettings();
            var binding = new BindingSettings
            {
                Properties = 
                {
                    new PropertyBindingSettings
                    {
                        ValueProvider = provider,
                    }
                }
            };

            binding.PropertyChanged += (sender, args) => raised = true;

            provider.TypeId = "foo";

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenValueProviderPropertyAdded_ThenBindingRaisesChanged()
        {
            var raised = false;
            var provider = new ValueProviderBindingSettings();
            var binding = new BindingSettings
            {
                Properties = 
                {
                    new PropertyBindingSettings
                    {
                        ValueProvider = provider,
                    }
                }
            };

            binding.PropertyChanged += (sender, args) => raised = true;

            provider.Properties.Add(new PropertyBindingSettings());

            Assert.True(raised);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenValueProviderPropertyChanged_ThenBindingRaisesChanged()
        {
            var raised = false;
            var property = new PropertyBindingSettings();
            var binding = new BindingSettings
            {
                Properties = 
                {
                    new PropertyBindingSettings
                    {
                        ValueProvider = new ValueProviderBindingSettings
                        {
                            Properties = 
                            {
                                property,
                            }
                        }
                    }
                }
            };

            binding.PropertyChanged += (sender, args) => raised = true;

            property.Value = "foo";

            Assert.True(raised);
        }
    }
}