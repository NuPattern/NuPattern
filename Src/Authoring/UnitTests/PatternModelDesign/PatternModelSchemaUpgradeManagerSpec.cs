using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.UnitTests.PatternModelDesign
{
    [TestClass]
    public class PatternModelSchemaUpgradeManagerSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenSchemaContextWithNoProcessors
        {
            private PatternModelSchemaUpgradeManager manager;
            private Mock<ISchemaUpgradeContext> context;

            [TestInitialize]
            public void InitializeContext()
            {
                this.context = new Mock<ISchemaUpgradeContext>();
                context.Setup(ctx => ctx.UpgradeProcessors)
                    .Returns(Enumerable.Empty<Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>>());
                this.manager = new PatternModelSchemaUpgradeManager();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithNullContext_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => this.manager.Execute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoProcessors_ThenDoesNothing()
            {
                this.manager.Execute(this.context.Object);

                this.context.VerifyGet(ctx => ctx.UpgradeProcessors, Times.Exactly(2));
                this.context.Verify(ctx => ctx.OpenSchema(), Times.Never());
                this.context.Verify(ctx => ctx.BackupSchema(), Times.Never());
                this.context.Verify(ctx => ctx.SaveSchema(), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSchemaVersionIsRuntimeVersion_ThenDoesNothing()
            {
                this.context.Setup(ctx => ctx.SchemaVersion).Returns(new Version(1, 0, 0, 0));
                this.context.Setup(ctx => ctx.RuntimeVersion).Returns(new Version(1, 0, 0, 0));

                this.manager.Execute(this.context.Object);

                this.context.VerifyGet(ctx => ctx.UpgradeProcessors, Times.Exactly(2));
                this.context.Verify(ctx => ctx.OpenSchema(), Times.Never());
                this.context.Verify(ctx => ctx.BackupSchema(), Times.Never());
                this.context.Verify(ctx => ctx.SaveSchema(), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSchemaVersionIsLessThanRuntimeVersion_ThenDoesNothing()
            {
                this.context.Setup(ctx => ctx.SchemaVersion).Returns(new Version(1, 0, 0, 0));
                this.context.Setup(ctx => ctx.RuntimeVersion).Returns(new Version(2, 0, 0, 0));

                this.manager.Execute(this.context.Object);

                this.context.VerifyGet(ctx => ctx.UpgradeProcessors, Times.Exactly(2));
                this.context.Verify(ctx => ctx.OpenSchema(), Times.Never());
                this.context.Verify(ctx => ctx.BackupSchema(), Times.Never());
                this.context.Verify(ctx => ctx.SaveSchema(), Times.Never());
            }
        }

        [TestClass]
        public class GivenSchemaContextWithProcessors
        {
            private PatternModelSchemaUpgradeManager manager;
            private List<Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>> processors;
            private Mock<ISchemaUpgradeContext> context;
            private XDocument document = XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<patternModel dslVersion=""1.3.0.0"" xmlns=""http://schemas.microsoft.com/visualstudio/patterning/runtime/patternmodel""  xmlns:dm0=""http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core"">
  <dm0:extensions>
  </dm0:extensions>
</patternModel>
");

            [TestInitialize]
            public void InitializeContext()
            {
                this.processors = new List<Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>>();
                this.context = new Mock<ISchemaUpgradeContext>();
                context.Setup(ctx => ctx.UpgradeProcessors).Returns(this.processors);
                context.Setup(ctx => ctx.OpenSchema()).Returns(document);
                this.manager = new PatternModelSchemaUpgradeManager();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSchemaVersionIsRuntimeVersionAndProcessorNotFiltered_ThenDoesNothing()
            {
                this.processors.Add(new Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>(Mock.Of<ISchemaUpgradeProccesorOptions>(op => op.TargetVersion == "1.0.0.0")));
                this.context.Setup(ctx => ctx.SchemaVersion).Returns(new Version(1, 0, 0, 0));
                this.context.Setup(ctx => ctx.RuntimeVersion).Returns(new Version(1, 0, 0, 0));

                this.manager.Execute(this.context.Object);

                this.context.VerifyGet(ctx => ctx.UpgradeProcessors, Times.Exactly(3));
                this.context.Verify(ctx => ctx.OpenSchema(), Times.Never());
                this.context.Verify(ctx => ctx.BackupSchema(), Times.Never());
                this.context.Verify(ctx => ctx.SaveSchema(), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSchemaVersionIsLessThanRuntimeVersionAndProcessorNoFiltered_ThenDoesNothing()
            {
                this.processors.Add(new Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>(Mock.Of<ISchemaUpgradeProccesorOptions>()));
                this.context.Setup(ctx => ctx.SchemaVersion).Returns(new Version(1, 0, 0, 0));
                this.context.Setup(ctx => ctx.RuntimeVersion).Returns(new Version(2, 0, 0, 0));

                this.manager.Execute(this.context.Object);

                this.context.VerifyGet(ctx => ctx.UpgradeProcessors, Times.Exactly(3));
                this.context.Verify(ctx => ctx.OpenSchema(), Times.Never());
                this.context.Verify(ctx => ctx.BackupSchema(), Times.Never());
                this.context.Verify(ctx => ctx.SaveSchema(), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProcessorMatchesTargetVersion_ThenProcessesSchemaBacksUpAndSaves()
            {
                var proc1 = new Mock<IPatternModelSchemaUpgradeProcessor>();
                var proc2 = new Mock<IPatternModelSchemaUpgradeProcessor>();
                var procOptions1 = Mock.Of<ISchemaUpgradeProccesorOptions>(op => op.TargetVersion == "1.0.0.0");
                var procOptions2 = Mock.Of<ISchemaUpgradeProccesorOptions>(op => op.TargetVersion == "2.0.0.0");

                this.processors.Add(new Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>(() => proc1.Object, procOptions1));
                this.processors.Add(new Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>(() => proc2.Object, procOptions2));
                this.context.Setup(ctx => ctx.SchemaVersion).Returns(new Version(1, 0, 0, 0));
                this.context.Setup(ctx => ctx.RuntimeVersion).Returns(new Version(2, 0, 0, 0));

                this.manager.Execute(this.context.Object);

                proc1.Verify(proc => proc.ProcessSchema(this.document), Times.Once());
                proc2.Verify(proc => proc.ProcessSchema(this.document), Times.Never());
                this.context.Verify(ctx => ctx.OpenSchema(), Times.Once());
                this.context.Verify(ctx => ctx.BackupSchema(), Times.Once());
                this.context.Verify(ctx => ctx.SaveSchema(), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProcessorsMatchesTargetVersionAndOrdered_ThenProcessesSchemaInOrderAndBacksUpAndSaves()
            {
                var proc1 = new Mock<IPatternModelSchemaUpgradeProcessor>();
                var proc2 = new Mock<IPatternModelSchemaUpgradeProcessor>();
                var procOptions1 = Mock.Of<ISchemaUpgradeProccesorOptions>(op => op.TargetVersion == "1.0.0.0" && op.Order == 2);
                var procOptions2 = Mock.Of<ISchemaUpgradeProccesorOptions>(op => op.TargetVersion == "1.0.0.0" && op.Order == 1);

                this.processors.Add(new Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>(() => proc1.Object, procOptions1));
                this.processors.Add(new Lazy<IPatternModelSchemaUpgradeProcessor, ISchemaUpgradeProccesorOptions>(() => proc2.Object, procOptions2));
                this.context.Setup(ctx => ctx.SchemaVersion).Returns(new Version(1, 0, 0, 0));
                this.context.Setup(ctx => ctx.RuntimeVersion).Returns(new Version(2, 0, 0, 0));

                var callOrder = 0;
                proc2.Setup(proc => proc.ProcessSchema(It.IsAny<XDocument>())).Callback(() => Assert.Equal(callOrder++, 0));
                proc1.Setup(proc => proc.ProcessSchema(It.IsAny<XDocument>())).Callback(() => Assert.Equal(callOrder++, 1));

                this.manager.Execute(this.context.Object);

                proc1.Verify(proc => proc.ProcessSchema(this.document), Times.Once());
                proc2.Verify(proc => proc.ProcessSchema(this.document), Times.Once());
                this.context.Verify(ctx => ctx.OpenSchema(), Times.Once());
                this.context.Verify(ctx => ctx.BackupSchema(), Times.Once());
                this.context.Verify(ctx => ctx.SaveSchema(), Times.Once());
            }
        }
    }
}
