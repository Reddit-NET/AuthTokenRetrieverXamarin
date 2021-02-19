using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuthTokenRetrieverXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RedditAuthPage : ContentPage
    {
        private MainPage MainPage { get; set; }

        public RedditAuthPage(MainPage mainPage)
        {
            InitializeComponent();

            MainPage = mainPage;
            MainPage.AuthTokenRetrieverLib.AwaitCallback();
            MainPage.CloseAuthPage += C_AuthClose;

            BrowserWindow.Source = MainPage.AuthTokenRetrieverLib.AuthURL();
            BrowserWindow.Navigated += C_NavigatedUpdated;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        public void C_NavigatedUpdated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Url.TrimEnd('/').EndsWith("reddit.com"))
            {
                BrowserWindow.Source = MainPage.AuthTokenRetrieverLib.AuthURL();
            }
        }

        public void C_AuthClose(object sender, object e)
        {
            MainPage.CloseAuthPage -= C_AuthClose;
            Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
        }
    }
}