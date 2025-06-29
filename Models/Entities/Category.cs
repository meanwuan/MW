using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace THweb.Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}