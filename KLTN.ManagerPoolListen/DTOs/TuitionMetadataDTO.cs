using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.ManagerPoolListen.DTOs
{
    public class TuitionMetadataDTO
    {
        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tuitionId")]
        public string TuitionId { get; set; }

        [JsonProperty("lecturerInCharge")]
        public string LecturerInCharge { get; set; }

        [JsonProperty("lecturerName")]
        public string LecturerName { get; set; }

        [JsonProperty("amountToken")]
        public string AmountToken { get; set; }

        [JsonProperty("amountCurency")]
        public string AmountCurency { get; set; }

        [JsonProperty("startTime")]
        public int StartTime { get; set; }

        [JsonProperty("endTime")]
        public int EndTime { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
