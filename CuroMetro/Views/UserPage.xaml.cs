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
using Microsoft.Phone.Shell;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace CrouMetro.Views
{
    public partial class UserPage : PhoneApplicationPage
    {
        private const int OffsetKnob = 20;
        private const int PostOffsetKnob = 7;

        public UserPage()
        {
            InitializeComponent();
        }

        public static InfiniteScrollingCollection UserCollection { get; set; }

        public static InfiniteScrollingHtmlParseImageCollection PictureCollection { get; set; }

        public static InfiniteScrollingUserCollection UserFollowerCollection { get; set; }

        public static InfiniteScrollingUserCollection UserFollowingCollection { get; set; }

        private UserEntity _selectedUser;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            string screenName = string.Empty;
            string msg = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("screenName", out msg))
            {
                screenName = msg;
            }
            _selectedUser = await UserManager.ShowUser(screenName, null, App.userAccountEntity);
            SetButtonEnabled(_selectedUser);
            _selectedUser = await UserManager.ShowUser(_selectedUser.ScreenName, _selectedUser.UserID, App.userAccountEntity);
            DataContext = _selectedUser;
            if (_selectedUser.IsCurrentUser)
            {
                UserIsFollowing.Text = "ユーザーは君だよ！";
            }
            else
            {
                UserIsFollowing.Text = _selectedUser.IsFollowing
                    ? "@" + _selectedUser.ScreenName + "をフォローしています"
                    : "@" + _selectedUser.ScreenName + "をフォローしていません";
            }
             BindToUserTimeline(App.userAccountEntity);
             BindToUserFollowerGallery(App.userAccountEntity);
             BindToUserFollowingGallery(App.userAccountEntity);
             BindPictureGallery(App.userAccountEntity);
            LoadingProgressBar.Visibility = Visibility.Collapsed;

        }

        private void SetButtonEnabled(UserEntity selectedUser)
        {
            var followButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            var unfollowButton = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            var secretMailButton = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            followButton.IsEnabled = !selectedUser.IsFollowing &&
                                     selectedUser.IsNotCurrentUser;
            unfollowButton.IsEnabled = selectedUser.IsFollowing &&
                                       selectedUser.IsNotCurrentUser;
            secretMailButton.IsEnabled = selectedUser.IsNotCurrentUser &&
                                         selectedUser.IsFollowing;
        }

        public async void BindToUserTimeline(UserAccountEntity userAccountEntry)
        {
            UserCollection = new InfiniteScrollingCollection
            {
                timeline = EndPoints.USER_TIMELINE,
                PostCollection = new ObservableCollection<PostEntity>(),
                userAccountEntity = userAccountEntry,
                userName = _selectedUser.ScreenName,
                UserId = _selectedUser.UserID
            };
            List<PostEntity> items =
                await
                    TimelineManager.GetUserTimeline(_selectedUser.ScreenName,
                        _selectedUser.UserID, null, null, null, null, App.userAccountEntity);
            if (items == null) return;
            foreach (PostEntity item in items)
            {
                UserCollection.PostCollection.Add(item);
            }
            UserCollection.MaxStatusId = items.Last().StatusID;
            userTimeLine.DataContext = UserCollection;
            userTimeLine.ItemRealized += userTimeLine_ItemRealized;
        }

        public async void BindPictureGallery(UserAccountEntity userAccountEntity)
        {
            PictureCollection = new InfiniteScrollingHtmlParseImageCollection
            {
                timeline = "Pictures",
                userAccountEntity = userAccountEntity,
                Offset = 0,
                userName = _selectedUser.ScreenName,
                MediaCollection = new ObservableCollection<MediaEntity>()
            };
            List<MediaEntity> items =
                await AlbumManager.GetAlbumList(0, _selectedUser.ScreenName, userAccountEntity);
            if (items == null) return;
            foreach (MediaEntity item in items)
            {
                PictureCollection.MediaCollection.Add(item);
            }
            PictureCollection.Offset = 20;
            albumGallery.DataContext = PictureCollection;
            albumGallery.ItemRealized += albumGallery_ItemRealized;
        }

        public async void BindToUserFollowerGallery(UserAccountEntity userAccountEntity)
        {
            UserFollowerCollection = new InfiniteScrollingUserCollection
            {
                timeline = "Follower",
                userId = _selectedUser.UserID,
                Offset = -1,
                userAccountEntity = userAccountEntity,
                UserCollection = new ObservableCollection<UserEntity>()
            };
            List<UserEntity> items =
                await UserManager.LookupFollowerUsers(0, _selectedUser.UserID, userAccountEntity);
            if (items == null) return;
            foreach (UserEntity item in items)
            {
                UserFollowerCollection.UserCollection.Add(item);
            }
            UserFollowerCollection.Offset = 0;
            followerList.DataContext = UserFollowerCollection;
            followerList.ItemRealized += followerTimeline_ItemRealized;
        }

        public async void BindToUserFollowingGallery(UserAccountEntity userAccountEntity)
        {
            UserFollowingCollection = new InfiniteScrollingUserCollection
            {
                timeline = "Following",
                userId = _selectedUser.UserID,
                Offset = -1,
                userAccountEntity = userAccountEntity,
                UserCollection = new ObservableCollection<UserEntity>()
            };
            List<UserEntity> items =
                await UserManager.LookupFollowingUsers(0, _selectedUser.UserID, userAccountEntity);
            if (items == null) return;
            foreach (UserEntity item in items)
            {
                UserFollowingCollection.UserCollection.Add(item);
            }
            UserFollowingCollection.Offset = 0;
            followingList.DataContext = UserFollowingCollection;
            followingList.ItemRealized += followingTimeline_ItemRealized;
        }

        private void albumGallery_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (PictureCollection.IsLoading || albumGallery.ItemsSource == null ||
                albumGallery.ItemsSource.Count < OffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var mediaEntity = e.Container.Content as MediaEntity;
            if (
                mediaEntity != null && mediaEntity.Equals(
                    albumGallery.ItemsSource[albumGallery.ItemsSource.Count - OffsetKnob]))
            {
                PictureCollection.LoadAlbum();
            }
        }

        private void followerTimeline_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (UserFollowerCollection.IsLoading || followerList.ItemsSource == null ||
                followerList.ItemsSource.Count < PostOffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var userEntity = e.Container.Content as UserEntity;
            if (
                userEntity != null && userEntity.Equals(
                    followerList.ItemsSource[followerList.ItemsSource.Count - PostOffsetKnob]))
            {
                UserFollowerCollection.LoadUserFollowerFollowingList();
            }
        }

        private void followingTimeline_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (UserFollowingCollection.IsLoading || followingList.ItemsSource == null ||
                followingList.ItemsSource.Count < PostOffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var userEntity = e.Container.Content as UserEntity;
            if (
                userEntity != null && userEntity.Equals(
                    followingList.ItemsSource[followingList.ItemsSource.Count - PostOffsetKnob]))
            {
                UserFollowingCollection.LoadUserFollowerFollowingList();
            }
        }

        private void userTimeLine_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (UserCollection.IsLoading || userTimeLine.ItemsSource == null ||
                userTimeLine.ItemsSource.Count < PostOffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var postEntity = e.Container.Content as PostEntity;
            if (
                postEntity != null && postEntity.Equals(
                    userTimeLine.ItemsSource[userTimeLine.ItemsSource.Count - PostOffsetKnob]))
            {
                UserCollection.LoadPosts(EndPoints.USER_TIMELINE);
            }
        }

        private void timeLine_Tap(object sender, GestureEventArgs e)
        {
            var post = ((FrameworkElement) e.OriginalSource).DataContext as PostEntity;
            if (post == null) return;
            App.ViewModel.SelectedPost = post;
            NavigationService.Navigate(new Uri("/Views/PostPage.xaml", UriKind.Relative));
        }

        private async void albumGallery_Tap(object sender, GestureEventArgs e)
        {
            var mediaEntity = ((FrameworkElement) e.OriginalSource).DataContext as MediaEntity;
            if (mediaEntity == null) return;
            PostEntity post = await StatusManager.GetPost(false, true, mediaEntity.StatusId, App.userAccountEntity);
            App.ViewModel.SelectedPost = post;
            NavigationService.Navigate(new Uri("/Views/PostPage.xaml", UriKind.Relative));
        }

        private async void FollowButton_Click(object sender, EventArgs e)
        {
            bool result =
                await FriendshipManager.CreateFriendship(_selectedUser.UserID, App.userAccountEntity);
            if (!result) return;
            var followButton = (ApplicationBarIconButton) ApplicationBar.Buttons[0];
            var unfollowButton = (ApplicationBarIconButton) ApplicationBar.Buttons[1];
            unfollowButton.IsEnabled = true;
            followButton.IsEnabled = false;
        }

        private async void UnfollowButton_Click(object sender, EventArgs e)
        {
            bool result =
                await FriendshipManager.DestroyFriendship(_selectedUser.UserID, App.userAccountEntity);
            if (!result) return;
            var followButton = (ApplicationBarIconButton) ApplicationBar.Buttons[0];
            var unfollowButton = (ApplicationBarIconButton) ApplicationBar.Buttons[1];
            unfollowButton.IsEnabled = false;
            followButton.IsEnabled = true;
        }

        private void ReplyButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(
                new Uri(
                    "/Views/MessagePost.xaml?statusId=" + App.ViewModel.SelectedPost.StatusID + " &screenName=@" +
                    App.ViewModel.SelectedPost.User.ScreenName, UriKind.Relative));
        }

        private void SecretMailButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(
                new Uri("/Views/MessagePost.xaml?IsSecretMail=True&userId=" + _selectedUser.UserID,
                    UriKind.Relative));
        }
    }
}