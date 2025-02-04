using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace imsWeb.Models
{
    public class Category
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int CategoryId { get; set; }
        public string? Name { get; set; }

        public required ICollection<Product> Products { get; set; }
    }
}