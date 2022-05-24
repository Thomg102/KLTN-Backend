using KLTN.Common.Enums;
using KLTN.DAL.Models.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.DAL.Models.Entities
{
    public class ProductOnSale
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string SaleAddress { get; set; }
        public string ProductHahIPFS { get; set; }
        public string AmountOnSale { get; set; }
        public long PriceOfOneItem { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductDescription { get; set; }
    }

    public class ProductType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string ProductTypeName { get; set; }
        public bool IsIdependentNFT { get; set; }
    }
}
