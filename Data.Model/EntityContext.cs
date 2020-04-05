using Microsoft.EntityFrameworkCore;
using SourceName.Data.Model.Role;
using SourceName.Data.Model.User;

namespace SourceName.Data.Model
{
    public class EntityContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }

        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EntityContext).Assembly);
        }
    }
}