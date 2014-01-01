using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;
using CrouMetro.Core.Tools;
using Microsoft.Phone.Controls;

namespace CrouMetro.Views
{
    public partial class ConnectPage : PhoneApplicationPage
    {
        public ConnectPage()
        {
            InitializeComponent();

            // Clear Cookie to remove current logged in user data
            //mWebBrowser.ClearCookiesAsync();
            // Go to Login url
            mWebBrowser.Navigate(
                new Uri("https://api.croudia.com/oauth/authorize?response_type=code&client_id=" + Constants.CONSUMER_KEY),
                null,
                "User-Agent: Mozilla/5.0 (Linux; U; Android 4.1.1; he-il; Nexus 7 Build/JRO03D) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Safari/534.30");
        }

        private string code { get; set; }

        private async void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            String uri = e.Uri.ToString();

            if (uri.StartsWith("https://twitter.com/innerlogic?code"))
            {
                // Remove junk text added by facebook from url
                if (uri.EndsWith("#_=_"))
                    uri = uri.Substring(0, uri.Length - 4);

                String queryString = e.Uri.Query;

                // Acquire the code from Query String
                IEnumerable<KeyValuePair<string, string>> pairs = queryString.ParseQueryString();
                code = pairs.GetValue("code");
                await Auth.RequestAccessToken(code);
                var userAccountEntity = new UserAccountEntity();
                await Auth.VerifyAccount(userAccountEntity);
                App.userAccountEntity = userAccountEntity;
                // Back to MainPage
                var rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (rootFrame != null)
                    rootFrame.Navigate(new Uri("/MainTimelinePivot.xaml", UriKind.Relative));
            }
        }
    }
}