using System;
using System.Windows;
using System.Windows.Navigation;
using CrouMetro.Core.Tools;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace CrouMetro
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var task = new WebBrowserTask
            {
                Uri =
                    new Uri("https://api.croudia.com/oauth/authorize?response_type=code&client_id=" +
                            Constants.CONSUMER_KEY)
            };
            task.Show();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
        }
    }
}