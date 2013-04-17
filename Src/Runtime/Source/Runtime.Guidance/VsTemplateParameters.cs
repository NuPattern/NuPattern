using System;
using System.Collections.Generic;
using NuPattern;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Exposes strong-typed accessors for the built-in template parameters 
    /// explained in at msdn.microsoft.com/en-us/library/eehb4faa.aspx.
    /// </summary>
    internal class VsTemplateParameters
    {
        public const string TimeKey = "$time$";
        public const string YearKey = "$year$";
        public const string UserNameKey = "$username$";
        public const string UserDomainKey = "$userdomain$";
        public const string MachineNameKey = "$machinename$";
        public const string ClrVersionKey = "$clrversion$";
        public const string RegisteredOrganizationKey = "$registeredorganization$";
        public const string RunSilentKey = "$runsilent$";
        public const string TargetFrameworkVersionKey = "$targetframeworkversion$";
        public const string ProjectNameKey = "$projectname$";
        public const string SafeProjectNameKey = "$safeprojectname$";
        public const string InstallPathKey = "$installpath$";
        public const string ExclusiveProjectKey = "$exclusiveproject$";
        public const string DestinationDirectoryKey = "$destinationdirectory$";

        private IDictionary<string, string> replacementsDictionary;

        public VsTemplateParameters(IDictionary<string, string> replacementsDictionary)
        {
            Guard.NotNull(() => replacementsDictionary, replacementsDictionary);

            this.replacementsDictionary = replacementsDictionary;
        }

        private static readonly System.Globalization.CultureInfo en_US_Culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");



        public DateTime? Time
        {
            get
            {
                return GetValue<DateTime?>(TimeKey, value =>
                {
                    DateTime result;
                    var success = DateTime.TryParse(value, en_US_Culture, System.Globalization.DateTimeStyles.None, out result);
                    return success ? result : default(DateTime?);
                });
            }
        }
        public int? Year { get { return GetValue<int?>(YearKey, s => int.Parse(s)); } }
        public Version TargetFrameworkVersion { get { return GetValue<Version>(TargetFrameworkVersionKey, s => Version.Parse(s)); } }
        public bool? ExclusiveProject
        {
            get
            {
                return GetValue<bool?>(ExclusiveProjectKey, s => bool.Parse(s));
            }
            set
            {
                if (value.HasValue)
                    replacementsDictionary[ExclusiveProjectKey] = Convert.ToString(value.Value);
                else
                    replacementsDictionary.Remove(ExclusiveProjectKey);
            }
        }

        public string UserName { get { return GetValue(UserNameKey); } }
        public string UserDomain { get { return GetValue(UserDomainKey); } }
        public string MachineName { get { return GetValue(MachineNameKey); } }
        public string ClrVersion { get { return GetValue(ClrVersionKey); } }
        public string RegisteredOrganization { get { return GetValue(RegisteredOrganizationKey); } }
        public string RunSilent { get { return GetValue(RunSilentKey); } }
        public string ProjectName { get { return GetValue(ProjectNameKey); } }
        public string SafeProjectName { get { return GetValue(SafeProjectNameKey); } }
        public string InstallPath { get { return GetValue(InstallPathKey); } }
        public string DestinationDirectory { get { return GetValue(DestinationDirectoryKey); } }

        private T GetValue<T>(string key, Func<string, T> parse)
        {
            var value = default(T);
            string temp;
            if (replacementsDictionary.TryGetValue(key, out temp))
                return parse(temp);
            else
                return value;
        }

        private string GetValue(string key)
        {
            return GetValue<string>(key, s => s);
        }
    }
}
