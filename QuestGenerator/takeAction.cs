using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using ThePlotLords.QuestBuilder;
using ThePlotLords.QuestBuilder.CustomBT;
using static ThePlotLords.QuestGenTestCampaignBehavior;

namespace ThePlotLords
{
    public class takeAction : actionTarget
    {
        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        [XmlIgnore]
        public Settlement settlementTarget;

        [XmlIgnore]
        public ItemObject itemTarget;

        public int itemAmount = 0;

        public int amountGot = 0;

        public int amountBeforeFight = 0;

        public bool settlementFlag = false;

        [XmlIgnore]
        public Dictionary<string, List<string>> itemsForEachCulture = new Dictionary<string, List<string>>();

        public takeAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
            itemsForEachCulture.Add("Looters", new List<string> { "Seax", "Falchion", "Small Spurred Axe", "Hatchet", "Sickle", "Pickaxe", "Wooden Hammer", "Blacksmith Hammer", "Pitchfork", "Pilgrim Hood", "Highland Wrapped Headcloth", "Padded Garments", "Torn Robe", "Commoner Shirt", "Commoner Tunic", "Rugged Gambeson", "Armwraps", "Leather Shoes", "Wrapped Shoes" });
            itemsForEachCulture.Add("Sea Raiders", new List<string> { "Gallogaich Axe", "Northern Roughhide Cap", "Scarf", "Northern Layered Cloth", "Wrapped Shoes", "Cleaver", "Harpoon", "Northern Warlord Helmet Over Mail", "Scarf", "Northern Leather Vest", "Wrapped Shoes", "Veteran Warriors Axe", "Northern Throwing Axe", "Northern Round Shield", "Northern Warlord Helmet", "Bear Pelt", "Decorated Northern Hauberk", "Mail Chausses" });
            itemsForEachCulture.Add("Mountain Bandits", new List<string> { "Falchion", "Northern Spiked Battle Axe", "Western Long Spear", "Highland Wrapped Headcloth", "Highland Shoulder Fur", "Baggy Trunks", "Armored Baggy Trunks", "Sleeveless Studded Fur Armor", "Guarded Armwraps", "Leather Shoes", "Highland Boots" });
            itemsForEachCulture.Add("Forest Bandits", new List<string> { "Simple Eastern Backsword", "Mountain Hunting Bow", "Woodland Garments", "Guarded Armwraps", "Ragged Boots", "Narrow Sword", "Ranger Bow", "Highland Hood", "Highland Cloak", "Rugged Gambeson", "Guarded Armwraps", "Ragged Boots", "One Handed Bearded Axe", "Woodland Longbow", "Wolf Head", "Wolf Shoulder", "Rough Fur Over Chain", "Woven Leather Braces", "Woodland Boots" });
            itemsForEachCulture.Add("Desert Bandits", new List<string> { "Southern Militia Mace", "Jagged Spear", "Southern Tribal Turban", "Southern Peasant Robe", "Wrapped Shoes", "Star Falchion", "Weighted Eastern Spear", "Open Head Scarf", "Southern Wrapped Scarf", "Southern Padded Cloth", "Guarded Padded Vambrace", "Wrapped Shoes", "Desert Long Sword", "Fine Steel Leaf Spear", "Jareed", "Makeshift Kite Shield", "Trailed Southern Helmet", "Southern Wrapped Scarf", "Southern Robe Over Mail", "Mail Mitten", "Mail Chausses" });
            itemsForEachCulture.Add("Steppe Bandits", new List<string> { "Eastern Light Mace", "Eastern Steppe Armor", "Wrapped Shoes", "Broad Idl", "Weighted Eastern Spear", "Heavy Recurve Bow", "Nomad Padded Hood", "Studded Steppe Leather", "Strapped Leather Boots", "Pilgrim Hood", "Highland Wrapped Headcloth", "Ridged Sabre", "Fine Steel Leaf Spear", "Steppe Recurve Bow", "Plumed Nomad Helmet", "Eastern Studded Shoulders", "Eastern Stitched Leather Coat", "Guarded Padded Vambrace", "Eastern Leather Boots" });

        }
        public takeAction() { }

