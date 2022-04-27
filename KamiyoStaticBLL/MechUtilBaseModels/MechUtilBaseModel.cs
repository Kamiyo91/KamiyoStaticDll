using System;
using System.Collections.Generic;
using KamiyoStaticBLL.Enums;
using LOR_XML;

namespace KamiyoStaticBLL.MechUtilBaseModels
{
    public class MechUtilBaseModel
    {
        public BattleUnitModel Owner { get; set; }
        public string EgoMapName { get; set; }
        public int Hp { get; set; }
        public int SetHp { get; set; }
        public float? BgY { get; set; }
        public float? FlY { get; set; }
        public List<int> OriginalMapStageIds { get; set; }
        public bool Survive { get; set; }
        public bool HasEgo { get; set; }
        public bool HasEgoAttack { get; set; }
        public bool EgoActivated { get; set; }
        public bool RefreshUI { get; set; }
        public bool IsSummonEgo { get; set; }
        public bool RecoverLightOnSurvive { get; set; }
        public bool EgoAttackCardExpire { get; set; }
        public bool HasAdditionalPassive { get; set; }
        public string SkinName { get; set; }
        public bool DieOnFightEnd { get; set; }
        public bool MapUsed { get; set; }
        public List<AbnormalityCardDialog> SurviveAbDialogList { get; set; }
        public List<AbnormalityCardDialog> EgoAbDialogList { get; set; }
        public bool HasEgoAbDialog { get; set; }
        public bool HasSurviveAbDialog { get; set; }
        public bool NearDeathBuffExist { get; set; }
        public AbColorType SurviveAbDialogColor { get; set; }
        public AbColorType EgoAbColorColor { get; set; }
        public Type NearDeathBuffType { get; set; }
        public Type EgoType { get; set; }
        public Type EgoMapType { get; set; }
        public LorId[] LorIdArray { get; set; } = null;
        public LorId EgoCardId { get; set; }
        public LorId SecondaryEgoCardId { get; set; }
        public LorId EgoAttackCardId { get; set; }
        public LorId AdditionalPassiveId { get; set; }
    }
}