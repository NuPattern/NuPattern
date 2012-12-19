using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.Runtime;

namespace NuPattern.Authoring.Automation.Commands
{
	internal static class DesignerCommandHelper
	{
		public static void DoActionOnDesigner(string fileName, Action<ModelingDocData> action, bool leaveItOpenAndVisible = false)
		{
			var dte = ServiceProvider.GlobalProvider.GetService<SDTE, DTE>();
			var rdt = new RunningDocumentTable(ServiceProvider.GlobalProvider);
			var documentInfo = rdt.FirstOrDefault(info =>
				info.Moniker.Equals(fileName, StringComparison.OrdinalIgnoreCase));

			if (!string.IsNullOrEmpty(documentInfo.Moniker))
			{
				ActivateDocument(dte, documentInfo.Moniker);

				var docdata = documentInfo.DocData as ModelingDocData;

				if (docdata == null)
				{
					//File is opened but not in designer view
					var projectItem = dte.Solution.FindProjectItem(documentInfo.Moniker);

					projectItem.Document.Close(vsSaveChanges.vsSaveChangesYes);

					var document = OpenDesigner(dte, fileName, false);

					docdata = GetModelingDocData(rdt, document);
				}

				action(docdata as ModelingDocData);
			}
			else
			{
				if (leaveItOpenAndVisible)
				{
					var document = OpenDesigner(dte, fileName, false);

					action(GetModelingDocData(rdt, document));

					document.Save();
				}
				else
				{
					var document = OpenDesigner(dte, fileName);

					action(GetModelingDocData(rdt, document));

					document.Close(vsSaveChanges.vsSaveChangesYes);
				}
			}
		}

		internal static ModelingDocData GetModelingDocData(RunningDocumentTable rdt, Document document)
		{
			var documentInfo = rdt.FirstOrDefault(info =>
			   info.Moniker.Equals(document.FullName, StringComparison.OrdinalIgnoreCase));

			return documentInfo.DocData as ModelingDocData;
		}

		internal static void ActivateDocument(DTE dte, string fileName)
		{
			var document = dte.Documents.OfType<Document>().FirstOrDefault(
				doc => doc.FullName.Equals(fileName, StringComparison.OrdinalIgnoreCase));

			if (document != null)
			{
				document.Activate();
			}
		}

		internal static Document OpenDesigner(DTE dte, string fileName, bool invisibleMode = true)
		{
			var projectItem = dte.Solution.FindProjectItem(fileName);

			//TODO: Open always in designer view 
			var window = projectItem.Open(EnvDTE.Constants.vsViewKindDesigner);

			if (!invisibleMode)
			{
				window.Visible = true;
				window.Activate();
			}

			return window.Document;
		}
	}
}