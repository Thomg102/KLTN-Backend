using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KLTN.DAL.Models.Entities
{
    public class CodeActivateProduct
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string ProductTypeName { get; set; }
        public long ProductNftId { get; set; }
        public bool IsUsed { get; set; }
        public string Code { get; set; }
    }
}
