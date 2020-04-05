namespace SourceName.Service.Model.Roles
{
    public class UserRole
    {
        int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public int ApplicationRoleId { get; set; }
        public Role Role { get; set; }
    }
}