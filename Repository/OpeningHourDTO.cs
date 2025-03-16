namespace CityScout.DTOs
{
    public class OpeningHourDto
    {
        public string DestinationId { get; set; }  
        public string DayOfWeek { get; set; }      
        public string OpenTime { get; set; }     
        public string CloseTime { get; set; }    
        public bool IsClosed { get; set; }      
    }
}
