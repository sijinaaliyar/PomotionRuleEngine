using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using PromotionEngine.ApplicationCore.Entities.CartAggregate;
using PromotionEngine.ApplicationCore.Entities.OrderAggregate;
using PromotionEngine.ApplicationCore.Entities.PromotionAggregate;
using PromotionEngine.ApplicationCore.Interfaces;
using PromotionEngine.ApplicationCore.Services;
using Xunit;
using Xunit.Sdk;

namespace PromotionEngine.ApplicationCore.UnitTests
{
    /// <summary>
    /// Test Data base price :
    /// A- 50
    /// B- 30
    /// C- 20
    /// D- 15
    /// </summary>
    public class PromotionRuleEngineTest
    {
        [Fact]
        public void CreateObjectWithNullPromotionRuleShouldThrowsException()
        {
            //Arrange
            var mockPromotionRepository = new Mock<IPromotionRepository>();
            //Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PromotionRuleEngine(mockPromotionRepository.Object, null));
        }

        [Fact]
        public void CreateObjectWithNullRepositoryShouldThrowsException()
        {
            //Arrange
            var mockPromotionRule = new Mock<IPromotionRule>();
            //Act & Assert
            Assert.Throws<ArgumentNullException>(() => new PromotionRuleEngine(null, mockPromotionRule.Object));
        }

        [Fact]
        public void ExecuteRulesWithNoCartItemsShouldThrowsException()
        {
            //Arrange
            var mockPromotionRepository = new Mock<IPromotionRepository>();
            var mockPromotionRule = new Mock<IPromotionRule>();
            var promotionList = new List<Promotion>()
            {
                new Promotion()
                {
                    PromotionName = "Jun-Promo-A",
                    EffectiveDate =  DateTime.Parse("06/01/2021"),
                    ExpiryDate = DateTime.Parse("06/30/2021"),
                    Criteria = new List<PromotionCriterion>()
                    {
                        new() {Quantity = 3, SKUId = 'A'}
                    },
                    OfferPrice = 130.00f
                },
                new Promotion()
                {
                    PromotionName = "Jun-Promo-B",            
                    EffectiveDate =  DateTime.Parse("06/01/2021"),
                    ExpiryDate = DateTime.Parse("06/30/2021"),
                    Criteria = new List<PromotionCriterion>()
                    {
                        new() {Quantity = 2, SKUId = 'B'}
                    },
                    OfferPrice = 45.00f
                },
                new Promotion()
                {
                    PromotionName = "Jun-Promo-C-D",
                    EffectiveDate =  DateTime.Parse("06/01/2021"),
                    ExpiryDate = DateTime.Parse("06/30/2021"),
                    Criteria = new List<PromotionCriterion>()
                    {
                        new() {Quantity = 1, SKUId = 'D'},
                        new() {Quantity = 1, SKUId = 'C'}
                    },
                    OfferPrice = 30.00f
                }
            };
            var cartItems = new List<CartItem>();
            mockPromotionRule.Setup(x => x.ApplyPromotion(cartItems, It.IsAny<Promotion>()))
                .Throws(new InvalidOperationException());
            mockPromotionRepository.Setup(x => x.GetActivePromotions()).Returns(promotionList);
            var engine = new PromotionRuleEngine(mockPromotionRepository.Object, mockPromotionRule.Object);
            //Act And Assert
            Assert.Throws<InvalidOperationException>(() => engine.ExecuteRules(new Cart() {Items = cartItems}));
        }


