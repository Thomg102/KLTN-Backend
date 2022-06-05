﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KLTN.DAL.Models.Entities
{
    public class ProductOnSale
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public string ProductId { get; set; }
        public long ProductNftId { get; set; }
        public string SaleAddress { get; set; }
        public string ProductHahIPFS { get; set; }
        public long AmountOnSale { get; set; }
        public string PriceOfOneItem { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductDescription { get; set; }
    }

    public class ProductType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductTypeAlias { get; set; }
        public bool IsIdependentNFT { get; set; }
    }
}
