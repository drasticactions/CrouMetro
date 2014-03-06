using System;
using System.Windows.Input;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace CrouMetro.Views
{
    public partial class PostPage : PhoneApplicationPage
    {
        public PostPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel.SelectedPost;
            var replyButton = (ApplicationBarIconButton) ApplicationBar.Buttons[0];
            var favoriteButton = (ApplicationBarIconButton) ApplicationBar.Buttons[1];
            var spreadButton = (ApplicationBarIconButton) ApplicationBar.Buttons[2];
            var removeButton = (ApplicationBarIconButton) ApplicationBar.Buttons[3];
            replyButton.IsEnabled = true;
            removeButton.IsEnabled = App.ViewModel.SelectedPost.IsCreator;
            favoriteButton.IsEnabled = App.ViewModel.SelectedPost.CanBeFavorited;
            spreadButton.IsEnabled = App.ViewModel.SelectedPost.CanBeSpread;
        }

        public PostEntity Post { get; set; }

        private void Reply_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(
                new Uri(
                    "/Views/MessagePost.xaml?statusId=" + App.ViewModel.SelectedPost.StatusID + " &screenName=@" +
                    App.ViewModel.SelectedPost.User.ScreenName, UriKind.Relative));
        }

        private async void Favorite_Click(object sender, EventArgs e)
        {
            await FavoriteManager.CreateFavorite(App.ViewModel.SelectedPost.StatusID, false, App.userAccountEntity);
            var favoriteButton = (ApplicationBarIconButton) ApplicationBar.Buttons[1];
            favoriteButton.IsEnabled = false;
        }

        private async void Spread_Click(object sender, EventArgs e)
        {
            await SpreadManager.CreateSpread(App.ViewModel.SelectedPost.StatusID, false, App.userAccountEntity);
            var spreadButton = (ApplicationBarIconButton) ApplicationBar.Buttons[2];
            spreadButton.IsEnabled = false;
        }

        private void UserInformationButton_Click(object sender, EventArgs e)
        {

            NavigationService.Navigate(new Uri("/Views/UserPage.xaml?screenName=" + App.ViewModel.SelectedPost.User.ScreenName, UriKind.Relative));
        }

        private async void RemoveButton_Click(object sender, EventArgs e)
        {
            await StatusManager.DestroyStatus(App.ViewModel.SelectedPost.StatusID, App.userAccountEntity);
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void ViewConversationButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ConversationView.xaml", UriKind.Relative));
        }

        private void UserInformation_Tap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/UserPage.xaml?screenName=" + App.ViewModel.SelectedPost.User.ScreenName, UriKind.Relative));
        }
    }
}