namespace KamiyoStaticHarmony.Harmony
{
    public class KamiyoStaticHarmonyInit : ModInitializer
    {
        public override void OnInitializeMod()
        {
            new HarmonyLib.Harmony("LOR.KamiyoHarmonyPatch_MOD").PatchAll();
        }
    }
}