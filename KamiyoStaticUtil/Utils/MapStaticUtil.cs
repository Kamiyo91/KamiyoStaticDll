using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using KamiyoStaticBLL.Models;
using UnityEngine;

namespace KamiyoStaticUtil.Utils
{
    public static class MapStaticUtil
    {
        public static void ActiveCreatureBattleCamFilterComponent(bool value = true)
        {
            var battleCamera = (Camera)typeof(BattleCamManager).GetField("_effectCam",
                AccessTools.all)?.GetValue(SingletonBehavior<BattleCamManager>.Instance);
            if (!(battleCamera is null)) battleCamera.GetComponent<CameraFilterPack_Drawing_Paper3>().enabled = value;
        }

        public static bool CheckStageMap(List<LorId> ids)
        {
            return ModParameters.PackageIds.Contains(Singleton<StageController>.Instance.GetStageModel().ClassInfo.id
                       .packageId) &&
                   ids.Contains(Singleton<StageController>.Instance.GetStageModel().ClassInfo.id);
        }

        public static void RemoveValueInAddedMap(string name, bool removeAll = false)
        {
            var mapList = (List<MapManager>)typeof(BattleSceneRoot).GetField("_addedMapList",
                AccessTools.all)?.GetValue(SingletonBehavior<BattleSceneRoot>.Instance);
            if (removeAll)
                mapList?.Clear();
            else
                mapList?.RemoveAll(x => x.name.Contains(name));
        }

        public static void MapChangedValue(bool value)
        {
            typeof(StageController).GetField("_mapChanged", AccessTools.all)
                ?.SetValue(Singleton<StageController>.Instance, value);
        }

        public static void LoadBoomEffect()
        {
            var map = Util.LoadPrefab("InvitationMaps/InvitationMap_BlackSilence4",
                SingletonBehavior<BattleSceneRoot>.Instance.transform);
            ModParameters.BoomEffectMap = map.GetComponent<MapManager>() as BlackSilence4thMapManager;
            Object.Destroy(map);
        }

        public static void UnloadBoomEffect()
        {
            Object.Destroy(ModParameters.BoomEffectMap);
            ModParameters.BoomEffectMap = null;
        }

        public static void GetArtWorks(DirectoryInfo dir)
        {
            if (dir.GetDirectories().Length != 0)
            {
                var directories = dir.GetDirectories();
                foreach (var t in directories) GetArtWorks(t);
            }

            foreach (var fileInfo in dir.GetFiles())
            {
                var texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                var value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                    new Vector2(0f, 0f));
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                ModParameters.ArtWorks[fileNameWithoutExtension] = value;
            }
        }
    }
}