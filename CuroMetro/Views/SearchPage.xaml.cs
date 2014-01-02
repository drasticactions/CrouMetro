using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;
using CrouMetro.Core.Tools;
using Microsoft.Phone.Controls;

namespace CrouMetro.Views
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private int _offsetKnob = 15;

        public SearchPage()
        {
            InitializeComponent();
        }

        public InfiniteScrollingCollection searchVoiceCollection { get; set; }

        public InfiniteScrollingUserCollection searchUserCollection { get; set; }

        public async Task<bool> BindToSearchTimeline(UserAccountEntity userAccountEntity)
        {
            progressBar.Visibility = Visibility.Visible;
            searchVoiceCollection = new InfiniteScrollingCollection();
            searchVoiceCollection.timeline = EndPoints.USER_TIMELINE;
            searchVoiceCollection.PostCollection = new ObservableCollection<PostEntity>();
            searchVoiceCollection.userAccountEntity = userAccountEntity;
            //searchVoiceCollection.userName = App.ViewModel.SelectedUser.ScreenName;
            //searchVoiceCollection.UserId = App.ViewModel.SelectedUser.UserID;
            searchVoiceCollection.Query = SearchBox.Text;
            SearchEntity searchEntity =
                await SearchManager.SearchStatusList(SearchBox.Text, null, null, null, false, true, userAccountEntity);
            List<PostEntity> items = searchEntity.PostList;
            foreach (PostEntity item in items)
            {
                searchVoiceCollection.PostCollection.Add(item);
            }
            searchVoiceCollection.MaxStatusId = items.Last().StatusID;
            voiceList.DataContext = searchVoiceCollection;
            voiceList.ItemRealized += voiceList_ItemRealized;
            progressBar.Visibility = Visibility.Collapsed;
            return true;
        }

        public async Task<bool> BindToUserTimeline(UserAccountEntity userAccountEntity)
        {
            progressBar.Visibility = Visibility.Visible;
            searchUserCollection = new InfiniteScrollingUserCollection();
            searchUserCollection.timeline = "Following";
            //searchUserCollection.userId = App.ViewModel.SelectedUser.UserID;
            searchUserCollection.Offset = 0;
            searchUserCollection.userAccountEntity = userAccountEntity;
            searchUserCollection.UserCollection = new ObservableCollection<UserEntity>();
            List<UserEntity> items =
                await SearchManager.SearchUserList(SearchUserBox.Text, null, null, false, userAccountEntity);
            foreach (UserEntity item in items)
            {
                searchUserCollection.UserCollection.Add(item);
            }
            searchUserCollection.Offset = 1;
            userList.DataContext = searchUserCollection;
            //userList.ItemRealized += followingTimeline_ItemRealized;
            progressBar.Visibility = Visibility.Collapsed;
            return true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void SearchVoiceButton_Click(object sender, RoutedEventArgs e)
        {
            await BindToSearchTimeline(App.userAccountEntity);
        }

        private void voiceList_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (searchVoiceCollection.IsLoading || voiceList.ItemsSource == null ||
                voiceList.ItemsSource.Count < _offsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var postEntity = e.Container.Content as PostEntity;
            if (
                postEntity != null && postEntity.Equals(
                    voiceList.ItemsSource[voiceList.ItemsSource.Count - _offsetKnob]))
            {
                //progressBar.Visibility = System.Windows.Visibility.Visible;
                searchVoiceCollection.LoadSearchResults();
                //progressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private async void SearchUserButton_Click(object sender, RoutedEventArgs e)
        {
            await BindToUserTimeline(App.userAccountEntity);
        }

        private void userList_Tap(object sender, GestureEventArgs e)
        {
            var user = ((FrameworkElement) e.OriginalSource).DataContext as UserEntity;
            if (user == null) return;
            App.ViewModel.SelectedUser = user;
            NavigationService.Navigate(new Uri("/Views/UserPage.xaml", UriKind.Relative));
        }

        private void voiceList_Tap(object sender, GestureEventArgs e)
        {
            var post = ((FrameworkElement) e.OriginalSource).DataContext as PostEntity;
            if (post == null) return;
            App.ViewModel.SelectedPost = post;
            NavigationService.Navigate(post.InReplyToUserID > 0
                ? new Uri("/Views/ConversationView.xaml", UriKind.Relative)
                : new Uri("/Views/PostPage.xaml", UriKind.Relative));
        }
    }
}