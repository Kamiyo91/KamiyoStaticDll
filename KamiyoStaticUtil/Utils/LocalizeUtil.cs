using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using HarmonyLib;
using KamiyoStaticBLL.Models;
using LOR_XML;
using Mod;
using UnityEngine;

namespace KamiyoStaticUtil.Utils
{
    public static class LocalizeUtil
    {
        public static void AddGlobalLocalize()
        {
            try
            {
                foreach (var item in ModParameters.PackageIds.Zip(ModParameters.Path,
                             (packageId, path) => new { path, packageId }))
                {
                    var dictionary =
                        typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all)
                                ?.GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as
                            Dictionary<string, BattleEffectText>;
                    var files = new DirectoryInfo(item.path + "/Localize/" + ModParameters.Language + "/EffectTexts")
                        .GetFiles();
                    foreach (var t in files)
                        using (var stringReader = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var battleEffectTextRoot =
                                (BattleEffectTextRoot)new XmlSerializer(typeof(BattleEffectTextRoot))
                                    .Deserialize(stringReader);
                            foreach (var battleEffectText in battleEffectTextRoot.effectTextList)
                            {
                                dictionary.Remove(battleEffectText.ID);
                                dictionary?.Add(battleEffectText.ID, battleEffectText);
                                ModParameters.EffectTexts.Remove(battleEffectText.ID);
                                ModParameters.EffectTexts.Add(battleEffectText.ID,
                                    new EffectTextModel { Name = battleEffectText.Name, Desc = battleEffectText.Desc });
                            }
                        }

                    files = new DirectoryInfo(item.path + "/Localize/" + ModParameters.Language + "/BattlesCards")
                        .GetFiles();
                    foreach (var t in files)
                        using (var stringReader2 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var battleCardDescRoot =
                                (BattleCardDescRoot)new XmlSerializer(typeof(BattleCardDescRoot)).Deserialize(
                                    stringReader2);
                            using (var enumerator =
                                   ItemXmlDataList.instance.GetAllWorkshopData()[item.packageId].GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    var card = enumerator.Current;
                                    card.workshopName = battleCardDescRoot.cardDescList
                                        .Find(x => x.cardID == card.id.id)
                                        .cardName;
                                }
                            }

                            typeof(ItemXmlDataList).GetField("_cardInfoTable", AccessTools.all)
                                .GetValue(ItemXmlDataList.instance);
                            using (var enumerator2 = ItemXmlDataList.instance.GetCardList()
                                       .FindAll(x => x.id.packageId == item.packageId).GetEnumerator())
                            {
                                while (enumerator2.MoveNext())
                                {
                                    var card = enumerator2.Current;
                                    card.workshopName = battleCardDescRoot.cardDescList
                                        .Find(x => x.cardID == card.id.id)
                                        .cardName;
                                    ItemXmlDataList.instance.GetCardItem(card.id).workshopName = card.workshopName;
                                }
                            }
                        }

                    files = new DirectoryInfo(item.path + "/Localize/" + ModParameters.Language + "/BattleDialog")
                        .GetFiles();
                    var dialogDictionary =
                        (Dictionary<string, BattleDialogRoot>)BattleDialogXmlList.Instance.GetType()
                            .GetField("_dictionary", AccessTools.all)
                            ?.GetValue(BattleDialogXmlList.Instance);
                    foreach (var t in files)
                        using (var stringReader = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var battleDialogList =
                                ((BattleDialogRoot)new XmlSerializer(typeof(BattleDialogRoot))
                                    .Deserialize(stringReader)).characterList;
                            foreach (var dialog in battleDialogList)
                            {
                                dialog.workshopId = item.packageId;
                                dialog.bookId = int.Parse(dialog.characterID);
                            }

                            if (!dialogDictionary.ContainsKey("Workshop")) continue;
                            dialogDictionary["Workshop"].characterList
                                .RemoveAll(x => x.workshopId.Equals(item.packageId));
                            if (dialogDictionary.ContainsKey("Workshop"))
                            {
                                dialogDictionary["Workshop"].characterList.AddRange(battleDialogList);
                            }
                            else
                            {
                                var battleDialogRoot = new BattleDialogRoot
                                {
                                    groupName = "Workshop",
                                    characterList = battleDialogList
                                };
                                dialogDictionary.Add("Workshop", battleDialogRoot);
                            }
                        }

                    files = new DirectoryInfo(item.path + "/Localize/" + ModParameters.Language + "/CharactersName")
                        .GetFiles();
                    foreach (var t in files)
                        using (var stringReader3 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var charactersNameRoot =
                                (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
                                    stringReader3);
                            using (var enumerator3 =
                                   Singleton<EnemyUnitClassInfoList>.Instance.GetAllWorkshopData()[item.packageId]
                                       .GetEnumerator())
                            {
                                while (enumerator3.MoveNext())
                                {
                                    var enemy = enumerator3.Current;
                                    enemy.name = charactersNameRoot.nameList.Find(x => x.ID == enemy.id.id).name;
                                    Singleton<EnemyUnitClassInfoList>.Instance.GetData(enemy.id).name = enemy.name;
                                    ModParameters.NameTexts.Remove(enemy.id.id.ToString());
                                    ModParameters.NameTexts.Add(enemy.id.id.ToString(), enemy.name);
                                }
                            }
                        }

                    files = new DirectoryInfo(item.path + "/Localize/" + ModParameters.Language + "/Books").GetFiles();
                    foreach (var t in files)
                        using (var stringReader4 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var bookDescRoot =
                                (BookDescRoot)new XmlSerializer(typeof(BookDescRoot)).Deserialize(stringReader4);
                            using (var enumerator4 =
                                   Singleton<BookXmlList>.Instance.GetAllWorkshopData()[item.packageId]
                                       .GetEnumerator())
                            {
                                while (enumerator4.MoveNext())
                                {
                                    var bookXml = enumerator4.Current;
                                    bookXml.InnerName = bookDescRoot.bookDescList.Find(x => x.bookID == bookXml.id.id)
                                        .bookName;
                                }
                            }

                            using (var enumerator5 = Singleton<BookXmlList>.Instance.GetList()
                                       .FindAll(x => x.id.packageId == item.packageId).GetEnumerator())
                            {
                                while (enumerator5.MoveNext())
                                {
                                    var bookXml = enumerator5.Current;
                                    bookXml.InnerName = bookDescRoot.bookDescList.Find(x => x.bookID == bookXml.id.id)
                                        .bookName;
                                    Singleton<BookXmlList>.Instance.GetData(bookXml.id).InnerName = bookXml.InnerName;
                                }
                            }

                            (typeof(BookDescXmlList).GetField("_dictionaryWorkshop", AccessTools.all)
                                        .GetValue(Singleton<BookDescXmlList>.Instance) as
                                    Dictionary<string, List<BookDesc>>)
                                [item.packageId] = bookDescRoot.bookDescList;
                        }

                    files = new DirectoryInfo(item.path + "/Localize/" + ModParameters.Language + "/DropBooks")
                        .GetFiles();
                    foreach (var t in files)
                        using (var stringReader5 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var charactersNameRoot2 =
                                (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
                                    stringReader5);
                            using (var enumerator6 =
                                   Singleton<DropBookXmlList>.Instance.GetAllWorkshopData()[item.packageId]
                                       .GetEnumerator())
                            {
                                while (enumerator6.MoveNext())
                                {
                                    var dropBook = enumerator6.Current;
                                    dropBook.workshopName =
                                        charactersNameRoot2.nameList.Find(x => x.ID == dropBook.id.id).name;
                                }
                            }

                            using (var enumerator7 = Singleton<DropBookXmlList>.Instance.GetList()
                                       .FindAll(x => x.id.packageId == item.packageId).GetEnumerator())
                            {
                                while (enumerator7.MoveNext())
                                {
                                    var dropBook = enumerator7.Current;
                                    dropBook.workshopName =
                                        charactersNameRoot2.nameList.Find(x => x.ID == dropBook.id.id).name;
                                    Singleton<DropBookXmlList>.Instance.GetData(dropBook.id).workshopName =
                                        dropBook.workshopName;
                                }
                            }
                        }

                    files = new DirectoryInfo(item.path + "/Localize/" + ModParameters.Language + "/StageName")
                        .GetFiles();
                    foreach (var t in files)
                        using (var stringReader6 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var charactersNameRoot3 =
                                (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
                                    stringReader6);
                            using (var enumerator8 =
                                   Singleton<StageClassInfoList>.Instance.GetAllWorkshopData()[item.packageId]
                                       .GetEnumerator())
                            {
                                while (enumerator8.MoveNext())
                                {
                                    var stage = enumerator8.Current;
                                    stage.stageName = charactersNameRoot3.nameList.Find(x => x.ID == stage.id.id).name;
                                }
                            }
                        }

                    files = new DirectoryInfo(item.path + "/Localize/" + ModParameters.Language + "/PassiveDesc")
                        .GetFiles();
                    foreach (var t in files)
                        using (var stringReader7 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var passiveDescRoot =
                                (PassiveDescRoot)new XmlSerializer(typeof(PassiveDescRoot)).Deserialize(stringReader7);
                            using (var enumerator9 = Singleton<PassiveXmlList>.Instance.GetDataAll()
                                       .FindAll(x => x.id.packageId == item.packageId).GetEnumerator())
                            {
                                while (enumerator9.MoveNext())
                                {
                                    var passive = enumerator9.Current;
                                    passive.name = passiveDescRoot.descList.Find(x => x.ID == passive.id.id).name;
                                    passive.desc = passiveDescRoot.descList.Find(x => x.ID == passive.id.id).desc;
                                }
                            }
                        }

                    var cardAbilityDictionary = typeof(BattleCardAbilityDescXmlList)
                            .GetField("_dictionary", AccessTools.all)
                            ?.GetValue(Singleton<BattleCardAbilityDescXmlList>.Instance) as
                        Dictionary<string, BattleCardAbilityDesc>;
                    files = new DirectoryInfo(item.path + "/Localize/" + ModParameters.Language +
                                              "/BattleCardAbilities").GetFiles();
                    foreach (var t in files)
                        using (var stringReader8 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            foreach (var battleCardAbilityDesc in
                                     ((BattleCardAbilityDescRoot)new XmlSerializer(typeof(BattleCardAbilityDescRoot))
                                         .Deserialize(stringReader8)).cardDescList)
                            {
                                cardAbilityDictionary.Remove(battleCardAbilityDesc.id);
                                cardAbilityDictionary.Add(battleCardAbilityDesc.id, battleCardAbilityDesc);
                            }
                        }
                }
            }
            catch (Exception)
            {
                Debug.LogError("Something failed while changing language");
            }

        }
        public static void AddLocalLocalize(string path,string packageId)
        {
            try
            {
                var dictionary =
                    typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all)
                        ?.GetValue(Singleton<BattleEffectTextsXmlList>
                            .Instance) as Dictionary<string, BattleEffectText>;
                var files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/EffectTexts")
                    .GetFiles();
                foreach (var t in files)
                    using (var stringReader = new StringReader(File.ReadAllText(t.FullName)))
                    {
                        var battleEffectTextRoot =
                            (BattleEffectTextRoot)new XmlSerializer(typeof(BattleEffectTextRoot))
                                .Deserialize(stringReader);
                        foreach (var battleEffectText in battleEffectTextRoot.effectTextList)
                        {
                            dictionary.Remove(battleEffectText.ID);
                            dictionary?.Add(battleEffectText.ID, battleEffectText);
                            ModParameters.EffectTexts.Remove(battleEffectText.ID);
                            ModParameters.EffectTexts.Add(battleEffectText.ID,
                                new EffectTextModel { Name = battleEffectText.Name, Desc = battleEffectText.Desc });
                        }
                    }

                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/BattlesCards")
                    .GetFiles();
                foreach (var t in files)
                    using (var stringReader2 = new StringReader(File.ReadAllText(t.FullName)))
                    {
                        var battleCardDescRoot =
                            (BattleCardDescRoot)new XmlSerializer(typeof(BattleCardDescRoot)).Deserialize(
                                stringReader2);
                        using (var enumerator =
                               ItemXmlDataList.instance.GetAllWorkshopData()[packageId].GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                var card = enumerator.Current;
                                card.workshopName = battleCardDescRoot.cardDescList.Find(x => x.cardID == card.id.id)
                                    .cardName;
                            }
                        }

                        typeof(ItemXmlDataList).GetField("_cardInfoTable", AccessTools.all)
                            .GetValue(ItemXmlDataList.instance);
                        using (var enumerator2 = ItemXmlDataList.instance.GetCardList()
                                   .FindAll(x => x.id.packageId == packageId).GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                var card = enumerator2.Current;
                                card.workshopName = battleCardDescRoot.cardDescList.Find(x => x.cardID == card.id.id)
                                    .cardName;
                                ItemXmlDataList.instance.GetCardItem(card.id).workshopName = card.workshopName;
                            }
                        }
                    }

                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/BattleDialog")
                    .GetFiles();
                var dialogDictionary =
                    (Dictionary<string, BattleDialogRoot>)BattleDialogXmlList.Instance.GetType()
                        .GetField("_dictionary", AccessTools.all)
                        ?.GetValue(BattleDialogXmlList.Instance);
                foreach (var t in files)
                    using (var stringReader = new StringReader(File.ReadAllText(t.FullName)))
                    {
                        var battleDialogList =
                            ((BattleDialogRoot)new XmlSerializer(typeof(BattleDialogRoot))
                                .Deserialize(stringReader)).characterList;
                        foreach (var dialog in battleDialogList)
                        {
                            dialog.workshopId = packageId;
                            dialog.bookId = int.Parse(dialog.characterID);
                        }

                        if (!dialogDictionary.ContainsKey("Workshop")) continue;
                        dialogDictionary["Workshop"].characterList
                            .RemoveAll(x => x.workshopId.Equals(packageId));
                        if (dialogDictionary.ContainsKey("Workshop"))
                        {
                            dialogDictionary["Workshop"].characterList.AddRange(battleDialogList);
                        }
                        else
                        {
                            var battleDialogRoot = new BattleDialogRoot
                            {
                                groupName = "Workshop",
                                characterList = battleDialogList
                            };
                            dialogDictionary.Add("Workshop", battleDialogRoot);
                        }
                    }

                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/CharactersName")
                    .GetFiles();
                foreach (var t in files)
                    using (var stringReader3 = new StringReader(File.ReadAllText(t.FullName)))
                    {
                        var charactersNameRoot =
                            (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
                                stringReader3);
                        using (var enumerator3 =
                               Singleton<EnemyUnitClassInfoList>.Instance.GetAllWorkshopData()[packageId]
                                   .GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                var enemy = enumerator3.Current;
                                enemy.name = charactersNameRoot.nameList.Find(x => x.ID == enemy.id.id).name;
                                Singleton<EnemyUnitClassInfoList>.Instance.GetData(enemy.id).name = enemy.name;
                                ModParameters.NameTexts.Remove(enemy.id.id.ToString());
                                ModParameters.NameTexts.Add(enemy.id.id.ToString(), enemy.name);
                            }
                        }
                    }

                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/Books").GetFiles();
                foreach (var t in files)
                    using (var stringReader4 = new StringReader(File.ReadAllText(t.FullName)))
                    {
                        var bookDescRoot =
                            (BookDescRoot)new XmlSerializer(typeof(BookDescRoot)).Deserialize(stringReader4);
                        using (var enumerator4 =
                               Singleton<BookXmlList>.Instance.GetAllWorkshopData()[packageId]
                                   .GetEnumerator())
                        {
                            while (enumerator4.MoveNext())
                            {
                                var bookXml = enumerator4.Current;
                                bookXml.InnerName = bookDescRoot.bookDescList.Find(x => x.bookID == bookXml.id.id)
                                    .bookName;
                            }
                        }

                        using (var enumerator5 = Singleton<BookXmlList>.Instance.GetList()
                                   .FindAll(x => x.id.packageId == packageId).GetEnumerator())
                        {
                            while (enumerator5.MoveNext())
                            {
                                var bookXml = enumerator5.Current;
                                bookXml.InnerName = bookDescRoot.bookDescList.Find(x => x.bookID == bookXml.id.id)
                                    .bookName;
                                Singleton<BookXmlList>.Instance.GetData(bookXml.id).InnerName = bookXml.InnerName;
                            }
                        }

                        (typeof(BookDescXmlList).GetField("_dictionaryWorkshop", AccessTools.all)
                                .GetValue(Singleton<BookDescXmlList>.Instance) as Dictionary<string, List<BookDesc>>)
                            [packageId] = bookDescRoot.bookDescList;
                    }

                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/DropBooks")
                    .GetFiles();
                foreach (var t in files)
                    using (var stringReader5 = new StringReader(File.ReadAllText(t.FullName)))
                    {
                        var charactersNameRoot2 =
                            (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
                                stringReader5);
                        using (var enumerator6 =
                               Singleton<DropBookXmlList>.Instance.GetAllWorkshopData()[packageId]
                                   .GetEnumerator())
                        {
                            while (enumerator6.MoveNext())
                            {
                                var dropBook = enumerator6.Current;
                                dropBook.workshopName =
                                    charactersNameRoot2.nameList.Find(x => x.ID == dropBook.id.id).name;
                            }
                        }

                        using (var enumerator7 = Singleton<DropBookXmlList>.Instance.GetList()
                                   .FindAll(x => x.id.packageId == packageId).GetEnumerator())
                        {
                            while (enumerator7.MoveNext())
                            {
                                var dropBook = enumerator7.Current;
                                dropBook.workshopName =
                                    charactersNameRoot2.nameList.Find(x => x.ID == dropBook.id.id).name;
                                Singleton<DropBookXmlList>.Instance.GetData(dropBook.id).workshopName =
                                    dropBook.workshopName;
                            }
                        }
                    }

                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/StageName")
                    .GetFiles();
                foreach (var t in files)
                    using (var stringReader6 = new StringReader(File.ReadAllText(t.FullName)))
                    {
                        var charactersNameRoot3 =
                            (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
                                stringReader6);
                        using (var enumerator8 =
                               Singleton<StageClassInfoList>.Instance.GetAllWorkshopData()[packageId]
                                   .GetEnumerator())
                        {
                            while (enumerator8.MoveNext())
                            {
                                var stage = enumerator8.Current;
                                stage.stageName = charactersNameRoot3.nameList.Find(x => x.ID == stage.id.id).name;
                            }
                        }
                    }

                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/PassiveDesc")
                    .GetFiles();
                foreach (var t in files)
                    using (var stringReader7 = new StringReader(File.ReadAllText(t.FullName)))
                    {
                        var passiveDescRoot =
                            (PassiveDescRoot)new XmlSerializer(typeof(PassiveDescRoot)).Deserialize(stringReader7);
                        using (var enumerator9 = Singleton<PassiveXmlList>.Instance.GetDataAll()
                                   .FindAll(x => x.id.packageId == packageId).GetEnumerator())
                        {
                            while (enumerator9.MoveNext())
                            {
                                var passive = enumerator9.Current;
                                passive.name = passiveDescRoot.descList.Find(x => x.ID == passive.id.id).name;
                                passive.desc = passiveDescRoot.descList.Find(x => x.ID == passive.id.id).desc;
                            }
                        }
                    }

                var cardAbilityDictionary = typeof(BattleCardAbilityDescXmlList)
                        .GetField("_dictionary", AccessTools.all)
                        ?.GetValue(Singleton<BattleCardAbilityDescXmlList>.Instance) as
                    Dictionary<string, BattleCardAbilityDesc>;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language +
                                          "/BattleCardAbilities").GetFiles();
                foreach (var t in files)
                    using (var stringReader8 = new StringReader(File.ReadAllText(t.FullName)))
                    {
                        foreach (var battleCardAbilityDesc in
                                 ((BattleCardAbilityDescRoot)new XmlSerializer(typeof(BattleCardAbilityDescRoot))
                                     .Deserialize(stringReader8)).cardDescList)
                        {
                            cardAbilityDictionary.Remove(battleCardAbilityDesc.id);
                            cardAbilityDictionary.Add(battleCardAbilityDesc.id, battleCardAbilityDesc);
                        }
                    }
            }
            catch (Exception)
            {
                Debug.LogError("Something failed while loading the language");
            }
        }
        public static void RemoveError()
        {
            Singleton<ModContentManager>.Instance.GetErrorLogs().RemoveAll(x => new List<string>
            {
                "0Harmony",
                "Mono.Cecil",
                "MonoMod.RuntimeDetour",
                "MonoMod.Utils"
            }.Exists(y => x.Contains("The same assembly name already exists. : " + y)));
        }
    }
}