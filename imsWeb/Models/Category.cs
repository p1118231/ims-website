using System.Collections.Generic;

namespace imsWeb.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }

        public required ICollection<Product> Products { get; set; }
    }
}