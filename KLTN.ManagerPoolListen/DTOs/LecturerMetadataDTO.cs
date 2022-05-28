using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.ManagerPoolListen.DTOs
{
    class LecturerMetadataDTO
    {
        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dateOfBirth")]
        public string DateOfBirth { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("lecturerId")]
        public string LecturerId { get; set; }

        [JsonProperty("faculty")]
        public string Faculty { get; set; }

        [JsonProperty("facultyShortName")]
        public string FacultyShortName { get; set; }

        [JsonProperty("walletAddress")]
        public string WalletAddress { get; set; }
    }

}
