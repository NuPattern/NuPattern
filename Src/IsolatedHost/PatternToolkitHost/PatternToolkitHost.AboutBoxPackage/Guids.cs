// Guids.cs
// MUST match guids.h
using System;

namespace PatternToolkitHost.AboutBoxPackage
{
    static class GuidList
    {
        public const string guidAboutBoxPackagePkgString = "b4f95cca-f646-40f9-b79d-012eed2b963b";
        public const string guidAboutBoxPackageCmdSetString = "471f9d31-98fe-4bf7-be01-571d2003aa84";

        public static readonly Guid guidAboutBoxPackageCmdSet = new Guid(guidAboutBoxPackageCmdSetString);
    };
}