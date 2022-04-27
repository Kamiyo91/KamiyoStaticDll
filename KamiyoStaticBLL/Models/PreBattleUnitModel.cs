using System.Collections.Generic;

namespace KamiyoStaticBLL.Models
{
    public class PreBattleUnitModel
    {
        public int UnitId { get; set; }
        public SephirahType SephirahUnit { get; set; }
        public string UnitNameId { get; set; }
        public string SkinName { get; set; }
        public List<LorId> PassiveIds { get; set; }
    }
}