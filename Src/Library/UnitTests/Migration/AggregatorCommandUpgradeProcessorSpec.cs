using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility.Bindings;
using NuPattern.Library.Commands;
using NuPattern.Library.Design;
using NuPattern.Library.Migration;
using NuPattern.Reflection;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.UnitTests.Migration
{
    [TestClass]
    public class AggregatorCommandUpgradeProcessorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private AggregatorCommandUpgradeProcessor processor;

            [TestInitialize]
            public void InitializeContext()
            {
                this.processor = new AggregatorCommandUpgradeProcessor();
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
        public class GivenASchemaDocumentWithNoCommandSettings
        {
            private AggregatorCommandUpgradeProcessor processor;

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
                this.processor = new AggregatorCommandUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingNoProperties
        {
            private AggregatorCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.AggregatorCommand"">
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
                this.processor = new AggregatorCommandUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingEmptyProperties
        {
            private AggregatorCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.AggregatorCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties/>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new AggregatorCommandUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingsWrongProperty
        {
            private AggregatorCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.AggregatorCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties>[
  {
    ""Name"": ""Foo"",
    ""Value"""",
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
                this.processor = new AggregatorCommandUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingsEmptyPropertyValue
        {
            private AggregatorCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.AggregatorCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties>[
  {
    ""Name"": ""CommandReferenceList"",
    ""Value"": null,
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
                this.processor = new AggregatorCommandUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingsUpgradedPropertyValue
        {
            private AggregatorCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.AggregatorCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties>[
  {
    ""Name"": ""CommandReferenceList"",
    ""Value"": ""[\r\n  {\r\n    \""CommandId\"": \""e52d1228-39f9-422d-b667-1e6f26249aa0\""\r\n  },\r\n  {\r\n    \""CommandId\"": \""9434ad4c-395d-4fd9-bec4-a3d9fac0e111\""\r\n  },\r\n  {\r\n    \""CommandId\"": \""4a0bf7de-d568-4076-ab02-e62138bc3e41\""\r\n  }\r\n]"",
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
                this.processor = new AggregatorCommandUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingsSingleStringPropertyValue
        {
            private AggregatorCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.AggregatorCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties>[
  {
    ""Name"": ""CommandReferenceList"",
    ""Value"": ""e52d1228-39f9-422d-b667-1e6f26249aa0"",
    ""ValueProvider"": null
  }
]</properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";
            private static readonly IList<IPropertyBindingSettings> resultBindings = new[]
                {
                        new PropertyBindingSettings
                        {
                            Name = Reflector<AggregatorCommand>.GetPropertyName(x => x.CommandReferenceList) ,
                            Value = BindingSerializer.Serialize<IEnumerable<CommandReference>>(new []
                                {
                                    new CommandReference(null)
                                        {
                                            CommandId = Guid.Parse("e52d1228-39f9-422d-b667-1e6f26249aa0"),
                                        }, 
                                }),
                        }, 
                };
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.AggregatorCommand"">
      <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
      </extendedElement>
      <properties>" + BindingSerializer.Serialize<IEnumerable<IPropertyBindingSettings>>(resultBindings) + @"</properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new AggregatorCommandUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingsMultipleStringPropertyValue
        {
            private AggregatorCommandUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.AggregatorCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties>[
  {
    ""Name"": ""CommandReferenceList"",
    ""Value"": ""e52d1228-39f9-422d-b667-1e6f26249aa0;9434ad4c-395d-4fd9-bec4-a3d9fac0e111;4a0bf7de-d568-4076-ab02-e62138bc3e41"",
    ""ValueProvider"": null
  }
]</properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";
            private static readonly IList<IPropertyBindingSettings> resultBindings = new[]
                {
                        new PropertyBindingSettings
                        {
                            Name = Reflector<AggregatorCommand>.GetPropertyName(x => x.CommandReferenceList) ,
                            Value = BindingSerializer.Serialize<IEnumerable<CommandReference>>(new []
                                {
                                    new CommandReference(null)
                                        {
                                            CommandId = Guid.Parse("e52d1228-39f9-422d-b667-1e6f26249aa0"),
                                        }, 
                                    new CommandReference(null)
                                        {
                                            CommandId = Guid.Parse("9434ad4c-395d-4fd9-bec4-a3d9fac0e111"),
                                        }, 
                                    new CommandReference(null)
                                        {
                                            CommandId = Guid.Parse("4a0bf7de-d568-4076-ab02-e62138bc3e41"),
                                        }, 
                                }),
                        }, 
                };
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""NuPattern.Library.Commands.AggregatorCommand"">
      <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
      </extendedElement>
      <properties>" + BindingSerializer.Serialize<IEnumerable<IPropertyBindingSettings>>(resultBindings) + @"</properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new AggregatorCommandUpgradeProcessor();
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
