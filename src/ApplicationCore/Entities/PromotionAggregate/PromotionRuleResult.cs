using System.Collections.Generic;
using PromotionEngine.ApplicationCore.Entities.CartAggregate;
using PromotionEngine.ApplicationCore.Entities.OrderAggregate;

namespace PromotionEngine.ApplicationCore.Entities.PromotionAggregate
{
    public class PromotionRuleResult
    {
      public  float SavedAmount { get; set; } 
      public   List<OrderItem> PromotionAppliedItems { get; set; } 
      public   List<CartItem> NonPromotionItems { get; set; }
      public   string ExecutionLog { get; set; }
    }
}