using HubTelCommerce.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HubTelCommerce.ViewModels
{
    public class AddCartItemViewModel
    {
        
        public string? CartId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        [Range(1,Int32.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]
        [Range(0.01,Double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }
}
