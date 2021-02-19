using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuthTokenRetrieverXamarin
{
    public partial class App : Application
    {
        public App(MainPage mainPage)
        {
            InitializeComponent();

            MainPage = new NavigationPage(mainPage);
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
