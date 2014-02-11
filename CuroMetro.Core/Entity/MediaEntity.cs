using System;

namespace CrouMetro.Core.Entity
{
    public class MediaEntity
    {
        public MediaEntity(long statusId, String url, String post, String type, int imageId)
        {
            this.StatusId = statusId;
            ImageType = type;
            LargeImageUrl = url + "?large";
            ImageUrl = url;
            ImageId = imageId;
            this.Post = post;
        }

        public String ImageUrl { get; set; }
        public String LargeImageUrl { get; set; }
        public String ImageType { get; set; }
        public String Post { get; set; }
        public UserEntity User { get; set; }
        public long StatusId { get; set; }
        public int ImageId { get; set; }
    }
}