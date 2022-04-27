using System;
using System.Collections.Generic;
using KamiyoStaticBLL.Enums;
using UnityEngine;

namespace KamiyoStaticBLL.Models
{
    public static class ModParameters
    {
        public static List<string> PackageIds;
        public static Dictionary<string, Sprite> ArtWorks;
        public static List<string> Path;
        public static string Language;
        public static readonly Dictionary<string, EffectTextModel> EffectTexts =
            new Dictionary<string, EffectTextModel>();
        public static Dictionary<string, string> NameTexts;

        public static List<Tuple<string, List<LorId>, LorId>> OnlyCardKeywords;

        public static Dictionary<LorId, int> DynamicNames;
        public static Dictionary<LorId, Tuple<string, SephirahType>> DynamicSephirahNames;
        public static List<LorId> BooksIds;
        public static List<LorId> CustomSkinTrue;
        public static List<LorId> PersonalCardList;

        public static List<LorId> EgoPersonalCardList;
        public static List<LorId> UntransferablePassives;

        public static List<Tuple<string, List<LorId>, string>> SkinNameIds;

        public static List<LorId> NoInventoryCardList;

        public static Dictionary<string, List<LorId>> SpritePreviewChange;

        public static List<LorId> NoEgoFloorUnit;

        public static Dictionary<string, List<LorId>> DefaultSpritePreviewChange;

        public static List<Tuple<LorId, List<PreBattleUnitModel>,List<SephirahType>,PreBattleUnitSpecialCases>> PreBattleUnits;

        public static Dictionary<LorId, bool> BannedEmotionStages;

        public static List<SkinNames> SkinParameters;

        public static List<LorId> OnlySephirahStage;

        public static Dictionary<LorId, ExtraRewards> ExtraReward;

        public static List<LorId> BannedEmotionSelectionUnit;

        public static List<Tuple<LorId, List<LorId>>> UniquePassives;

        public static List<Tuple<LorId, LorId>> ExtraConditionPassives;

        public static List<Tuple<LorId, LorId>> ChainRelease;
        public static List<LorId> ExtraMotions;
        public static List<LorId> BookList;
        public static List<LorId> EmotionExcludePassive;
        public static Dictionary<LorId, List<SephirahType>> UniqueUnitStages;

        public static int EgoEmotionLevel;
    }
}
