namespace KamiyoStaticUtil.CommonBuffs
{
    public class BattleUnitBuf_KamiyoLockedUnit : BattleUnitBuf
    {
        private int _breakedDice;
        private bool _motionChange;

        public override bool IsTargetable()
        {
            return false;
        }

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            if (!string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) ||
                owner.UnitData.unitData.bookItem != owner.UnitData.unitData.CustomBookItem) return;
            _motionChange = true;
            owner.view.charAppearance.ChangeMotion(ActionDetail.Damaged);
        }

        public override void OnRoundEnd()
        {
            if (_motionChange)
            {
                _motionChange = false;
                _owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
            }

            Destroy();
        }

        public override void OnRollSpeedDice()
        {
            _breakedDice = _owner.view.speedDiceSetterUI.SpeedDicesCount;
            for (var i = 0; i < _breakedDice; i++)
            {
                _owner.speedDiceResult[i].value = 0;
                _owner.speedDiceResult[i].breaked = true;
                _owner.view.speedDiceSetterUI.GetSpeedDiceByIndex(i).BreakDice(true, true);
            }
        }

        public override int SpeedDiceBreakedAdder()
        {
            return _breakedDice;
        }

        public override void OnRoundStart()
        {
            _owner.turnState = BattleUnitTurnState.BREAK;
        }
    }
}