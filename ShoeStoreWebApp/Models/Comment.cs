using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ShoeStoreWebApp.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 🔗 User
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        // 🔗 Product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // 🔁 REPLY
        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }

        public ICollection<Comment> Replies { get; set; }
    }
}
