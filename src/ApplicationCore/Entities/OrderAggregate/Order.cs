using System.Collections.Generic;

namespace PromotionEngine.ApplicationCore.Entities.OrderAggregate
{
    public class Order
    {
        public List<OrderItem> OrderItems { get; set; }
        public float TotalPrice { get; set; }
        public List<string> PromotionExecutedLog { get; set; }
    }
}