using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using KamiyoStaticBLL.Enums;
using KamiyoStaticBLL.Models;
using KamiyoStaticUtil.Utils;
using LOR_DiceSystem;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Workshop;
using Object = UnityEngine.Object;

namespace KamiyoStaticHarmony.Harmony
{
    [HarmonyPatch]
    public class KamiyoHarmoyPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIBookStoryChapterSlot), "SetEpisodeSlots")]
        public static void UIBookStoryChapterSlot_SetEpisodeSlots(UIBookStoryChapterSlot __instance,
            UIBookStoryPanel ___panel, List<UIBookStoryEpisodeSlot> ___EpisodeSlots)
        {
            SkinUtil.SetEpisodeSlots(__instance, ___panel, ___EpisodeSlots);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookModel), "GetThumbSprite")]
        [HarmonyPatch(typeof(BookXmlInfo), "GetThumbSprite")]
        public static void General_GetThumbSprite(object __instance, ref Sprite __result)
        {
            switch (__instance)
            {
                case BookXmlInfo bookInfo:
                    SkinUtil.GetThumbSprite(bookInfo.id, ref __result);
                    break;
                case BookModel bookModel:
                    SkinUtil.GetThumbSprite(bookModel.BookId, ref __result);
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIBookStoryPanel), "OnSelectEpisodeSlot")]
        public static void UIBookStoryPanel_OnSelectEpisodeSlot(UIBookStoryPanel __instance,
            UIBookStoryEpisodeSlot slot, TextMeshProUGUI ___selectedEpisodeText, Image ___selectedEpisodeIcon,
            Image ___selectedEpisodeIconGlow)
        {
            if (slot == null || !ModParameters.PackageIds.Contains(___selectedEpisodeText.text)) return;
            var selectedSlot = ___selectedEpisodeText.text;
            ___selectedEpisodeText.text = ModParameters.EffectTexts
                .FirstOrDefault(x => x.Key.Equals(selectedSlot)).Value
                .Name;
            ___selectedEpisodeIcon.sprite = ModParameters.ArtWorks[selectedSlot];
            ___selectedEpisodeIconGlow.sprite = ModParameters.ArtWorks[selectedSlot];
            __instance.UpdateBookSlots();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIBattleSettingPanel), "SetToggles")]
        public static void UIBattleSettingPanel_SetToggles(UIBattleSettingPanel __instance)
        {
            if (!ModParameters.PackageIds.Contains(Singleton<StageController>.Instance.GetStageModel().ClassInfo.id
                    .packageId)) return;
            if (!ModParameters.PreBattleUnits.Exists(x => x.Item1 == Singleton<StageController>.Instance.GetStageModel()
                    .ClassInfo
                    .id)) return;
            foreach (var currentAvailbleUnitslot in __instance.currentAvailbleUnitslots)
            {
                currentAvailbleUnitslot.SetToggle(false);
                currentAvailbleUnitslot.SetYesToggleState();
            }

            __instance.SetAvailibleText();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookModel), "SetXmlInfo")]
        public static void BookModel_SetXmlInfo(BookModel __instance, ref List<DiceCardXmlInfo> ____onlyCards)
        {
            if (!ModParameters.PackageIds.Contains(__instance.BookId.packageId)) return;
            var onlyCards = ModParameters.OnlyCardKeywords.FirstOrDefault(x => x.Item3 == __instance.BookId);
            if (onlyCards != null)
                ____onlyCards.AddRange(onlyCards.Item2.Select(id =>
                    ItemXmlDataList.instance.GetCardItem(id)));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnitDataModel), "IsLockUnit")]
        public static void UnitDataModel_IsLockUnit(UnitDataModel __instance, ref bool __result,
            SephirahType ____ownerSephirah)
        {
            if (UI.UIController.Instance.CurrentUIPhase != UIPhase.BattleSetting) return;
            var stageModel = Singleton<StageController>.Instance.GetStageModel();
            if (stageModel == null || !ModParameters.PackageIds.Contains(stageModel.ClassInfo.id.packageId)) return;
            if (ModParameters.OnlySephirahStage.Contains(stageModel.ClassInfo.id))
                __result = !__instance.isSephirah && ____ownerSephirah != SephirahType.None;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StageLibraryFloorModel), "InitUnitList")]
        public static void StageLibraryFloorModel_InitUnitList(StageLibraryFloorModel __instance,
            List<UnitBattleDataModel> ____unitList, StageModel stage)
        {
            if (!ModParameters.PackageIds.Contains(stage.ClassInfo.id.packageId)) return;
            if (!ModParameters.PreBattleUnits.Exists(x => x.Item1 == stage.ClassInfo.id)) return;
            var sephirahTypes = ModParameters.PreBattleUnits
                .FirstOrDefault(x => x.Item1 == stage.ClassInfo.id);
            if (sephirahTypes == null) return;
            if (sephirahTypes.Item3.Contains(__instance.Sephirah)) ____unitList.Clear();
            switch (sephirahTypes.Item4)
            {
                case PreBattleUnitSpecialCases.CustomUnits:
                    UnitUtil.AddCustomUnits(__instance, stage, ____unitList, stage.ClassInfo.id,
                        stage.ClassInfo.id.packageId);
                    break;
                case PreBattleUnitSpecialCases.Sephirah4:
                    UnitUtil.Add4SephirahUnits(stage, ____unitList);
                    break;
                default:
                    return;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkshopSkinDataSetter), "SetMotionData")]
        public static void WorkshopSkinDataSetter_SetMotionData(WorkshopSkinDataSetter __instance, ActionDetail motion,
            ClothCustomizeData data)
        {
            if (__instance.Appearance.GetCharacterMotion(motion) != null ||
                !ModParameters.SkinParameters.Exists(x => data.spritePath.Contains(x.Name))) return;
            var item = SkinUtil.CopyCharacterMotion(__instance.Appearance, motion);
            __instance.Appearance._motionList.Add(item);
            if (__instance.Appearance._motionList.Count <= 0) return;
            foreach (var characterMotion in __instance.Appearance._motionList.Where(characterMotion =>
                         !__instance.Appearance.CharacterMotions.ContainsKey(characterMotion.actionDetail)))
            {
                __instance.Appearance.CharacterMotions.Add(characterMotion.actionDetail, characterMotion);
                characterMotion.gameObject.SetActive(false);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UnitDataModel), "EquipBook")]
        public static void UnitDataModel_EquipBookPrefix(UnitDataModel __instance, BookModel newBook, bool force,
            ref BookModel __state)
        {
            if (force) return;
            __state = newBook;
            if (!ModParameters.PackageIds.Contains(__instance.bookItem.ClassInfo.id.packageId)) return;
            if (ModParameters.DynamicNames.ContainsKey(__instance.bookItem.ClassInfo.id))
            {
                __instance.ResetTempName();
                __instance.customizeData.SetCustomData(true);
            }
            if (!ModParameters.DynamicSephirahNames.ContainsKey(__instance.bookItem.ClassInfo.id)) return;
            __instance.ResetTempName();
            __instance.customizeData.SetCustomData(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnitDataModel), "EquipBook")]
        public static void UnitDataModel_EquipBookPostfix(UnitDataModel __instance, BookModel newBook,
            bool isEnemySetting, bool force, BookModel __state)
        {
            if (force) return;
            if (newBook == null || !ModParameters.PackageIds.Contains(newBook.ClassInfo.id.packageId)) return;
            if (ModParameters.SkinNameIds != null && ModParameters.SkinNameIds.Any(x =>
                    x.Item2.Contains(newBook.ClassInfo.id) && newBook.ClassInfo.CharacterSkin.Contains(x.Item1)))
            {
                newBook.ClassInfo.CharacterSkin = new List<string>
                {
                    ModParameters.SkinNameIds.FirstOrDefault(x => newBook.ClassInfo.CharacterSkin.Contains(x.Item1))
                        ?.Item3
                };
            }

            if (__state != null && ModParameters.DynamicSephirahNames.ContainsKey(__state.ClassInfo.id))
            {
                if (!ModParameters.CustomSkinTrue.Contains(__state.ClassInfo.id))
                    __instance.customizeData.SetCustomData(false);
                __instance.EquipCustomCoreBook(null);
                __instance.workshopSkin = "";
                ModParameters.DynamicSephirahNames.TryGetValue(__state.ClassInfo.id, out var name);
                __instance.SetTempName(ModParameters.EffectTexts.FirstOrDefault(x => x.Key.Equals(name?.Item1)).Value
                    .Name);
                __instance.EquipBook(__state, isEnemySetting, true);
                return;
            }
            if (!ModParameters.DynamicNames.ContainsKey(newBook.ClassInfo.id)) return;
            if (UnitUtil.CheckSkinUnitData(__instance)) return;
            if (!ModParameters.CustomSkinTrue.Contains(newBook.ClassInfo.id))
                __instance.customizeData.SetCustomData(false);
            var nameId = ModParameters.DynamicNames[newBook.ClassInfo.id];
            __instance.SetTempName(ModParameters.NameTexts[nameId]);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookModel), "CanSuccessionPassive")]
        public static void BookModel_CanSuccessionPassive(BookModel __instance, PassiveModel targetpassive,
            ref GivePassiveState haspassiveState, ref bool __result)
        {
            var passiveItem =
                ModParameters.UniquePassives.FirstOrDefault(x => x.Item1 == targetpassive.originData.currentpassive.id);
            if (passiveItem != null && !__instance.GetPassiveModelList()
                    .Exists(x => passiveItem.Item2.Contains(x.reservedData.currentpassive.id)))
            {
                haspassiveState = GivePassiveState.Lock;
                __result = false;
                return;
            }

            var passiveDeck =
                ModParameters.MultiDeckPassive.FirstOrDefault(x =>
                    x.Item1 == targetpassive.originData.currentpassive.id);
            if (passiveDeck != null && !__instance.GetPassiveModelList()
                    .Exists(x => passiveDeck.Item2.Contains(x.reservedData.currentpassive.id)) &&
                (__instance.ClassInfo.categoryList.Contains(BookCategory.DeckFixed) ||
                 __instance.ClassInfo.optionList.Contains(BookOption.MultiDeck) || __instance.IsMultiDeck()))
            {
                haspassiveState = GivePassiveState.Lock;
                __result = false;
                return;
            }

            var passiveItemExtra =
                ModParameters.ExtraConditionPassives.FirstOrDefault(x =>
                    x.Item1 == targetpassive.originData.currentpassive.id);
            if (passiveItemExtra == null || !__instance.GetPassiveModelList()
                    .Exists(x => passiveItemExtra.Item2 == x.reservedData.currentpassive.id)) return;
            haspassiveState = GivePassiveState.Lock;
            __result = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookModel), "IsMultiDeck")]
        public static void BookModel_IsMultiDeck(BookModel __instance, ref bool __result)
        {
            try
            {
                __result = __instance.GetPassiveInfoList()
                               .Exists(x => ModParameters.MultiDeckPassiveIds.Contains(x.passive.id)) ||
                           __result;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UILibrarianEquipDeckPanel), "IsMultiDeck")]
        public static void UILibrarianEquipDeckPanel_IsMultiDeck(UILibrarianEquipDeckPanel __instance,
            ref bool __result)
        {
            __result = __instance.Unitdata != null && __instance.Unitdata.bookItem.GetPassiveInfoList()
                .Exists(x => ModParameters.MultiDeckPassiveIds.Contains(x.passive.id)) || __result;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BookModel), "ReleasePassive")]
        public static void BookModel_ReleasePassive(BookModel __instance, PassiveModel passive)
        {
            var currentPassive = passive.originData.currentpassive.id != new LorId(9999999)
                ? passive.originData.currentpassive
                : passive.reservedData.currentpassive;
            var passiveItem = ModParameters.ChainRelease.FirstOrDefault(x => x.Item1 == currentPassive.id);
            if (passiveItem != null && __instance.GetPassiveModelList()
                    .Exists(x => x.reservedData.currentpassive.id == passiveItem.Item2))
                __instance.ReleasePassive(__instance.GetPassiveModelList()
                    .Find(x => x.reservedData.currentpassive.id == passiveItem.Item2));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BookModel), "UnEquipGivePassiveBook")]
        public static void BookModel_UnEquipGivePassiveBook(BookModel __instance, BookModel unequipbook)
        {
            var passiveItem =
                ModParameters.ChainRelease.FirstOrDefault(x =>
                    unequipbook.GetPassiveModelList().Exists(y =>
                        x.Item1 == y.originData.currentpassive.id || x.Item1 == y.reservedData.currentpassive.id));
            if (passiveItem == null) return;
            try
            {
                var chainPassive = __instance.GetPassiveModelList()
                    .FirstOrDefault(x =>
                        x.reservedData.currentpassive.id == passiveItem.Item2 ||
                        x.originData.currentpassive.id == passiveItem.Item2);
                if (chainPassive == null) return;
                __instance.ReleasePassive(chainPassive);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PassiveModel), "ReleaseSuccesionGivePassive")]
        public static void PassiveModel_ReleaseSuccesionGivePassive(PassiveModel __instance)
        {
            var currentPassive = __instance.originData.currentpassive.id != new LorId(9999999)
                ? __instance.originData
                : __instance.reservedData;
            var passiveItem =
                ModParameters.ChainRelease.FirstOrDefault(x => x.Item1 == currentPassive.currentpassive.id);
            if (passiveItem == null) return;
            var book = Singleton<BookInventoryModel>.Instance.GetBookByInstanceId(currentPassive.givePassiveBookId);
            var passiveModel = book != null
                ? book.GetPassiveModelList().FirstOrDefault(x =>
                    x.originData.currentpassive.id == passiveItem.Item2)
                : Singleton<BookInventoryModel>.Instance.GetBlackSilenceBook().GetPassiveModelList().FirstOrDefault(x =>
                    x.originData.currentpassive.id == passiveItem.Item2);
            passiveModel?.ReleaseSuccesionReceivePassive(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnitDataModel), "LoadFromSaveData")]
        public static void UnitDataModel_LoadFromSaveData(UnitDataModel __instance)
        {
            if ((!string.IsNullOrEmpty(__instance.workshopSkin) || __instance.bookItem != __instance.CustomBookItem) &&
                ModParameters.PackageIds.Contains(__instance.bookItem.ClassInfo.id.packageId) &&
                ModParameters.DynamicNames.ContainsKey(__instance.bookItem.ClassInfo.id))
                __instance.ResetTempName();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UICustomizePopup), "OnClickSave")]
        public static void UICustomizePopup_OnClickSave(UICustomizePopup __instance)
        {
            if (ModParameters.PackageIds.Contains(__instance.SelectedUnit.bookItem.ClassInfo.id.packageId) ||
                !ModParameters.DynamicNames.ContainsKey(__instance.SelectedUnit.bookItem.ClassInfo.id)) return;
            var tempName =
                (string)__instance.SelectedUnit.GetType().GetField("_tempName", AccessTools.all)
                    ?.GetValue(__instance.SelectedUnit);
            __instance.SelectedUnit.ResetTempName();
            if (__instance.SelectedUnit.bookItem == __instance.SelectedUnit.CustomBookItem &&
                string.IsNullOrEmpty(__instance.SelectedUnit.workshopSkin))
            {
                __instance.previewData.Name = __instance.SelectedUnit.name;
                var nameId = ModParameters.DynamicNames[__instance.SelectedUnit.bookItem.ClassInfo.id];
                __instance.SelectedUnit.SetTempName(ModParameters.NameTexts[nameId]);
            }
            else
            {
                if (string.IsNullOrEmpty(tempName) || __instance.previewData.Name == tempName)
                    __instance.previewData.Name = __instance.SelectedUnit.name;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TextDataModel), "InitTextData")]
        public static void TextDataModel_InitTextData(string currentLanguage)
        {
            ModParameters.Language = currentLanguage;
            LocalizeUtil.AddGlobalLocalize();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UISettingInvenEquipPageListSlot), "SetBooksData")]
        [HarmonyPatch(typeof(UIInvenEquipPageListSlot), "SetBooksData")]
        public static void General_SetBooksData(object __instance,
            List<BookModel> books, UIStoryKeyData storyKey)
        {
            var uiOrigin = __instance as UIOriginEquipPageList;
            SkinUtil.SetBooksData(uiOrigin, books, storyKey);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISpriteDataManager), "Init")]
        public static void UISpriteDataManager_Init(UISpriteDataManager __instance)
        {
            foreach (var artWork in ModParameters.ArtWorks.Where(x =>
                         !x.Key.Contains("Glow") && !__instance._storyicons.Exists(y => y.type.Equals(x.Key))))
                __instance._storyicons.Add(new UIIconManager.IconSet
                {
                    type = artWork.Key,
                    icon = artWork.Value,
                    iconGlow = ModParameters.ArtWorks.FirstOrDefault(x => x.Key.Equals($"{artWork.Key}Glow")).Value ??
                               artWork.Value
                });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleUnitView), "ChangeSkin")]
        public static void BattleUnitView_ChangeSkin(BattleUnitView __instance, string charName)
        {
            var skin = ModParameters.SkinNameIds.Find(x => x.Item1.Contains(charName));
            if (skin == null) return;
            if (typeof(BattleUnitView).GetField("_skinInfo", AccessTools.all)?.GetValue(__instance) is
                BattleUnitView.SkinInfo skinInfo)
            {
                skinInfo.state = BattleUnitView.SkinState.Changed;
                skinInfo.skinName = charName;
            }

            var currentMotionDetail = __instance.charAppearance.GetCurrentMotionDetail();
            __instance.DestroySkin();
            var gameObject =
                Object.Instantiate(
                    Singleton<AssetBundleManagerRemake>.Instance.LoadCharacterPrefab(charName, "",
                        out var resourceName), __instance.model.view.characterRotationCenter);
            var workshopBookSkinData =
                Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(
                    skin.Item2.FirstOrDefault()?.packageId, charName);
            gameObject.GetComponent<WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
            __instance.charAppearance = gameObject.GetComponent<CharacterAppearance>();
            __instance.charAppearance.Initialize(resourceName);
            __instance.charAppearance.ChangeMotion(currentMotionDetail);
            __instance.charAppearance.ChangeLayer("Character");
            __instance.charAppearance.SetLibrarianOnlySprites(__instance.model.faction);
            __instance.model.UnitData.unitData.bookItem.ClassInfo.CharacterSkin = new List<string> { charName };
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DropBookInventoryModel), "LoadFromSaveData")]
        public static void DropBookInventoryModel_LoadFromSaveData(DropBookInventoryModel __instance)
        {
            foreach (var book in ModParameters.BookList)
            {
                var bookCount = __instance.GetBookCount(book);
                if (bookCount < 99) __instance.AddBook(book, 99 - bookCount);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StageController), "BonusRewardWithPopup")]
        public static void BonusRewardWithPopup(LorId stageId)
        {
            if (!ModParameters.PackageIds.Contains(stageId.packageId)) return;
            if (!ModParameters.ExtraReward.ContainsKey(stageId)) return;
            var message = false;
            var parameters = ModParameters.ExtraReward.FirstOrDefault(y => y.Key.Equals(stageId.id));
            if (parameters.Value.DroppedBooks != null)
            {
                message = true;
                foreach (var book in parameters.Value.DroppedBooks)
                    Singleton<DropBookInventoryModel>.Instance.AddBook(book.BookId,
                        book.Quantity);
            }

            if (parameters.Value.DroppedKeypages != null)
                foreach (var keypageId in parameters.Value.DroppedKeypages.Where(keypageId =>
                             !Singleton<BookInventoryModel>.Instance.GetBookListAll().Exists(x =>
                                 x.GetBookClassInfoId() == keypageId)))
                {
                    if (!message) message = true;
                    Singleton<BookInventoryModel>.Instance.CreateBook(keypageId);
                }

            if (message)
                UIAlarmPopup.instance.SetAlarmText(ModParameters.EffectTexts.FirstOrDefault(x =>
                        x.Key.Contains(parameters.Value.MessageId))
                    .Value
                    .Desc);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleUnitCardsInHandUI), "UpdateCardList")]
        public static void BattleUnitCardsInHandUI_UpdateCardList(BattleUnitCardsInHandUI __instance,
            List<BattleDiceCardUI> ____activatedCardList, ref float ____xInterval)
        {
            if (__instance.CurrentHandState != BattleUnitCardsInHandUI.HandState.EgoCard) return;
            var unit = __instance.SelectedModel ?? __instance.HOveredModel;
            if (!ModParameters.PackageIds.Contains(unit.UnitData.unitData.bookItem.BookId.packageId) ||
                !ModParameters.NoEgoFloorUnit.Contains(unit.UnitData.unitData.bookItem.BookId)) return;
            var list = SkinUtil.ReloadEgoHandUI(__instance, __instance.GetCardUIList(), unit, ____activatedCardList,
                ref ____xInterval).ToList();
            __instance.SetSelectedCardUI(null);
            for (var i = list.Count; i < __instance.GetCardUIList().Count; i++)
                __instance.GetCardUIList()[i].gameObject.SetActive(false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UICharacterListPanel), "RefreshBattleUnitDataModel")]
        public static void RefreshBattleUnitDataModel(UICharacterListPanel __instance,
            UnitDataModel data)
        {
            if (Singleton<StageController>.Instance.GetStageModel() == null ||
                !ModParameters.PackageIds.Contains(Singleton<StageController>.Instance.GetStageModel().ClassInfo.id
                    .packageId) ||
                !ModParameters.UniqueUnitStages.ContainsKey(Singleton<StageController>.Instance.GetStageModel()
                    .ClassInfo.id)) return;
            var slot =
                typeof(UICharacterListPanel).GetField("CharacterList", AccessTools.all)?.GetValue(__instance) as
                    UICharacterList;
            var stageModel = Singleton<StageController>.Instance.GetStageModel();
            var list = UnitUtil.UnitsToRecover(stageModel, data);
            foreach (var unit in list)
            {
                unit.Refreshhp();
                var uicharacterSlot = slot?.slotList.Find(x => x.unitBattleData == unit);
                if (uicharacterSlot == null || uicharacterSlot.unitBattleData == null) continue;
                uicharacterSlot.ReloadHpBattleSettingSlot();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIInvenEquipPageSlot), "SetOperatingPanel")]
        [HarmonyPatch(typeof(UIInvenLeftEquipPageSlot), "SetOperatingPanel")]
        [HarmonyPatch(typeof(UISettingInvenEquipPageSlot), "SetOperatingPanel")]
        [HarmonyPatch(typeof(UISettingInvenEquipPageLeftSlot), "SetOperatingPanel")]
        public static void General_SetOperatingPanel(object __instance,
            UICustomGraphicObject ___button_Equip, TextMeshProUGUI ___txt_equipButton, BookModel ____bookDataModel)
        {
            var uiOrigin = __instance as UIOriginEquipPageSlot;
            SephiraUtil.SetOperationPanel(uiOrigin, ___button_Equip, ___txt_equipButton, ____bookDataModel);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILibrarianAppearanceInfoPanel), "OnClickCustomizeButton")]
        public static bool UILibrarianAppearanceInfoPanel_OnClickCustomizeButton(
            UILibrarianAppearanceInfoPanel __instance)
        {
            if (ModParameters.PackageIds.Contains(__instance.unitData.bookItem.BookId.packageId) ||
                !ModParameters.DynamicSephirahNames.ContainsKey(__instance.unitData.bookItem.BookId)) return true;
            UIAlarmPopup.instance.SetAlarmText(ModParameters.EffectTexts
                .FirstOrDefault(x =>
                    x.Key.Equals(ModParameters.DynamicSephirahNames
                        .FirstOrDefault(y => __instance.unitData.bookItem.BookId == y.Key).Value.Item1)).Value.Desc);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEquipDeckCardList), "SetDeckLayout")]
        public static void UIEquipDeckCardList_SetDeckLayout(UIEquipDeckCardList __instance,
            GameObject ___multiDeckLayout)
        {
            if (ModParameters.MultiDeckUnits.Contains(__instance.currentunit.bookItem.BookId) || __instance
                    .currentunit.bookItem.GetPassiveInfoList().Exists(x =>
                        ModParameters.MultiDeckPassiveIds.Contains(x.passive.id)))
            {
                var labels = ModParameters.MultiDeckLabels.FirstOrDefault(x => __instance
                                                                                   .currentunit.bookItem
                                                                                   .GetPassiveInfoList()
                                                                                   .Exists(y =>
                                                                                       y.passive.id == x.Item1) ||
                                                                               __instance.currentunit.bookItem.BookId ==
                                                                               x.Item2)
                    ?.Item3;
                ModParameters.ChangedMultiView = true;
                if (__instance.currentunit.bookItem.GetCurrentDeckIndex() > 1)
                    __instance.currentunit.ReEquipDeck();
                SkinUtil.PrepareMultiDeckUI(___multiDeckLayout, labels);
            }
            else if (!ModParameters.MultiDeckUnits.Contains(__instance.currentunit.bookItem.BookId) &&
                     !__instance.currentunit.bookItem.GetPassiveModelList().Exists(x =>
                         ModParameters.MultiDeckPassiveIds.Contains(x.originData.currentpassive.id)) &&
                     ModParameters.ChangedMultiView)
            {
                ModParameters.ChangedMultiView = false;
                SkinUtil.RevertMultiDeckUI(___multiDeckLayout);
                __instance.GetType().GetMethod("SetDeckLayout", AccessTools.all)
                    ?.Invoke(__instance, Array.Empty<object>());
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIPassiveSuccessionPopup), "Close")]
        public static void UIPassiveSuccessionPopup_Close(UIPassiveSuccessionPopup __instance)
        {
            if (__instance.CurrentBookModel == null) return;
            try
            {
                SingletonBehavior<UIEquipDeckCardList>.Instance.GetType()
                    .GetMethod("SetDeckLayout", AccessTools.all)
                    ?.Invoke(SingletonBehavior<UIEquipDeckCardList>.Instance, Array.Empty<object>());
            }
            catch (Exception)
            {
                // ignored
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookInventoryModel), "LoadFromSaveData")]
        public static void BookInventoryModel_LoadFromSaveData(BookInventoryModel __instance)
        {
            foreach (var keypageId in ModParameters.KeypageIds.Where(keypageId =>
                         !Singleton<BookInventoryModel>.Instance.GetBookListAll().Exists(x =>
                             x.GetBookClassInfoId() == keypageId)))
                __instance.CreateBook(keypageId);
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISpriteDataManager), "GetStoryIcon")]
        public static void UISpriteDataManager_GetStoryIcon(ref string story)
        {
            if (story.Contains("Binah_Se21341"))
                story = "Chapter1";
        }
    }
}