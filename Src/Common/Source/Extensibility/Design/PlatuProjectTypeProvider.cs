using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Input;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Runtime;
using VSLangProj;
using Ole = Microsoft.VisualStudio.OLE.Interop;

namespace NuPattern.Extensibility
{
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IPlatuProjectTypeProvider))]
	class PlatuProjectTypeProvider : IPlatuProjectTypeProvider
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<PlatuProjectTypeProvider>();
		private readonly IServiceProvider serviceProvider;

		[ImportingConstructor]
		public PlatuProjectTypeProvider([Import(typeof(SVsServiceProvider))]IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public IEnumerable<Type> GetTypes<T>()
		{
			using (new MouseCursor(Cursors.Wait))
			{
				return GetAvailableTypes(typeof(T));
			}
		}

		private IEnumerable<Type> GetAvailableTypes(Type assignableTo)
		{
			var allTypes = new List<Type>();

			var selection = this.serviceProvider.GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelectionFixed;
			if (selection != null)
			{
				IVsHierarchy sourceProject;
				uint pitemid;
				IVsMultiItemSelect ppMIS;
				ISelectionContainer ppSC;

				if (selection.GetCurrentSelection(out sourceProject, out pitemid, out ppMIS, out ppSC) == Microsoft.VisualStudio.VSConstants.S_OK)
				{
					if (sourceProject != null)
					{
						var dteProject = default(Project);

						try
						{
							dteProject = ToDteProject(sourceProject);
						}
						catch (Exception ex)
						{
							tracer.TraceWarning(ex.Message);
							return Enumerable.Empty<Type>();
						} 
						
						var vslangProject = dteProject.Object as VSProject;
						var vsProject = sourceProject as IVsProject;

						if (vsProject != null)
						{
							Ole.IServiceProvider olesp;
							// The design-time assembly resolution (DTAR) must be retrieved from a 
							// local service provider for the project. This is the magic goo.
							if (vsProject.GetItemContext(Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT, out olesp) == Microsoft.VisualStudio.VSConstants.S_OK)
							{
								var localservices = new ServiceProvider(olesp);
								var openScope = this.serviceProvider.GetService(typeof(SVsSmartOpenScope)) as IVsSmartOpenScope;
								var dtar = (IVsDesignTimeAssemblyResolution)localservices.GetService(typeof(SVsDesignTimeAssemblyResolution));

								// As suggested by Christy Henriksson, we reuse the type discovery service 
								// but just for the IDesignTimeAssemblyLoader interface. The actual 
								// assembly reading is done by the TFP using metadata only :)
								var dts = (DynamicTypeService)this.serviceProvider.GetService(typeof(DynamicTypeService));
								var ds = dts.GetTypeDiscoveryService(sourceProject);
								var dtal = ds as IDesignTimeAssemblyLoader;

								var provider = new VsTargetFrameworkProvider(
									dtar,
									dtal,
									openScope);

								foreach (var reference in vslangProject.References.OfType<Reference>())
								{
									try
									{
										var name = AssemblyName.GetAssemblyName(reference.Path);

										allTypes.AddRange(provider
											.GetReflectionAssembly(name)
											.GetExportedTypes()
											.Where(t => t.IsAssignableTo(assignableTo)));
									}
									catch (Exception)
									{
										continue;
									}
								}
							}
						}
					}
				}
			}

			return allTypes;
		}

		private static Project ToDteProject(IVsHierarchy hierarchy)
		{
			if (hierarchy == null) throw new ArgumentNullException("hierarchy");

			object prjObject = null;
			if (hierarchy.GetProperty(0xfffffffe, -2027, out prjObject) >= 0)
			{
				return (EnvDTE.Project)prjObject;
			}
			else
			{
				throw new ArgumentException("Hierarchy is not a project.");
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public bool SupportsAssemblyLoading
		{
			get { return false; }
		}

		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("18291FD1-A1DD-4264-AEAD-6AFD616BF15A")]
		private interface IVsTrackSelectionExFixed : ITrackSelection
		{
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int OnSelectChangeEx([In] IntPtr pHier, [In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] uint itemid, [In, MarshalAs(UnmanagedType.Interface)] IVsMultiItemSelect pMIS, [In] IntPtr pSC);
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int IsMyHierarchyCurrent([ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] out int pfCurrent);
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int OnElementValueChange([In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSSELELEMID")] uint elementid, [In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] int fDontPropagate, [In, MarshalAs(UnmanagedType.Struct)] object varValue);
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IVsHierarchy ppHier, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] out uint pitemid, [MarshalAs(UnmanagedType.Interface)] out IVsMultiItemSelect ppMIS, [MarshalAs(UnmanagedType.Interface)] out ISelectionContainer ppSC);
		}

		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("55AB9450-F9C7-4305-94E8-BEF12065338D")]
		private interface IVsMonitorSelectionFixed
		{
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IVsHierarchy ppHier, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSITEMID")] out uint pitemid, [MarshalAs(UnmanagedType.Interface)] out IVsMultiItemSelect ppMIS, [MarshalAs(UnmanagedType.Interface)] out ISelectionContainer ppSC);
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int AdviseSelectionEvents([In, MarshalAs(UnmanagedType.Interface)] IVsSelectionEvents pSink, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] out uint pdwCookie);
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int UnadviseSelectionEvents([In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] uint dwCookie);
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int GetCurrentElementValue([In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSSELELEMID")] uint elementid, [MarshalAs(UnmanagedType.Struct)] out object pvarValue);
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int GetCmdUIContextCookie([In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.REFGUID")] ref Guid rguidCmdUI, [ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] out uint pdwCmdUICookie);
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int IsCmdUIContextActive([In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] uint dwCmdUICookie, [ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] out int pfActive);
			[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int SetCmdUIContext([In, ComAliasName("Microsoft.VisualStudio.Shell.Interop.VSCOOKIE")] uint dwCmdUICookie, [In, ComAliasName("Microsoft.VisualStudio.OLE.Interop.BOOL")] int fActive);
		}
	}
}

