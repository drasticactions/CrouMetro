using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;

namespace CrouMetro.Core.Tools
{
    public class InfiniteScrollingUserCollection : INotifyPropertyChanged
    {
        public long? MaxStatusId;
        private bool _isLoadingData = false;
        private int _page;

        public InfiniteScrollingUserCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
            _page = 0;
        }

        public bool HasMoreItems { get; protected set; }
        public bool IsLoading { get; set; }

        public int Offset { get; set; }
        public string timeline { get; set; }
        public string userName { get; set; }
        public long userId { get; set; }
        public UserAccountEntity userAccountEntity { get; set; }

        public ObservableCollection<UserEntity> UserCollection { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public async void LoadSearchUserList()
        {
            IsLoading = true;
            var items = new List<UserEntity>();
        }

        public async void LoadUserFollowerFollowingList()
        {
            IsLoading = true;
            var items = new List<UserEntity>();
            if (timeline.Equals("Follower"))
                items = await UserManager.LookupFollowerUsers(Offset, userId, userAccountEntity);
            if (timeline.Equals("Following"))
                items = await UserManager.LookupFollowingUsers(Offset, userId, userAccountEntity);
            foreach (UserEntity item in items)
            {
                UserCollection.Add(item);
            }
            if (items.Any())
            {
                HasMoreItems = true;
                Offset += 1;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
        }
    }
}