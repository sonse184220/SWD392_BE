namespace CityScout.DTOs
{
    public class DestinationCreateDto
    {
        public string DestinationName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public double? Rate { get; set; }
        public string CategoryId { get; set; }
        public string Ward { get; set; }
        public string Status { get; set; }
        public string DistrictId { get; set; }
    }
}
