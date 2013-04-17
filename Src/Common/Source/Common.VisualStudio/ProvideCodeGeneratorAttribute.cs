using System;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.Shell;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio
{
    /// <summary>
    /// Attribute class to provide registration of a code generator
    /// </summary>
    /// <remarks>
    /// Should be applied to the package class itself, not the generator.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ProvideCodeGeneratorAttribute : RegistrationAttribute
    {
        /// <summary />
        public ProvideCodeGeneratorAttribute(
            string projectSystem,
            Type type,
            string name,
            string description,
            bool generatesDesignTimeSource)
        {
            Guard.NotNull(() => type, type);
            Guard.NotNullOrEmpty(() => name, name);

            this.ProjectSystem = projectSystem;
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.GeneratesDesignTimeSource = generatesDesignTimeSource;
        }

        /// <summary>
        /// Gets a human readable description of this generator
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets whether to flag this code generator as providing design-time source code
        /// </summary>
        public bool GeneratesDesignTimeSource { get; private set; }

        /// <summary>
        /// Gets the name of this generator
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the project system that this code generator is registered with
        /// </summary>
        public string ProjectSystem { get; set; }

        /// <summary>
        /// Gets the type implementing the Code Generator
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Register this generator
        /// </summary>
        /// <param name="context"></param>
        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            Guard.NotNull(() => context, context);

            using (RegistrationAttribute.Key generatorsKey = context.CreateKey("Generators"))
            {
                using (RegistrationAttribute.Key projectSystemKey = generatorsKey.CreateSubkey(this.ProjectSystem))
                using (RegistrationAttribute.Key generatorKey = projectSystemKey.CreateSubkey(this.Name))
                {
                    generatorKey.SetValue("", this.Description);
                    generatorKey.SetValue("CLSID", "{" + this.Type.GUID + "}");
                    generatorKey.SetValue("GeneratesDesignTimeSource", Convert.ToInt32(this.GeneratesDesignTimeSource));
                }

                using (RegistrationAttribute.Key clsIdKey = context.CreateKey("CLSID"))
                using (RegistrationAttribute.Key registrationKey = clsIdKey.CreateSubkey("{" + this.Type.GUID + "}"))
                {
                    registrationKey.SetValue("", this.Description);
                    registrationKey.SetValue("Class", this.Type.FullName);
                    registrationKey.SetValue("InprocServer32", context.InprocServerPath);
                    registrationKey.SetValue("ThreadingModel", "Both");
                    if (context.RegistrationMethod == RegistrationMethod.CodeBase)
                    {
                        var fileName = Path.GetFileName(this.Type.Assembly.CodeBase);
                        registrationKey.SetValue("CodeBase", Path.Combine(context.ComponentPath, fileName));
                    }
                    else
                    {
                        registrationKey.SetValue("Assembly", this.Type.Assembly.FullName);
                    }
                }
            }

            context.Log.WriteLine(string.Format(
                CultureInfo.CurrentCulture,
                Resources.ProvideCodeGeneratorAttribute_RegisterLog,
                this.Name,
                this.Type.GUID));
        }

        /// <summary>
        /// Delete our specified keys
        /// </summary>
        /// <param name="context">The context that this registration attribute is being used in</param>
        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey(@"Generators\" + this.ProjectSystem + @"\" + this.Name);
            context.RemoveKey(@"CLSID\{" + this.Type.GUID + "}");
            context.Log.WriteLine(string.Format(
                CultureInfo.CurrentCulture,
                Resources.ProvideCodeGeneratorAttribute_UnregisterLog,
                this.Name,
                this.Type.GUID));
        }
    }
}