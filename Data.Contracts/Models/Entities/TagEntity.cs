using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class TagEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual IEnumerable<PostTagEntity> Posts { get; set; }

        public long CountOfUsage { get; set; }
    }
}
