using System.ComponentModel.DataAnnotations;

namespace imsWeb.Models;

public class CartItem{

    public int ProductId{get; set;}

    public String? ProductName{get; set;} 

    public int Quantity {get; set;}

    public decimal Price {get;set;}

    public String? Image{get; set;}



}