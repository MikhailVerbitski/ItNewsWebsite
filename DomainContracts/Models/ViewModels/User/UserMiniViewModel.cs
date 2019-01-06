namespace Domain.Contracts.Models.ViewModels.User
{
    public class UserMiniViewModel
    {
        public string ApplicationUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Avatar { get; set; }

        public string Role { get; set; }

        public int CountOfLikes { get; set; }
    }
}
