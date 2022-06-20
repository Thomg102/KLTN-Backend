namespace KLTN.Core.TuitionServices.DTOs
{
    public class TuitionDTO
    {
        public int ChainNetworkId { get; set; }
        public string Img { get; set; }
        public string TuitionId { get; set; }
        public string TuitionName { get; set; }
        public string TuitionAddress { get; set; }
        public string TuitionDescription { get; set; }
        public string TuitionHashIPFS { get; set; }
        public string TuitionStatus { get; set; }
        public int SchoolYear { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public decimal TokenAmount { get; set; }
        public decimal CurrencyAmount { get; set; }
        public string LecturerInCharge { get; set; }
        public string LecturerName { get; set; }
    }
}
