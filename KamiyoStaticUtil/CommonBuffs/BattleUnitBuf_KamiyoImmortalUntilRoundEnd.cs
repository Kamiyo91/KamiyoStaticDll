namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoImmortalUntilRoundEnd : BattleUnitBuf
    {
        public override bool IsImmortal()
        {
            return true;
        }

        public override bool IsInvincibleHp(BattleUnitModel attacker)
        {
            return true;
        }

        public override void OnRoundEnd()
        {
            Destroy();
        }

        public override bool CanRecoverHp(int amount)
        {
            return false;
        }
    }
}