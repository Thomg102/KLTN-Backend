using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.DAL.Models.Entities
{
    public class SubcribedContractsListenEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string SubcribedContracts { get; set; }
    }
}
