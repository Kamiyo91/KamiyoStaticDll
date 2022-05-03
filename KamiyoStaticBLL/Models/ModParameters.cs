using System;
using System.Collections.Generic;
using KamiyoStaticBLL.Enums;
using UnityEngine;

namespace KamiyoStaticBLL.Models
{
    public static class ModParameters
    {
        public static List<string> PackageIds = new List<string>();
        public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();
        public static List<string> Path = new List<string>();
        public static string Language = string.Empty;

        public static readonly Dictionary<string, EffectTextModel> EffectTexts =
            new Dictionary<string, EffectTextModel>();

        public static Dictionary<LorId, string> NameTexts = new Dictionary<LorId, string>();

        public static List<Tuple<List<string>, List<LorId>, LorId>> OnlyCardKeywords =
            new List<Tuple<List<string>, List<LorId>, LorId>>();

        public static Dictionary<LorId, LorId> DynamicNames = new Dictionary<LorId, LorId>();

        public static Dictionary<LorId, Tuple<string, SephirahType>> DynamicSephirahNames =
            new Dictionary<LorId, Tuple<string, SephirahType>>();

        public static List<LorId> BooksIds = new List<LorId>();
        public static List<LorId> CustomSkinTrue = new List<LorId>();
        public static List<LorId> PersonalCardList = new List<LorId>();
        public static List<LorId> EgoPersonalCardList = new List<LorId>();
        public static List<LorId> UntransferablePassives = new List<LorId>();

        public static List<Tuple<string, List<LorId>, string>> SkinNameIds =
            new List<Tuple<string, List<LorId>, string>>();

        public static List<LorId> OriginalNoInventoryCardList = new List<LorId>();
        public static List<LorId> NoInventoryCardList = new List<LorId>();
        public static Dictionary<string, List<LorId>> SpritePreviewChange = new Dictionary<string, List<LorId>>();
        public static List<LorId> NoEgoFloorUnit = new List<LorId>();

        public static Dictionary<string, List<LorId>>
            DefaultSpritePreviewChange = new Dictionary<string, List<LorId>>();

        public static List<Tuple<LorId, List<PreBattleUnitModel>, List<SephirahType>, PreBattleUnitSpecialCases>>
            PreBattleUnits =
                new List<Tuple<LorId, List<PreBattleUnitModel>, List<SephirahType>, PreBattleUnitSpecialCases>>();

        public static Dictionary<LorId, bool> BannedEmotionStages = new Dictionary<LorId, bool>();
        public static List<SkinNames> SkinParameters = new List<SkinNames>();
        public static List<LorId> OnlySephirahStage = new List<LorId>();
        public static Dictionary<LorId, ExtraRewards> ExtraReward = new Dictionary<LorId, ExtraRewards>();
        public static List<LorId> BannedEmotionSelectionUnit = new List<LorId>();
        public static List<Tuple<LorId, List<LorId>>> UniquePassives = new List<Tuple<LorId, List<LorId>>>();
        public static List<Tuple<LorId, LorId>> ExtraConditionPassives = new List<Tuple<LorId, LorId>>();
        public static List<Tuple<LorId, LorId>> ChainRelease = new List<Tuple<LorId, LorId>>();
        public static List<LorId> ExtraMotions = new List<LorId>();
        public static List<LorId> BookList = new List<LorId>();
        public static List<LorId> EmotionExcludePassive = new List<LorId>();

        public static Dictionary<LorId, List<SephirahType>> UniqueUnitStages =
            new Dictionary<LorId, List<SephirahType>>();

        public static List<Tuple<LorId, List<LorId>>> MultiDeckPassive = new List<Tuple<LorId, List<LorId>>>();
        public static List<LorId> MultiDeckPassiveIds = new List<LorId>();

        public static List<Tuple<LorId, LorId, List<string>>> MultiDeckLabels =
            new List<Tuple<LorId, LorId, List<string>>>();

        public static List<LorId> MultiDeckUnits = new List<LorId>();
        public static Dictionary<string, string> DefaultKeyword = new Dictionary<string, string>();
        public static Dictionary<int, List<LorId>> SameInnerIdPassives = new Dictionary<int, List<LorId>>();
        public static BlackSilence4thMapManager BoomEffectMap = null;
        public static Dictionary<LorId, LorId> DialogList = new Dictionary<LorId, LorId>();
        public static List<LorId> KeypageIds = new List<LorId>();
        public static List<string> NoCredenza = new List<string>();
        public static List<LorId> OneSideCards = new List<LorId>();
        public static List<LorId> OneSideClashPassive = new List<LorId>();
        public static List<LorId> SupportCharPassive = new List<LorId>();
        public static bool ChangedMultiView = false;
    }
}