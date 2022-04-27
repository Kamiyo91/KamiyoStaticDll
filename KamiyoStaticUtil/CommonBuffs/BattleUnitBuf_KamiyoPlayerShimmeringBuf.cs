using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoPlayerShimmeringBuf : BattleUnitBuf
    {
        public override int GetCardCostAdder(BattleDiceCardModel card)
        {
            return -999;
        }
    }
}
