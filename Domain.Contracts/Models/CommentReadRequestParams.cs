namespace Domain.Contracts.Models
{
    public class CommentReadRequestParams
    {
        public string type { get; set; }
        public int? skip { get; set; }
        public int? count { get; set; }
        public string ApplicationUserId { get; set; }
        public int? PostId { get; set; }
        public string orderBy { get; set; }
    }
}
