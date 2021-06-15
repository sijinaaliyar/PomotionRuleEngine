using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PromotionEngine.ApplicationCore.Entities.PromotionAggregate
{
    public class Promotion
    {
        public string PromotionName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        
        public List<PromotionCriterion> Criteria { get; set; }
        
        public float OfferPrice { get; set; }
    }
}