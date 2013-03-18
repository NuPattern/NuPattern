using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.IntegrationTests.PatternModelDesign
{
    [TestClass]
    public class SchemaUpgradeContextSpec
    {
        private const string DefaultNamespace = SchemaConstants.DefaultNamespace;
        private static readonly XName PatternModelRootElementName = XName.Get("patternModel", DefaultNamespace);
        private static readonly XName PatternModelDiagramRootElementName = XName.Get("patternModelDiagram", "");
        private const string PatternModelVersionAttributeName = "dslVersion";
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [DeploymentItem(@"Authoring.IntegrationTests.Content\SchemaUpgradeContext", @"SchemaUpgradeContext")]
        public class GivenOlderVersionSchema : IntegrationTest
        {
            private SchemaUpgradeContext context;
            private string schemaFilePath;
            private DateTime createdTime;
            private List<string> diagrams;

            [TestInitialize]
            public void InitializeContext()
            {
                this.schemaFilePath = PathTo("SchemaUpgradeContext\\OlderVersion.patterndefinition");
                this.diagrams = new List<string>
                    {
                        Path.Combine(Path.GetDirectoryName(this.schemaFilePath), "OlderVersion.View1.patterndefinition.diagram"),
                        Path.Combine(Path.GetDirectoryName(this.schemaFilePath), "OlderVersion.View2.patterndefinition.diagram"),
                    };
                this.createdTime = new FileInfo(this.schemaFilePath).LastWriteTime;
                this.context = new SchemaUpgradeContext(schemaFilePath, this.diagrams);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenSchemaFilePathReturnsPath()
            {
                var result = this.context.SchemaFilePath;

                Assert.Equal(this.schemaFilePath, result);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenSchemaVersionReturnsOlderVersion()
            {
                var result = this.context.SchemaVersion;

                Assert.Equal("1.0.0.0", result.ToString(4));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenRuntimeVersionReturnsCurrentVersion()
            {
                var result = this.context.RuntimeVersion;

                Assert.Equal(SchemaConstants.SchemaVersion, result.ToString(4));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenOpenSchemaReturnsXDocument()
            {
                var result = this.context.OpenSchema();

                Assert.Equal("1.0.0.0", result.Descendants(PatternModelRootElementName).FirstOrDefault().Attribute(PatternModelVersionAttributeName).Value);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenUpdateSchemaVersionAndSave_ThenSchemaSaved()
            {
                var result = this.context.OpenSchema();
                this.context.SchemaVersion = new Version(9, 9, 9, 9);
                System.Threading.Thread.Sleep(2000); // Enforced delay
                this.context.SaveSchema();

                Assert.Equal("9.9.9.9", result.Descendants(PatternModelRootElementName).FirstOrDefault().Attribute(PatternModelVersionAttributeName).Value);
                // File updated
                Assert.True(new FileInfo(this.schemaFilePath).LastWriteTime > this.createdTime);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenUpdateSchemaVersionToCurrentVersionAndSave_ThenSchemaSaved()
            {
                var result = this.context.OpenSchema();
                this.context.SchemaVersion = this.context.RuntimeVersion;
                System.Threading.Thread.Sleep(2000); // Enforced delay
                this.context.SaveSchema();

                Assert.Equal(this.context.RuntimeVersion.ToString(4), result.Descendants(PatternModelRootElementName).FirstOrDefault().Attribute(PatternModelVersionAttributeName).Value);
                // File updated
                Assert.True(new FileInfo(this.schemaFilePath).LastWriteTime > this.createdTime);

                // Ensure diagrams are updated and saved
                var diagram1 = XDocument.Load(this.diagrams[0]);
                Assert.Equal(this.context.RuntimeVersion.ToString(4), diagram1.Descendants(PatternModelDiagramRootElementName).FirstOrDefault().Attribute(PatternModelVersionAttributeName).Value);
                var diagram2 = XDocument.Load(this.diagrams[1]);
                Assert.Equal(this.context.RuntimeVersion.ToString(4), diagram2.Descendants(PatternModelDiagramRootElementName).FirstOrDefault().Attribute(PatternModelVersionAttributeName).Value);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenBackupSchema_ThenSchemaBackedUp()
            {
                this.context.BackupSchema();

                var result = Path.Combine(Path.GetDirectoryName(this.schemaFilePath),
                    "OlderVersion.patterndefinition.1.0.0.0.backup");

                Assert.True(File.Exists(result));

                // Assert content is unchanged
                var doc1 = new XmlDocument();
                doc1.Load(result);
                var doc2 = new XmlDocument();
                doc2.Load(this.schemaFilePath);

                Assert.Equal(doc1.InnerXml, doc2.InnerXml);
            }
        }

        [TestClass]
        [DeploymentItem(@"Authoring.IntegrationTests.Content\SchemaUpgradeContext", @"SchemaUpgradeContext")]
        public class GivenCurrentVersionSchema : IntegrationTest
        {
            private SchemaUpgradeContext context;
            private string schemaFilePath;
            private DateTime createdTime;

            [TestInitialize]
            public void InitializeContext()
            {
                this.schemaFilePath = PathTo("SchemaUpgradeContext\\CurrentVersion.gen.patterndefinition");
                this.createdTime = new FileInfo(this.schemaFilePath).LastWriteTime;
                this.context = new SchemaUpgradeContext(schemaFilePath, null);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenSchemaVersionReturnsCurrentVersion()
            {
                var result = this.context.SchemaVersion;

                Assert.Equal(SchemaConstants.SchemaVersion, result.ToString(4));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void ThenOpenSchemaReturnsXDocument()
            {
                var result = this.context.OpenSchema();

                Assert.Equal(SchemaConstants.SchemaVersion, result.Descendants(PatternModelRootElementName).FirstOrDefault().Attribute(PatternModelVersionAttributeName).Value);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenUpdateSchemaVersionToCurrentVersionAndSave_ThenSchemaNotSaved()
            {
                var result = this.context.OpenSchema();
                this.context.SchemaVersion = this.context.RuntimeVersion;
                System.Threading.Thread.Sleep(2000); // Enforced delay
                this.context.SaveSchema();

                Assert.Equal(this.context.RuntimeVersion.ToString(4), result.Descendants(PatternModelRootElementName).FirstOrDefault().Attribute(PatternModelVersionAttributeName).Value);
                // Not saved
                Assert.Equal(new FileInfo(this.schemaFilePath).LastWriteTime, this.createdTime);
            }
        }
    }
}
