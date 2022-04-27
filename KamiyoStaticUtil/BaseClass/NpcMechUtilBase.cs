using System;
using System.Linq;
using KamiyoStaticBLL.MechUtilBaseModels;
using KamiyoStaticUtil.CommonBuffs;
using KamiyoStaticUtil.Utils;

namespace KamiyoStaticUtil.BaseClass
{
    public class NpcMechUtilBase : MechUtilBase
    {
        private readonly NpcMechUtilBaseModel _model;

        public NpcMechUtilBase(NpcMechUtilBaseModel model) : base(model)
        {
            _model = model;
        }

        public virtual void OnUseCardResetCount(BattlePlayingCardDataInUnitModel curCard)
        {
            if (_model.LorIdEgoMassAttack != curCard.card.GetID()) return;
            _model.Counter = 0;
            _model.Owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
        }

        public virtual void MechHpCheck(int dmg)
        {
            if (_model.Owner.hp - dmg > _model.MechHp || !_model.HasMechOnHp) return;
            _model.HasMechOnHp = false;
            _model.Owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_KamiyoImmortalUntilRoundEnd());
            _model.Owner.SetHp(_model.MechHp);
            _model.Owner.breakDetail.ResetGauge();
            _model.Owner.breakDetail.RecoverBreakLife(1, true);
            _model.Owner.breakDetail.nextTurnBreak = false;
        }

        public override void SurviveCheck(int dmg)
        {
            if (_model.Owner.hp - dmg > _model.Hp || !_model.Survive) return;
            _model.Survive = false;
            if (_model.ReloadMassAttackOnLethal) SetCounter(_model.MaxCounter);
            _model.Owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_KamiyoImmortalUntilRoundEnd());
            _model.Owner.SetHp(_model.SetHp);
            _model.Owner.breakDetail.ResetGauge();
            _model.Owner.breakDetail.RecoverBreakLife(1, true);
            _model.Owner.breakDetail.nextTurnBreak = false;
            if (_model.HasSurviveAbDialog)
                UnitUtil.BattleAbDialog(_model.Owner.view.dialogUI, _model.SurviveAbDialogList,
                    _model.SurviveAbDialogColor);
            if (_model.NearDeathBuffExist)
                _model.Owner.bufListDetail.AddBufWithoutDuplication(
                    (BattleUnitBuf)Activator.CreateInstance(_model.NearDeathBuffType));
        }

        public virtual int AlwaysAimToTheSlowestDice(BattleUnitModel target)
        {
            var speedValue = 999;
            var finalTarget = 0;
            foreach (var dice in target.speedDiceResult.Select((x, i) => new { i, x }))
            {
                if (speedValue <= dice.x.value) continue;
                speedValue = dice.x.value;
                finalTarget = dice.i;
            }

            return finalTarget;
        }

        public virtual void RaiseCounter()
        {
            if (_model.MassAttackStartCount && _model.Counter < _model.MaxCounter) _model.Counter++;
        }

        public virtual void AddAdditionalPassive()
        {
            _model.Owner.passiveDetail.AddPassive(_model.AdditionalPassiveId);
        }

        public virtual void SetMassAttack(bool value)
        {
            _model.MassAttackStartCount = value;
        }

        public virtual void SetOneTurnCard(bool value)
        {
            _model.OneTurnCard = value;
        }

        public virtual void SetCounter(int value)
        {
            _model.Counter = value;
        }

        public virtual void OnSelectCardPutMassAttack(ref BattleDiceCardModel origin)
        {
            if (!_model.MassAttackStartCount || _model.Counter < _model.MaxCounter || _model.OneTurnCard)
                return;
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(_model.LorIdEgoMassAttack));
            SetOneTurnCard(true);
        }

        public virtual void ExhaustEgoAttackCards()
        {
            var cards = _model.Owner.allyCardDetail.GetAllDeck().Where(x => x.GetID() == _model.LorIdEgoMassAttack);
            foreach (var card in cards) _model.Owner.allyCardDetail.ExhaustACardAnywhere(card);
        }

        public virtual BattleUnitModel ChooseEgoAttackTarget(LorId cardId)
        {
            if (cardId != _model.LorIdEgoMassAttack) return null;
            if (BattleObjectManager.instance
                .GetAliveList(Faction.Player).Any(x => !x.UnitData.unitData.isSephirah))
                return RandomUtil.SelectOne(BattleObjectManager.instance.GetAliveList(Faction.Player)
                    .Where(x => !x.UnitData.unitData.isSephirah).ToList());
            return null;
        }

        public virtual bool UseSpecialBuffCard()
        {
            if (_model.Owner.cardSlotDetail.PlayPoint < _model.SpecialCardCost || !_model.Owner.bufListDetail
                    .GetActivatedBufList()
                    .Exists(x => _model.SpecialBufType.IsInstanceOfType(x))) return false;
            _model.Owner.bufListDetail.RemoveBufAll(_model.SpecialBufType);
            _model.Owner.cardSlotDetail.RecoverPlayPoint(-_model.SpecialCardCost);
            _model.Owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, 1, _model.Owner);
            _model.Owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Endurance, 1, _model.Owner);
            return true;
        }
    }
}