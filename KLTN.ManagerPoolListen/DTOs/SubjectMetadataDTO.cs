using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.ManagerPoolListen.DTOs
{
    class SubjectMetadataDTO
    {
        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("classId")]
        public string ClassId { get; set; }

        [JsonProperty("maxEntrant")]
        public string MaxEntrant { get; set; }

        [JsonProperty("lecturerInCharge")]
        public string LecturerInCharge { get; set; }

        [JsonProperty("startTime")]
        public int StartTime { get; set; }

        [JsonProperty("faculty")]
        public string Faculty { get; set; }

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
