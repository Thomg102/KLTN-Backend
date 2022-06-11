using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.ManagerPoolListen.DTOs
{
    public class ScholarshipMetadataDTO
    {
        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("award")]
        public string Award { get; set; }

        [JsonProperty("scholarshipId")]
        public string ScholarshipId { get; set; }

        [JsonProperty("lecturerInCharge")]
        public string LecturerInCharge { get; set; }

        [JsonProperty("lecturerName")]
        public string LecturerName { get; set; }

        [JsonProperty("startTime")]
        public int StartTime { get; set; }

        [JsonProperty("endTime")]
        public int EndTime { get; set; }

        [JsonProperty("endTimeToRegister")]
        public int EndTimeToRegister { get; set; }

        [JsonProperty("endTimeToConfirm")]
        public int EndTimeToConfirm { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }


}
