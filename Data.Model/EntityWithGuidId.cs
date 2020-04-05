using System;

namespace SourceName.Data.Model
{
    public abstract class EntityWithGuidId
    {
        public Guid Id { get; set; }
    }
}