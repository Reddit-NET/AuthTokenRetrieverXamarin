using Reddit;
using Reddit.AuthTokenRetriever;
using Reddit.AuthTokenRetriever.EventArgs;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace AuthTokenRetrieverXamarin
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private const string AppID = "YourAppIdGoesHere";
        private const int AuthListenPort = 50080;
        private const string AuthListenHost = "127.0.0.1";

        private string AccessToken { get; set; }
        private string RefreshToken { get; set; }

        public AuthTokenRetrieverLib AuthTokenRetrieverLib
        {
            get
            {
                if (authTokenRetrieverLib == null)
                {
                    authTokenRetrieverLib = new AuthTokenRetrieverLib(AppID, AuthListenPort, AuthListenHost);
                }
                return authTokenRetrieverLib;
            }
            set => authTokenRetrieverLib = value;
        }
        private AuthTokenRetrieverLib authTokenRetrieverLib;

        private RedditClient Reddit
        {
            get
            {
                if (reddit == null && !string.IsNullOrWhiteSpace(RefreshToken))
                {
                    reddit = new RedditClient(appId: AppID, refreshToken: RefreshToken, accessToken: AccessToken);
                }
                return reddit;
            }
            set => reddit = value;
        }
        private RedditClient reddit;

        public event EventHandler CloseAuthPage;

        public bool RedditAuthenticated
        {
            get => (Reddit != null && Reddit.Account != null && Reddit.Account.Me != null && Reddit.Account.Me.Name != null
                    && !string.IsNullOrWhiteSpace(RefreshToken));
            private set { }
        }

        public MainPage()
        {
            InitializeComponent();

            AuthTokenRetrieverLib.AuthSuccess += C_AuthSuccess;
            Navigation.PushAsync(new RedditAuthPage(this));
        }

        private void Validate()
        {
            if (!RedditAuthenticated)
            {
                throw new Exception("Reddit authentication failed.");
            }
        }

        public void C_AuthSuccess(object sender, AuthSuccessEventArgs e)
        {
            RefreshToken = e.RefreshToken;
            AccessToken = e.AccessToken;

            Validate();

            AuthTokenRetrieverLib.AuthSuccess -= C_AuthSuccess;
            AuthTokenRetrieverLib.StopListening();

            CloseAuthPage?.Invoke(this, null);
            Device.BeginInvokeOnMainThread(() => PopulateData());
        }

        private void PopulateData()
        {
            Validate();

            StackLayout_Main.Children.Add(new Label
            {
                Text = "u/" + Reddit.Account.Me.Name,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Start
            });

            StackLayout_Main.Children.Add(new Label
            {
                Text = "Cake Day: " + Reddit.Account.Me.Created.ToString("D"),
                FontSize = Device.GetNamedSize(NamedSize.Subtitle, typeof(Label)),
                VerticalOptions = LayoutOptions.Start
            });

            StackLayout_Main.Children.Add(new Label
            {
                Text = "Most Recent Post: " + Reddit.Account.Me.GetPostHistory(sort: "newForced")[0].Title,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                VerticalOptions = LayoutOptions.Center
            });
        }
    }
}
