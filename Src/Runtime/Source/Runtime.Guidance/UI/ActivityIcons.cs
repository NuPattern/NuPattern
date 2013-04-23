using System.Globalization;
using System.Windows.Media;

namespace NuPattern.Runtime.Guidance.UI
{
    internal static class ActivityIcons
    {
        static ActivityIcons()
        {
            ActionIcon = GetImageSource(@"Resources/NodeGuidanceAction.png");
            InitialIcon = GetImageSource(@"Resources/NodeGuidanceInitial.png");
            FinalIcon = GetImageSource(@"Resources/NodeGuidanceFinal.png");
            DecisionIcon = GetImageSource(@"Resources/NodeGuidanceDecision.png");
            MergeIcon = GetImageSource(@"Resources/NodeGuidanceMerge.png");
            ForkIcon = GetImageSource(@"Resources/NodeGuidanceFork.png");
            JoinIcon = GetImageSource(@"Resources/NodeGuidanceJoin.png");
        }

        public static ImageSource ActionIcon { get; private set; }
        public static ImageSource InitialIcon { get; private set; }
        public static ImageSource FinalIcon { get; private set; }
        public static ImageSource DecisionIcon { get; private set; }
        public static ImageSource MergeIcon { get; private set; }
        public static ImageSource ForkIcon { get; private set; }
        public static ImageSource JoinIcon { get; private set; }

        private static ImageSource GetImageSource(string resourceName)
        {
            var assemblyName = typeof(ActivityIcons).Assembly.GetName().Name;
            var packUri = string.Format(CultureInfo.InvariantCulture,
                @"pack://application:,,,/{0};component/{1}", assemblyName, resourceName);

            return new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }
    }
}