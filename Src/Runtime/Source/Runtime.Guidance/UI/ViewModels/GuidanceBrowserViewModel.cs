using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.UI.ViewModels;
using NuPattern;
using NuPattern.Presentation;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal class GuidanceBrowserViewModel : ViewModel
    {
        private GuidanceBrowserContext context;
        private IServiceProvider serviceProvider;
        private IFeatureManager featureManager;
        private IUriReferenceService uriReferenceService;
        private INode node;
        private IFeatureExtension featureExtension;
        private Uri link;
        private string errorMessage;
        private bool hasErrorMessage;
        private bool hasLink;

        public GuidanceBrowserViewModel(GuidanceBrowserContext context, IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.context = context;
            this.serviceProvider = serviceProvider;

            this.NavigationCommand = new RelayCommand<Uri>(uri => this.Navigate(uri));
            this.featureManager = context.FeatureManager;
            this.uriReferenceService = context.UriReferenceService;
        }

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            private set
            {
                if (this.errorMessage != value)
                {
                    this.errorMessage = value;
                    this.OnPropertyChanged(() => this.ErrorMessage);
                    this.HasErrorMessage = value != null;
                }
            }
        }

        public bool HasErrorMessage
        {
            get
            {
                return this.hasErrorMessage;
            }
            private set
            {
                if (this.hasErrorMessage != value)
                {
                    this.hasErrorMessage = value;
                    this.OnPropertyChanged(() => this.HasErrorMessage);
                }
            }
        }

        public bool HasLink
        {
            get
            {
                return this.hasLink;
            }
            set
            {
                if (this.hasLink != value)
                {
                    this.hasLink = value;
                    this.OnPropertyChanged(() => this.HasLink);
                }
            }
        }

        public ICommand NavigationCommand { get; private set; }

        public INode Node
        {
            get
            {
                return this.node;
            }
            set
            {
                if (this.node != value)
                {
                    this.node = value;
                    this.OnPropertyChanged(() => this.Node);
                    this.featureExtension = this.GetFeatureExtension();
                    this.Link = this.ResolveUri(value == null || value.Link == null ? null : new Uri(value.Link));
                }
            }
        }

        public Uri Link
        {
            get
            {
                return this.link;
            }
            set
            {
                if (this.link != value)
                {
                    this.link = value;
                    this.OnPropertyChanged(() => this.Link);
                    this.HasLink = value != null;
                }
            }
        }

        private IFeatureExtension GetFeatureExtension()
        {
            if (this.node == null)
            {
                return null;
            }

            var workflow = this.node.Predecessors.Traverse(n => n.Predecessors).OfType<IGuidanceWorkflow>().First();
            return this.featureManager.InstantiatedFeatures.First(fe => fe.GuidanceWorkflow == workflow);
        }

        private void Navigate(Uri uri)
        {
            if (uri == null)
                return;

            if (this.uriReferenceService.IsSchemeRegistered(uri))
            {
                // To avoid exceptions
                var launchPoint = this.TryResolveUri<object>(uri) as ILaunchPoint;
                if (launchPoint != null)
                {
                    if (launchPoint.CanExecute(featureExtension))
                    {
                        launchPoint.Execute(featureExtension);
                    }
                }
                else
                {
                    this.Link = this.TryResolveUri<Uri>(uri);
                    if (this.Link == null)
                    {
                        FeatureContent content = this.TryResolveUri<FeatureContent>(uri);
                        IGuidanceAction matchingNode = null;
                        matchingNode = FindNodeBasedOnContentLink(featureExtension, uri.ToString());
                        if (matchingNode != null)
                        {
                            featureExtension.GuidanceWorkflow.Focus(matchingNode);
                            GuidanceWorkflowViewModel.Current.GoToFocusedAction();
                        }
                        else
                            if (content != null)
                                this.Link = new Uri(content.Path);
                    }
                }
            }
            else
            {
                this.Link = uri;
            }
        }

        private IGuidanceAction FindNodeBasedOnContentLink(IFeatureExtension feature, string link)
        {
            IGuidanceAction focusedAction = feature.GuidanceWorkflow.Successors
                                .Traverse<INode>(s => s.Successors)
                                .OfType<IGuidanceAction>()
                                .FirstOrDefault(a => (a.Link == null ? "~~~" : a.Link.ToLower()) == link.ToLower());

            return focusedAction;
        }


        private Uri ResolveUri(Uri uri)
        {
            if (uri != null)
            {
                //
                // First see if it's a "content://" URI
                //
                var content = this.TryResolveUri<FeatureContent>(uri);
                if (content != null)
                    return new Uri(content.Path);

                //
                // Ok, not content, let's see if it's a "launchpoint://" URI
                //
                var launchPoint = this.TryResolveUri<ILaunchPoint>(uri);
                if (launchPoint != null)
                {
                    if (launchPoint.CanExecute(featureExtension))
                    {
                        launchPoint.Execute(featureExtension);
                        return (FeatureCallContext.Current.GuidanceBrowserControl.CurrentLink);
                    }
                }


                if (uri.OriginalString != null &&
                    uri.OriginalString.ToLower().StartsWith("http"))
                    return uri;
                else
                    return (null);
            }

            return uri;
        }

        private T TryResolveUri<T>(Uri uri) where T : class
        {
            try
            {
                this.ErrorMessage = null;
                return this.uriReferenceService.ResolveUri<T>(uri);
            }
            catch (FileNotFoundException e)
            {
                this.ErrorMessage = e.Message;
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }
    }
}
