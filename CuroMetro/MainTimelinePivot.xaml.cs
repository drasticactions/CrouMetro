using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;
using CrouMetro.Core.Tools;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;

namespace CrouMetro
{
    public partial class MainTimelinePivot : PhoneApplicationPage
    {
        private const int OffsetKnob = 15;
        private string _selectedIndex;

        public MainTimelinePivot()
        {
            InitializeComponent();
            //var dispatcherHomeTimer = new DispatcherTimer();
            //dispatcherHomeTimer.Tick += refreshHomeFeed;
            //dispatcherHomeTimer.Interval = new TimeSpan(0, 1, 0);
            //dispatcherHomeTimer.Start();

            //var dispatcherPublicTimer = new DispatcherTimer();
            //dispatcherPublicTimer.Tick += refreshPublicFeed;
            //dispatcherPublicTimer.Interval = new TimeSpan(0, 1, 0);
            //dispatcherPublicTimer.Start();
        }

        public static InfiniteScrollingCollection PublicCollection { get; set; }
        public static InfiniteScrollingCollection HomeCollection { get; set; }
        public static InfiniteScrollingCollection MentionsCollection { get; set; }
        public static InfiniteScrollingHtmlParseImageCollection PictureCollection { get; set; }

        public async Task<bool> BindToPublicTimeline(UserAccountEntity userAccountEntry)
        {
            progressBar.Visibility = Visibility.Visible;
            PublicCollection = new InfiniteScrollingCollection
            {
                timeline = EndPoints.PUBLIC_TIMELINE,
                PostCollection = new ObservableCollection<PostEntity>(),
                userAccountEntity = userAccountEntry
            };
            List<PostEntity> items =
                await TimelineManager.GetPublicTimeline(false, null, null, null, App.userAccountEntity);
            foreach (PostEntity item in items)
            {
                PublicCollection.PostCollection.Add(item);
            }
            PublicCollection.MaxStatusId = items.Last().StatusID;
            publicTimeLine.DataContext = PublicCollection;
            publicTimeLine.ItemRealized += publicTimeLine_ItemRealized;
            progressBar.Visibility = Visibility.Collapsed;
            return true;
        }

        public async Task<bool> BindToHomeTimeline(UserAccountEntity userAccountEntry)
        {
            HomeCollection = new InfiniteScrollingCollection
            {
                timeline = EndPoints.HOME_TIMELINE,
                PostCollection = new ObservableCollection<PostEntity>(),
                userAccountEntity = userAccountEntry
            };
            List<PostEntity> items =
                await TimelineManager.GetHomeTimeline(false, null, null, null, App.userAccountEntity);
            foreach (PostEntity item in items)
            {
                HomeCollection.PostCollection.Add(item);
            }
            HomeCollection.MaxStatusId = items.Last().StatusID;
            homeTimeLine.DataContext = HomeCollection;
            homeTimeLine.ItemRealized += homeTimeLine_ItemRealized;
            return true;
        }

        public async Task<bool> BindToMentionsTimeline(UserAccountEntity userAccountEntry)
        {
            MentionsCollection = new InfiniteScrollingCollection
            {
                timeline = EndPoints.MENTIONS_TIMELINE,
                PostCollection = new ObservableCollection<PostEntity>(),
                userAccountEntity = userAccountEntry
            };
            List<PostEntity> items = await TimelineManager.GetMentions(false, null, null, null, App.userAccountEntity);
            foreach (PostEntity item in items)
            {
                MentionsCollection.PostCollection.Add(item);
            }
            MentionsCollection.MaxStatusId = items.Last().StatusID;
            MentionsTimeLine.DataContext = MentionsCollection;
            MentionsTimeLine.ItemRealized += mentionsTimeLine_ItemRealized;
            return true;
        }

