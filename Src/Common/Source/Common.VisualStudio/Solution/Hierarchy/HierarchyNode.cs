using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.VisualStudio.Properties;
using NuPattern.VisualStudio.Solution.Hierarchy.Design;

namespace NuPattern.VisualStudio.Solution.Hierarchy
{
    /// <summary>
    /// Node in the solution explorer
    /// </summary>
    [TypeConverter(typeof(HierarchyNodeConverter))]
    internal class HierarchyNode : IDisposable, IHierarchyNode
    {
        #region NativeMethods class

        private sealed class NativeMethods
        {
            private NativeMethods() { }
            [StructLayout(LayoutKind.Sequential)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public IntPtr iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            };

            public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010; // use passed dwFileAttribute
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

            [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
            [DllImport("comctl32.dll")]
            public extern static IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);
        }

        #endregion

        /// <summary>
        /// Constructs a HierarchyNode at the solution root
        /// </summary>
        /// <param name="vsSolution"></param>
        [SecurityCritical]
        public HierarchyNode(IVsSolution vsSolution)
            : this(vsSolution, Guid.Empty)
        {
        }

        /// <summary>
        /// Constructs a HierarchyNode given the unique string identifier
        /// </summary>
        /// <param name="vsSolution"></param>
        /// <param name="projectUniqueName"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsSolution.GetProjectOfUniqueName(System.String,Microsoft.VisualStudio.Shell.Interop.IVsHierarchy@)")]
        [SecurityCritical]
        public HierarchyNode(IVsSolution vsSolution, string projectUniqueName)
        {
            Guard.NotNull(() => vsSolution, vsSolution);
            Guard.NotNullOrEmpty(() => projectUniqueName, projectUniqueName);

            IVsHierarchy rootHierarchy = null;
            if (projectUniqueName.StartsWith("{", StringComparison.OrdinalIgnoreCase) &&
                projectUniqueName.EndsWith("}", StringComparison.OrdinalIgnoreCase))
            {
                projectUniqueName = projectUniqueName.Substring(1, projectUniqueName.Length - 2);
            }

            if (projectUniqueName.Length == Guid.Empty.ToString().Length &&
                projectUniqueName.Split('-').Length == 5)
            {
                Guid projectGuid = new Guid(projectUniqueName);
                int hr = vsSolution.GetProjectOfGuid(ref projectGuid, out rootHierarchy);
                Marshal.ThrowExceptionForHR(hr);
            }
            else
            {
                int hr = vsSolution.GetProjectOfUniqueName(projectUniqueName, out rootHierarchy);
                if (rootHierarchy == null)
                {
                    //Thrown if Project doesn't exist on solution
                    throw new InvalidOperationException(
                        String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProjectNode_InvalidProjectUniqueName, projectUniqueName));
                }
            }
            Init(vsSolution, rootHierarchy, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        /// Constructs a HierarchyNode given the projectGuid
        /// </summary>
        /// <param name="vsSolution"></param>
        /// <param name="projectGuid"></param>
        // FXCOP: False positive
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
        [SecurityCritical]
        public HierarchyNode(IVsSolution vsSolution, Guid projectGuid)
        {
            Guard.NotNull(() => vsSolution, vsSolution);

            IVsHierarchy rootHierarchy = null;
            int hr = vsSolution.GetProjectOfGuid(ref projectGuid, out rootHierarchy);
            if (rootHierarchy == null)
            {
                throw new InvalidOperationException(
                    String.Format(CultureInfo.CurrentCulture, Resources.HierarchyNode_ProjectDoesNotExist, projectGuid.ToString("b")),
                    Marshal.GetExceptionForHR(hr));
            }
            Init(vsSolution, rootHierarchy, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        /// Constructs a hierarchy node at the root level of hierarchy
        /// </summary>
        /// <param name="vsSolution"></param>
        /// <param name="hierarchy"></param>
        [SecurityCritical]
        public HierarchyNode(IVsSolution vsSolution, IVsHierarchy hierarchy)
        {
            Guard.NotNull(() => vsSolution, vsSolution);

            Init(vsSolution, hierarchy, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        /// Builds a child HierarchyNode from the parent node
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childId"></param>
        [SecurityCritical]
        public HierarchyNode(HierarchyNode parent, uint childId)
        {
            Guard.NotNull(() => parent, parent);

            Init(parent.solution, parent.hierarchy, childId);
        }

        /// <summary>
        /// Queries the type T to the internal hierarchy object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T GetObject<T>()
            where T : class
        {
            return (hierarchy as T);
        }

        /// <summary>
        /// Returns true if this a root node of another node
        /// </summary>
        public bool IsRoot
        {
            get { return (VSConstants.VSITEMID_ROOT == itemId); }
        }

        /// <summary>
        /// Name of this node
        /// </summary>
        public string Name
        {
            get { return GetProperty<string>(__VSHPROPID.VSHPROPID_Name); }
        }

        /// <summary>
        /// Document cookie
        /// </summary>
        public uint DocCookie
        {
            get { return GetProperty<uint>(__VSHPROPID.VSHPROPID_ItemDocCookie); }
        }

        /// <summary>
        /// Name of this node
        /// </summary>

        public string CanonicalName
        {
            [SecurityCritical]
            get
            {
                string name = string.Empty;
                int hr = hierarchy.GetCanonicalName(itemId, out name);
                Marshal.ThrowExceptionForHR(hr);
                if (name != null)
                {
                    return name;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the unique string that identifies this node in the solution
        /// </summary>
        public string UniqueName
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get
            {
                string uniqueName = string.Empty;
                int hr = solution.GetUniqueNameOfProject(hierarchy, out uniqueName);
                Marshal.ThrowExceptionForHR(hr);
                return uniqueName;
            }
        }

        /// <summary>
        /// Returns the Project GUID
        /// </summary>
        public Guid ProjectGuid
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get { return GetGuidProperty(__VSHPROPID.VSHPROPID_ProjectIDGuid); }
        }

        /// <summary>
        /// Returns true if the current node is the solution root
        /// </summary>
        public bool IsSolution
        {
            get
            {
                return (Parent == null);
            }
        }

        /// <summary>
        /// Returns the TypeGUID
        /// </summary>
        public Guid TypeGuid
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get
            {
                // If the root node is a solution, then there is no TypeGuid
                if (IsSolution)
                {
                    return Guid.Empty;
                }
                else
                {
                    return GetGuidProperty(__VSHPROPID.VSHPROPID_TypeGuid, this.ItemId);
                }
            }
        }

        /// <summary>
        /// Icon handle of the node
        /// </summary>
        public IntPtr IconHandle
        {
            get { return new IntPtr(GetProperty<int>(__VSHPROPID.VSHPROPID_IconHandle)); }
        }

        /// <summary>
        /// Icon index of the node
        /// </summary>
        public int IconIndex
        {
            get { return GetProperty<int>(__VSHPROPID.VSHPROPID_IconIndex); }
        }

        /// <summary>
        /// True if the Icon index of the node is valid
        /// </summary>
        public bool HasIconIndex
        {
            get { return HasProperty(__VSHPROPID.VSHPROPID_IconIndex); }
        }

        /// <summary>
        /// StateIcon index of the node
        /// </summary>
        public int StateIconIndex
        {
            get { return GetProperty<int>(__VSHPROPID.VSHPROPID_StateIconIndex); }
        }

        /// <summary>
        /// OverlayIcon index of the node
        /// </summary>
        public int OverlayIconIndex
        {
            get { return GetProperty<int>(__VSHPROPID.VSHPROPID_OverlayIconIndex); }
        }

        /// <summary>
        /// Imagelist Handle
        /// </summary>
        public IntPtr ImageListHandle
        {
            get { return new IntPtr(GetProperty<int>(__VSHPROPID.VSHPROPID_IconImgList)); }
        }

        private string iconKey;
        /// <summary>
        /// Returns the Key to index icons in an image collection
        /// </summary>
        public string IconKey
        {
            get
            {
                if (iconKey == null)
                {
                    if (HasIconIndex)
                    {
                        iconKey = TypeGuid.ToString("b", CultureInfo.InvariantCulture) + "." + IconIndex.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (IsValidFullPathName(SaveName))
                    {
                        iconKey = new FileInfo(SaveName).Extension;
                    }
                    else
                    {
                        iconKey = string.Empty;
                    }
                }
                return iconKey;
            }
        }

        /// <summary>
        /// item id
        /// </summary>
        private uint itemId;

        public uint ItemId
        {
            get { return itemId; }
        }

        /// <summary>
        /// hierarchy object
        /// </summary>
        private IVsHierarchy hierarchy;

        protected IVsHierarchy Hierarchy
        {
            get { return hierarchy; }
        }

        /// <summary>
        /// Solution service
        /// </summary>
        private IVsSolution solution;

        protected IVsSolution Solution
        {
            get { return solution; }
        }

        protected static bool IsValidFullPathName(string fileName)
        {
            Debug.Assert(fileName != null);
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            int i = fileName.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
            if (i == -1)
            {
                return IsValidFileName(fileName);
            }
            else
            {
                string pathPart = fileName.Substring(0, i + 1);
                if (IsValidPath(pathPart))
                {
                    string filePart = fileName.Substring(i + 1);
                    return IsValidFileName(filePart);
                }
            }
            return false;
        }

        protected static bool IsValidPath(string pathPart)
        {
            Debug.Assert(pathPart != null);
            if (string.IsNullOrEmpty(pathPart))
            {
                return true;
            }
            foreach (char c in System.IO.Path.GetInvalidPathChars())
            {
                if (pathPart.IndexOf(c) != -1)
                {
                    return false;
                }
            }
            return true;
        }

        protected static bool IsValidFileName(string filePart)
        {
            Debug.Assert(filePart != null);
            if (string.IsNullOrEmpty(filePart))
            {
                return false;
            }
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                if (filePart.IndexOf(c) != -1)
                {
                    return false;
                }
            }
            return true;
        }


        private Icon icon;
        /// <summary>
        /// Returns the icon of the node
        /// </summary>
        public Icon Icon
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get
            {
                if (icon == null)
                {
                    if (ImageListHandle != IntPtr.Zero && HasIconIndex)
                    {
                        IntPtr hIcon = NativeMethods.ImageList_GetIcon(ImageListHandle, IconIndex, 0);
                        icon = Icon.FromHandle(hIcon);
                    }
                    else if (IconHandle != IntPtr.Zero)
                    {
                        icon = Icon.FromHandle(IconHandle);
                    }
                    else if (IsValidFullPathName(SaveName))
                    {
                        // The following comes from kb 319350
                        NativeMethods.SHFILEINFO shinfo = new NativeMethods.SHFILEINFO();
                        NativeMethods.SHGetFileInfo(
                            new FileInfo(SaveName).Extension,
                            NativeMethods.FILE_ATTRIBUTE_NORMAL,
                            ref shinfo, (uint)Marshal.SizeOf(shinfo),
                            NativeMethods.SHGFI_USEFILEATTRIBUTES | NativeMethods.SHGFI_ICON | NativeMethods.SHGFI_SMALLICON);
                        if (shinfo.hIcon != IntPtr.Zero)
                        {
                            icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                        }
                    }
                }
                return icon;
            }
        }

        /// <summary>
        /// Returns true is there is al least one child under this node
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return (FirstChildId != VSConstants.VSITEMID_NIL);
            }
        }

        /// <summary>
        /// Returns the item id of the first child
        /// </summary>
        public uint FirstChildId
        {
            get
            {
                //Get the first child node of the current hierarchy being walked
                // NOTE: to work around a bug with the Solution implementation of VSHPROPID_FirstChild,
                // we keep track of the recursion level. If we are asking for the first child under
                // the Solution, we use VSHPROPID_FirstVisibleChild instead of _FirstChild. 
                // In VS 2005 and earlier, the Solution improperly enumerates all nested projects
                // in the Solution (at any depth) as if they are immediate children of the Solution.
                // Its implementation _FirstVisibleChild is correct however, and given that there is
                // not a feature to hide a SolutionFolder or a Project, thus _FirstVisibleChild is 
                // expected to return the identical results as _FirstChild.
                return GetItemId(GetProperty<object>(IsSolution ? __VSHPROPID.VSHPROPID_FirstVisibleChild : __VSHPROPID.VSHPROPID_FirstChild));
            }
        }
        /// <summary>
        /// Gets the next child id from the passed childId
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsHierarchy.GetProperty(System.UInt32,System.Int32,System.Object@)")]
        [SecurityCritical]
        public uint GetNextChildId(uint childId)
        {
            object nextChild = null;
            // NOTE: to work around a bug with the Solution implementation of VSHPROPID_NextSibling,
            // we keep track of the recursion level. If we are asking for the next sibling under
            // the Solution, we use VSHPROPID_NextVisibleSibling instead of _NextSibling. 
            // In VS 2005 and earlier, the Solution improperly enumerates all nested projects
            // in the Solution (at any depth) as if they are immediate children of the Solution.
            // Its implementation   _NextVisibleSibling is correct however, and given that there is
            // not a feature to hide a SolutionFolder or a Project, thus _NextVisibleSibling is 
            // expected to return the identical results as _NextSibling.
            hierarchy.GetProperty(childId,
                    (int)(IsSolution ? __VSHPROPID.VSHPROPID_NextVisibleSibling : __VSHPROPID.VSHPROPID_NextSibling),
                    out nextChild);
            return GetItemId(nextChild);
        }

        /// <summary>
        /// Returns the file name of the hierarcynode
        /// </summary>
        public string SaveName
        {
            get { return GetProperty<string>(__VSHPROPID.VSHPROPID_SaveName); }
        }

        /// <summary>
        /// Returns the project directory
        /// </summary>
        public string ProjectDir
        {
            get { return GetProperty<string>(__VSHPROPID.VSHPROPID_ProjectDir); }
        }

        /// <summary>
        /// Returns the full path
        /// </summary>
        /// <returns></returns>
        public string Path
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]

            get
            {
                string path = string.Empty;
                if (hierarchy is IVsProject)
                {
                    int hr = ((IVsProject)hierarchy).GetMkDocument(itemId, out path);
                    Marshal.ThrowExceptionForHR(hr);
                    return path;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string RelativePath
        {
            get
            {
                if (IsRoot)
                {
                    return ProjectDir;
                }
                else if (ParentNode != null)
                {
                    return System.IO.Path.Combine(ParentNode.RelativePath, Name);
                }
                else
                {
                    return Name;
                }
            }
        }

        /// <summary>
        /// Returns the extensibility object
        /// </summary>
        public object ExtObject
        {
            get { return GetProperty<object>(__VSHPROPID.VSHPROPID_ExtObject); }
        }

        [SecurityCritical]
        private Guid GetGuidProperty(__VSHPROPID propId, uint itemid)
        {
            Guid guid = Guid.Empty;
            int hr = hierarchy.GetGuidProperty(itemid, (int)propId, out guid);
            // in case of failure, we simply trace the error and return silently with an empty guid
            // so the caller can resolve what to do without blowing out execution with an exception
            if (hr != 0) Trace.TraceError(Marshal.GetExceptionForHR(hr).ToString());
            return guid;
        }

        [SecurityCritical]
        private Guid GetGuidProperty(__VSHPROPID propId)
        {
            return GetGuidProperty(propId, VSConstants.VSITEMID_ROOT);
        }

        private bool HasProperty(__VSHPROPID propId)
        {
            object value = null;
            int hr = hierarchy.GetProperty(this.itemId, (int)propId, out value);
            if (hr != VSConstants.S_OK || value == null)
            {
                return false;
            }
            return true;
        }

        private T GetProperty<T>(__VSHPROPID propId, uint itemid)
        {
            object value = null;
            int hr = hierarchy.GetProperty(itemid, (int)propId, out value);
            if (hr != VSConstants.S_OK || value == null)
            {
                return default(T);
            }
            return (T)value;
        }

        private T GetProperty<T>(__VSHPROPID propId)
        {
            return GetProperty<T>(propId, this.itemId);
        }

        private static uint GetItemId(object pvar)
        {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        [SecurityCritical]
        public HierarchyNode CreateSolutionFolder(string folderName)
        {
            Guard.NotNullOrEmpty(() => folderName, "folderName");
            IntPtr ptr = IntPtr.Zero;
            Guid solutionFolderGuid = new Guid(Constants.SolutionFolderType);
            Guid iidProject = typeof(IVsHierarchy).GUID;
            int hr = solution.CreateProject(
                ref solutionFolderGuid,
                null,
                null,
                folderName,
                0,
                ref iidProject,
                out ptr);
            if (hr == VSConstants.S_OK && ptr != IntPtr.Zero)
            {
                IVsHierarchy vsHierarchy = (IVsHierarchy)Marshal.GetObjectForIUnknown(ptr);
                Debug.Assert(vsHierarchy != null);
                return new HierarchyNode(solution, vsHierarchy);
            }
            return null;
        }

        public HierarchyNode Find(Predicate<HierarchyNode> func)
        {
            foreach (HierarchyNode child in this.Children)
            {
                if (func(child))
                {
                    return child;
                }
            }
            return null;
        }

        // FXCOP: False positive
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "child")]
        public void ForEach(Action<HierarchyNode> func)
        {
            foreach (HierarchyNode child in this.Children)
            {
                func(child);
            }
        }

        public void RecursiveForEach(Action<HierarchyNode> func)
        {
            func(this);
            foreach (HierarchyNode child in this.Children)
            {
                child.RecursiveForEach(func);
            }
        }

        public HierarchyNode RecursiveFind(Predicate<HierarchyNode> func)
        {
            if (func(this))
            {
                return this;
            }
            foreach (HierarchyNode child in this.Children)
            {
                HierarchyNode found = child.RecursiveFind(func);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public HierarchyNode FindByName(string name)
        {
            return Find(delegate(HierarchyNode node)
            {
                return (node.Name != null && node.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            });
        }

        public HierarchyNode RecursiveFindByName(string name)
        {
            if (name.IndexOf(System.IO.Path.DirectorySeparatorChar) == -1)
            {
                return RecursiveFind(delegate(HierarchyNode node)
                {
                    return (node.Name != null && node.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                });
            }

            HierarchyNode folder = null;
            foreach (string part in name.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
            {
                folder = folder == null ? this.FindByName(part) : folder.FindByName(part);
                if (folder == null)
                {
                    break;
                }
            }
            return folder;
        }

        [SecurityCritical]
        public HierarchyNode FindOrCreateSolutionFolder(string name)
        {
            HierarchyNode folder = FindByName(name);
            if (folder == null)
            {
                folder = CreateSolutionFolder(name);
            }
            return folder;
        }

        public HierarchyNode Parent
        {
            get
            {
                if (!IsRoot)
                {
                    return new HierarchyNode(solution, hierarchy);
                }
                else
                {
                    IVsHierarchy vsHierarchy = GetProperty<IVsHierarchy>(__VSHPROPID.VSHPROPID_ParentHierarchy, VSConstants.VSITEMID_ROOT);
                    if (vsHierarchy == null)
                    {
                        return null;
                    }
                    return new HierarchyNode(solution, vsHierarchy);
                }
            }
        }

        public void Remove()
        {
            Debug.Assert(Parent != null);
            Parent.RemoveItem(itemId);
        }

        private bool RemoveItem(uint vsItemId)
        {
            IVsProject2 vsProject = hierarchy as IVsProject2;
            if (vsProject == null)
            {
                return false;
            }
            int result = 0;
            int hr = vsProject.RemoveItem(0, vsItemId, out result);
            return (hr == VSConstants.S_OK && result == 1);
        }

        #region IDisposable Members

        private bool disposed;

        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.

                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.

            }
            disposed = true;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~HierarchyNode()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

        public IEnumerable<IHierarchyNode> Children
        {
            get { return new HierarchyNodeCollection(this); }
        }

        [SecurityCritical]
        private void Init(IVsSolution vsSolution, IVsHierarchy vsHierarchy, uint vsItemId)
        {
            this.solution = vsSolution;
            int hr = VSConstants.E_FAIL;
            if (vsHierarchy == null)
            {
                Guid emptyGuid = Guid.Empty;
                hr = this.solution.GetProjectOfGuid(ref emptyGuid, out this.hierarchy);
                Marshal.ThrowExceptionForHR(hr);
            }
            else
            {
                this.hierarchy = vsHierarchy;
            }
            this.itemId = vsItemId;

            IntPtr nestedHierarchyObj;
            uint nestedItemId;
            Guid hierGuid = typeof(IVsHierarchy).GUID;

            // Check first if this node has a nested hierarchy. If so, then there really are two 
            // identities for this node: 1. hierarchy/itemid 2. nestedHierarchy/nestedItemId.
            // We will convert this node using the inner nestedHierarchy/nestedItemId identity.
            hr = this.hierarchy.GetNestedHierarchy(this.itemId, ref hierGuid, out nestedHierarchyObj, out nestedItemId);
            if (VSConstants.S_OK == hr && IntPtr.Zero != nestedHierarchyObj)
            {
                IVsHierarchy nestedHierarchy = Marshal.GetObjectForIUnknown(nestedHierarchyObj) as IVsHierarchy;
                Marshal.Release(nestedHierarchyObj); // we are responsible to release the refcount on the out IntPtr parameter
                if (nestedHierarchy != null)
                {
                    this.hierarchy = nestedHierarchy;
                    this.itemId = nestedItemId;
                }
            }
        }

        public HierarchyNode ParentNode
        {
            get
            {
                HierarchyNode parent = this.Parent;
                if (parent == this)
                    return parent;
                parent = parent.RecursiveFind(x => x.Children.FirstOrDefault(child => child.ItemId == this.ItemId) != null);
                return parent;
            }
        }

        public string SolutionRelativeName
        {
            get
            {
                if (IsSolution)
                {
                    return string.Empty;
                }
                else if (ParentNode != null)
                {
                    string parentRelativeName = ParentNode.SolutionRelativeName;
                    if (!String.IsNullOrEmpty(parentRelativeName))
                        return parentRelativeName + "\\" + Name;
                }

                return Name;
            }
        }
    }
}
