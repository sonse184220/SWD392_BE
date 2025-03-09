namespace Repository.RequestModels
{
    public class UpdateProfileRequest
    {
        public string UserId { get; set; } 
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
