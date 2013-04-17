using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Guidance.Extensions
{
    /// <summary>
    /// BlackboardManager provides a single, global, key/string value pair API available to all
    /// FeatureExtension instances.
    /// Updated 2010-05-19 to not persist until IsPersistent is set to true
    /// <para/>
    /// Note:  It is *not* threadsafe (at least not yet) so do not WRITE to the Blackboard in background threads
    /// </summary>
    [Export]
    public class BlackboardManager
    {
        /// <summary>
        /// Local static to keep the singleton instance
        /// </summary>
        private static BlackboardManager current;

        /// <summary>
        /// Name of the solution folder in which to store the Blackboard
        /// </summary>
        private static string solutionFolderName = "Solution Items";

        /// <summary>
        /// Name of the file in which the Blackboard data is stored
        /// </summary>
        private static string defaultBlackboardFileNameBase = ".guidancestate";

        /// <summary>
        /// Empty dictionary contents with which we pre-populate the Blackboard on creation
        /// </summary>
        private static string emptyDictionary = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ArrayOfKeyValueOfstringstring xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\"></ArrayOfKeyValueOfstringstring>";

        /// <summary>
        /// The calculated absolute path to the blackboard file (for serialization)
        /// </summary>
        private string blackboardFileName = string.Empty;

        /// <summary>
        /// The live "in-memory" instance of the blackboard
        /// </summary>
        private Dictionary<string, string> blackboard = null;

        private bool isPersisted;

        /// <summary>
        /// Initializes a new instance of the BlackboardManager class.
        /// Used by MEF to instantiate BlackboardManager in Feature.cs
        /// </summary>
        public BlackboardManager()
        {
            current = this;
            isPersisted = false;
        }

        /// <summary>
        /// Gets the singleton instance of the Blackboard
        /// Static property returning singleton instance of BlackboardManager.
        /// Note:  We do it this way instead of accessing the instance of BlackboardManager
        /// saved in Feature.cs because commands can't directly reference the specific feature
        /// type since they are in a referred-to assembly.
        /// </summary>
        public static BlackboardManager Current
        {
            get { return current; }
        }

        /// <summary>
        /// Gets or sets an indicator of whether or not the data is
        /// stored on disk.
        /// </summary>
        public bool IsPersistent
        {
            get { return this.isPersisted; }
            set
            {
                this.isPersisted = value;
                if (value)
                {
                    // Note: processSolutionFolder both creates the FeatureExtensionData solution folder (if necessary)
                    // and returns the path to the Blackboard File
                    this.blackboardFileName = this.ProcessSolutionFolder(this.Solution);
                    this.WriteBlackboardDataToFile();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current Visual Studio solution
        /// </summary>
        [Import]
        private ISolution Solution { get; set; }

        /// <summary>
        /// Creates the FeatureExtensionData solution folder (if necessary) and
        /// reads in the current state of the Blackboard
        /// </summary>
        public void Initialize()
        {
            //
            // Make multiple calls to Initialize safe
            //
            if (this.blackboard == null)
                this.blackboard = new Dictionary<string, string>();

            //
            // If the file exists, we must read it on initialize
            //
            try
            {
                string bbFileName = Path.Combine(Path.GetDirectoryName(Solution.PhysicalPath), Solution.Name + defaultBlackboardFileNameBase);
                if (File.Exists(bbFileName))
                {
                    using (XmlReader reader = XmlReader.Create(bbFileName))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, string>));

                        this.blackboard = (Dictionary<string, string>)serializer.ReadObject(reader);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Reinitializes the Blackboard to an empty state.
        /// Called by the FeatureManager.Close method.
        /// </summary>
        public void Clear()
        {
            this.blackboard = new Dictionary<string, string>();
        }

        /// <summary>
        /// Retrieve a string from the global Blackboard
        /// </summary>
        /// <param name="key">Key to retrieve</param>
        /// <returns>Value for the specified key or null if key not present</returns>
        public string Get(string key)
        {
            this.blackboard = this.GetBlackboardDataFromFile();
            string value;
            this.blackboard.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Stores a new or updated value on the global Blackboard
        /// </summary>
        /// <param name="key">Key under which value should be saved</param>
        /// <param name="value">Data to store under the specified key</param>
        public void Set(string key, string value)
        {
            // Delete existing content for this key, if present
            // so we can "overwrite"
            if (this.blackboard.ContainsKey(key))
            {
                // Don't change it if it's the same value.
                // Makes it easier to leave the XML file open in the editor while
                // the workflow is running.
                if (this.blackboard[key].Equals(value))
                {
                    return;
                }
            }

            this.blackboard[key] = value;
            this.WriteBlackboardDataToFile();
        }

        /// <summary>
        /// Loads the Blackboard from the global XML file
        /// </summary>
        /// <returns>A &lt;string,string&gt; dictionary deserialized from the file</returns>
        private Dictionary<string, string> GetBlackboardDataFromFile()
        {
            Dictionary<string, string> result;

            if (!IsPersistent)
                return this.blackboard;

            //
            // Check it out if it's under source control
            //
            VsHelper.CheckOut(blackboardFileName);

            if (!File.Exists(this.blackboardFileName))
            {
                this.blackboard = new Dictionary<string, string>();
                this.WriteBlackboardDataToFile();
            }

            using (XmlReader reader = XmlReader.Create(this.blackboardFileName))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, string>));

                result = (Dictionary<string, string>)serializer.ReadObject(reader);
            }

            return result;
        }

        /// <summary>
        /// Saves the Blackboard to the global XML file
        /// </summary>
        private void WriteBlackboardDataToFile()
        {
            if (!IsPersistent)
                return;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(this.blackboardFileName, settings))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, string>));
                serializer.WriteObject(writer, this.blackboard);
            }
        }

        /// <summary>
        /// Create, if necessary, the solution folder for the Blackboard
        /// Adds an empty Blackboard data file when the folder is created
        /// </summary>
        /// <param name="solution">The current VS Solution abstraction from import</param>
        /// <returns>Path to Solution</returns>
        private string ProcessSolutionFolder(ISolution solution)
        {
            ISolutionFolder blackboardFolder = null;
            bool solutionFolderExists = false;

            foreach (var item in solution.Items)
            {
                if (item.Name == solutionFolderName)
                {
                    solutionFolderExists = true;
                    blackboardFolder = item as ISolutionFolder;
                    break;
                }
            }

            if (!solutionFolderExists)
            {
                blackboardFolder = solution.CreateSolutionFolder(solutionFolderName);
                blackboardFolder.AddContent(emptyDictionary, solution.Name + defaultBlackboardFileNameBase, false, false);
            }
            else
            {
                //
                // Ok, the solution folder exists, let's see if our file exists
                //
                string bbFileName = Path.Combine(Path.GetDirectoryName(solution.PhysicalPath), solution.Name + defaultBlackboardFileNameBase);
                if (!File.Exists(bbFileName))
                {
                    blackboardFolder.AddContent(emptyDictionary, solution.Name + defaultBlackboardFileNameBase, false, false);
                }
            }

            return Path.Combine(Path.GetDirectoryName(solution.PhysicalPath), solution.Name + defaultBlackboardFileNameBase);
        }
    }
}