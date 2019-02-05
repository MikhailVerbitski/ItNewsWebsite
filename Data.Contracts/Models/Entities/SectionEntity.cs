using System.Collections.Generic;

namespace Data.Contracts.Models.Entities
{
    public class SectionEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CountOfUsage { get; set; }

        public virtual IEnumerable<PostEntity> Posts { get; set; }
    }
}
