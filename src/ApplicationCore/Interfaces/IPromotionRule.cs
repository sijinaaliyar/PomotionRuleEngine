using System.Collections.Generic;
using PromotionEngine.ApplicationCore.Entities.CartAggregate;
using PromotionEngine.ApplicationCore.Entities.PromotionAggregate;

namespace PromotionEngine.ApplicationCore.Interfaces
{
    public interface IPromotionRule
    {
        PromotionRuleResult ApplyPromotion(List<CartItem> items,
            Promotion promotion);
    }
}