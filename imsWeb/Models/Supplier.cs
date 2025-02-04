using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using imsWeb.Models;

namespace imsWeb.Models{
    public class Supplier{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int SupplierId{get; set;}
        public string? Name{get; set;}

        public string? Address{get; set;}
        public string? Email{get; set;}
        public string? Phone{get; set;}
        

        public required ICollection<Product> Products { get; set; }
    }
}