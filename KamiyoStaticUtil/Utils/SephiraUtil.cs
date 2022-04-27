using System;
using System.Linq;
using KamiyoStaticBLL.Models;
using LOR_DiceSystem;
using TMPro;
using UI;

namespace KamiyoStaticUtil.Utils
{
    public static class SephiraUtil
    {
        public static void SetOperationPanel(UIOriginEquipPageSlot instance,
            UICustomGraphicObject button_Equip, TextMeshProUGUI txt_equipButton, BookModel bookDataModel)
        {
            if (bookDataModel == null || !ModParameters.PackageIds.Contains(bookDataModel.ClassInfo.id.packageId) ||
                instance.BookDataModel == null ||
                instance.BookDataModel.owner != null) return;
            var currentUnit = UI.UIController.Instance.CurrentUnit;
            if (currentUnit == null) return;
            if (!ModParameters.DynamicSephirahNames.ContainsKey(bookDataModel.ClassInfo.id)) return;
            var mainItem = ModParameters.DynamicSephirahNames.FirstOrDefault(x => x.Key == bookDataModel.ClassInfo.id.id);
            if (mainItem.Value.Item2 == currentUnit.OwnerSephirah && !currentUnit.isSephirah)
            {
                button_Equip.interactable = false;
                txt_equipButton.text = TextDataModel.GetText("ui_equippage_notequip", Array.Empty<object>());
                return;
            }

            if (!IsLockedCharacter(currentUnit)) return;
            button_Equip.interactable = true;
            txt_equipButton.text = TextDataModel.GetText("ui_bookinventory_equipbook", Array.Empty<object>());
        }
        private static bool IsLockedCharacter(UnitDataModel unitData)
        {
            return unitData.isSephirah && (unitData.OwnerSephirah == SephirahType.Binah ||
                                           unitData.OwnerSephirah == SephirahType.Keter);
        }
        public static void PrepareBlackSilenceDeck(BattleUnitModel owner)
        {
            var furiosoCard = owner.personalEgoDetail.GetCardAll()
                .FirstOrDefault(x => x.GetID().IsBasic() && x.GetID().id == 702010);
            if (furiosoCard != null)
            {
                furiosoCard.CopySelf();
                var num = 0;
                foreach (var furiosoDice in furiosoCard.GetBehaviourList())
                {
                    if (num == 0)
                    {
                        furiosoDice.MotionDetail = MotionDetail.J2;
                        furiosoDice.EffectRes = "BS4DurandalDown_J2";
                        furiosoDice.ActionScript = "BlackSilence_SpecialDurandal_Ego_Se21341";
                    }
                    else
                    {
                        ChangeCardDiceEffect(furiosoDice);
                    }

                    num++;
                }
            }

            foreach (var card in owner.allyCardDetail.GetAllDeck())
            {
                card.CopySelf();
                foreach (var dice in card.GetBehaviourList())
                    ChangeCardDiceEffect(dice);
            }
        }

        private static void ChangeCardDiceEffect(DiceBehaviour dice)
        {
            switch (dice.MotionDetail)
            {
                case MotionDetail.Z:
                    dice.MotionDetail = MotionDetail.S11;
                    dice.EffectRes = "BlackSilence_4th_Lance_S11";
                    break;
                case MotionDetail.S10:
                    dice.MotionDetail = MotionDetail.S7;
                    dice.EffectRes = "BlackSilence_4th_GreatSword_S7";
                    break;
                case MotionDetail.S8:
                case MotionDetail.S9:
                    dice.MotionDetail = MotionDetail.S5;
                    dice.EffectRes = "BlackSilence_4th_MaceAxe_S5";
                    break;
                case MotionDetail.H:
                    dice.MotionDetail = MotionDetail.S6;
                    dice.EffectRes = "BlackSilence_4th_Hammer_S6";
                    break;
                case MotionDetail.S4:
                    dice.MotionDetail = MotionDetail.S2;
                    dice.ActionScript = "";
                    dice.EffectRes = "BlackSilence_4th_LongSword_S2";
                    break;
                case MotionDetail.S5:
                case MotionDetail.S6:
                    dice.MotionDetail = MotionDetail.S2;
                    dice.EffectRes = "BlackSilence_4th_Gauntlet_S3";
                    break;
                case MotionDetail.S7:
                    dice.MotionDetail = MotionDetail.S4;
                    dice.EffectRes = "BlackSilence_4th_ShortSword_S4";
                    break;
                case MotionDetail.J:
                    dice.MotionDetail = MotionDetail.S9;
                    dice.EffectRes = "BlackSilence_4th_DualWield1_S9";
                    break;
                case MotionDetail.S15:
                    dice.MotionDetail = MotionDetail.S10;
                    dice.EffectRes = "BlackSilence_4th_DualWield2_S10";
                    break;
                case MotionDetail.S2:
                    dice.MotionDetail = MotionDetail.S1;
                    break;
                case MotionDetail.S11:
                    dice.MotionDetail = MotionDetail.S8;
                    dice.EffectRes = "BlackSilence_4th_Shotgun_S8";
                    break;
                case MotionDetail.S12:
                    dice.MotionDetail = MotionDetail.J;
                    dice.EffectRes = "BS4DurandalUp_J";
                    break;
                case MotionDetail.S13:
                    dice.MotionDetail = MotionDetail.J2;
                    dice.EffectRes = "BS4DurandalDown_J2";
                    break;
            }
        }
    }
}
