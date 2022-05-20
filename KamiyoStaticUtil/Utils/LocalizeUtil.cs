using System;
using System.Collections.Generic;
using System.IO;
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
            foreach (var item in ModParameters.LocalizePackageIdAndPath)
            {
                var error = false;
                FileInfo[] files;
                try
                {
                    var dictionary =
                        typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all)
                                ?.GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as
                            Dictionary<string, BattleEffectText>;
                    files = new DirectoryInfo(item.Value + "/Localize/" + ModParameters.Language + "/EffectTexts")
                        .GetFiles();
                    error = true;
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
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Effect Texts packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    files = new DirectoryInfo(item.Value + "/Localize/" + ModParameters.Language + "/BattlesCards")
                        .GetFiles();
                    error = true;
                    foreach (var t in files)
                        using (var stringReader2 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var battleCardDescRoot =
                                (BattleCardDescRoot)new XmlSerializer(typeof(BattleCardDescRoot)).Deserialize(
                                    stringReader2);
                            using (var enumerator =
                                   ItemXmlDataList.instance.GetAllWorkshopData()[item.Key].GetEnumerator())
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
                                       .FindAll(x => x.id.packageId == item.Key).GetEnumerator())
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
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Battle Cards Texts packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    files = new DirectoryInfo(item.Value + "/Localize/" + ModParameters.Language + "/BattleDialog")
                        .GetFiles();
                    var dialogDictionary =
                        (Dictionary<string, BattleDialogRoot>)BattleDialogXmlList.Instance.GetType()
                            .GetField("_dictionary", AccessTools.all)
                            ?.GetValue(BattleDialogXmlList.Instance);
                    error = true;
                    foreach (var t in files)
                        using (var stringReader = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var battleDialogList =
                                ((BattleDialogRoot)new XmlSerializer(typeof(BattleDialogRoot))
                                    .Deserialize(stringReader)).characterList;
                            foreach (var dialog in battleDialogList)
                            {
                                dialog.workshopId = item.Key;
                                dialog.bookId = int.Parse(dialog.characterID);
                            }

                            if (!dialogDictionary.ContainsKey("Workshop")) continue;
                            dialogDictionary["Workshop"].characterList
                                .RemoveAll(x => x.workshopId.Equals(item.Key));
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
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Battle Dialogs Texts packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    files = new DirectoryInfo(item.Value + "/Localize/" + ModParameters.Language + "/CharactersName")
                        .GetFiles();
                    error = true;
                    foreach (var t in files)
                        using (var stringReader3 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var charactersNameRoot =
                                (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
                                    stringReader3);
                            using (var enumerator3 =
                                   Singleton<EnemyUnitClassInfoList>.Instance.GetAllWorkshopData()[item.Key]
                                       .GetEnumerator())
                            {
                                while (enumerator3.MoveNext())
                                {
                                    var enemy = enumerator3.Current;
                                    enemy.name = charactersNameRoot.nameList.Find(x => x.ID == enemy.id.id).name;
                                    Singleton<EnemyUnitClassInfoList>.Instance.GetData(enemy.id).name = enemy.name;
                                    ModParameters.NameTexts.Remove(enemy.id);
                                    ModParameters.NameTexts.Add(enemy.id, enemy.name);
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Characters Name Texts packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    files = new DirectoryInfo(item.Value + "/Localize/" + ModParameters.Language + "/Books").GetFiles();
                    error = true;
                    foreach (var t in files)
                        using (var stringReader4 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var bookDescRoot =
                                (BookDescRoot)new XmlSerializer(typeof(BookDescRoot)).Deserialize(stringReader4);
                            using (var enumerator4 =
                                   Singleton<BookXmlList>.Instance.GetAllWorkshopData()[item.Key]
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
                                       .FindAll(x => x.id.packageId == item.Value).GetEnumerator())
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
                                [item.Key] = bookDescRoot.bookDescList;
                        }
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Books Texts packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    files = new DirectoryInfo(item.Value + "/Localize/" + ModParameters.Language + "/DropBooks")
                        .GetFiles();
                    error = true;
                    foreach (var t in files)
                        using (var stringReader5 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var charactersNameRoot2 =
                                (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
                                    stringReader5);
                            using (var enumerator6 =
                                   Singleton<DropBookXmlList>.Instance.GetAllWorkshopData()[item.Key]
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
                                       .FindAll(x => x.id.packageId == item.Key).GetEnumerator())
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
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Drop Books packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    files = new DirectoryInfo(item.Value + "/Localize/" + ModParameters.Language + "/StageName")
                        .GetFiles();
                    error = true;
                    foreach (var t in files)
                        using (var stringReader6 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var charactersNameRoot3 =
                                (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
                                    stringReader6);
                            using (var enumerator8 =
                                   Singleton<StageClassInfoList>.Instance.GetAllWorkshopData()[item.Key]
                                       .GetEnumerator())
                            {
                                while (enumerator8.MoveNext())
                                {
                                    var stage = enumerator8.Current;
                                    stage.stageName = charactersNameRoot3.nameList.Find(x => x.ID == stage.id.id).name;
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Stage Names Texts packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    files = new DirectoryInfo(item.Value + "/Localize/" + ModParameters.Language + "/PassiveDesc")
                        .GetFiles();
                    error = true;
                    foreach (var t in files)
                        using (var stringReader7 = new StringReader(File.ReadAllText(t.FullName)))
                        {
                            var passiveDescRoot =
                                (PassiveDescRoot)new XmlSerializer(typeof(PassiveDescRoot)).Deserialize(stringReader7);
                            using (var enumerator9 = Singleton<PassiveXmlList>.Instance.GetDataAll()
                                       .FindAll(x => x.id.packageId == item.Key).GetEnumerator())
                            {
                                while (enumerator9.MoveNext())
                                {
                                    var passive = enumerator9.Current;
                                    passive.name = passiveDescRoot.descList.Find(x => x.ID == passive.id.id).name;
                                    passive.desc = passiveDescRoot.descList.Find(x => x.ID == passive.id.id).desc;
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Passive Desc Texts packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    var cardAbilityDictionary = typeof(BattleCardAbilityDescXmlList)
                            .GetField("_dictionary", AccessTools.all)
                            ?.GetValue(Singleton<BattleCardAbilityDescXmlList>.Instance) as
                        Dictionary<string, BattleCardAbilityDesc>;
                    files = new DirectoryInfo(item.Value + "/Localize/" + ModParameters.Language +
                                              "/BattleCardAbilities").GetFiles();
                    error = true;
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
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Battle Card Abilities Texts packageId : " + item.Key +
                                       " Language : " + ModParameters.Language + " Error : " + ex.Message);
                }
            }
        }

        public static void AddLocalLocalize(string path, string packageId)
        {
            var error = false;
            FileInfo[] files;
            try
            {
                var dictionary =
                    typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all)
                            ?.GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as
                        Dictionary<string, BattleEffectText>;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/EffectTexts")
                    .GetFiles();
                error = true;
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
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Effect Texts packageId : " + packageId + " Language : " +
                                   ModParameters.Language + " Error : " + ex.Message);
            }

            try
            {
                error = false;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/BattlesCards")
                    .GetFiles();
                error = true;
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
                                card.workshopName = battleCardDescRoot.cardDescList
                                    .Find(x => x.cardID == card.id.id)
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
                                card.workshopName = battleCardDescRoot.cardDescList
                                    .Find(x => x.cardID == card.id.id)
                                    .cardName;
                                ItemXmlDataList.instance.GetCardItem(card.id).workshopName = card.workshopName;
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Battle Cards Texts packageId : " + packageId + " Language : " +
                                   ModParameters.Language + " Error : " + ex.Message);
            }

            try
            {
                error = false;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/BattleDialog")
                    .GetFiles();
                var dialogDictionary =
                    (Dictionary<string, BattleDialogRoot>)BattleDialogXmlList.Instance.GetType()
                        .GetField("_dictionary", AccessTools.all)
                        ?.GetValue(BattleDialogXmlList.Instance);
                error = true;
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
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Battle Dialogs Texts packageId : " + packageId + " Language : " +
                                   ModParameters.Language + " Error : " + ex.Message);
            }

            try
            {
                error = false;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/CharactersName")
                    .GetFiles();
                error = true;
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
                                ModParameters.NameTexts.Remove(enemy.id);
                                ModParameters.NameTexts.Add(enemy.id, enemy.name);
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Characters Name Texts packageId : " + packageId + " Language : " +
                                   ModParameters.Language + " Error : " + ex.Message);
            }

            try
            {
                error = false;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/Books").GetFiles();
                error = true;
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
                                    .GetValue(Singleton<BookDescXmlList>.Instance) as
                                Dictionary<string, List<BookDesc>>)
                            [packageId] = bookDescRoot.bookDescList;
                    }
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Books Texts packageId : " + packageId + " Language : " +
                                   ModParameters.Language + " Error : " + ex.Message);
            }

            try
            {
                error = false;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/DropBooks")
                    .GetFiles();
                error = true;
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
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Drop Books Texts packageId : " + packageId + " Language : " +
                                   ModParameters.Language + " Error : " + ex.Message);
            }

            try
            {
                error = false;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/StageName")
                    .GetFiles();
                error = true;
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
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Stage Names Texts packageId : " + packageId + " Language : " +
                                   ModParameters.Language + " Error : " + ex.Message);
            }

            try
            {
                error = false;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language + "/PassiveDesc")
                    .GetFiles();
                error = true;
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
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Passive Desc Texts packageId : " + packageId + " Language : " +
                                   ModParameters.Language + " Error : " + ex.Message);
            }

            try
            {
                error = false;
                var cardAbilityDictionary = typeof(BattleCardAbilityDescXmlList)
                        .GetField("_dictionary", AccessTools.all)
                        ?.GetValue(Singleton<BattleCardAbilityDescXmlList>.Instance) as
                    Dictionary<string, BattleCardAbilityDesc>;
                files = new DirectoryInfo(path + "/Localize/" + ModParameters.Language +
                                          "/BattleCardAbilities").GetFiles();
                error = true;
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
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Battle Card Abilities Texts packageId : " + packageId +
                                   " Language : " +
                                   ModParameters.Language + " Error : " + ex.Message);
            }
        }


        public static void RemoveError()
        {
            Singleton<ModContentManager>.Instance.GetErrorLogs().RemoveAll(x => new List<string>
            {
                "0Harmony",
                "Mono.Cecil",
                "MonoMod.RuntimeDetour",
                "MonoMod.Utils",
                "0KamiyoStaticHarmony",
                "KamiyoStaticBLL",
                "KamiyoStaticUtil"
            }.Exists(y => x.Contains("The same assembly name already exists. : " + y)));
        }
    }
}