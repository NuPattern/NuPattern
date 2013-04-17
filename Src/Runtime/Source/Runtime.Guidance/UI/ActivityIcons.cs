using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal static class ActivityIcons
    {
        static ActivityIcons()
        {
            ActionIcon = GetImageSource("ActionNode.png");
            InitialIcon = GetImageSource("InitialNode.png");
            FinalIcon = GetImageSource("FinalNode.png");
            DecisionIcon = GetImageSource("DecisionNode.png");
            MergeIcon = GetImageSource("MergeNode.png");
            ForkIcon = GetImageSource("ForkNode.png");
            JoinIcon = GetImageSource("JoinNode.png");
        }

        //public static System.Collections.Generic.IEnumerable<Tuple<string,ImageSource>> AllIcons
        //{
        //    get
        //    {
        //        yield return Tuple.Create("ActionIcon", ActionIcon) ;
        //        yield return Tuple.Create("InitialIcon", InitialIcon) ;
        //        yield return Tuple.Create("FinalIcon", FinalIcon)  ;
        //        yield return Tuple.Create("DecisionIcon", DecisionIcon);
        //        yield return Tuple.Create("MergeIcon", MergeIcon)  ;
        //        yield return Tuple.Create("ForkIcon", ForkIcon) ;
        //        yield return Tuple.Create("JoinIcon", JoinIcon) ;
        //    }
        //}

        public static ImageSource ActionIcon { get; private set; }
        public static ImageSource InitialIcon { get; private set; }
        public static ImageSource FinalIcon { get; private set; }
        public static ImageSource DecisionIcon { get; private set; }
        public static ImageSource MergeIcon { get; private set; }
        public static ImageSource ForkIcon { get; private set; }
        public static ImageSource JoinIcon { get; private set; }


        const string prefix = "Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.ProcessGuidance.UI.Icons.";

        private static ImageSource GetImageSource(string resourceName)
        {
            try
            {
                var stream = typeof(ActivityIcons).Assembly.GetManifestResourceStream(prefix + resourceName);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();

                return image;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}