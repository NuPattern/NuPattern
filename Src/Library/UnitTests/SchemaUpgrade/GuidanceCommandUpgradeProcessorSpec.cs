using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Library.SchemaUpgrade;

namespace NuPattern.Library.UnitTests.SchemaUpgrade
{
    [TestClass]
    public class GuidanceCommandUpgradeProcessorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private GuidanceCommandUpgradeProcessor processor;

            [TestInitialize]
            public void InitializeContext()
            {
                this.processor = new GuidanceCommandUpgradeProcessor();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsModifiedIsFalse()
            {
                Assert.False(this.processor.IsModified);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenProcessSchemaWithNullDocumentThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                                                     this.processor.ProcessSchema(null));
            }
        }

        [TestClass]
        public class GivenASchemaDocumentWithNoGuidanceCommand
        {
            private GuidanceCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new GuidanceCommandUpgradeProcessor();
                this.processor.ProcessSchema(this.document);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDocumentNotChanged()
            {
                Assert.False(this.processor.IsModified);
                Assert.Equal(this.document.ToString(), XDocument.Parse(SchemaXml).ToString());
            }
        }

        [TestClass]
        public class GivenASchemaDocumentWithCommandSettingForActivateCommand
        {
            private GuidanceCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""55555555-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.ActivateFeatureCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""55555555-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.ActivateGuidanceWorkflowCommand"">
      <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
      </extendedElement>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new GuidanceCommandUpgradeProcessor();
                this.processor.ProcessSchema(this.document);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDocumentModifed()
            {
                Assert.True(this.processor.IsModified);
                Assert.Equal(XDocument.Parse(resultXml).ToString(), this.document.ToString());
            }
        }

        [TestClass]
        public class GivenASchemaDocumentWithCommandSettingForInstantiateCommand
        {
            private GuidanceCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""88888888-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.InstantiateFeatureCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
      <properties>[
  {
    ""Name"": ""FeatureId"",
    ""Value"": ""9f6dc301-6f66-4d21-9f9c-b37412b162f6"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""DefaultInstanceName"",
    ""Value"": ""Creating Pattern Toolkits"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""SharedInstance"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""ActivateOnInstantiation"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  }
]</properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""88888888-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.InstantiateGuidanceWorkflowCommand"">
      <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
      </extendedElement>
      <properties>[
  {
    ""Name"": ""ExtensionId"",
    ""Value"": ""9f6dc301-6f66-4d21-9f9c-b37412b162f6"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""DefaultInstanceName"",
    ""Value"": ""Creating Pattern Toolkits"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""SharedInstance"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""ActivateOnInstantiation"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  }
]</properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new GuidanceCommandUpgradeProcessor();
                this.processor.ProcessSchema(this.document);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDocumentModifed()
            {
                Assert.True(this.processor.IsModified);
                Assert.Equal(XDocument.Parse(resultXml).ToString(), this.document.ToString());
            }
        }

        [TestClass]
        public class GivenASchemaDocumentWithCommandSettingForAllCommands
        {
            private GuidanceCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""55555555-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.ActivateFeatureCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
    </commandSettings>
    <commandSettings Id=""88888888-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.InstantiateFeatureCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
      <properties>[
  {
    ""Name"": ""FeatureId"",
    ""Value"": ""9f6dc301-6f66-4d21-9f9c-b37412b162f6"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""DefaultInstanceName"",
    ""Value"": ""Creating Pattern Toolkits"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""SharedInstance"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""ActivateOnInstantiation"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  }
]</properties>
    </commandSettings>
    <commandSettings Id=""77777777-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.ActivateOrInstantiateSharedFeatureCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
      <properties>[
  {
    ""Name"": ""FeatureId"",
    ""Value"": ""9f6dc301-6f66-4d21-9f9c-b37412b162f6"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""DefaultInstanceName"",
    ""Value"": ""Creating Pattern Toolkits"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""SharedInstance"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""ActivateOnInstantiation"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  }
]</properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""55555555-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.ActivateGuidanceWorkflowCommand"">
      <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
      </extendedElement>
    </commandSettings>
    <commandSettings Id=""88888888-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.InstantiateGuidanceWorkflowCommand"">
      <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
      </extendedElement>
      <properties>[
  {
    ""Name"": ""ExtensionId"",
    ""Value"": ""9f6dc301-6f66-4d21-9f9c-b37412b162f6"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""DefaultInstanceName"",
    ""Value"": ""Creating Pattern Toolkits"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""SharedInstance"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""ActivateOnInstantiation"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  }
]</properties>
    </commandSettings>
    <commandSettings Id=""77777777-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.ActivateOrInstantiateSharedGuidanceWorkflowCommand"">
      <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
      </extendedElement>
      <properties>[
  {
    ""Name"": ""ExtensionId"",
    ""Value"": ""9f6dc301-6f66-4d21-9f9c-b37412b162f6"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""DefaultInstanceName"",
    ""Value"": ""Creating Pattern Toolkits"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""SharedInstance"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  },
  {
    ""Name"": ""ActivateOnInstantiation"",
    ""Value"": ""True"",
    ""ValueProvider"": null
  }
]</properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new GuidanceCommandUpgradeProcessor();
                this.processor.ProcessSchema(this.document);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDocumentModifed()
            {
                Assert.True(this.processor.IsModified);
                Assert.Equal(XDocument.Parse(resultXml).ToString(), this.document.ToString());
            }
        }
    }
}
