using System;
using System.ComponentModel.DataAnnotations;

namespace THweb.Models.Entities
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required, StringLength(500)]
        public string Comment { get; set; }

        public int Rating { get; set; } // Từ 1 đến 5

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}