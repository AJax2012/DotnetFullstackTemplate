namespace SourceName.Api.Model.Roles
{
    public class UserRoleResource
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public RoleResource Role { get; set; }
    }
}