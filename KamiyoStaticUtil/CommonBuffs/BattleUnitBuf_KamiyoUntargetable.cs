namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoUntargetable : BattleUnitBuf
    {
        public override bool IsTargetable()
        {
            return false;
        }

        public override void OnRoundEnd()
        {
            Destroy();
        }
    }
}