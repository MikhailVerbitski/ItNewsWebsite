using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts.Models
{
    public class UserImage
    {
        public byte[] Data { get; set; }
        public string Extension { get; set; }
    }
}
