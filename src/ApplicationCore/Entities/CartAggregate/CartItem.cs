namespace PromotionEngine.ApplicationCore.Entities.CartAggregate
{
    public class CartItem
    {
        public char SKUId { get; set; }
        public float UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}