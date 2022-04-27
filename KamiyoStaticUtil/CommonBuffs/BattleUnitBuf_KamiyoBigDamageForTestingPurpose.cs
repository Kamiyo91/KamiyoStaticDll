namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoBigDamageForTestingPurpose : BattleUnitBuf
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    min = 50,
                    max = 50
                });
        }
    }
}