using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.Patterning.Extensibility.References;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Library.IntegrationTests
{
	[TestClass]
	[CLSCompliant(false)]
	[DeploymentItem(@"Content\\EmptySolution.sln")]
	public class TemplateAutomationExtensionSpec : VsHostedSpec
	{
		private static readonly IAssertion Assert = new Assertion();

		private static bool ShouldOpenSolution = true;
		private IPatternManager manager;
		private IInstalledToolkitInfo toolkit;

		[TestInitialize]
		public override void Initialize()
		{
			base.Initialize();

			if (ShouldOpenSolution && Dte != null)
			{
				OpenSolution(GetFullPath(@"EmptySolution.sln"));
				ShouldOpenSolution = false;
			}

			this.manager = ServiceProvider.GetService<IPatternManager>();
			var componentModel = ServiceProvider.GetService<SComponentModel, IComponentModel>();
			var installedFactories = componentModel.GetService<IEnumerable<IInstalledToolkitInfo>>();

			this.toolkit = installedFactories
				.SingleOrDefault(toolkit => toolkit.Id.Equals("1530D736-97EA-4B4A-8A25-C6C7A089D1BB", StringComparison.OrdinalIgnoreCase));
		}

		[HostType("VS IDE")]
		[Ignore]
		[TestMethod]
		public void WhenCreatingAProduct_ThenSetsArtifactLinks()
		{
			IProduct product = null;

			UIThreadInvoker.Invoke((Action)(() =>
				{
					VsHostedSpec.DoActionWithWait(4000,
						() =>
						{
							product = this.manager.CreateProduct(toolkit, "Foo");
						});
				}));

			Assert.NotNull(product);
			Assert.NotNull(product.TryGetReference(ReferenceKindConstants.ArtifactLink));

			Assert.True(product.Views.Count() > 0);
			var view = product.Views.ElementAt(0);

			Assert.True(view.Elements.Count() > 0);
			var element = view.Elements.ElementAt(0);

			Assert.NotNull(element.TryGetReference(ReferenceKindConstants.ArtifactLink));
		}
	}
}
