using KamiyoStaticUtil.Utils;

namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoLoseOnDeathBuff : BattleUnitBuf
    {
        public BattleUnitBuf_KamiyoLoseOnDeathBuff()
        {
            stack = 0;
        }

        public override int paramInBufDesc => 0;
        protected override string keywordId => "KamiyoLoseOnDeathBuff";
        protected override string keywordIconId => "KamiyoLoseOnDeathBuff";

        public override void OnDie()
        {
            UnitUtil.VipDeathPlayer();
        }
    }
}