using System;
using System.ComponentModel.DataAnnotations;

namespace puremy.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
    }
}