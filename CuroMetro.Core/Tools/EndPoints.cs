using System;

namespace CrouMetro.Core.Tools
{
    public class EndPoints
    {
        private static readonly String CROUDIA = "https://api.croudia.com/";

        public static readonly String OAUTH_TOKEN = CROUDIA + "oauth/token";

        public static readonly String OAUTH_AUTHORIZE = CROUDIA + "oauth/authorize";

        //Timeline
        public static readonly String PUBLIC_TIMELINE = CROUDIA + "statuses/public_timeline.json";

        public static readonly String HOME_TIMELINE = CROUDIA + "statuses/home_timeline.json";

        public static readonly String MENTIONS_TIMELINE = CROUDIA + "statuses/mentions.json";

        public static readonly String USER_TIMELINE = CROUDIA + "statuses/user_timeline.json";

        //Statuses
        public static readonly String STATUS_UPDATE = CROUDIA + "statuses/update.json";

        public static readonly String STATUS_UPDATE_WITH_MEDIA = CROUDIA + "statuses/update_with_media.json";

        public static readonly String STATUS_DESTROY = CROUDIA + "statuses/destroy/{0}.json";

        public static readonly String STATUS_SHOW = CROUDIA + "statuses/show/{0}.json";

        //Messages
        public static readonly String MESSAGE_MAILS = CROUDIA + "secret_mails.json";

        public static readonly String MESSAGE_SENT = CROUDIA + "secret_mails/sent.json";

        public static readonly String MESSAGE_NEW = CROUDIA + "secret_mails/new.json";

        public static readonly String MESSAGE_DESTROY = CROUDIA + "secret_mails/destroy/{0}.json";

        public static readonly String MESSAGE_SHOW = CROUDIA + "secret_mails/show/{0}.json";

        //Users
        public static readonly String USERS_SHOW = CROUDIA + "users/show.json";

        public static readonly String USERS_LOOKUP = CROUDIA + "users/lookup.json";

        //Account
        public static readonly String ACCOUNT_VERIFY = CROUDIA + "account/verify_credentials.json";

        public static readonly String ACCOUNT_UPDATE_IMAGE = CROUDIA + "account/update_profile_image.json";

        public static readonly String ACCOUNT_UPDATE = CROUDIA + "account/update_profile.json";

        //FriendShips
        public static readonly String FRIEND_CREATE = CROUDIA + "friendships/create.json";

        public static readonly String FRIEND_DESTROY = CROUDIA + "friendships/destroy.json";

        public static readonly String FRIEND_SHOW = CROUDIA + "friendships/show.json";

        public static readonly String FRIEND_LOOKUP = CROUDIA + "friendships/lookup.json";

        //Friends
        public static readonly String FRIEND_LIST = CROUDIA + "friends/list.json";

        public static readonly String FRIEND_IDS = CROUDIA + "friends/ids.json";

        //Followers

        public static readonly String FOLLOWERS_LIST = CROUDIA + "followers/list.json";

        public static readonly String FOLLOWERS_IDS = CROUDIA + "followers/ids.json";

        //Favorites
        public static readonly String FAVORITES = CROUDIA + "favorites.json";

        public static readonly String FAVORITE_CREATE = CROUDIA + "favorites/create/{0}.json";

        public static readonly String FAVORITE_DESTROY = CROUDIA + "favorites/destroy/{0}.json";

        //Spread
        public static readonly String SPREAD_CREATE = CROUDIA + "statuses/spread/{0}.json";

        public static readonly String SPREAD_DESTORY = STATUS_DESTROY;

        //Search

        public static readonly String SEARCH_VOICES = CROUDIA + "search/voices.json";

        public static readonly String SEARCH_USERS = CROUDIA + "users/search.json";

        //Trends

        public static readonly String TrendsPlace = CROUDIA + "/trends/place.json?id={0}";

        public static readonly int WorldwideTrendId = 1;
    }
}