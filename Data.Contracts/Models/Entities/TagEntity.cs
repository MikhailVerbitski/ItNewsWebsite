using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class TagEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } // Name

        public IEnumerable<PostEntity> Posts { get; set; } // Posts

        public long CountOfUsage { get; set; } // Count of Usage
    }
}
