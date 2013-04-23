using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.Settings;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Helper methods for dealing with Visual Studio extension manifests.
    /// </summary>
    /// <remarks>
    /// This class is used for some integration tests that verify the contents of NuPattern VSIXes.
    /// This class is intended to be used outside of the VS environment, 
    /// and therefore does not use services from within VS.
    /// </remarks>
    [CLSCompliant(false)]
    public static class Vsix
    {
        private const long BufferSize = 4096;
        private static readonly Microsoft.VisualStudio.Settings.SettingsManager FakeSettings = new FakeSettingsManager();
        private const string ExtensionManagerServiceTypeFormat = "Microsoft.VisualStudio.ExtensionManager.ExtensionManagerService, Microsoft.VisualStudio.ExtensionManager.Implementation, Version={0}.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        private const string PackageFileExtension = ".vsix";
        private const string ManifestFileExtension = "extension.vsixmanifest";

        /// <summary>
        /// Unzips the contents of the vsix file to the target location. If the 
        /// target folder exists, it will be deleted before unzipping.
        /// </summary>
        /// <param name="vsixFile">The vsix file to unzip.</param>
        /// <param name="targetDir">The target directory.</param>
        public static void Unzip(string vsixFile, string targetDir)
        {
            Guard.NotNullOrEmpty(() => vsixFile, vsixFile);
            Guard.NotNullOrEmpty(() => targetDir, targetDir);

            var sourceFile = new FileInfo(vsixFile);
            var baseDir = new DirectoryInfo(targetDir);

            if (baseDir.Exists)
            {
                baseDir.Delete(true);
            }

            baseDir.Create();

            using (var package = ZipPackage.Open(sourceFile.FullName, FileMode.Open))
            {
                foreach (var part in package.GetParts())
                {
                    var targetFile = BuildTargetFileName(baseDir.FullName, part.Uri);

                    var subDir = new DirectoryInfo(Path.GetDirectoryName(targetFile));
                    if (!subDir.Exists)
                    {
                        subDir.Create();
                    }

                    using (var partStream = part.GetStream())
                    {
                        using (var fileStream = File.Open(targetFile, FileMode.Create))
                        {
                            CopyStream(partStream, fileStream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unzips the contents of the vsix stream to a collection of streams. 
        /// </summary>
        /// <param name="vsixStream">The vsix stream to unzip.</param>
        /// <returns>A dictionary of stream objects.</returns>
        public static Dictionary<string, Stream> Unzip(Stream vsixStream)
        {
            Guard.NotNull(() => vsixStream, vsixStream);

            Dictionary<string, Stream> partsStream = new Dictionary<string, Stream>();
            using (var package = ZipPackage.Open(vsixStream))
            {
                foreach (var part in package.GetParts())
                {
                    var key = part.Uri.ToString();
                    partsStream.Add(key, part.GetStream());
                }
            }

            return partsStream;
        }

        /// <summary>
        /// Reads the manifest out of either a .vsix file or 
        /// the extension.vsixmanifest file.
        /// </summary>
        /// <param name="vsixOrManifest">The path to the vsix or manifest file.</param>
        /// <returns>The in-memory extension manifest.</returns>
        public static IExtension ReadManifest(string vsixOrManifest)
        {
            Guard.NotNullOrEmpty(() => vsixOrManifest, vsixOrManifest);

            if (vsixOrManifest.EndsWith(PackageFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                // Unzip just the manifest to temp location.
                using (var package = ZipPackage.Open(vsixOrManifest, FileMode.Open))
                {
                    var vsixPart = package.GetParts()
                        .SingleOrDefault(part => part.Uri.OriginalString.EndsWith(ManifestFileExtension, StringComparison.OrdinalIgnoreCase));
                    if (vsixPart == null)
                    {
                        throw new ArgumentException(String.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Vsix_ManifestMissing,
                            vsixOrManifest));
                    }

                    vsixOrManifest = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), ManifestFileExtension);
                    Directory.CreateDirectory(Path.GetDirectoryName(vsixOrManifest));

                    using (var partStream = vsixPart.GetStream())
                    {
                        using (var fileStream = File.Open(vsixOrManifest, FileMode.Create))
                        {
                            CopyStream(partStream, fileStream);
                        }
                    }
                }
            }

            return CreateExtension(vsixOrManifest);
        }

        /// <summary>
        /// Reads the manifest out of a stream.
        /// </summary>
        /// <param name="vsixOrManifestStream">The stream of the vsix or manifest file.</param>
        /// <returns>The in-memory extension manifest.</returns>
        public static IExtension ReadManifest(Stream vsixOrManifestStream)
        {
            var vsixOrManifest = @"From Stream";

            //// Unzip just the manifest to temp location.
            using (var package = ZipPackage.Open(vsixOrManifestStream))
            {
                var vsixPart = package.GetParts()
                    .SingleOrDefault(part => part.Uri.OriginalString.EndsWith(ManifestFileExtension, StringComparison.OrdinalIgnoreCase));
                if (vsixPart == null)
                {
                    throw new ArgumentException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Vsix_ManifestMissing,
                        vsixOrManifest));
                }

                vsixOrManifest = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), ManifestFileExtension);
                var dir = Path.GetDirectoryName(vsixOrManifest);
                if (!String.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);

                    using (var partStream = vsixPart.GetStream())
                    {
                        using (var fileStream = File.Open(vsixOrManifest, FileMode.Create))
                        {
                            CopyStream(partStream, fileStream);
                        }
                    }
                }
            }

            return CreateExtension(vsixOrManifest);
        }

        /// <summary>
        /// Reads the manifest identifier.
        /// </summary>
        public static string ReadManifestIdentifier(string manifest)
        {
            using (var reader = XmlReader.Create(manifest, new XmlReaderSettings { IgnoreWhitespace = true }))
            {
                reader.MoveToContent();
                if (reader.ReadToDescendant("Identifier", "http://schemas.microsoft.com/developer/vsx-schema/2010"))
                {
                    return reader.GetAttribute("Id");
                }
            }

            return null;
        }

        private static IExtension CreateExtension(string vsixOrManifest)
        {
            //// \o/ TODO: we couldn't find a public API for reading the manifest outside of VS.
            dynamic manager = Activator.CreateInstance(GetExtensionManagerType(), FakeSettings);

            return new VsExtension(manager.CreateExtension(vsixOrManifest));
        }

        private static Type GetExtensionManagerType()
        {
#if VSVER11
            var version = @"11.0";
#endif
#if VSVER10
            var version = @"10.0";
#endif
            var type = string.Format(ExtensionManagerServiceTypeFormat, version);
            return Type.GetType(type, true);
        }

        private static string BuildTargetFileName(string baseDir, Uri partUri)
        {
            var targetFile = partUri.OriginalString.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (targetFile.StartsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
            {
                targetFile = targetFile.Substring(1);
            }

            return Path.Combine(baseDir, targetFile);
        }

        private static void CopyStream(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            long bufferSize = inputStream.Length < BufferSize ? inputStream.Length : BufferSize;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
        }

        /// <summary>
        /// Satisfies the ExtensionManagerService ctor minimal requirements.
        /// </summary>
        private class FakeSettingsManager : Microsoft.VisualStudio.Settings.SettingsManager
        {
            /// <summary>
            /// Gets the application data folder.
            /// </summary>
            /// <param name="folder">The folder.</param>
            /// <returns>The application data folder.</returns>
            public override string GetApplicationDataFolder(ApplicationDataFolder folder)
            {
                return Path.GetTempPath();
            }

            /// <summary>
            /// Gets the collection scopes.
            /// </summary>
            /// <param name="collectionPath">The collection path.</param>
            /// <returns>The collection scopes.</returns>
            public override EnclosingScopes GetCollectionScopes(string collectionPath)
            {
                return EnclosingScopes.None;
            }

            /// <summary>
            /// Gets the common extensions search paths.
            /// </summary>
            /// <returns>The common extensions search paths.</returns>
            public override IEnumerable<string> GetCommonExtensionsSearchPaths()
            {
                yield return Path.GetTempPath();
            }

            /// <summary>
            /// Gets the property scopes.
            /// </summary>
            /// <param name="collectionPath">The collection path.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <returns>The property scopes.</returns>
            public override EnclosingScopes GetPropertyScopes(string collectionPath, string propertyName)
            {
                return EnclosingScopes.None;
            }

            /// <summary>
            /// Gets the read only settings state.
            /// </summary>
            /// <param name="scope">The scope.</param>
            /// <returns>The read only settings state.</returns>
            public override SettingsStore GetReadOnlySettingsStore(SettingsScope scope)
            {
                return new FakeSettingsStore();
            }

            /// <summary>
            /// Gets the writable settings state.
            /// </summary>
            /// <param name="scope">The scope.</param>
            /// <returns>The writable settings state.</returns>
            public override WritableSettingsStore GetWritableSettingsStore(SettingsScope scope)
            {
                return new FakeWritableSettingsStore();
            }

            /// <summary>
            /// Represents a fake settings state.
            /// </summary>
            private class FakeSettingsStore : SettingsStore
            {
                /// <summary>
                /// Check if the collection exists.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <returns>A value indicating whether the collection exists.</returns>
                public override bool CollectionExists(string collectionPath)
                {
                    return false;
                }

                /// <summary>
                /// Gets the <see cref="bool"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <returns>The <see cref="bool"/> representing the property value.</returns>
                public override bool GetBoolean(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="bool"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <param name="defaultValue">If set to <c>true</c> [default value].</param>
                /// <returns>The <see cref="bool"/> representing the property value.</returns>
                public override bool GetBoolean(string collectionPath, string propertyName, bool defaultValue)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="int"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <returns>The <see cref="int"/> representing the property value.</returns>
                public override int GetInt32(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="int"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <param name="defaultValue">The default value.</param>
                /// <returns>The <see cref="int"/> representing the property value.</returns>
                public override int GetInt32(string collectionPath, string propertyName, int defaultValue)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="long"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <returns>The <see cref="long"/> representing the property value.</returns>
                public override long GetInt64(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="long"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <param name="defaultValue">The default value.</param>
                /// <returns>The <see cref="long"/> representing the property value.</returns>
                public override long GetInt64(string collectionPath, string propertyName, long defaultValue)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the last write time.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <returns>The last write time.</returns>
                public override DateTime GetLastWriteTime(string collectionPath)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the memory stream.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <returns>The memory stream.</returns>
                public override MemoryStream GetMemoryStream(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the property count.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <returns>The property count.</returns>
                public override int GetPropertyCount(string collectionPath)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the property names.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <returns>The property names.</returns>
                public override IEnumerable<string> GetPropertyNames(string collectionPath)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the type of the property.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <returns>The type of the property.</returns>
                public override SettingsType GetPropertyType(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="string"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <returns>The <see cref="string"/> representing the property value.</returns>
                public override string GetString(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="string"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <param name="defaultValue">The default value.</param>
                /// <returns>The <see cref="string"/> representing the property value.</returns>
                public override string GetString(string collectionPath, string propertyName, string defaultValue)
                {
                    return defaultValue;
                }

                /// <summary>
                /// Gets the sub collection count.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <returns>The sub collection count.</returns>
                public override int GetSubCollectionCount(string collectionPath)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the sub collection property names.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <returns>The sub collection property names.</returns>
                public override IEnumerable<string> GetSubCollectionNames(string collectionPath)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="uint"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <returns>The <see cref="uint"/> representing the property value.</returns>
                public override uint GetUInt32(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="uint"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <param name="defaultValue">The default value.</param>
                /// <returns>The <see cref="uint"/> representing the property value.</returns>
                public override uint GetUInt32(string collectionPath, string propertyName, uint defaultValue)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="ulong"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <returns>The <see cref="ulong"/> representing the property value.</returns>
                public override ulong GetUInt64(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Gets the <see cref="ulong"/> from the <paramref name="collectionPath"/> using <paramref name="propertyName"/>.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <param name="defaultValue">The default value.</param>
                /// <returns>The <see cref="ulong"/> representing the property value.</returns>
                public override ulong GetUInt64(string collectionPath, string propertyName, ulong defaultValue)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Indicates whether a property exists.
                /// </summary>
                /// <param name="collectionPath">The collection path.</param>
                /// <param name="propertyName">Name of the property.</param>
                /// <returns>Returns <c>true</c> if the property exists; otherwise <c>false</c>.</returns>
                public override bool PropertyExists(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Represents a fake writable settings store.
            /// </summary>
            private class FakeWritableSettingsStore : WritableSettingsStore
            {
                public override void CreateCollection(string collectionPath)
                {
                }

                public override bool DeleteCollection(string collectionPath)
                {
                    return true;
                }

                public override bool DeleteProperty(string collectionPath, string propertyName)
                {
                    return true;
                }

                public override void SetBoolean(string collectionPath, string propertyName, bool value)
                {
                }

                public override void SetInt32(string collectionPath, string propertyName, int value)
                {
                }

                public override void SetInt64(string collectionPath, string propertyName, long value)
                {
                }

                public override void SetMemoryStream(string collectionPath, string propertyName, MemoryStream value)
                {
                }

                public override void SetString(string collectionPath, string propertyName, string value)
                {
                }

                public override void SetUInt32(string collectionPath, string propertyName, uint value)
                {
                }

                public override void SetUInt64(string collectionPath, string propertyName, ulong value)
                {
                }

                public override bool CollectionExists(string collectionPath)
                {
                    return false;
                }

                public override bool GetBoolean(string collectionPath, string propertyName, bool defaultValue)
                {
                    return defaultValue;
                }

                public override bool GetBoolean(string collectionPath, string propertyName)
                {
                    return false;
                }

                public override int GetInt32(string collectionPath, string propertyName, int defaultValue)
                {
                    return defaultValue;
                }

                public override int GetInt32(string collectionPath, string propertyName)
                {
                    return int.MinValue;
                }

                public override long GetInt64(string collectionPath, string propertyName, long defaultValue)
                {
                    return defaultValue;
                }

                public override long GetInt64(string collectionPath, string propertyName)
                {
                    return long.MinValue;
                }

                public override DateTime GetLastWriteTime(string collectionPath)
                {
                    return DateTime.Now;
                }

                public override MemoryStream GetMemoryStream(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                public override int GetPropertyCount(string collectionPath)
                {
                    return 0;
                }

                public override IEnumerable<string> GetPropertyNames(string collectionPath)
                {
                    throw new NotImplementedException();
                }

                public override SettingsType GetPropertyType(string collectionPath, string propertyName)
                {
                    throw new NotImplementedException();
                }

                public override string GetString(string collectionPath, string propertyName, string defaultValue)
                {
                    return defaultValue;
                }

                public override string GetString(string collectionPath, string propertyName)
                {
                    return string.Empty;
                }

                public override int GetSubCollectionCount(string collectionPath)
                {
                    throw new NotImplementedException();
                }

                public override IEnumerable<string> GetSubCollectionNames(string collectionPath)
                {
                    throw new NotImplementedException();
                }

                public override uint GetUInt32(string collectionPath, string propertyName, uint defaultValue)
                {
                    return defaultValue;
                }

                public override uint GetUInt32(string collectionPath, string propertyName)
                {
                    return uint.MinValue;
                }

                public override ulong GetUInt64(string collectionPath, string propertyName, ulong defaultValue)
                {
                    return defaultValue;
                }

                public override ulong GetUInt64(string collectionPath, string propertyName)
                {
                    return ulong.MinValue;
                }

                public override bool PropertyExists(string collectionPath, string propertyName)
                {
                    return true;
                }
            }
        }
    }
}
