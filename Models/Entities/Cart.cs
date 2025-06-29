using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace THweb.Models.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}