using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Library.SchemaUpgrade;

namespace NuPattern.Library.UnitTests.SchemaUpgrade
{
    [TestClass]
    public class GuidanceExtensionUpgradeProcessorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private GuidanceExtensionUpgradeProcessor processor;

            [TestInitialize]
            public void InitializeContext()
            {
                this.processor = new GuidanceExtensionUpgradeProcessor();
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
        public class GivenASchemaDocumentWithNoGuidanceExtension
        {
            private GuidanceExtensionUpgradeProcessor processor;

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
                this.processor = new GuidanceExtensionUpgradeProcessor();
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
        public class GivenASchemaDocumentWithGuidanceExtensionWithNoAttribute
        {
            private GuidanceExtensionUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <guidanceExtension Id=""26f05e22-b518-494a-9287-de75ed5499c5"">
      <extendedElement>
        <patternSchemaMoniker Id=""c034429e-01f9-48dd-a478-0321fb708dd3"" />
      </extendedElement>
    </guidanceExtension>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new GuidanceExtensionUpgradeProcessor();
                this.processor.ProcessSchema(this.document);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDocumentNotModifed()
            {
                Assert.False(this.processor.IsModified);
            }
        }

        [TestClass]
        public class GivenASchemaDocumentWithGuidanceExtensionWithEmptyAttribute
        {
            private GuidanceExtensionUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <guidanceExtension Id=""26f05e22-b518-494a-9287-de75ed5499c5"" guidanceInstanceName=""Creating Pattern Toolkits"" guidanceFeatureId="""" guidanceSharedInstance=""true"">
      <extendedElement>
        <patternSchemaMoniker Id=""c034429e-01f9-48dd-a478-0321fb708dd3"" />
      </extendedElement>
    </guidanceExtension>
  </dm0:extensions>
</patternModel>
";
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <guidanceExtension Id=""26f05e22-b518-494a-9287-de75ed5499c5"" guidanceInstanceName=""Creating Pattern Toolkits"" guidanceSharedInstance=""true"" extensionId="""">
      <extendedElement>
        <patternSchemaMoniker Id=""c034429e-01f9-48dd-a478-0321fb708dd3"" />
      </extendedElement>
    </guidanceExtension>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new GuidanceExtensionUpgradeProcessor();
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
        public class GivenASchemaDocumentWithGuidanceExtensionWithAttributeValue
        {
            private GuidanceExtensionUpgradeProcessor processor;

            private XDocument document;
            private const string SchemaXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <guidanceExtension Id=""26f05e22-b518-494a-9287-de75ed5499c5"" guidanceInstanceName=""Creating Pattern Toolkits"" guidanceFeatureId=""9f6dc301-6f66-4d21-9f9c-b37412b162f6"" guidanceSharedInstance=""true"">
      <extendedElement>
        <patternSchemaMoniker Id=""c034429e-01f9-48dd-a478-0321fb708dd3"" />
      </extendedElement>
    </guidanceExtension>
  </dm0:extensions>
</patternModel>
";
            private static readonly string resultXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel"">
  <dm0:extensions>
    <guidanceExtension Id=""26f05e22-b518-494a-9287-de75ed5499c5"" guidanceInstanceName=""Creating Pattern Toolkits"" guidanceSharedInstance=""true"" extensionId=""9f6dc301-6f66-4d21-9f9c-b37412b162f6"">
      <extendedElement>
        <patternSchemaMoniker Id=""c034429e-01f9-48dd-a478-0321fb708dd3"" />
      </extendedElement>
    </guidanceExtension>
  </dm0:extensions>
</patternModel>
";

            [TestInitialize]
            public void InitializeContext()
            {
                this.document = XDocument.Parse(SchemaXml);
                this.processor = new GuidanceExtensionUpgradeProcessor();
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