        public override void SetItemAmount(int newIA)
        {
            itemAmount = newIA;
        }

        public override Hero GetHeroTarget()
        {
            return heroTarget;
        }

        public override void SetHeroTarget(Hero newH)
        {
            heroTarget = newH;
        }

        public override ItemObject GetItemTarget()
        {
            return itemTarget;
        }

        public override void SetItemTarget(ItemObject newI)
        {
            itemTarget = newI;
        }

        public override void bringTargetsBack()
        {
            if (heroTarget == null && !settlementFlag)
            {
                var setName = this.Action.param[0].target;

                heroTarget = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
            if (settlementTarget == null && settlementFlag)
            {
                var setName = this.Action.param[0].target;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("take action - line 90"));
                }
                if (array.Length == 1)
                {
                    settlementTarget = array[0];
                }
            }

            if (itemTarget == null)
            {
                var setName = this.Action.param[1].target;

                ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                if (array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("gather action - 74"));
                }
                if (array.Length >= 1)
                {
                    itemTarget = array[0];
                }

            }

            if (questGiver == null)
            {
                var setName = questGiverString;

                questGiver = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }

            if (itemsForEachCulture.IsEmpty())
            {
                itemsForEachCulture.Add("Looters", new List<string> { "Seax", "Falchion", "Small Spurred Axe", "Hatchet", "Sickle", "Pickaxe", "Wooden Hammer", "Blacksmith Hammer", "Pitchfork", "Pilgrim Hood", "Highland Wrapped Headcloth", "Padded Garments", "Torn Robe", "Commoner Shirt", "Commoner Tunic", "Rugged Gambeson", "Armwraps", "Leather Shoes", "Wrapped Shoes" });
                itemsForEachCulture.Add("Sea Raiders", new List<string> { "Gallogaich Axe", "Northern Roughhide Cap", "Scarf", "Northern Layered Cloth", "Wrapped Shoes", "Cleaver", "Harpoon", "Northern Warlord Helmet Over Mail", "Scarf", "Northern Leather Vest", "Wrapped Shoes", "Veteran Warriors Axe", "Northern Throwing Axe", "Northern Round Shield", "Northern Warlord Helmet", "Bear Pelt", "Decorated Northern Hauberk", "Mail Chausses" });
                itemsForEachCulture.Add("Mountain Bandits", new List<string> { "Falchion", "Northern Spiked Battle Axe", "Western Long Spear", "Highland Wrapped Headcloth", "Highland Shoulder Fur", "Baggy Trunks", "Armored Baggy Trunks", "Sleeveless Studded Fur Armor", "Guarded Armwraps", "Leather Shoes", "Highland Boots" });
                itemsForEachCulture.Add("Forest Bandits", new List<string> { "Simple Eastern Backsword", "Mountain Hunting Bow", "Woodland Garments", "Guarded Armwraps", "Ragged Boots", "Narrow Sword", "Ranger Bow", "Highland Hood", "Highland Cloak", "Rugged Gambeson", "Guarded Armwraps", "Ragged Boots", "One Handed Bearded Axe", "Woodland Longbow", "Wolf Head", "Wolf Shoulder", "Rough Fur Over Chain", "Woven Leather Braces", "Woodland Boots" });
                itemsForEachCulture.Add("Desert Bandits", new List<string> { "Southern Militia Mace", "Jagged Spear", "Southern Tribal Turban", "Southern Peasant Robe", "Wrapped Shoes", "Star Falchion", "Weighted Eastern Spear", "Open Head Scarf", "Southern Wrapped Scarf", "Southern Padded Cloth", "Guarded Padded Vambrace", "Wrapped Shoes", "Desert Long Sword", "Fine Steel Leaf Spear", "Jareed", "Makeshift Kite Shield", "Trailed Southern Helmet", "Southern Wrapped Scarf", "Southern Robe Over Mail", "Mail Mitten", "Mail Chausses" });
                itemsForEachCulture.Add("Steppe Bandits", new List<string> { "Eastern Light Mace", "Eastern Steppe Armor", "Wrapped Shoes", "Broad Idl", "Weighted Eastern Spear", "Heavy Recurve Bow", "Nomad Padded Hood", "Studded Steppe Leather", "Strapped Leather Boots", "Pilgrim Hood", "Highland Wrapped Headcloth", "Ridged Sabre", "Fine Steel Leaf Spear", "Steppe Recurve Bow", "Plumed Nomad Helmet", "Eastern Studded Shoulders", "Eastern Stitched Leather Coat", "Guarded Padded Vambrace", "Eastern Leather Boots" });

            }

        }

        public override void IssueQ(IssueBase questBase, QuestGenTestCampaignBehavior.QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("enemy"))
            {
                if (alternative)
                {
                    if (questGen.alternativeActionsInOrder[index - 1].action == "damage")
                    {
                        this.Action.param[0].target = questGen.alternativeActionsInOrder[index - 1].Action.param[0].target;
                        settlementFlag = true;
                        settlementTarget = SettlementHelper.FindNearestSettlement((Settlement x) => x.Name.ToString() == this.Action.param[0].target);
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("take action - line 146"));
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[index - 1].action == "damage")
                    {
                        this.Action.param[0].target = questGen.actionsInOrder[index - 1].Action.param[0].target;
                        settlementFlag = true;
                        settlementTarget = SettlementHelper.FindNearestSettlement((Settlement x) => x.Name.ToString() == this.Action.param[0].target);
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("take action - line 159"));
                    }
                }
            }

            if (this.Action.param[1].target.Contains("item"))
            {
                //string path = @"..\..\Modules\ThePlotLords\allItems.txt";
                //List<string> itemss = new List<string>();
                //foreach (ItemObject io in Items.All)
                //{
                //    itemss.Add(io.Name.ToString());
                //}
                //JsonSerialization.WriteToJsonFile<List<string>>(path, itemss);
                string itemNumb = this.Action.param[1].target;
                if (heroTarget != null && heroTarget.PartyBelongedTo != null)
                {
                    Hero toGiveHero = heroTarget;

                    TroopRoster troop = toGiveHero.PartyBelongedTo.MemberRoster;
                    int t = rnd.Next(troop.GetTroopRoster().Count);

                    TroopRosterElement troopRosterElement = troop.GetTroopRoster()[t];
                    IEnumerable<Equipment> battleequipment = troopRosterElement.Character.BattleEquipments;

                    int e = rnd.Next(battleequipment.Count());
                    Equipment equipment = battleequipment.ElementAt(e);

                    for (EquipmentIndex equipmentIndex = 0; equipmentIndex < EquipmentIndex.ArmorItemEndSlot; equipmentIndex++)
                    {
                        EquipmentElement equipmentElement = equipment[equipmentIndex];
                        if (!equipmentElement.IsEmpty && equipmentElement.Item.Value <= 300)
                        {
                            if (alternative)
                            {
                                questGen.alternativeMission.updateItemTargets(itemNumb, equipmentElement.Item);
                            }
                            else
                            {
                                questGen.chosenMission.updateItemTargets(itemNumb, equipmentElement.Item);
                            }
                            int amount = 300 / this.GetItemTarget().Value;
                            if (amount <= 0)
                            {
                                amount = 1;
                            }
                            itemAmount = amount;
                            break;
                        }
                    }


                }
                else if (settlementFlag)
                {
                    int i = index;

                    int amount = 20;

                    while (itemTarget == null && amount > 0)
                    {
                        foreach (ItemRosterElement itemRosterElement in settlementTarget.ItemRoster)
                        {
                            if (itemRosterElement.Amount >= amount)
                            {
                                if (alternative)
                                {
                                    questGen.alternativeMission.updateItemTargets(itemNumb, itemRosterElement.EquipmentElement.Item);
                                }
                                else
                                {
                                    questGen.chosenMission.updateItemTargets(itemNumb, itemRosterElement.EquipmentElement.Item);
                                }

                                int amount2 = 300 / this.GetItemTarget().Value;
                                if (amount2 <= 0)
                                {
                                    amount2 = 1;
                                }
                                if (amount2 >= amount)
                                {
                                    amount2 = amount;
                                }
                                itemAmount = amount2;
                                break;
                            }
                        }
                        amount -= 1;
                    }
                }
                else
                {
                    foreach (MobileParty m in MobileParty.All)
                    {
                        if (m.IsBandit && m.ActualClan.Culture.Name.ToString() == this.Action.param[0].target)
                        {
                            //TroopRoster troop = m.MemberRoster;
                            //int t = rnd.Next(troop.GetTroopRoster().Count);

                            //TroopRosterElement troopRosterElement = troop.GetTroopRoster()[t];
                            //Equipment equipment = troopRosterElement.Character.FirstBattleEquipment;

                            //for (EquipmentIndex equipmentIndex = 0; equipmentIndex < EquipmentIndex.ArmorItemEndSlot; equipmentIndex++)
                            //{
                            //    EquipmentElement equipmentElement = equipment[equipmentIndex];
                            //    if (!equipmentElement.IsEmpty && equipmentElement.Item.Value <= 300)
                            //    {
                            //        if (alternative)
                            //        {
                            //            questGen.alternativeMission.updateItemTargets(itemNumb, equipmentElement.Item);
                            //        }
                            //        else
                            //        {
                            //            questGen.chosenMission.updateItemTargets(itemNumb, equipmentElement.Item);
                            //        }
                            //        int amount = 300 / this.GetItemTarget().Value;
                            //        if (amount <= 0)
                            //        {
                            //            amount = 1;
                            //        }
                            //        this.itemAmount = amount;
                            //        break;
                            //    }
                            //}
                            if (itemsForEachCulture.ContainsKey(m.ActualClan.Culture.Name.ToString()))
                            {
                                int it = rnd.Next(itemsForEachCulture[m.ActualClan.Culture.Name.ToString()].Count);
                                var setName = itemsForEachCulture[m.ActualClan.Culture.Name.ToString()][it];

                                ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                                if (array.Length == 0)
                                {
                                    InformationManager.DisplayMessage(new InformationMessage("take action - line 106"));
                                }
                                if (array.Length >= 1)
                                {
                                    if (alternative)
                                    {
                                        questGen.alternativeMission.updateItemTargets(itemNumb, array[0]);
                                    }
                                    else
                                    {
                                        questGen.chosenMission.updateItemTargets(itemNumb, array[0]);
                                    };
                                }

                                if (itemTarget != null)
                                {
                                    int amount = 300 / itemTarget.Value;
                                    if (amount <= 0)
                                    {
                                        amount = 1;
                                    }
                                    itemAmount = amount;
                                    break;
                                }
                            }

                        }
                    }
                    if (itemTarget == null)
                    {
                        var setName = "Commoner Tunic";

                        ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                        if (array.Length == 0)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("take action - line 106"));
                        }
                        if (array.Length >= 1)
                        {
                            if (alternative)
                            {
                                questGen.alternativeMission.updateItemTargets(itemNumb, array[0]);
                            }
                            else
                            {
                                questGen.chosenMission.updateItemTargets(itemNumb, array[0]);
                            }
                        }

                        if (itemTarget != null)
                        {
                            int amount = 300 / this.itemTarget.Value;
                            if (amount <= 0)
                            {
                                amount = 1;
                            }
                            itemAmount = amount;
                        }
                        else
                        {
                            var setName2 = "Commoner Tunic";

                            ItemObject[] array2 = (from x in Items.All where (x.Name.ToString().Replace("\"", "") == setName2) select x).ToArray<ItemObject>();

                            if (array2.Length == 0)
                            {
                                InformationManager.DisplayMessage(new InformationMessage("take action - line 106"));
                            }
                            if (array2.Length >= 1)
                            {
                                if (alternative)
                                {
                                    questGen.alternativeMission.updateItemTargets(itemNumb, array2[0]);
                                }
                                else
                                {
                                    questGen.chosenMission.updateItemTargets(itemNumb, array2[0]);
                                }
                            }

                            int amount = 300 / this.itemTarget.Value;
                            if (amount <= 0)
                            {
                                amount = 1;
                            }
                            itemAmount = amount;
                        }

                    }

                }

            }

            else if (this.GetItemTarget() != null && this.GetItemAmount() == 0)
            {
                int amount = 300 / this.GetItemTarget().Value;
                if (amount <= 0)
                {
                    amount = 1;
                }
                this.SetItemAmount(amount);
            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestCampaignBehavior.QuestGenTestQuest questGen)
        {
            if (!actioncomplete && !actionInLog)
            {
                if (index == 0)
                {
                    actionInLog = true;
                    if (heroTarget != null)
                    {
                        questBase.AddTrackedObject(heroTarget.PartyBelongedTo);
                        TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {HERO}.", null);
                        textObject.SetTextVariable("HERO", heroTarget.Name);
                        textObject.SetTextVariable("ITEM", itemTarget.Name);
                        textObject.SetTextVariable("AMOUNT", itemAmount);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                    }
                    else if (settlementFlag)
                    {
                        questBase.AddTrackedObject(settlementTarget);
                        TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {SETTLEMENT}.", null);
                        textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                        textObject.SetTextVariable("ITEM", itemTarget.Name);
                        textObject.SetTextVariable("AMOUNT", itemAmount);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                    }
                    else
                    {
                        TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from a party of " + this.Action.param[0].target + ".", null);
                        textObject.SetTextVariable("ITEM", itemTarget.Name);
                        textObject.SetTextVariable("AMOUNT", itemAmount);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                    }
                }
                else
                {

                    if ((questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index) || ((questGen.actionsInOrder[index - 1].action == "damage" || questGen.actionsInOrder[index - 1].action == "kill") && questGen.actionsInOrder[index - 1].actionInLog))
                    {
                        actionInLog = true;
                        if (heroTarget != null)
                        {
                            questBase.AddTrackedObject(heroTarget.PartyBelongedTo);
                            TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {HERO}.", null);
                            textObject.SetTextVariable("HERO", heroTarget.Name);
                            textObject.SetTextVariable("ITEM", itemTarget.Name);
                            textObject.SetTextVariable("AMOUNT", itemAmount);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                            amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                        }
                        else if (settlementFlag)
                        {
                            questBase.AddTrackedObject(settlementTarget);
                            TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {SETTLEMENT}.", null);
                            textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                            textObject.SetTextVariable("ITEM", itemTarget.Name);
                            textObject.SetTextVariable("AMOUNT", itemAmount);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                            amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                        }
                        else
                        {
                            TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from a party of " + this.Action.param[0].target + ".", null);
                            textObject.SetTextVariable("ITEM", itemTarget.Name);
                            textObject.SetTextVariable("AMOUNT", itemAmount);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                            amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                        }
                    }
                }
            }


        }

        public override void MapEventEnded(MapEvent mapEvent, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (mapEvent.WinningSide != BattleSideEnum.None && mapEvent.DefeatedSide != BattleSideEnum.None)
            {
                MapEventSide mapEventSide = mapEvent.GetMapEventSide(mapEvent.WinningSide);
                MapEventSide mapEventSide2 = mapEvent.GetMapEventSide(mapEvent.DefeatedSide);
                if (heroTarget != null)
                {
                    if (mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party == heroTarget.PartyBelongedTo.Party))
                    {
                        int current = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                        amountGot += current - amountBeforeFight;

                        if (amountGot >= itemAmount)
                        {
                            this.takeConsequences(index, questBase, questGen);
                        }
                        else
                        {
                            questGen.UpdateQuestTaskS(questGen.journalLogs[index], amountGot);
                        }
                    }

                }
                else if (settlementFlag)
                {
                    if (mapEventSide.IsMainPartyAmongParties() && mapEvent.MapEventSettlement == settlementTarget)
                    {
                        int current = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                        amountGot += current - amountBeforeFight;

                        if (amountGot >= itemAmount)
                        {
                            this.takeConsequences(index, questBase, questGen);
                        }
                        else
                        {
                            questGen.UpdateQuestTaskS(questGen.journalLogs[index], amountGot);
                        }
                    }
                }
                else
                {
                    if (mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party.Culture.Name.ToString() == this.Action.param[0].target))
                    {
                        amountGot += 0;
                        int current = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                        amountGot += current - amountBeforeFight;

                        if (amountGot >= itemAmount)
                        {
                            this.takeConsequences(index, questBase, questGen);
                        }
                        else
                        {
                            questGen.UpdateQuestTaskS(questGen.journalLogs[index], amountGot);
                        }
                    }
                }

            }
        }

        public override void MapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

            MapEventSide mapEventSide = mapEvent.AttackerSide;
            MapEventSide mapEventSide2 = mapEvent.DefenderSide;
            if (heroTarget != null)
            {
                if ((mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party == heroTarget.PartyBelongedTo.Party))
                    || (mapEventSide2.IsMainPartyAmongParties() && mapEventSide.Parties.Any((MapEventParty t) => t.Party == heroTarget.PartyBelongedTo.Party)))
                {
                    amountBeforeFight = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                }

            }
            else if (settlementFlag)
            {
                if (mapEventSide.IsMainPartyAmongParties())
                {
                    foreach (MapEventParty t in mapEventSide2.Parties)
                    {
                        if (t.Party.Settlement != null)
                        {
                            if (t.Party.Settlement == settlementTarget)
                            {
                                amountBeforeFight = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                            }
                        }
                    }
                }
                else if (mapEventSide2.IsMainPartyAmongParties())
                {
                    foreach (MapEventParty t in mapEventSide.Parties)
                    {
                        if (t.Party.Settlement != null)
                        {
                            if (t.Party.Settlement == settlementTarget)
                            {
                                amountBeforeFight = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                            }
                        }
                    }
                }
            }
            else
            {
                if ((mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party.Culture.Name.ToString() == this.Action.param[0].target))
                    || (mapEventSide2.IsMainPartyAmongParties() && mapEventSide.Parties.Any((MapEventParty t) => t.Party.Culture.Name.ToString() == this.Action.param[0].target)))
                {
                    amountBeforeFight = PartyBase.MainParty.ItemRoster.GetItemNumber(itemTarget);
                }
            }


        }

        private void takeConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                if (index > 0)
                {
                    if ((questGen.actionsInOrder[index - 1].action == "damage" || questGen.actionsInOrder[index - 1].action == "kill"))
                    {
                        if (questGen.actionsInOrder[index - 1].actioncomplete)
                        {
                            questGen.currentActionIndex++;
                        }
                    }
                }

                else
                {
                    questGen.currentActionIndex++;
                }


                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], itemAmount);

                if (heroTarget != null || settlementTarget != null)
                {
                    MakePeaceAction.Apply(Hero.MainHero.MapFaction, questGiver.MapFaction);
                    //FactionManager.DeclareAlliance(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                }
                actioncomplete = true;
                questGen.chosenMission.run(CustomBTStep.questQ, questBase, questGen);

                if (questGen.currentActionIndex < questGen.actionsInOrder.Count)
                {
                    questGen.currentAction = questGen.actionsInOrder[questGen.currentActionIndex];
                }
                else
                {

                    questGen.SuccessConsequences();
                }
            }
        }

        public override void updateHeroTargets(string targetString, Hero targetHero)
        {
            foreach (Parameter p in this.Action.param)
            {
                if (p.target == targetString)
                {
                    p.target = targetHero.Name.ToString();
                    heroTarget = targetHero;
                    break;
                }
            }
        }

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
            foreach (Parameter p in this.Action.param)
            {
                if (p.target == targetString)
                {
                    p.target = targetItem.Name.ToString();
                    itemTarget = targetItem;
                    break;
                }
            }
        }

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
        }
        public override TextObject getDescription(string strategy, int pair)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Steal stuff":
                    if (heroTarget != null)
                    {
                        strat = new TextObject("There're some items I need you to steal from {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("There're some items I need you to steal from {SETTLEMENT}. Are you up for the task?", null);
                        strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("There're some items I need you to steal from a group of {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;

                case "Gather raw materials":
                    strat = new TextObject("I need you to gather {ITEM}. Can you do that for me?", null);
                    strat.SetTextVariable("HERO", itemTarget.Name);
                    break;

                case "Steal valuables for resale":
                    if (heroTarget != null)
                    {
                        strat = new TextObject("There're some items I need you to steal from {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("There're some items I need you to steal from {SETTLEMENT}. Are you up for the task?", null);
                        strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("There're some items I need you to steal from a group of {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
                case "Steal supplies":
                    if (heroTarget != null)
                    {
                        strat = new TextObject("There're some items I need you to steal from {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("There're some items I need you to steal from {SETTLEMENT}. Are you up for the task?", null);
                        strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("There're some items I need you to steal from a group of {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
            }
            return strat;
        }

        public override TextObject getTitle(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Steal stuff":
                    if (heroTarget != null)
                    {
                        strat = new TextObject("Steal from {HERO}.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Steal from {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("Steal from a group of {HERO}.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;

                case "Gather raw materials":
                    strat = new TextObject("Gather {ITEM}.", null);
                    strat.SetTextVariable("ITEM", itemTarget.Name);
                    break;

                case "Steal valuables for resale":
                    if (heroTarget != null)
                    {
                        strat = new TextObject("Steal from {HERO}.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Steal from {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("Steal from a group of {HERO}.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
                case "Steal supplies":
                    if (heroTarget != null)
                    {
                        strat = new TextObject("Steal from {HERO}.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Steal from {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("Steal from a group of {HERO}.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
            }
            return strat;
        }

        public override string getListenString(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Steal stuff":
                    if (heroTarget != null)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", heroTarget.LastSeenPlace.Name);
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                        strat.SetTextVariable("FACTION", settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;

                case "Gather raw materials":
                    strat = new TextObject("{ITEM} is a type of {TYPE}, belonging to the category of {CATEGORY} and is valued around {VALUE} gold coins.", null);
                    strat.SetTextVariable("ITEM", itemTarget.Name);
                    strat.SetTextVariable("TYPE", itemTarget.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", itemTarget.ItemCategory.ToString());
                    strat.SetTextVariable("VALUE", itemTarget.Value);
                    break;

                case "Steal valuables for resale":
                    if (heroTarget != null)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", heroTarget.LastSeenPlace.Name);
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                        strat.SetTextVariable("FACTION", settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
                case "Steal supplies":
                    if (heroTarget != null)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", heroTarget.LastSeenPlace.Name);
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                        strat.SetTextVariable("FACTION", settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
            }
            return strat.ToString();
        }

        public override TextObject getStepDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            if (heroTarget != null)
            {
                strat = new TextObject("Steal {AMOUNT} {ITEM} from {HERO}.", null);
                strat.SetTextVariable("HERO", heroTarget.Name);
                strat.SetTextVariable("AMOUNT", itemAmount);
                strat.SetTextVariable("ITEM", itemTarget.Name);
            }
            else if (settlementFlag)
            {
                strat = new TextObject("Steal {AMOUNT} {ITEM} from {SETTLEMENT}.", null);
                strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                strat.SetTextVariable("AMOUNT", itemAmount);
                strat.SetTextVariable("ITEM", itemTarget.Name);
            }
            else
            {
                strat = new TextObject("Steal {AMOUNT} {ITEM} from a group of {HERO}.", null);
                strat.SetTextVariable("HERO", this.Action.param[0].target);
                strat.SetTextVariable("AMOUNT", itemAmount);
                strat.SetTextVariable("ITEM", itemTarget.Name);
            }
            return strat;
        }

    }
}
