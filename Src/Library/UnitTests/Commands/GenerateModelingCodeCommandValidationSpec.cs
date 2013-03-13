using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Extensibility;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Binding;
using NuPattern.Library.Automation;
using NuPattern.Library.Commands;
using NuPattern.Library.Properties;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.UnitTests.Commands
{
    [TestClass]
    public class GenerateModelingCodeCommandValidationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAConfiguredTextTemplateOnAnElement
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));
            private GenerateModelingCodeCommandValidation validation;
            private Mock<IServiceProvider> serviceProvider;
            private Mock<IFxrUriReferenceService> uriService;
            private ValidationContext validationContext;
            private CommandSettings settings;
            private AutomationSettingsSchema element;
            private ISolution solution = new Solution
                {
                    Name = "Solution.sln",
                    PhysicalPath = "C:\\Temp",
                    Items = 
                    {
                        new Project
                        {
                            Name = "Project",
                            Items = 
                            {
                                new Item 
                                { 
                                    Name = "TextTemplate.t4",
                                }
                            }
                        },
                    }
                };
            private IItem textTemplateFile;

            [TestInitialize]
            public void InitializeContext()
            {
                this.textTemplateFile = this.solution.Find<IItem>().First();

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = patternModel.Create<PatternSchema>();
                    var view = pattern.CreateViewSchema();
                    var parent = view.CreateElementSchema();
                    this.element = parent.CreateAutomationSettingsSchema() as AutomationSettingsSchema;
                    this.settings = element.AddExtension<CommandSettings>();

                    this.settings.TypeId = typeof(GenerateModelingCodeCommand).Name;
                    ((ICommandSettings)this.settings).Properties.Add(new PropertyBindingSettings { Name = Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName) });
                    ((ICommandSettings)this.settings).Properties.Add(new PropertyBindingSettings { Name = Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath) });
                    ((ICommandSettings)this.settings).Properties.Add(new PropertyBindingSettings { Name = Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri) });
                    ((ICommandSettings)this.settings).Properties.Add(new PropertyBindingSettings { Name = Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateUri) });

                });

                this.serviceProvider = new Mock<IServiceProvider>();
                this.uriService = new Mock<IFxrUriReferenceService>();
                this.serviceProvider.Setup(sp => sp.GetService(typeof(IFxrUriReferenceService))).Returns(this.uriService.Object);
                this.validation = new GenerateModelingCodeCommandValidation
                {
                    serviceProvider = this.serviceProvider.Object,
                };

                this.validationContext = new ValidationContext(ValidationCategories.Custom, this.settings);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTextTemplateNotDefined_ThenValidationFails()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), string.Empty);

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 1);
                Assert.Equal(Resources.Validate_GenerateModelingCodeAuthoringUriIsInvalidCode, this.validationContext.CurrentViolations.First().Code);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTextTemplateNotExistInSolution_ThenValidationFails()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), "solution://projectguid/fileguid");
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Throws<UriFormatException>();

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 1);
                Assert.Equal(Resources.Validate_GenerateModelingCodeAuthoringUriIsInvalidCode, this.validationContext.CurrentViolations.First().Code);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTextTemplateExistsButNotContent_ThenValidationFails()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), "solution://projectguid/fileguid");

                this.textTemplateFile.Data.ItemType = "NotContent";
                this.textTemplateFile.Data.IncludeInVSIX = "false";
                this.textTemplateFile.Data.IncludeInVSIXAs = string.Empty;
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(this.textTemplateFile);

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 2);
                Assert.Equal(Resources.Validate_GenerateModelingCodeCommandItemTypeShouldBeSetToContentCode, this.validationContext.CurrentViolations[0].Code);
                Assert.Equal(Resources.Validate_GenerateModelingCodeCommandIIncludeInVSIXShouldBeSetToTrueCode, this.validationContext.CurrentViolations[1].Code);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTextTemplateExistsButNotIncludeInVSIX_ThenValidationFails()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), "solution://projectguid/fileguid");

                this.textTemplateFile.Data.ItemType = "Content";
                this.textTemplateFile.Data.IncludeInVSIX = "false";
                this.textTemplateFile.Data.IncludeInVSIXAs = string.Empty;
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(this.textTemplateFile);

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 1);
                Assert.Equal(Resources.Validate_GenerateModelingCodeCommandIIncludeInVSIXShouldBeSetToTrueCode, this.validationContext.CurrentViolations.First().Code);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTextTemplateExistsAndIncludeInVSIX_ThenValidationSucceeds()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), "solution://projectguid/fileguid");

                this.textTemplateFile.Data.ItemType = "Content";
                this.textTemplateFile.Data.IncludeInVSIX = "true";
                this.textTemplateFile.Data.IncludeInVSIXAs = string.Empty;
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(this.textTemplateFile);

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTextTemplateExistsAndIncludeInVSIXAs_ThenValidationSucceeds()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), "solution://projectguid/fileguid");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateUri), "t4://extension/vsixguid/foo");

                this.textTemplateFile.Data.ItemType = "Content";
                this.textTemplateFile.Data.IncludeInVSIX = "false";
                this.textTemplateFile.Data.IncludeInVSIXAs = "foo";
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(this.textTemplateFile);

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTextTemplateExistsAndIncludeInVSIXAsWrongFilename_ThenValidationFails()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), "solution://projectguid/fileguid");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateUri), "t4://extension/vsixguid/foo");

                this.textTemplateFile.Data.ItemType = "Content";
                this.textTemplateFile.Data.IncludeInVSIX = "false";
                this.textTemplateFile.Data.IncludeInVSIXAs = "foo2";
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(this.textTemplateFile);

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 1);
                Assert.Equal(Resources.Validate_GenerateModelingCodeCommandIIncludeInVSIXAsShouldBeSameAsFileCode, this.validationContext.CurrentViolations.First().Code);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTextTemplateExistsAndIncludeInVSIXDuplicate_ThenValidationFails()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), "solution://projectguid/fileguid");

                this.textTemplateFile.Data.ItemType = "Content";
                this.textTemplateFile.Data.IncludeInVSIX = "true";
                this.textTemplateFile.Data.IncludeInVSIXAs = "foo";
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(this.textTemplateFile);

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 1);
                Assert.Equal(Resources.Validate_GenerateModelingCodeCommandIIncludeInVSIXDuplicateCode, this.validationContext.CurrentViolations.First().Code);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTargetFileNameEmpty_ThenValidationFails()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), string.Empty);
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), "solution://projectguid/fileguid");

                this.textTemplateFile.Data.ItemType = "Content";
                this.textTemplateFile.Data.IncludeInVSIX = "true";
                this.textTemplateFile.Data.IncludeInVSIXAs = string.Empty;
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(this.textTemplateFile);

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 1);
                Assert.Equal(Resources.Validate_GenerateModelingCodeCommandTargetFilenameEmptyCode, this.validationContext.CurrentViolations.First().Code);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTargetPathStartsWithTildaOnNonProductElement_ThenValidationFails()
            {
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), PathResolver.ResolveArtifactLinkCharacter + "foo");
                this.settings.SetPropertyValue<string>(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), "solution://projectguid/fileguid");

                this.textTemplateFile.Data.ItemType = "Content";
                this.textTemplateFile.Data.IncludeInVSIX = "true";
                this.textTemplateFile.Data.IncludeInVSIXAs = string.Empty;
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(this.textTemplateFile);

                this.validation.Validate(this.validationContext, this.element, this.settings);

                Assert.True(this.validationContext.CurrentViolations.Count == 1);
                Assert.Equal(Resources.Validate_GenerateModelingCodeCommandTargetPathStartsWithResolverCode, this.validationContext.CurrentViolations.First().Code);
            }
        }
    }
}
