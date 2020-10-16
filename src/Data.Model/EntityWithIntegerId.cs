using System.ComponentModel.DataAnnotations;

namespace SourceName.Data.Model
{
    public abstract class EntityWithIntegerId
    {
        [Key]
        public int Id { get; set; }
    }
}