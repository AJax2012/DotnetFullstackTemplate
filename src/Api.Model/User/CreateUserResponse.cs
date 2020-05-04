namespace SourceName.Api.Model.User
{
    public class CreateUserResponse
    {
        public bool IsUserCreated { get; set; }
        public string Message { get; set; }
        public UserResource UserResource { get; set; }
    }
}