        public async Task<bool> BindPictureGallery(UserAccountEntity userAccountEntity)
        {
            PictureCollection = new InfiniteScrollingHtmlParseImageCollection
            {
                timeline = "Pictures",
                userAccountEntity = userAccountEntity,
                Offset = 0,
                userName = userAccountEntity.GetUserEntity().ScreenName,
                MediaCollection = new ObservableCollection<MediaEntity>()
            };
            List<MediaEntity> items =
                await AlbumManager.GetAlbumList(0, userAccountEntity.GetUserEntity().ScreenName, userAccountEntity);
            foreach (MediaEntity item in items)
            {
                PictureCollection.MediaCollection.Add(item);
            }
            PictureCollection.Offset = 20;
            albumGallery.DataContext = PictureCollection;
            albumGallery.ItemRealized += albumGallery_ItemRealized;
            return true;
        }

        // Load data for the ViewModel Items
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            NavigationService.RemoveBackEntry();
            if (PublicCollection == null)
            {
                await BindToPublicTimeline(App.userAccountEntity);
            }
            else
            {
                publicTimeLine.DataContext = PublicCollection;
                publicTimeLine.ItemRealized += publicTimeLine_ItemRealized;
            }
            if (HomeCollection != null)
            {
                homeTimeLine.DataContext = HomeCollection;
                homeTimeLine.ItemRealized += homeTimeLine_ItemRealized;
            }
            if (MentionsCollection != null)
            {
                MentionsTimeLine.DataContext = MentionsCollection;
                MentionsTimeLine.ItemRealized += mentionsTimeLine_ItemRealized;
            }
            if (PictureCollection != null)
            {
                albumGallery.DataContext = PictureCollection;
                albumGallery.ItemRealized += albumGallery_ItemRealized;
            }
            progressBar.Visibility = Visibility.Collapsed;
        }

        private async void albumGallery_Tap(object sender, GestureEventArgs e)
        {
            var mediaEntity = ((FrameworkElement) e.OriginalSource).DataContext as MediaEntity;
            if (mediaEntity == null) return;
            PostEntity post = await StatusManager.GetPost(false, true, mediaEntity.StatusId, App.userAccountEntity);
            App.ViewModel.SelectedPost = post;
            NavigationService.Navigate(new Uri("/Views/PostPage.xaml", UriKind.Relative));
        }

