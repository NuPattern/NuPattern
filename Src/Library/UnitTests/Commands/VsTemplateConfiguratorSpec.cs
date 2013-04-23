using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.UnitTests.Commands
{
    [DeploymentItem("Library.UnitTests.Content\\VsTemplateConfigurator-Project.vstemplate", ".")]
    [DeploymentItem("Library.UnitTests.Content\\VsTemplateConfigurator-Item.vstemplate", ".")]
    [TestClass]
    public class VsTemplateConfiguratorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private Mock<IServiceProvider> services;
        private Item templateItem;
        private VsTemplateConfigurator configurator;

        [TestInitialize]
        public virtual void Initialize()
        {
            this.services = new Mock<IServiceProvider>();
            this.configurator = new VsTemplateConfigurator(this.services.Object);

            this.templateItem = new Item
            {
                Parent = new Item
                {
                    Items = 
                    {
                        new Item
                        {
                            Name = "Foo.txt",
                        }
                    },
                },
            };
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenProjectTemplateConfigured_ThenSetsVsTemplateBuildAction()
        {
            this.templateItem.PhysicalPath = "VsTemplateConfigurator-Project.vstemplate";
            this.configurator.Configure(this.templateItem, "foo", "bar", "foo.bar.path");

            Assert.Equal("ProjectTemplate", (string)this.templateItem.Data.ItemType);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenProjectTemplateConfigured_ThenSetsContentItemProperties()
        {
            this.templateItem.PhysicalPath = "VsTemplateConfigurator-Project.vstemplate";
            this.configurator.Configure(this.templateItem, "foo", "bar", "foo.bar.path");

            var content = (IItem)this.templateItem.Parent.Items.First();

            Assert.Equal((int)CopyToOutput.PreserveNewest, (int)content.Data.CopyToOutputDirectory);
            Assert.Equal("Content", (string)content.Data.ItemType);
            Assert.Equal("false", (string)content.Data.IncludeInVSIX);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenItemTemplateConfigured_ThenSetsVsTemplateBuildAction()
        {
            this.templateItem.PhysicalPath = "VsTemplateConfigurator-Item.vstemplate";
            this.configurator.Configure(this.templateItem, "foo", "bar", "foo.bar.path");

            Assert.Equal("ItemTemplate", (string)this.templateItem.Data.ItemType);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenItemTemplateConfigured_ThenSetsContentItemProperties()
        {
            this.templateItem.PhysicalPath = "VsTemplateConfigurator-Item.vstemplate";
            this.configurator.Configure(this.templateItem, "foo", "bar", "foo.bar.path");

            var content = (IItem)this.templateItem.Parent.Items.First();

            Assert.Equal((int)CopyToOutput.PreserveNewest, (int)content.Data.CopyToOutputDirectory);
            Assert.Equal("Content", (string)content.Data.ItemType);
            Assert.Equal("false", (string)content.Data.IncludeInVSIX);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenItemTemplateConfigured_ThenDefaultNameIsSanitized()
        {
            this.templateItem.PhysicalPath = "VsTemplateConfigurator-Item.vstemplate";
            var template = this.configurator.Configure(this.templateItem, "A Single Sentence", "bar", "foo.bar.path");

            Assert.Equal("ASingleSentence", template.TemplateData.DefaultName);
        }
    }
}
