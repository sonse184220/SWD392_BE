namespace Repository.ResponseModels
{
    public class ProfileResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ProfilePicture { get; set; }
        public bool? IsActive { get; set; }
    }
}