        private void CreateMessage_Tap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MessagePost.xaml", UriKind.Relative));
        }

        private void publicTimeLine_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (PublicCollection.IsLoading || publicTimeLine.ItemsSource == null ||
                publicTimeLine.ItemsSource.Count < OffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var postEntity = e.Container.Content as PostEntity;
            if (
                postEntity != null && postEntity.Equals(
                    publicTimeLine.ItemsSource[publicTimeLine.ItemsSource.Count - OffsetKnob]))
            {
                PublicCollection.LoadPosts(EndPoints.PUBLIC_TIMELINE);
            }
        }

        private void homeTimeLine_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (HomeCollection.IsLoading || homeTimeLine.ItemsSource == null ||
                homeTimeLine.ItemsSource.Count < OffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var postEntity = e.Container.Content as PostEntity;
            if (postEntity != null && !postEntity.Equals(
                homeTimeLine.ItemsSource[homeTimeLine.ItemsSource.Count - OffsetKnob])) return;
            progressBar.Visibility = Visibility.Visible;
            HomeCollection.LoadPosts(EndPoints.HOME_TIMELINE);
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void mentionsTimeLine_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (MentionsCollection.IsLoading || MentionsTimeLine.ItemsSource == null ||
                MentionsTimeLine.ItemsSource.Count < OffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var postEntity = e.Container.Content as PostEntity;
            if (postEntity == null || !postEntity.Equals(
                MentionsTimeLine.ItemsSource[MentionsTimeLine.ItemsSource.Count - OffsetKnob])) return;
            progressBar.Visibility = Visibility.Visible;
            MentionsCollection.LoadPosts(EndPoints.MENTIONS_TIMELINE);
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void albumGallery_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (PictureCollection.IsLoading || albumGallery.ItemsSource == null ||
                albumGallery.ItemsSource.Count < OffsetKnob) return;
            if (e.ItemKind != LongListSelectorItemKind.Item) return;
            var mediaEntity = e.Container.Content as MediaEntity;
            if (mediaEntity == null || !mediaEntity.Equals(
                albumGallery.ItemsSource[albumGallery.ItemsSource.Count - OffsetKnob])) return;
            progressBar.Visibility = Visibility.Visible;
            PictureCollection.LoadAlbum();
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void timeLine_Tap(object sender, GestureEventArgs e)
        {
            var post = ((FrameworkElement) e.OriginalSource).DataContext as PostEntity;
            if (post != null)
            {
                App.ViewModel.SelectedPost = post;
                if (post.InReplyToUserID > 0)
                {
                    NavigationService.Navigate(new Uri("/Views/ConversationView.xaml", UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Views/PostPage.xaml", UriKind.Relative));
                }
            }
        }

        private void MessageButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MessagePost.xaml", UriKind.Relative));
        }

        private void UserConfigurationButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/UserConfigurationPage.xaml", UriKind.Relative));
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            appSettings["refreshToken"] = "";
            appSettings["accessToken"] = "";
            appSettings.Save();
            NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            PostEntity post;
            switch (_selectedIndex)
            {
                case "PublicTimeline":
                    if (PublicCollection == null) break;
                    post = PublicCollection.PostCollection.FirstOrDefault();
                    List<PostEntity> items =
                        await TimelineManager.GetPublicTimeline(false, post.StatusID, null, null, App.userAccountEntity);
                    items.Reverse();
                    foreach (PostEntity item in items)
                    {
                        PublicCollection.PostCollection.Insert(0, item);
                    }
                    publicTimeLine.ScrollTo(PublicCollection.PostCollection.FirstOrDefault());
                    break;
                case "HomeTimeline":
                    if (HomeCollection == null) break;
                    post = HomeCollection.PostCollection.FirstOrDefault();
                    List<PostEntity> homeitems =
                        await TimelineManager.GetHomeTimeline(false, post.StatusID, null, null, App.userAccountEntity);
                    homeitems.Reverse();
                    foreach (PostEntity item in homeitems)
                    {
                        HomeCollection.PostCollection.Insert(0, item);
                    }
                    homeTimeLine.ScrollTo(HomeCollection.PostCollection.FirstOrDefault());
                    break;
                case "MentionsTimeline":
                    if (MentionsCollection == null) break;
                    post = MentionsCollection.PostCollection.FirstOrDefault();
                    List<PostEntity> mentionitems =
                        await TimelineManager.GetMentions(false, null, null, null, App.userAccountEntity);
                    mentionitems.Reverse();
                    foreach (PostEntity item in mentionitems)
                    {
                        MentionsCollection.PostCollection.Insert(0, item);
                    }
                    MentionsTimeLine.ScrollTo(MentionsCollection.PostCollection.FirstOrDefault());
                    break;
                case "Album":
                    break;
            }
            progressBar.Visibility = Visibility.Collapsed;
        }

        private async void MainTimelinePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(TimelinePivot.SelectedIndex);
            progressBar.Visibility = Visibility.Visible;
            switch (TimelinePivot.SelectedIndex)
            {
                case 0:
                    _selectedIndex = "PublicTimeline";
                    break;
                case 1:
                    _selectedIndex = "HomeTimeline";
                    if (HomeCollection == null)
                    {
                        await BindToHomeTimeline(App.userAccountEntity);
                    }
                    break;
                case 2:
                    _selectedIndex = "MentionsTimeline";
                    if (MentionsCollection == null)
                    {
                        await BindToMentionsTimeline(App.userAccountEntity);
                    }
                    break;
                case 3:
                    _selectedIndex = "Album";
                    if (PictureCollection == null)
                    {
                        await BindPictureGallery(App.userAccountEntity);
                    }
                    break;
            }
            progressBar.Visibility = Visibility.Collapsed;
        }

        private void SecretMailButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SecretMailView.xaml", UriKind.Relative));
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SearchPage.xaml", UriKind.Relative));
        }
    }
}