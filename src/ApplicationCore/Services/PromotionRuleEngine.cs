using System;
using System.Collections.Generic;
using System.Linq;
using PromotionEngine.ApplicationCore.Entities.CartAggregate;
using PromotionEngine.ApplicationCore.Entities.OrderAggregate;
using PromotionEngine.ApplicationCore.Entities.PromotionAggregate;
using PromotionEngine.ApplicationCore.Interfaces;

namespace PromotionEngine.ApplicationCore.Services
{
    public class PromotionRuleEngine : IPromotionRuleEngine
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionRule _promotionRule;
        public PromotionRuleEngine(IPromotionRepository promotionRepository, IPromotionRule promotionRule)
        {
            _promotionRepository = promotionRepository ?? throw new ArgumentNullException();
            _promotionRule = promotionRule ?? throw new ArgumentNullException();
        }
        
        public Order ExecuteRules(Cart cart)
        {
            var promotions = _promotionRepository.GetActivePromotions();
            var orderItems = new List<OrderItem>();
            var remainingCartItems = cart?.Items ?? throw new InvalidOperationException();
            if (!cart.Items.Any())
            {
                throw new InvalidOperationException();
            }
            foreach (var promotion in promotions)
            {
                if (!remainingCartItems.Any())
                {
                    break;
                }
                var promotionResult = _promotionRule.ApplyPromotion(remainingCartItems, promotion);
                remainingCartItems = promotionResult.NonPromotionItems;
                orderItems.AddRange(promotionResult.PromotionAppliedItems);
            }
            if (remainingCartItems.Any())
            {
                var promotionResult =_promotionRule.ApplyPromotion(remainingCartItems, new Promotion() { PromotionName = "ZeroOffer"});
                orderItems.AddRange(promotionResult.PromotionAppliedItems);
            }

            return new Order()
            {
                OrderItems = orderItems, TotalPrice = orderItems.Sum(x => x.AdjustmentAfterPromotion)
            };
        }
    }
}