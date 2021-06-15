using System.Collections.Generic;

namespace PromotionEngine.ApplicationCore.Entities.CartAggregate
{
    public class Cart
    {
        public List<CartItem> Items { get; set; }
    }
}