using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Patterning.Runtime.Shell.Properties;
using NuGet;

namespace Microsoft.VisualStudio.Patterning.Runtime.Shell
{
    /// <summary>
    /// Deploys dependencies using NuGet
    /// </summary>
    internal class DependencyDeployer
    {
        private IServiceProvider provider;
        private string packageName;
        private Version packageVersion;

        /// <summary>
        /// Creates a new instance of the <see cref="DependencyDeployer"/> class.
        /// </summary>
        /// <param name="provider">A service provider</param>
        /// <param name="packageName">The name of the of package to deploy</param>
        /// <param name="packageVersion">The version of the package to deploy</param>
        public DependencyDeployer(IServiceProvider provider, string packageName, Version packageVersion = null)
        {
            Guard.NotNull(() => provider, provider);
            Guard.NotNullOrEmpty(() => packageName, packageName);

            this.provider = provider;
            this.packageName = packageName;
            this.packageVersion = packageVersion;
        }

        /// <summary>
        /// Deploys the dependency.
        /// </summary>
        /// <param name="targetDirectory">The directory to deploy to</param>
        /// <param name="selectPackageDirectory">The relative directory in the package to copy files from</param>
        /// <param name="selectFileMask">A file mask of files within package to copy</param>
        public void Deploy(string targetDirectory, string selectPackageDirectory, string selectFileMask)
        {
            var componentModel = this.provider.GetService<SComponentModel, IComponentModel>();

            // Need to be version independent here, becuase we dont know what version of NuGet is installed.
            var packageManager = componentModel.DefaultExportProvider.GetExportedValue<dynamic>(typeof(IPackageRepository).FullName);  //.GetExportedValue<IPackageRepository>() 
            var repository = packageManager; //as IFindPackagesRepository
            if (repository != null)
            {
                var packages = repository.FindPackagesById(this.packageName);
                if (packages != null)
                {
                    dynamic packageVersion = GetVersionedPackage(packages);
                    if (packageVersion != null)
                    {
                        var tempDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));

                        try
                        {
                            //Unzip the files to temp directory
                            using (System.IO.Packaging.Package package = System.IO.Packaging.ZipPackage.Open(packageVersion.GetStream(), FileMode.Open, FileAccess.Read))
                            {
                                package.GetParts().ToList()
                                    .ForEach(p =>
                                    {
                                        var tmpPath = Path.Combine(tempDir.FullName, ConvertUriToLocal(p.Uri.OriginalString));
                                        Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
                                        using (var stream = p.GetStream())
                                        {
                                            File.WriteAllBytes(tmpPath, stream.ReadAllBytes());
                                        }
                                    });
                            }

                            //Copy selected files to runtime directory
                            var targetDir = Path.Combine(tempDir.FullName, selectPackageDirectory);
                            Directory.GetFiles(targetDir, selectFileMask).ToList()
                                .ForEach(f =>
                                {
                                    var destPath = Path.Combine(targetDirectory, Path.GetFileName(f));
                                    File.Copy(f, destPath);
                                });
                        }
                        finally
                        {
                            //Delete temp dir
                            if (tempDir != null)
                            {
                                tempDir.Delete(true);
                            }
                        }
                    }
                    else
                    {
                        //Can't find a version of package!!!
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.RuntimeShellPackage_DependencyNotAvailable, this.packageName));
                    }
                }
                else
                {
                    //Can't find package!!!
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.RuntimeShellPackage_DependencyNotAvailable, this.packageName));
                }
            }
            else
            {
                // NuGet not installed!
                throw new InvalidOperationException(Resources.RuntimeShellPackage_PackageManagerNotInstalled);
            }

        }

        /// <summary>
        /// Whether the depency is deployed.
        /// </summary>
        public bool IsDeployed(string markerFile)
        {
            Guard.NotNullOrEmpty(() => markerFile, markerFile);

            return File.Exists(markerFile);
        }

        private dynamic GetVersionedPackage(dynamic packages)
        {
            // Get specified version, or latest version (or highest listed version)
            dynamic versionedPackage = null;
            foreach (var package in packages)
            {
                if (this.packageVersion == null)
                {
                    // Get latest version
                    if (package.IsLatestVersion)
                    {
                        versionedPackage = package;
                        break;
                    }
                    else
                    {
                        if (package.Listed)
                        {
                            if ((versionedPackage == null)
                                || (package.Version > versionedPackage.Version))
                            {
                                versionedPackage = package;
                            }
                        }
                    }
                }
                else
                {
                    // Get specific version
                    var ver = new Version(package.Version as string);
                    if (ver == this.packageVersion)
                    {
                        versionedPackage = package;
                        break;
                    }
                }
            }

            return versionedPackage;
        }

        private static string ConvertUriToLocal(string uri)
        {
            var localUri = new Uri(Uri.UriSchemeFile + Uri.SchemeDelimiter + uri.TrimStart(Path.AltDirectorySeparatorChar)).LocalPath;
            return localUri.TrimStart(Path.DirectorySeparatorChar);
        }
    }
}
