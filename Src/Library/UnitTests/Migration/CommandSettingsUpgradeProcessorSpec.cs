using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility.Bindings;
using NuPattern.Library.Commands;
using NuPattern.Library.Migration;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.UnitTests.Migration
{
    [TestClass]
    public class CommandSettingsUpgradeProcessorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private CommandSettingsUpgradeProcessor processor;

            [TestInitialize]
            public void InitializeContext()
            {
                this.processor = new CommandSettingsUpgradeProcessor();
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
            private CommandSettingsUpgradeProcessor processor;

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
                this.processor = new CommandSettingsUpgradeProcessor();
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
            private CommandSettingsUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
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
                this.processor = new CommandSettingsUpgradeProcessor();
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
            private CommandSettingsUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
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
                this.processor = new CommandSettingsUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingsSingleProperty
        {
            private CommandSettingsUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties>
            <propertySettings Id=""70512bc5-7fd0-4803-9054-eaef09ce93dd"" value=""False"" name=""Open"" />
        </properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";
            private static readonly IList<IPropertyBindingSettings> resultBindings = new[]
                {
                    new PropertyBindingSettings
                        {
                            Name = "Open",
                            Value = "False"
                        }, 
                };
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
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
                this.processor = new CommandSettingsUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingsCollectionProperty
        {
            private CommandSettingsUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties>
            <propertySettings Id=""9aeeebd6-6e39-42c8-b8b4-e5e06acb1fb3"" value=""Update"" name=""Action"" />
            <propertySettings Id=""1145f6b0-def5-40e5-ba0d-16c56a80ffe2"" name=""NewValue"" />
            <propertySettings Id=""af26c223-1928-4a56-a6da-a007c8d02c74"" name=""SourcePath"" />
            <propertySettings Id=""cfceca70-2e82-45d1-8faf-c8b6802bc005"" value=""[&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo1&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar1&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo2&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar2&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo3&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar3&quot;&#xD;&#xA;  }&#xD;&#xA;]"" name=""Namespaces"" />
            <propertySettings Id=""2b6092e1-61ab-43a5-9318-c6e60163e6a3"" name=""XmlPath""/>
        </properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";
            private static readonly IList<IPropertyBindingSettings> resultBindings = new[]
                {
                    new PropertyBindingSettings
                        {
                            Name = "Action",
                            Value = "Update"
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "NewValue",
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "SourcePath",
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "Namespaces",
                            Value = BindingSerializer.Serialize<IEnumerable<XmlNamespace>>(new []
                                {
                                    new XmlNamespace
                                        {
                                            Prefix = "foo1",
                                            Namespace = "bar1",
                                        }, 
                                    new XmlNamespace
                                        {
                                            Prefix = "foo2",
                                            Namespace = "bar2",
                                        }, 
                                    new XmlNamespace
                                        {
                                            Prefix = "foo3",
                                            Namespace = "bar3",
                                        },
                                }),
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "XmlPath",
                        }, 
                };
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
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
                this.processor = new CommandSettingsUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingsNestedProperties
        {
            private CommandSettingsUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties>
            <propertySettings Id=""9aeeebd6-6e39-42c8-b8b4-e5e06acb1fb3"" value=""Update"" name=""Action"" />
            <propertySettings Id=""1145f6b0-def5-40e5-ba0d-16c56a80ffe2"" name=""NewValue"" />
            <propertySettings Id=""af26c223-1928-4a56-a6da-a007c8d02c74"" name=""SourcePath"" />
            <propertySettings Id=""cfceca70-2e82-45d1-8faf-c8b6802bc005"" value=""[&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo1&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar1&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo2&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar2&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo3&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar3&quot;&#xD;&#xA;  }&#xD;&#xA;]"" name=""Namespaces"" />
            <propertySettings Id=""2b6092e1-61ab-43a5-9318-c6e60163e6a3"" name=""XmlPath"">
                <valueProvider>
                <valueProviderSettings Id=""ac98a1fe-5317-4d9c-99df-921b9e2d1a92"" typeId=""FooProvider"">
                    <properties>
                    <propertySettingsMoniker Id=""b44f6cbd-335f-49f5-b485-4b587bd46afe"" />
                    <propertySettingsMoniker Id=""c670440b-1688-420e-83af-d350396541c2"" />
                    <propertySettingsMoniker Id=""2f7dccf2-2da9-47a7-9de6-108250f5ce88"" />
                    </properties>
                </valueProviderSettings>
                </valueProvider>
            </propertySettings>
            <propertySettings Id=""b44f6cbd-335f-49f5-b485-4b587bd46afe"" name=""XmlPath.FooProvider.SourcePath"" />
            <propertySettings Id=""c670440b-1688-420e-83af-d350396541c2"" value=""[&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo1&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar1&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo2&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar2&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo3&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar3&quot;&#xD;&#xA;  }&#xD;&#xA;]"" name=""XmlPath.FooProvider.Namespaces"" />
            <propertySettings Id=""2f7dccf2-2da9-47a7-9de6-108250f5ce88"" name=""XmlPath.FooProvider.XmlPath""/>
        </properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";
            private static readonly IList<IPropertyBindingSettings> resultBindings = new[]
                {
                    new PropertyBindingSettings
                        {
                            Name = "Action",
                            Value = "Update"
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "NewValue",
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "SourcePath",
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "Namespaces",
                            Value = BindingSerializer.Serialize<IEnumerable<XmlNamespace>>(new []
                                {
                                    new XmlNamespace
                                        {
                                            Prefix = "foo1",
                                            Namespace = "bar1",
                                        }, 
                                    new XmlNamespace
                                        {
                                            Prefix = "foo2",
                                            Namespace = "bar2",
                                        }, 
                                    new XmlNamespace
                                        {
                                            Prefix = "foo3",
                                            Namespace = "bar3",
                                        },
                                }),
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "XmlPath",
                            ValueProvider = new ValueProviderBindingSettings
                                {
                                    TypeId = "FooProvider",
                                    Properties = 
                                        {
                                            new PropertyBindingSettings
                                                {
                                                    Name = "SourcePath",
                                                }, 
                                            new PropertyBindingSettings
                                                {
                                                    Name = "Namespaces",
                                                    Value = BindingSerializer.Serialize<IEnumerable<XmlNamespace>>(new []
                                                        {
                                                            new XmlNamespace
                                                                {
                                                                    Prefix = "foo1",
                                                                    Namespace = "bar1",
                                                                }, 
                                                            new XmlNamespace
                                                                {
                                                                    Prefix = "foo2",
                                                                    Namespace = "bar2",
                                                                }, 
                                                            new XmlNamespace
                                                                {
                                                                    Prefix = "foo3",
                                                                    Namespace = "bar3",
                                                                },
                                                        }),
                                                }, 
                                            new PropertyBindingSettings
                                                {
                                                    Name = "XmlPath",
                                                }, 
                                        },
                                },
                        }, 
                };
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
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
                this.processor = new CommandSettingsUpgradeProcessor();
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
        public class GivenASchemaDocumentWithCommandSettingsMultiLevelNestedProperties
        {
            private CommandSettingsUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
        <extendedElement>
        <automationSettingsSchemaMoniker Id=""9c9210e1-118d-47cf-b44a-84368ac1a6e9"" />
        </extendedElement>
        <properties>
            <propertySettings Id=""9aeeebd6-6e39-42c8-b8b4-e5e06acb1fb3"" value=""Update"" name=""Action"" />
            <propertySettings Id=""1145f6b0-def5-40e5-ba0d-16c56a80ffe2"" name=""NewValue"" />
            <propertySettings Id=""af26c223-1928-4a56-a6da-a007c8d02c74"" value=""sp1"" name=""SourcePath"" />
            <propertySettings Id=""cfceca70-2e82-45d1-8faf-c8b6802bc005"" value=""[&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo1&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar1&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo2&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar2&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo3&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar3&quot;&#xD;&#xA;  }&#xD;&#xA;]"" name=""Namespaces"" />
            <propertySettings Id=""2b6092e1-61ab-43a5-9318-c6e60163e6a3"" name=""XmlPath"">
                <valueProvider>
                <valueProviderSettings Id=""ac98a1fe-5317-4d9c-99df-921b9e2d1a92"" typeId=""FooProvider"">
                    <properties>
                    <propertySettingsMoniker Id=""b44f6cbd-335f-49f5-b485-4b587bd46afe"" />
                    <propertySettingsMoniker Id=""c670440b-1688-420e-83af-d350396541c2"" />
                    <propertySettingsMoniker Id=""2f7dccf2-2da9-47a7-9de6-108250f5ce88"" />
                    </properties>
                </valueProviderSettings>
                </valueProvider>
            </propertySettings>
            <propertySettings Id=""b44f6cbd-335f-49f5-b485-4b587bd46afe"" value=""sp2"" name=""XmlPath.FooProvider.SourcePath"" />
            <propertySettings Id=""c670440b-1688-420e-83af-d350396541c2"" value=""[&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo1&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar1&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo2&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar2&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo3&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar3&quot;&#xD;&#xA;  }&#xD;&#xA;]"" name=""XmlPath.FooProvider.Namespaces"" />
            <propertySettings Id=""2f7dccf2-2da9-47a7-9de6-108250f5ce88"" name=""XmlPath.FooProvider.XmlPath"">
                <valueProvider>
                <valueProviderSettings Id=""479e6d9a-39db-4628-8860-117435606cf2"" typeId=""Foo2Provider"">
                    <properties>
                    <propertySettingsMoniker Id=""3f3a0fea-5b6b-4b0e-b78d-d72d5384ae05"" />
                    <propertySettingsMoniker Id=""e465fb0c-35da-4d6b-9681-5e91239af50b"" />
                    <propertySettingsMoniker Id=""e57ec1e6-8467-441c-be48-9289ecb36efb"" />
                    </properties>
                </valueProviderSettings>
                </valueProvider>
            </propertySettings>
            <propertySettings Id=""3f3a0fea-5b6b-4b0e-b78d-d72d5384ae05"" value=""sp3"" name=""XmlPath.FooProvider.XmlPath.FooProvider.SourcePath"" />
            <propertySettings Id=""e465fb0c-35da-4d6b-9681-5e91239af50b"" value=""[&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo1&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar1&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo2&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar2&quot;&#xD;&#xA;  },&#xD;&#xA;  {&#xD;&#xA;    &quot;Prefix&quot;: &quot;foo3&quot;,&#xD;&#xA;    &quot;Namespace&quot;: &quot;bar3&quot;&#xD;&#xA;  }&#xD;&#xA;]"" name=""XmlPath.FooProvider.XmlPath.FooProvider.Namespaces"" />
            <propertySettings Id=""e57ec1e6-8467-441c-be48-9289ecb36efb"" value=""xp3"" name=""XmlPath.FooProvider.XmlPath.FooProvider.XmlPath"" />
        </properties>
    </commandSettings>
  </dm0:extensions>
</patternModel>
";
            private static readonly IList<IPropertyBindingSettings> resultBindings = new[]
                {
                    new PropertyBindingSettings
                        {
                            Name = "Action",
                            Value = "Update"
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "NewValue",
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "SourcePath",
                            Value="sp1"
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "Namespaces",
                            Value = BindingSerializer.Serialize<IEnumerable<XmlNamespace>>(new []
                                {
                                    new XmlNamespace
                                        {
                                            Prefix = "foo1",
                                            Namespace = "bar1",
                                        }, 
                                    new XmlNamespace
                                        {
                                            Prefix = "foo2",
                                            Namespace = "bar2",
                                        }, 
                                    new XmlNamespace
                                        {
                                            Prefix = "foo3",
                                            Namespace = "bar3",
                                        },
                                }),
                        }, 
                    new PropertyBindingSettings
                        {
                            Name = "XmlPath",
                            ValueProvider = new ValueProviderBindingSettings
                                {
                                    TypeId = "FooProvider",
                                    Properties = 
                                        {
                                            new PropertyBindingSettings
                                                {
                                                    Name = "SourcePath",
                                                    Value="sp2"
                                                }, 
                                            new PropertyBindingSettings
                                                {
                                                    Name = "Namespaces",
                                                    Value = BindingSerializer.Serialize<IEnumerable<XmlNamespace>>(new []
                                                        {
                                                            new XmlNamespace
                                                                {
                                                                    Prefix = "foo1",
                                                                    Namespace = "bar1",
                                                                }, 
                                                            new XmlNamespace
                                                                {
                                                                    Prefix = "foo2",
                                                                    Namespace = "bar2",
                                                                }, 
                                                            new XmlNamespace
                                                                {
                                                                    Prefix = "foo3",
                                                                    Namespace = "bar3",
                                                                },
                                                        }),
                                                }, 
                                            new PropertyBindingSettings
                                                {
                                                    Name = "XmlPath",
                                                    ValueProvider = new ValueProviderBindingSettings
                                                        {
                                                            TypeId = "Foo2Provider",
                                                            Properties = 
                                                                {
                                                                    new PropertyBindingSettings
                                                                        {
                                                                            Name = "SourcePath",
                                                                            Value="sp3"
                                                                        }, 
                                                                    new PropertyBindingSettings
                                                                        {
                                                                            Name = "Namespaces",
                                                                            Value = BindingSerializer.Serialize<IEnumerable<XmlNamespace>>(new []
                                                                                {
                                                                                    new XmlNamespace
                                                                                        {
                                                                                            Prefix = "foo1",
                                                                                            Namespace = "bar1",
                                                                                        }, 
                                                                                    new XmlNamespace
                                                                                        {
                                                                                            Prefix = "foo2",
                                                                                            Namespace = "bar2",
                                                                                        }, 
                                                                                    new XmlNamespace
                                                                                        {
                                                                                            Prefix = "foo3",
                                                                                            Namespace = "bar3",
                                                                                        },
                                                                                }),
                                                                        }, 
                                                                    new PropertyBindingSettings
                                                                        {
                                                                            Name = "XmlPath",
                                                                            Value="xp3"
                                                                        }, 
                                                                },
                                                        },
                                                }, 
                                        },
                                },
                        }, 
                };
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <commandSettings Id=""df260ed4-e3e6-4e2c-a1c7-7ec0bbee334"" typeId=""FooCommand"">
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
                this.processor = new CommandSettingsUpgradeProcessor();
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
