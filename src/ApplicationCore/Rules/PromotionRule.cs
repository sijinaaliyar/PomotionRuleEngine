using System;
using System.Collections.Generic;
using System.Linq;
using PromotionEngine.ApplicationCore.Entities.CartAggregate;
using PromotionEngine.ApplicationCore.Entities.OrderAggregate;
using PromotionEngine.ApplicationCore.Entities.PromotionAggregate;
using PromotionEngine.ApplicationCore.Interfaces;

namespace PromotionEngine.ApplicationCore.Rules
{
    public class PromotionRule: IPromotionRule
    {
        public PromotionRuleResult ApplyPromotion(List<CartItem> items, Promotion promotion)
        {
            var orderItems = new List<OrderItem>();
            var remainingItems = new List<CartItem>();
            var savedAmount = 0f;
            if (promotion.PromotionName == "ZeroOffer")
            {
                items.ForEach(i => orderItems.Add(new OrderItem()
                {
                    OrderQuantity = i.Quantity, UnitPrice = i.UnitPrice , SKUId =  i.SKUId , AdjustmentAfterPromotion = i.Quantity * i.UnitPrice
                }) );
               
            }
            else
            {
                remainingItems.AddRange(items);
                var promoItems = (from criterion in promotion.Criteria
                    join item in items on criterion.SKUId equals item.SKUId into g
                    from cartItem in g.DefaultIfEmpty()
                    select new {criterion, cartItem = cartItem}).ToList();
                if (promoItems.All(i => i.cartItem != null))
                {
                    var promotionMultiplier = promoItems.Min(x => x.cartItem.Quantity / x.criterion.Quantity);
                    var totalValue = 0f;
                    var extraPrice = 0f;
                    foreach (var item in promoItems)
                    {
                        orderItems.Add(new OrderItem() { OrderQuantity = item.cartItem.Quantity , 
                            UnitPrice = item.cartItem.UnitPrice , AdjustmentAfterPromotion = 0f,SKUId = item.cartItem.SKUId});
                        totalValue = totalValue + item.cartItem.UnitPrice * item.cartItem.Quantity;
                        var promotionNotApplicableQty =
                            item.cartItem.Quantity - (promotionMultiplier * item.criterion.Quantity);
                        extraPrice = extraPrice + (promotionNotApplicableQty * item.cartItem.UnitPrice);
                        remainingItems.Remove(item.cartItem);
                    }
                    
                    savedAmount = totalValue - (extraPrice+ (promotionMultiplier * promotion.OfferPrice));
                    orderItems.Last().AdjustmentAfterPromotion =
                        extraPrice + (promotionMultiplier * promotion.OfferPrice);
                }
            }

            return new PromotionRuleResult() { SavedAmount = savedAmount,NonPromotionItems = remainingItems ,PromotionAppliedItems = orderItems};
        }
    }
}