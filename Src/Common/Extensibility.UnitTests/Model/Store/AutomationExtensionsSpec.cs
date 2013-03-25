using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Extensibility.UnitTests.Model.Store
{
    [TestClass]
    public class AutomationExtensionsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenResolvingExtensionWithNullContainer_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                AutomationExtensionExtensions.ResolveAutomationReference<TestAutomation>(null, Guid.Empty));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenResolvingExtensionAndNoMatches_ThenReturnsNull()
        {
            var container = new Mock<IAutomationExtension>();
            container.Setup(c => c.Owner.AutomationExtensions).Returns(new List<IAutomationExtension>());
            container.Setup(c => c.Owner.Info.AutomationSettings).Returns(new List<IAutomationSettingsInfo>());

            var extension = AutomationExtensionExtensions.ResolveAutomationReference<TestAutomation>(container.Object, Guid.Empty);

            Assert.Null(extension);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenResolvingExtension_ThenReturnsMatchingByTypeAndIdAndName()
        {
            var extensions = new List<IAutomationExtension>
			{
				Mocks.Of<IAutomationExtension>().First(x => x.Name == "Foo"), 
				new TestAutomation { Name = "Foo" }, 
				new TestAutomation { Name = "Bar" },
			};

            var extensionGuid = Guid.NewGuid();

            var settings = new List<IAutomationSettingsInfo>
			{
				Mocks.Of<IAutomationSettingsInfo>().First(x => x.Id == Guid.Empty && x.Name == "Foo"), 
				Mocks.Of<IAutomationSettingsInfo>().First(x => x.Id == extensionGuid && x.Name == "Bar"), 
			};

            Mock.Get(settings[0]).Setup(i => i.As<IAutomationSettings>()).Returns(
                Mocks.Of<IAutomationSettings>().First(x => x.Id == Guid.Empty && x.Name == "Foo"));

            Mock.Get(settings[1]).Setup(i => i.As<IAutomationSettings>()).Returns(
                Mocks.Of<IAutomationSettings>().First(x => x.Id == extensionGuid && x.Name == "Bar"));

            var container = new Mock<IAutomationExtension>();
            container.Setup(c => c.Owner.AutomationExtensions).Returns(extensions);
            container.Setup(c => c.Owner.Info.AutomationSettings).Returns(settings);

            var extension = AutomationExtensionExtensions.ResolveAutomationReference<ITestAutomation>(container.Object, extensionGuid);

            Assert.NotNull(extension);
        }

        public interface ITestAutomation : IAutomationExtension
        {
        }

        public class TestAutomation : ITestAutomation
        {
            public string Name { get; set; }

            public void Execute()
            {
            }

            public void Execute(IDynamicBindingContext context)
            {
            }

            public IProductElement Owner { get; set; }
        }
    }
}
