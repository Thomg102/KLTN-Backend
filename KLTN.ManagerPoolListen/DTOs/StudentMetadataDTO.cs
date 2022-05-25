using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.ManagerPoolListen.DTOs
{
    class StudentMetadataDTO
    {
        [JsonProperty("imgUrl")]
        public string ImgUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("studentId")]
        public string StudentId { get; set; }

        [JsonProperty("birthday")]
        public int Birthday { get; set; }

        [JsonProperty("placeOfBirth")]
        public string PlaceOfBirth { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("nation")]
        public string Nation { get; set; }

        [JsonProperty("cmnd")]
        public string Cmnd { get; set; }

        [JsonProperty("issuancePlace")]
        public string IssuancePlace { get; set; }

        [JsonProperty("issuranceDate")]
        public int IssuranceDate { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("faculty")]
        public string Faculty { get; set; }

        [JsonProperty("facultyShortName")]
        public string FacultyShortName { get; set; }

        [JsonProperty("major")]
        public string Major { get; set; }

        [JsonProperty("schoolYear")]
        public string SchoolYear { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("walletAddress")]
        public string WalletAddress { get; set; }
    }


}
