using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels
{
    public class UserClaim : IEqualityComparer<UserClaim>
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public bool Equals(UserClaim x, UserClaim y)
        {
            return x.ClaimType == y.ClaimType;
        }

        public int GetHashCode(UserClaim obj)
        {
            return base.GetHashCode();
        }
    }
}
