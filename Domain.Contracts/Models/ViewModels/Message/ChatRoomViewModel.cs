using Domain.Contracts.Models.ViewModels.User;
using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels.Message
{
    public class ChatRoomViewModel
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public IEnumerable<UserMiniViewModel> Users { get; set; }
        public IEnumerable<MessageViewModel> Messages { get; set; }
    }
}
