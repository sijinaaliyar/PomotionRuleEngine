using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PromotionEngine.ApplicationCore.Entities.CartAggregate;
using PromotionEngine.ApplicationCore.Entities.PromotionAggregate;
using PromotionEngine.ApplicationCore.Rules;
using Xunit;

namespace PromotionEngine.ApplicationCore.UnitTests
{
    /// <summary>
    /// Test Data base price :
    /// A- 50
    /// B- 30
    /// C- 20
    /// D- 15
    /// </summary>
    public class PromotionRuleTests
    {
        [Fact]
        public void ApplyPromotionWhenMultipleSKUMatchAndFewSKUNonMatchShouldReturnAppliedAndNonAppliedList()
        {
            //Arrange
            var rule = new PromotionRule();
            var cartItems = new List<CartItem>()
            {
                new() {Quantity = 3, UnitPrice = 50.00f, SKUId = 'A'},
                new() {Quantity = 5, UnitPrice = 30.00f, SKUId = 'B'},
                new() {Quantity = 1, UnitPrice = 20.00f, SKUId = 'C'},
                new() {Quantity = 1, UnitPrice = 15.00f, SKUId = 'D'}
            };
            var promotion = new Promotion()
            {
                PromotionName = "Jun-Promo-CAndD",
                EffectiveDate =  DateTime.Parse("06/01/2021"),
                ExpiryDate = DateTime.Parse("06/30/2021"),
                Criteria = new List<PromotionCriterion>()
                {
                    new() {Quantity = 1, SKUId = 'D'},
                    new() {Quantity = 1, SKUId = 'C'}
                },
                OfferPrice = 30.00f
            };
            //Act
            var result = rule.ApplyPromotion(cartItems, promotion);
            //Assert
            Assert.Equal(2, result.PromotionAppliedItems.Count);
            Assert.Equal(2, result.NonPromotionItems.Count);
            Assert.Equal(5.00f, result.SavedAmount);
        }

        [Fact]
        public void ApplyPromotionNTimesWhenMultipleSKUMatchAndFewSKUNonMatchShouldReturnAppliedAndNonAppliedList()
        {
            //Arrange
            var rule = new PromotionRule();
            var cartItems = new List<CartItem>()
            {
                new() {Quantity = 3, UnitPrice = 50.00f, SKUId = 'A'},
                new() {Quantity = 5, UnitPrice = 30.00f, SKUId = 'B'},
                new() {Quantity = 5, UnitPrice = 20.00f, SKUId = 'C'},
                new() {Quantity = 4, UnitPrice = 15.00f, SKUId = 'D'}
            };
            var promotion = new Promotion()
            {
                PromotionName = "Jun-Promo-CAndD",
                EffectiveDate =  DateTime.Parse("06/01/2021"),
                ExpiryDate = DateTime.Parse("06/30/2021"),
                Criteria = new List<PromotionCriterion>()
                {
                    new() {Quantity = 1, SKUId = 'D'},
                    new() {Quantity = 1, SKUId = 'C'}
                },
                OfferPrice = 30.00f
            };
            //Act
            var result = rule.ApplyPromotion(cartItems, promotion);
            //Assert
            Assert.Equal(2, result.PromotionAppliedItems.Count);
            Assert.Equal(2, result.NonPromotionItems.Count);
            Assert.Equal(20.00f, result.SavedAmount);
        }

      
        [Fact]
        public void ApplyPromotionToOneSKUShouldReturnResult()
        {
            //Arrange
            var rule = new PromotionRule();
            var cartItems = new List<CartItem>()
            {
                new() {Quantity = 4, UnitPrice = 50.00f, SKUId = 'A'},
                new() {Quantity = 5, UnitPrice = 30.00f, SKUId = 'B'},
                new() {Quantity = 1, UnitPrice = 20.00f, SKUId = 'C'},
                new() {Quantity = 1, UnitPrice = 15.00f, SKUId = 'D'}
            };
            var promotion = new Promotion()
            {
                PromotionName = "Jun-Promo-A",
                EffectiveDate =  DateTime.Parse("06/01/2021"),
                ExpiryDate = DateTime.Parse("06/30/2021"),
                Criteria = new List<PromotionCriterion>()
                {
                    new() {Quantity = 3, SKUId = 'A'}
                },
                OfferPrice = 130.00f
            };
            //Act
            var result = rule.ApplyPromotion(cartItems, promotion);
            //Assert
            Assert.Single(result.PromotionAppliedItems);
            Assert.Equal(3, result.NonPromotionItems.Count);
            Assert.Equal(20.00f, result.SavedAmount);
        }
    }
}