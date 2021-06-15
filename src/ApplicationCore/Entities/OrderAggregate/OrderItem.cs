using System.Linq.Expressions;

namespace PromotionEngine.ApplicationCore.Entities.OrderAggregate
{
    public class OrderItem
    {
        public char SKUId { get; set; }
        public float UnitPrice { get; set; }
        public int OrderQuantity { get; set; }
        public float AdjustmentAfterPromotion { get; set; }
        public int AppliedPromotionId { get; set; }
    }
}