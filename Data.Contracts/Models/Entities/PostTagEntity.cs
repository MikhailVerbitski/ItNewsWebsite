﻿namespace Data.Contracts.Models.Entities
{
    public class PostTagEntity
    {
        public int Id { get; set; }

        public int? PostId { get; set; }

        public PostEntity Post { get; set; }

        public int? TagId { get; set; }

        public TagEntity Tag { get; set; }
    }
}
