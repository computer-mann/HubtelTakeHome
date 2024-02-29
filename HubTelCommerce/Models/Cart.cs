using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HubTelCommerce.Models
{
    public class Cart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CartId { get; set; }
        public string CustomerId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }

        [DataType(DataType.Currency), Column(TypeName = "decimal(6,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}
