namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoUntargetableUntilRoundEnd : BattleUnitBuf
    {
        public override bool IsTargetable()
        {
            return false;
        }

        public override int SpeedDiceBreakedAdder()
        {
            return 10;
        }

        public override void OnRoundEnd()
        {
            Destroy();
        }

        public override bool IsImmortal()
        {
            return true;
        }
    }
}