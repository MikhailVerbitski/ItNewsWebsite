using System.Collections.Generic;

namespace Domain.Contracts.Models.ViewModels
{
    public class TokenViewModel
    {
        public string Token { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
