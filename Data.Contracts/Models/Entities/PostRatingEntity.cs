using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Contracts.Models.Entities
{
    public class PostRatingEntity
    {
        public int Id { get; set; }

        public int? PostId { get; set; }

        public PostEntity Post { get; set; }

        public int? UserProfileId { get; set; }

        public UserProfileEntity UserProfile { get; set; }

        public byte Score { get; set; }
    }
}
