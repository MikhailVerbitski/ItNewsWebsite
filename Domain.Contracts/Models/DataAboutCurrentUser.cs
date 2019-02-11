using Domain.Contracts.Models.ViewModels;
using System.Collections.Generic;

namespace Domain.Contracts.Models
{
    public class DataAboutCurrentUser
    {
        public string Login { get; set; }
        public int UserProfileId { get; set; }
        public int Priority { get; set; }
        public List<UserClaim> claims { get; set; }
    }
}
