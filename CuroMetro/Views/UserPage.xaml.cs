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

namespace CrouMetro.Views
{
    public partial class UserPage : PhoneApplicationPage
    {
        private int _offsetKnob = 20;
        private int _postOffsetKnob = 7;

        public UserPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel.SelectedUser;
            var followButton = (ApplicationBarIconButton) ApplicationBar.Buttons[0];
            var unfollowButton = (ApplicationBarIconButton) ApplicationBar.Buttons[1];
            var secretMailButton = (ApplicationBarIconButton) ApplicationBar.Buttons[3];
            followButton.IsEnabled = !App.ViewModel.SelectedUser.IsFollowing &&
                                     App.ViewModel.SelectedUser.IsNotCurrentUser;
            unfollowButton.IsEnabled = App.ViewModel.SelectedUser.IsFollowing &&
                                       App.ViewModel.SelectedUser.IsNotCurrentUser;
            secretMailButton.IsEnabled = App.ViewModel.SelectedUser.IsNotCurrentUser &&
                                         App.ViewModel.SelectedUser.IsFollowing;
        }

        public static InfiniteScrollingCollection userCollection { get; set; }

        public static InfiniteScrollingHtmlParseImageCollection pictureCollection { get; set; }

        public static InfiniteScrollingUserCollection userFollowerCollection { get; set; }

