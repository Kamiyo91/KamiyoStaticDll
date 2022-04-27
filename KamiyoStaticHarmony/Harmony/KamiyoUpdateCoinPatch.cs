using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using KamiyoStaticBLL.Models;

namespace KamiyoStaticHarmony.Harmony
{
    [HarmonyPatch(typeof(EmotionBattleTeamModel), "UpdateCoin")]
    public class KamiyoUpdateCoinPatch
    {
        private static int _coinsToRemove;

        private static Predicate<BattleUnitModel> Match => x => x.passiveDetail.PassiveList.Exists(y =>
            ModParameters.EmotionExcludePassive.Contains(y.id)) ||
            ModParameters.BannedEmotionStages.ContainsKey(Singleton<StageController>.Instance.GetStageModel().ClassInfo
                .id);

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> codeInstructions, ILGenerator iL)
        {
            var codes = new List<CodeInstruction>(codeInstructions);
            var pos0 = -1;
            var pos1 = -1;
            var pos2 = -1;
            List<CodeInstruction> addedCodes0 = null;
            List<CodeInstruction> addedCodes1 = null;
            List<CodeInstruction> addedCodes2 = null;
            for (var i = 0; i < codes.Count; i++)
            {
                if (pos0 < 0 &&
                    codes[i].opcode == OpCodes.Ldc_I4_0 &&
                    codes[i + 1].opcode == OpCodes.Stfld &&
                    codes[i + 1].operand
                        .Equals(AccessTools.Field(typeof(EmotionBattleTeamModel), "_emotionTotalCoinNumber")))
                {
                    addedCodes0 = new List<CodeInstruction>
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld,
                            AccessTools.Field(typeof(EmotionBattleTeamModel), "_emotionTotalCoinNumber")),
                        new CodeInstruction(OpCodes.Ldsfld,
                            AccessTools.Field(typeof(KamiyoUpdateCoinPatch), "_coinsToRemove")),
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(OpCodes.Stfld,
                            AccessTools.Field(typeof(EmotionBattleTeamModel), "_emotionTotalCoinNumber"))
                    };
                    pos0 = i + 2;
                }

                if (pos0 >= 0 && pos1 < 0 && codes[i].opcode == OpCodes.Leave)
                {
                    var label1 = iL.DefineLabel();
                    addedCodes1 = new List<CodeInstruction>
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld,
                            AccessTools.Field(typeof(EmotionBattleTeamModel), "_emotionTotalCoinNumber")),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Bge_S, label1),

                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stfld,
                            AccessTools.Field(typeof(EmotionBattleTeamModel), "_emotionTotalCoinNumber"))
                    };
                    var endPatch1 = new CodeInstruction(OpCodes.Nop);
                    endPatch1.labels.Add(label1);
                    addedCodes1.Add(endPatch1);
                    pos1 = i;
                }

                if (pos1 < 0 || codes[i].opcode != OpCodes.Endfinally) continue;
                var label2 = iL.DefineLabel();
                addedCodes2 = new List<CodeInstruction>
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld,
                        AccessTools.Field(typeof(EmotionBattleTeamModel), "_emotionTotalCoinNumber")),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bge_S, label2),

                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stfld,
                        AccessTools.Field(typeof(EmotionBattleTeamModel), "_emotionTotalCoinNumber"))
                };
                var endPatch2 = new CodeInstruction(OpCodes.Nop);
                endPatch2.labels.Add(label2);
                addedCodes2.Add(endPatch2);
                pos2 = i;
                break;
            }

            if (pos0 < 0 || addedCodes0 == null || pos1 < 0 || addedCodes1 == null || pos2 < 0 || addedCodes2 == null)
                return codes.AsEnumerable();
            codes.InsertRange(pos2, addedCodes2);
            codes.InsertRange(pos1, addedCodes1);
            codes.InsertRange(pos0, addedCodes0);
            return codes.AsEnumerable();
        }

        [HarmonyPrefix]
        public static void PrePatch(EmotionBattleTeamModel __instance, List<UnitBattleDataModel> ____unitlist,
            ref List<UnitBattleDataModel> __state)
        {
            _coinsToRemove = 0;
            __state = new List<UnitBattleDataModel>();
            if (StageController.StageState.Battle != StageController.Instance.State) return;
            var models = BattleObjectManager.instance.GetAliveList(__instance.faction).FindAll(Match)
                .Select(x => x.UnitData).ToList();
            if (models.Count < ____unitlist.Count)
                __state.AddRange(models.Where(____unitlist.Remove));
            else
                foreach (var num in ____unitlist.Select(x => x.emotionDetail.GetAccumulatedEmotionCoinNum()))
                    _coinsToRemove += num;
        }

        [HarmonyPostfix]
        public static void PostPatch(List<UnitBattleDataModel> ____unitlist, ref List<UnitBattleDataModel> __state)
        {
            ____unitlist.AddRange(__state);
        }
    }
}
