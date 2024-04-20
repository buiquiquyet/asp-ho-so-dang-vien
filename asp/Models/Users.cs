using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace asp.Models
{
    public class Users
    {
        /*public Users()
        {
        }*/
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
    }
}