        public static InfiniteScrollingUserCollection userFollowingCollection { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.ViewModel.SelectedUser.IsCurrentUser)
            {
                UserIsFollowing.Text = "ユーザーは君だよ！";
            }
            else
            {
                UserIsFollowing.Text = App.ViewModel.SelectedUser.IsFollowing
                    ? "@" + App.ViewModel.SelectedUser.ScreenName + "をフォローしています"
                    : "@" + App.ViewModel.SelectedUser.ScreenName + "をフォローしていません";
            }
            BindToUserTimeline(App.userAccountEntity);
            BindToUserFollowerGallery(App.userAccountEntity);
            BindToUserFollowingGallery(App.userAccountEntity);
            await BindPictureGallery(App.userAccountEntity);
        }

        public async Task<bool> BindToUserTimeline(UserAccountEntity userAccountEntry)
        {
            userCollection = new InfiniteScrollingCollection
            {
                timeline = EndPoints.USER_TIMELINE,
                PostCollection = new ObservableCollection<PostEntity>(),
                userAccountEntity = userAccountEntry,
                userName = App.ViewModel.SelectedUser.ScreenName,
                UserId = App.ViewModel.SelectedUser.UserID
            };
            List<PostEntity> items =
                await
                    TimelineManager.GetUserTimeline(App.ViewModel.SelectedUser.ScreenName,
                        App.ViewModel.SelectedUser.UserID, null, null, null, null, App.userAccountEntity);
            foreach (PostEntity item in items)
            {
                userCollection.PostCollection.Add(item);
            }
            userCollection.MaxStatusId = items.Last().StatusID;
            userTimeLine.DataContext = userCollection;
            userTimeLine.ItemRealized += userTimeLine_ItemRealized;
            return true;
        }

        public async Task<bool> BindPictureGallery(UserAccountEntity userAccountEntity)
        {
            pictureCollection = new InfiniteScrollingHtmlParseImageCollection();
            pictureCollection.timeline = "Pictures";
            pictureCollection.userAccountEntity = userAccountEntity;
            pictureCollection.Offset = 0;
            pictureCollection.userName = App.ViewModel.SelectedUser.ScreenName;
            pictureCollection.MediaCollection = new ObservableCollection<MediaEntity>();
            List<MediaEntity> items =
                await AlbumManager.GetAlbumList(0, App.ViewModel.SelectedUser.ScreenName, userAccountEntity);
            foreach (MediaEntity item in items)
            {
                pictureCollection.MediaCollection.Add(item);
            }
            pictureCollection.Offset = 20;
            albumGallery.DataContext = pictureCollection;
            albumGallery.ItemRealized += albumGallery_ItemRealized;
            return true;
        }

        public async Task<bool> BindToUserFollowerGallery(UserAccountEntity userAccountEntity)
        {
            userFollowerCollection = new InfiniteScrollingUserCollection
            {
                timeline = "Follower",
                userId = App.ViewModel.SelectedUser.UserID,
                Offset = -1,
                userAccountEntity = userAccountEntity,
                UserCollection = new ObservableCollection<UserEntity>()
            };
            List<UserEntity> items =
                await UserManager.LookupFollowerUsers(0, App.ViewModel.SelectedUser.UserID, userAccountEntity);
            foreach (UserEntity item in items)
            {
                userFollowerCollection.UserCollection.Add(item);
            }
            userFollowerCollection.Offset = 0;
            followerList.DataContext = userFollowerCollection;
            followerList.ItemRealized += followerTimeline_ItemRealized;
            return true;
        }

        public async Task<bool> BindToUserFollowingGallery(UserAccountEntity userAccountEntity)
        {
            userFollowingCollection = new InfiniteScrollingUserCollection
            {
                timeline = "Following",
                userId = App.ViewModel.SelectedUser.UserID,
                Offset = -1,
                userAccountEntity = userAccountEntity,
                UserCollection = new ObservableCollection<UserEntity>()
            };
            List<UserEntity> items =
                await UserManager.LookupFollowingUsers(0, App.ViewModel.SelectedUser.UserID, userAccountEntity);
            foreach (UserEntity item in items)
            {
                userFollowingCollection.UserCollection.Add(item);
            }
            userFollowingCollection.Offset = 0;
            followingList.DataContext = userFollowingCollection;
            followingList.ItemRealized += followingTimeline_ItemRealized;
            return true;
        }

        private void albumGallery_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (pictureCollection.IsLoading || albumGallery.ItemsSource == null ||
                albumGallery.ItemsSource.Count < _offsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var mediaEntity = e.Container.Content as MediaEntity;
            if (
                mediaEntity != null && mediaEntity.Equals(
                    albumGallery.ItemsSource[albumGallery.ItemsSource.Count - _offsetKnob]))
            {
                pictureCollection.LoadAlbum();
            }
        }

        private void followerTimeline_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (userFollowerCollection.IsLoading || followerList.ItemsSource == null ||
                followerList.ItemsSource.Count < _postOffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var userEntity = e.Container.Content as UserEntity;
            if (
                userEntity != null && userEntity.Equals(
                    followerList.ItemsSource[followerList.ItemsSource.Count - _postOffsetKnob]))
            {
                userFollowerCollection.LoadUserFollowerFollowingList();
            }
        }

        private void followingTimeline_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (userFollowingCollection.IsLoading || followingList.ItemsSource == null ||
                followingList.ItemsSource.Count < _postOffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var userEntity = e.Container.Content as UserEntity;
            if (
                userEntity != null && userEntity.Equals(
                    followingList.ItemsSource[followingList.ItemsSource.Count - _postOffsetKnob]))
            {
                userFollowingCollection.LoadUserFollowerFollowingList();
            }
        }

        private void userTimeLine_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (userCollection.IsLoading || userTimeLine.ItemsSource == null ||
                userTimeLine.ItemsSource.Count < _postOffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var postEntity = e.Container.Content as PostEntity;
            if (
                postEntity != null && postEntity.Equals(
                    userTimeLine.ItemsSource[userTimeLine.ItemsSource.Count - _postOffsetKnob]))
            {
                userCollection.LoadPosts(EndPoints.USER_TIMELINE);
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
                await FriendshipManager.CreateFriendship(App.ViewModel.SelectedUser.UserID, App.userAccountEntity);
            if (!result) return;
            var followButton = (ApplicationBarIconButton) ApplicationBar.Buttons[0];
            var unfollowButton = (ApplicationBarIconButton) ApplicationBar.Buttons[1];
            unfollowButton.IsEnabled = true;
            followButton.IsEnabled = false;
        }

        private async void UnfollowButton_Click(object sender, EventArgs e)
        {
            bool result =
                await FriendshipManager.DestroyFriendship(App.ViewModel.SelectedUser.UserID, App.userAccountEntity);
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
                new Uri("/Views/MessagePost.xaml?IsSecretMail=True&userId=" + App.ViewModel.SelectedUser.UserID,
                    UriKind.Relative));
        }
    }
}