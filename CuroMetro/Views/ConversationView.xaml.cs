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
    public partial class ConversationView : PhoneApplicationPage
    {
        public ConversationView()
        {
            InitializeComponent();
        }

        public static InfiniteScrollingCollection conversationCollection { get; set; }

        public async Task<bool> BindToConversationTimeline(UserAccountEntity userAccountEntry)
        {
            conversationCollection = new InfiniteScrollingCollection();
            conversationCollection.timeline = EndPoints.PUBLIC_TIMELINE;
            conversationCollection.PostCollection = new ObservableCollection<PostEntity>();
            conversationCollection.userAccountEntity = userAccountEntry;
            List<PostEntity> items =
                await TimelineManager.GetConversation(App.ViewModel.SelectedPost, App.userAccountEntity);
            foreach (PostEntity item in items)
            {
                conversationCollection.PostCollection.Add(item);
            }
            conversationCollection.MaxStatusId = items.Last().StatusID;
            conversationTimeLine.DataContext = conversationCollection;
            return true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await BindToConversationTimeline(App.userAccountEntity);
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