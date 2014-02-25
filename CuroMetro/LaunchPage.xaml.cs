using System;
using System.IO.IsolatedStorage;
using System.Windows.Navigation;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;
using Microsoft.Phone.Controls;

namespace CrouMetro
{
    public partial class LaunchPage : PhoneApplicationPage
    {
        private static IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        public LaunchPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string authCode;
            /*
             * If we're coming from IE with our AuthenticationManager Code, parse it out and get the Access Token.
             * Else check if we already have it.
             * */
            if (NavigationContext.QueryString.TryGetValue("code", out authCode))
            {
                App.userAccountEntity = new UserAccountEntity();
                await AuthenticationManager.RequestAccessToken(authCode);
                LoginTest();
            }
            else
            {
                LoginTest();
            }
        }

        private async void LoginTest()
        {
            //bool LoginTest = await CroudiaAuthManager.RefreshAccessToken();
            App.userAccountEntity = new UserAccountEntity();
            bool LoginTest = await AuthenticationManager.RefreshAccessToken(App.userAccountEntity);
            if (LoginTest)
            {
                await AuthenticationManager.VerifyAccount(App.userAccountEntity);
                NavigationService.Navigate(new Uri("/MainTimelinePivot.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
            }
        }
    }
}