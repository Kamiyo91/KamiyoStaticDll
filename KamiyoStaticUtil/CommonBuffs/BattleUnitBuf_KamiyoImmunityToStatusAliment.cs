namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoImmunityToStatusAlimentUntilRoundEnd : BattleUnitBuf
    {
        public override bool IsImmune(BufPositiveType posType)
        {
            return posType == BufPositiveType.Negative;
        }

        public override void OnRoundEnd()
        {
            Destroy();
        }
    }
}