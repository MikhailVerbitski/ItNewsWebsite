﻿namespace Domain.Contracts.Models
{
    public class PostReadRequestParams
    {
        public string type { get; set; }
        public int? skip { get; set; }
        public int? count { get; set; }
        public string where { get; set; }
        public string orderBy { get; set; }
    }
}
