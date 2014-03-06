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
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace CrouMetro.Views
{
    public partial class ConversationView : PhoneApplicationPage
    {
        public ConversationView()
        {
            InitializeComponent();
        }

        public static InfiniteScrollingCollection ConversationCollection { get; set; }

        public async Task<bool> BindToConversationTimeline(UserAccountEntity userAccountEntry)
        {
            ConversationCollection = new InfiniteScrollingCollection
            {
                timeline = EndPoints.PUBLIC_TIMELINE,
                PostCollection = new ObservableCollection<PostEntity>(),
                userAccountEntity = userAccountEntry
            };
            List<PostEntity> items =
                await TimelineManager.GetConversation(App.ViewModel.SelectedPost, App.userAccountEntity);
            foreach (PostEntity item in items)
            {
                ConversationCollection.PostCollection.Add(item);
            }
            ConversationCollection.MaxStatusId = items.Last().StatusID;
            conversationTimeLine.DataContext = ConversationCollection;
            return true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            await BindToConversationTimeline(App.userAccountEntity);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void timeLine_Tap(object sender, GestureEventArgs e)
        {
            var post = ((FrameworkElement) e.OriginalSource).DataContext as PostEntity;
            if (post != null)
            {
                App.ViewModel.SelectedPost = post;
                NavigationService.Navigate(new Uri("/Views/PostPage.xaml", UriKind.Relative));
            }
        }
    }
}