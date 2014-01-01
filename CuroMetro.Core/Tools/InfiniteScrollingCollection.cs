using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;

namespace CrouMetro.Core.Tools
{
    public class InfiniteScrollingCollection : INotifyPropertyChanged
    {
        public long? MaxStatusId;
        private bool _isLoading;
        private int _page;

        public InfiniteScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
            _page = 0;
        }

        public bool HasMoreItems { get; protected set; }

        public string timeline { get; set; }
        public string userName { get; set; }
        public long UserId { get; set; }

        public string Query { get; set; }
        public UserAccountEntity userAccountEntity { get; set; }

        public ObservableCollection<PostEntity> PostCollection { get; set; }

        public bool IsLoading
        {
            get { return _isLoading; }

            private set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async void LoadSearchResults()
        {
            IsLoading = true;
            var items = new List<PostEntity>();
            var searchEntity = new SearchEntity();
            searchEntity =
                await SearchManager.SearchStatusList(Query, MaxStatusId, null, null, false, true, userAccountEntity);
            items = searchEntity.PostList;
            foreach (PostEntity item in items)
            {
                PostCollection.Add(item);
            }
            if (items.Any())
            {
                HasMoreItems = true;
                MaxStatusId = items.Last().StatusID;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
        }

        public async void LoadPosts(string timeline)
        {
            IsLoading = true;
            var items = new List<PostEntity>();
            if (timeline.Equals(EndPoints.HOME_TIMELINE))
                items = await TimelineManager.GetHomeTimeline(false, null, MaxStatusId, null, userAccountEntity);
            if (timeline.Equals(EndPoints.PUBLIC_TIMELINE))
                items = await TimelineManager.GetPublicTimeline(false, null, MaxStatusId, null, userAccountEntity);
            if (timeline.Equals(EndPoints.MENTIONS_TIMELINE))
                items = await TimelineManager.GetMentions(false, null, MaxStatusId, null, userAccountEntity);
            if (timeline.Equals(EndPoints.USER_TIMELINE))
                items =
                    await
                        TimelineManager.GetUserTimeline(userName, UserId, false, null, MaxStatusId, null,
                            userAccountEntity);

            foreach (PostEntity item in items)
            {
                PostCollection.Add(item);
            }
            if (items.Any())
            {
                HasMoreItems = true;
                MaxStatusId = items.Last().StatusID;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
        }
    }
}