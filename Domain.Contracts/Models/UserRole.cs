using System.Collections.Generic;

namespace Domain.Contracts.Models
{
    public class UserRole : IEqualityComparer<UserRole>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public bool Equals(UserRole x, UserRole y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(UserRole obj)
        {
            return base.GetHashCode();
        }
    }
}
