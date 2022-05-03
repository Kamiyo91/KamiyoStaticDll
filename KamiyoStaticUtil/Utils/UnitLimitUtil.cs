using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UI;

namespace KamiyoStaticUtil.Utils
{
    public static class UnitLimitUtil
    {
        public static void AddFormationPosition(FormationModel Formation)
        {
            var list = (List<FormationPosition>)Formation.GetType().GetField("_postionList", AccessTools.all)
                ?.GetValue(Formation);
            if (list == null) return;
            for (var i = list.Count; i < 99; i++)
            {
                var info = new FormationPositionXmlData
                {
                    name = "E" + i,
                    vector = new XmlVector2
                    {
                        x = GetVector2X(i - 4),
                        y = GetVector2Y(i - 4)
                    },
                    eventList = null
                };
                var item = new FormationPosition(info)
                {
                    eventList = new List<FormationPositionEvent>(),
                    index = i
                };
                list.Add(item);
            }
        }

        public static int GetVector2X(int i)
        {
            int result;
            switch (i)
            {
                case 1:
                    result = 12;
                    break;
                case 2:
                    result = 12;
                    break;
                case 3:
                    result = 9;
                    break;
                case 4:
                    result = 9;
                    break;
                case 5:
                    result = 8;
                    break;
                case 6:
                    result = 8;
                    break;
                case 7:
                    result = 21;
                    break;
                case 8:
                    result = 21;
                    break;
                case 9:
                    result = 20;
                    break;
                case 10:
                    result = 20;
                    break;
                case 11:
                    result = 2;
                    break;
                case 12:
                    result = 2;
                    break;
                case 13:
                    result = 22;
                    break;
                case 14:
                    result = 22;
                    break;
                case 15:
                    result = 22;
                    break;
                default:
                    result = 12;
                    break;
            }

            return result;
        }

        public static int GetVector2Y(int i)
        {
            int result;
            switch (i)
            {
                case 1:
                    result = 7;
                    break;
                case 2:
                    result = -9;
                    break;
                case 3:
                    result = -5;
                    break;
                case 4:
                    result = -15;
                    break;
                case 5:
                    result = 19;
                    break;
                case 6:
                    result = 9;
                    break;
                case 7:
                    result = 19;
                    break;
                case 8:
                    result = 9;
                    break;
                case 9:
                    result = -5;
                    break;
                case 10:
                    result = -15;
                    break;
                case 11:
                    result = -14;
                    break;
                case 12:
                    result = 14;
                    break;
                case 13:
                    result = -16;
                    break;
                case 14:
                    result = 0;
                    break;
                case 15:
                    result = 16;
                    break;
                default:
                    result = 0;
                    break;
            }

            return result;
        }

        public static void AddFormationPositionForEnemy(FormationModel Formation)
        {
            var list = (List<FormationPosition>)Formation.GetType().GetField("_postionList", AccessTools.all)
                ?.GetValue(Formation);
            var num = -23;
            var num2 = 18;
            if (list == null) return;
            for (var i = list.Count; i < 99; i++)
            {
                var info = new FormationPositionXmlData
                {
                    name = "E" + i,
                    vector = new XmlVector2
                    {
                        x = num,
                        y = num2
                    },
                    eventList = null
                };
                num += 5;
                if (num > -3)
                {
                    num2 -= 7;
                    num = -23;
                }

                if (num2 < -17)
                {
                    num = -12;
                    num2 = 0;
                }

                var item = new FormationPosition(info)
                {
                    eventList = new List<FormationPositionEvent>(),
                    index = i
                };
                list.Add(item);
            }
        }

        public static IEnumerator RenderCam_2(int index, UICharacterRenderer renderer)
        {
            yield return YieldCache.waitFrame;
            renderer.cameraList[index].targetTexture.Release();
            renderer.cameraList[index].Render();
        }
    }
}