namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoImmortalStagger : BattleUnitBuf
    {
        public override bool IsImmortal()
        {
            return true;
        }

        public override bool IsInvincibleBp(BattleUnitModel attacker)
        {
            return true;
        }
    }
}