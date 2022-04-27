using KamiyoStaticUtil.Utils;

namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoImmortalForTestPurpose : BattleUnitBuf
    {
        public override bool IsImmortal()
        {
            return true;
        }

        public override bool IsInvincibleHp(BattleUnitModel attacker)
        {
            return true;
        }

        public override bool IsInvincibleBp(BattleUnitModel attacker)
        {
            return true;
        }

        public override void OnRoundStart()
        {
            _owner.cardSlotDetail.RecoverPlayPoint(_owner.cardSlotDetail.GetMaxPlayPoint());
            UnitUtil.DrawUntilX(_owner, 6);
        }
    }
}