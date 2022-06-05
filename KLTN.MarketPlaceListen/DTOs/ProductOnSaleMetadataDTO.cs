using Newtonsoft.Json;

namespace KLTN.MarketPlaceListen.DTOs
{
    public class ProductOnSaleMetadataDTO
    {
        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("productType")]
        public string ProductType { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
