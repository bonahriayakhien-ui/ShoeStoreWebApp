using System.ComponentModel.DataAnnotations;

namespace ShoeStoreWebApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; } // 🆕 Tổng tiền

        public List<OrderDetail> OrderDetails { get; set; }

        public string OrderStatus { get; set; } = "Pending";
    }
}
