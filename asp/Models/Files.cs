using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace asp.Models
{
    public class Files
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? record_id { get; set; }
        public string? ten { get; set; }

    }
}
