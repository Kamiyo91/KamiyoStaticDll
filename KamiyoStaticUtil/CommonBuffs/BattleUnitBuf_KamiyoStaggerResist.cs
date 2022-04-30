namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoStaggerResist : BattleUnitBuf
    {
        public override bool IsInvincibleBp(BattleUnitModel attacker)
        {
            return true;
        }
    }
}