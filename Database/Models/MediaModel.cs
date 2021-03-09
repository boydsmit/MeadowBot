using MongoDB.Bson;

namespace BunniBot.Database.Models
{
    public class MediaModel
    {
        public BsonObjectId Id;
        public string MediaType;
        public string[] MediaItems;

        public MediaModel(string mediaType, string[] mediaItems)
        {
            MediaType = mediaType;
            MediaItems = mediaItems;
        }

        public string GetMediaType()
        {
            return MediaType;
        }

        public string[] GetMediaItems()
        {
            return MediaItems;
        }
    }
}