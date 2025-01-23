using System.Collections.Generic;
using imsWeb.Models;

namespace imsWeb.Models{
    public class Supplier{
        public int SupplierId{get; set;}
        public string? Name{get; set;}
        

        public required ICollection<Product> Products { get; set; }
    }
}