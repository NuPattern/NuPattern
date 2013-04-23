using System;
using System.Collections.Generic;
using System.Drawing;
using NuPattern.Runtime.Guidance;
using NuPattern.VisualStudio.Extensions;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Runtime.Shell
{
    internal class GuidanceRegistration : IGuidanceExtensionRegistration
    {
        public string ExtensionId { get; set; }
        public string DefaultName { get; set; }
        public string InstallPath { get; set; }
        public Icon Icon { get; set; }
        public Bitmap PreviewImage { get; set; }
        public IInstalledExtension InstalledExtension { get; set; }
        public IExtension ExtensionManifest { get; set; }
        public IEnumerable<IVsTemplate> Templates
        {
            get { throw new NotImplementedException(); }
        }
    }
}