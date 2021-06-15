using PromotionEngine.ApplicationCore.Entities.CartAggregate;
using PromotionEngine.ApplicationCore.Entities.OrderAggregate;

namespace PromotionEngine.ApplicationCore.Interfaces
{
    public interface IPromotionRuleEngine
    {
        Order ExecuteRules(Cart cart);
    }
}