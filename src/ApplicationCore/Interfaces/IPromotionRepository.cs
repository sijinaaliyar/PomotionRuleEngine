using System.Collections.Generic;
using PromotionEngine.ApplicationCore.Entities.PromotionAggregate;

namespace PromotionEngine.ApplicationCore.Interfaces
{
    public interface IPromotionRepository
    {
        List<Promotion> GetActivePromotions();
    }
}