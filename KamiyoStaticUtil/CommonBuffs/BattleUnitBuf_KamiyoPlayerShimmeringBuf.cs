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