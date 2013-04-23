using System;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell.Interop;

/// <summary>
/// A helper class to provide access to DslDesigner functionality.
/// </summary>
[CLSCompliant(false)]
public sealed class DslDesigner
{
    private IServiceProvider serviceProvider;
    private SingleDiagramDocView docView;

    public DslDesigner(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        this.docView = GetDocView(this.serviceProvider);

        while (this.docView == null)
        {
            Thread.Sleep(10);
        }
    }

    /// <summary>
    /// Gets the DocView for this designer.
    /// </summary>
    [CLSCompliant(false)]
    public SingleDiagramDocView DocView
    {
        get { return this.docView; }
    }

    /// <summary>
    /// Gets the DocData for this DocView.
    /// </summary>
    public ModelingDocData DocData
    {
        get { return this.docView.DocData as ModelingDocData; }
    }

    /// <summary>
    /// Returns the current diagram view.
    /// </summary>
    private static SingleDiagramDocView GetDocView(IServiceProvider serviceProvider)
    {
        var monitorSelectionService = (IVsMonitorSelection)serviceProvider.GetService(typeof(IVsMonitorSelection));

        object frameObject;

        ErrorHandler.ThrowOnFailure(monitorSelectionService.GetCurrentElementValue(2, out frameObject));

        var windowFrame = frameObject as IVsWindowFrame;

        if (windowFrame != null)
        {
            object propertyValue;

            ErrorHandler.ThrowOnFailure(windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out propertyValue));

            return propertyValue as SingleDiagramDocView;
        }

        return null;
    }
}