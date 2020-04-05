namespace SourceName.Api.Model.Roles
{
    public class ApplicationUserRoleResource
    {
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public int ApplicationRoleId { get; set; }
        public ApplicationRoleResource Role { get; set; }
    }
}