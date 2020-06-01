using Domain.Contracts.Models.ViewModels.User;
using System;

namespace Domain.Contracts.Models.ViewModels.Message
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public UserMiniViewModel Author { get; set; }
    }
}
