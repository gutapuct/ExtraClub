using System;
using System.Collections.Generic;
using System.Linq;
using TonusClub.Entities;
using TonusClub.ServiceModel;

namespace Sync.Models
{
    public class DivisionStarsModel
    {
        public DivisionStarsModel(Guid divisionId)
        {
            DivisionId = divisionId;
            DivisionStars = _stars;
            AvgStars = (DivisionStars.Any()) ? (decimal)DivisionStars.Select(i => i.Rating).Average() : 5;
        }

        public Guid DivisionId { get; set; }
        public decimal AvgStars { get; set; }
        public List<DivisionStar> DivisionStars { get; set; }

        private List<DivisionStar> _stars
        {
            get
            {
                using (var context = new TonusEntities())
                {
                    return context.DivisionStars.Where(i => i.DivisionId == DivisionId).ToList();
                }
            }
        }
    }
}