        [Fact]
        public void ExecuteRulesWithZeroPromotionShouldReturnOrder()
        {
            //Arrange
            var mockPromotionRepository = new Mock<IPromotionRepository>();
            var mockPromotionRule = new Mock<IPromotionRule>();
            var promoCD = new Promotion()
            {
                PromotionName = "Jun-Promo-C-D",
                EffectiveDate = DateTime.Parse("06/01/2021"),
                ExpiryDate = DateTime.Parse("06/30/2021"),
                Criteria = new List<PromotionCriterion>()
                {
                    new() {Quantity = 1, SKUId = 'D'},
                    new() {Quantity = 1, SKUId = 'C'}
                },
                OfferPrice = 30.00f
            };
            var promoA = new Promotion()
            {
                PromotionName = "Jun-Promo-A",
                EffectiveDate = DateTime.Parse("06/01/2021"),
                ExpiryDate = DateTime.Parse("06/30/2021"),
                Criteria = new List<PromotionCriterion>()
                {
                    new() {Quantity = 3, SKUId = 'A'}
                },
                OfferPrice = 130.00f
            };
            var promoB = new Promotion()
            {
                PromotionName = "Jun-Promo-B",
                EffectiveDate = DateTime.Parse("06/01/2021"),
                ExpiryDate = DateTime.Parse("06/30/2021"),
                Criteria = new List<PromotionCriterion>()
                {
                    new() {Quantity = 2, SKUId = 'B'}
                },
                OfferPrice = 45.00f
            };
            var promotionList = new List<Promotion>()
            {
                promoA,promoB,promoCD
            };
            var cartItems = new List<CartItem>()
            {
                new() {Quantity = 3, UnitPrice = 50.00f, SKUId = 'A'},
                new() {Quantity = 5, UnitPrice = 30.00f, SKUId = 'B'},
                new() {Quantity = 1, UnitPrice = 20.00f, SKUId = 'C'},
                new() {Quantity = 1, UnitPrice = 15.00f, SKUId = 'D'}
            };

            var expectedOrders = new List<OrderItem>()
            {
                new() {OrderQuantity = 3, UnitPrice = 50.00f, SKUId = 'A', AdjustmentAfterPromotion = 130f},
                new() {OrderQuantity = 5, UnitPrice = 30.00f, SKUId = 'B', AdjustmentAfterPromotion = 120f},
                new() {OrderQuantity = 1, UnitPrice = 20.00f, SKUId = 'C', AdjustmentAfterPromotion = 0f},
                new() {OrderQuantity = 1, UnitPrice = 15.00f, SKUId = 'D', AdjustmentAfterPromotion = 30f}
            };

            var expectedOrder = new Order() {OrderItems = expectedOrders, TotalPrice = 280f};

            mockPromotionRule.Setup(x => x.ApplyPromotion(It.IsAny<List<CartItem>>(),
                    It.Is<Promotion>( i => i.PromotionName.Contains(promoA.PromotionName) )))
                .Returns(new PromotionRuleResult()
                {
                    SavedAmount = 20f,
                    NonPromotionItems = cartItems.GetRange(1, 3),
                    PromotionAppliedItems = expectedOrders.GetRange(0, 1),
                    ExecutionLog = $"Applied 'Jun-Promo-A' 1 time(s)."
                });

            mockPromotionRule.Setup(x => x.ApplyPromotion(It.IsAny<List<CartItem>>(),
                    It.Is<Promotion>( i => i.PromotionName.Contains(promoB.PromotionName) )))
                .Returns(new PromotionRuleResult()
                {
                    SavedAmount = 30f,
                    NonPromotionItems = cartItems.GetRange(2, 2),
                    PromotionAppliedItems = expectedOrders.GetRange(1, 1),
                    ExecutionLog = $"Applied 'Jun-Promo-B' 2a time(s)."
                });

            mockPromotionRule.Setup(x => x.ApplyPromotion(It.IsAny<List<CartItem>>(),
                    It.Is<Promotion>( i => i.PromotionName.Contains(promoCD.PromotionName) )))
                .Returns(new PromotionRuleResult()
                {
                    SavedAmount = 5f,
                    NonPromotionItems = new List<CartItem>(),
                    PromotionAppliedItems = expectedOrders.GetRange(2, 2),
                    ExecutionLog = $"Applied 'Jun-Promo-C-D' 2a time(s)."
                });

            mockPromotionRepository.Setup(x => x.GetActivePromotions()).Returns(promotionList);
            var engine = new PromotionRuleEngine(mockPromotionRepository.Object, mockPromotionRule.Object);
            //Act
            var actualOrder = engine.ExecuteRules(new Cart() {Items = cartItems});
            //Assert
            Assert.Equal(expectedOrder.OrderItems.Count, actualOrder.OrderItems.Count);
            Assert.Equal(expectedOrder.TotalPrice, actualOrder.TotalPrice);
            Assert.Equal(expectedOrders.Find(i => i.SKUId == 'A')?.AdjustmentAfterPromotion,
                actualOrder.OrderItems.Find(i => i.SKUId == 'A')?.AdjustmentAfterPromotion);
        }
    }
}