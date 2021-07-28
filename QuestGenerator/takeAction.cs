using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using System.Xml.Serialization;
using static QuestGenerator.QuestGenTestCampaignBehavior;
using QuestGenerator.QuestBuilder;
using Helpers;
using QuestGenerator.QuestBuilder.CustomBT;

namespace QuestGenerator
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

        public takeAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
            itemsForEachCulture.Add("Looters", new List<string> { "Seax", "Falchion", "Small Spurred Axe", "Hatchet", "Sickle", "Pickaxe", "Wooden Hammer", "Blacksmith Hammer", "Pitchfork", "Pilgrim Hood", "Highland Wrapped Headcloth", "Padded Garments", "Torn Robe", "Commoner Shirt", "Commoner Tunic", "Rugged Gambeson", "Armwraps", "Leather Shoes", "Wrapped Shoes" });
            itemsForEachCulture.Add("Sea Raiders", new List<string> { "Gallogaich Axe", "Northern Roughhide Cap", "Scarf", "Northern Layered Cloth", "Wrapped Shoes", "Cleaver", "Harpoon", "Northern Warlord Helmet Over Mail", "Scarf", "Northern Leather Vest", "Wrapped Shoes", "Veteran Warriors Axe", "Northern Throwing Axe", "Northern Round Shield", "Northern Warlord Helmet", "Bear Pelt", "Decorated Northern Hauberk", "Mail Chausses"});
            itemsForEachCulture.Add("Mountain Bandits", new List<string> { "Falchion", "Northern Spiked Battle Axe", "Western Long Spear", "Highland Wrapped Headcloth", "Highland Shoulder Fur", "Baggy Trunks", "Armored Baggy Trunks", "Sleeveless Studded Fur Armor", "Guarded Armwraps", "Leather Shoes", "Highland Boots" });
            itemsForEachCulture.Add("Forest Bandits", new List<string> { "Simple Eastern Backsword", "Mountain Hunting Bow", "Woodland Garments", "Guarded Armwraps", "Ragged Boots", "Narrow Sword", "Ranger Bow", "Highland Hood", "Highland Cloak", "Rugged Gambeson", "Guarded Armwraps", "Ragged Boots", "One Handed Bearded Axe", "Woodland Longbow", "Wolf Head", "Wolf Shoulder", "Rough Fur Over Chain", "Woven Leather Braces", "Woodland Boots" });
            itemsForEachCulture.Add("Desert Bandits", new List<string> { "Southern Militia Mace", "Jagged Spear", "Southern Tribal Turban", "Southern Peasant Robe", "Wrapped Shoes", "Star Falchion", "Weighted Eastern Spear", "Open Head Scarf", "Southern Wrapped Scarf", "Southern Padded Cloth", "Guarded Padded Vambrace", "Wrapped Shoes", "Desert Long Sword", "Fine Steel Leaf Spear", "Jareed", "Makeshift Kite Shield", "Trailed Southern Helmet", "Southern Wrapped Scarf", "Southern Robe Over Mail", "Mail Mitten", "Mail Chausses" });
            itemsForEachCulture.Add("Steppe Bandits", new List<string> { "Eastern Light Mace", "Eastern Steppe Armor", "Wrapped Shoes", "Broad Idl", "Weighted Eastern Spear", "Heavy Recurve Bow", "Nomad Padded Hood", "Studded Steppe Leather", "Strapped Leather Boots", "Pilgrim Hood", "Highland Wrapped Headcloth", "Ridged Sabre", "Fine Steel Leaf Spear", "Steppe Recurve Bow", "Plumed Nomad Helmet", "Eastern Studded Shoulders", "Eastern Stitched Leather Coat", "Guarded Padded Vambrace", "Eastern Leather Boots" });

        }
        public takeAction() { }

        public override void SetItemAmount(int newIA)
        {
            this.itemAmount = newIA;
        }

        public override Hero GetHeroTarget()
        {
            return this.heroTarget;
        }

        public override void SetHeroTarget(Hero newH)
        {
            this.heroTarget = newH;
        }

        public override ItemObject GetItemTarget()
        {
            return this.itemTarget;
        }

        public override void SetItemTarget(ItemObject newI)
        {
            this.itemTarget = newI;
        }

        public override void bringTargetsBack()
        {
            if (this.heroTarget == null && !this.settlementFlag)
            {
                var setName = this.Action.param[0].target;

                this.heroTarget = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
            if (this.settlementTarget == null && settlementFlag)
            {
                var setName = this.Action.param[0].target;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("take action - line 90"));
                }
                if (array.Length == 1)
                {
                    this.settlementTarget = array[0];
                }
            }

            if (this.GetItemTarget() == null)
            {
                var setName = this.Action.param[1].target;

                ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                if (array.Length > 1 || array.Length == 0)
                {
                    //InformationManager.DisplayMessage(new InformationMessage("take action - line 106"));
                }
                if (array.Length == 1)
                {
                    this.itemTarget = array[0];
                }

            }

            if (this.questGiver == null)
            {
                var setName = this.questGiverString;

                this.questGiver = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }

            if (this.itemsForEachCulture.IsEmpty())
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
                    if (questGen.alternativeActionsInOrder[this.index - 1].action == "damage")
                    {
                        this.Action.param[0].target = questGen.alternativeActionsInOrder[this.index - 1].Action.param[0].target;
                        this.settlementFlag = true;
                        this.settlementTarget = SettlementHelper.FindNearestSettlement((Settlement x) => x.Name.ToString() == this.Action.param[0].target);
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("take action - line 146"));
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[this.index - 1].action == "damage")
                    {
                        this.Action.param[0].target = questGen.actionsInOrder[this.index - 1].Action.param[0].target;
                        this.settlementFlag = true;
                        this.settlementTarget = SettlementHelper.FindNearestSettlement((Settlement x) => x.Name.ToString() == this.Action.param[0].target);
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("take action - line 159"));
                    }
                }
            }

            if (this.Action.param[1].target.Contains("item"))
            {
                string itemNumb = this.Action.param[1].target;
                if (this.heroTarget != null && this.heroTarget.PartyBelongedTo != null)
                {
                    Hero toGiveHero = this.heroTarget;

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
                            this.itemAmount = amount;
                            break;
                        }
                    }


                }
                else if (settlementFlag)
                {
                    int i = this.index;

                    int amount = 20;

                    while (this.itemTarget == null && amount > 0)
                    {
                        foreach (ItemRosterElement itemRosterElement in this.settlementTarget.ItemRoster)
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
                                this.itemAmount = amount2;
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
                            int it = rnd.Next(this.itemsForEachCulture[m.ActualClan.Culture.Name.ToString()].Count);
                            var setName = this.itemsForEachCulture[m.ActualClan.Culture.Name.ToString()][it];

                            ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                            if (array.Length > 1 || array.Length == 0)
                            {
                                InformationManager.DisplayMessage(new InformationMessage("take action - line 106"));
                            }
                            if (array.Length == 1)
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

                            
                            int amount = 300 / this.GetItemTarget().Value;
                            if (amount <= 0)
                            {
                                amount = 1;
                            }
                            this.itemAmount = amount;
                            break;
                        }
                    }
                    if (this.itemTarget == null)
                    {
                        var setName = "Commoner Tunic";

                        ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                        if (array.Length > 1 || array.Length == 0)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("take action - line 106"));
                        }
                        if (array.Length == 1)
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
                        int amount = 300 / this.GetItemTarget().Value;
                        if (amount <= 0)
                        {
                            amount = 1;
                        }
                        this.itemAmount = amount;
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
            if (!actioncomplete && !this.actionInLog)
            {
                if (this.index == 0)
                {
                    this.actionInLog = true;
                    if (this.heroTarget != null)
                    {
                        questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                        TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {HERO}.", null);
                        textObject.SetTextVariable("HERO", this.heroTarget.Name);
                        textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                        textObject.SetTextVariable("AMOUNT", this.itemAmount);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, this.itemAmount, null, false);
                        this.amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                    }
                    else if (this.settlementFlag)
                    {
                        questBase.AddTrackedObject(this.settlementTarget);
                        TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {SETTLEMENT}.", null);
                        textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                        textObject.SetTextVariable("AMOUNT", this.itemAmount);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, this.itemAmount, null, false);
                        this.amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                    }
                    else
                    {
                        TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from a party of " + this.Action.param[0].target + ".", null);
                        textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                        textObject.SetTextVariable("AMOUNT", this.itemAmount);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, this.itemAmount, null, false);
                        this.amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                    }
                }
                else
                {

                    if (questGen.actionsInOrder[this.index - 1].actioncomplete || ((questGen.actionsInOrder[this.index - 1].action == "damage" || questGen.actionsInOrder[this.index - 1].action == "kill") && questGen.actionsInOrder[this.index - 1].actionInLog))
                    {
                        this.actionInLog = true;
                        if (this.heroTarget != null)
                        {
                            questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                            TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {HERO}.", null);
                            textObject.SetTextVariable("HERO", this.heroTarget.Name);
                            textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                            textObject.SetTextVariable("AMOUNT", this.itemAmount);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, this.itemAmount, null, false);
                            this.amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                        }
                        else if (this.settlementFlag)
                        {
                            questBase.AddTrackedObject(this.settlementTarget);
                            TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {SETTLEMENT}.", null);
                            textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                            textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                            textObject.SetTextVariable("AMOUNT", this.itemAmount);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, this.itemAmount, null, false);
                            this.amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                        }
                        else
                        {
                            TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from a party of " + this.Action.param[0].target + ".", null);
                            textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                            textObject.SetTextVariable("AMOUNT", this.itemAmount);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, this.itemAmount, null, false);
                            this.amountGot = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
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
                if (this.heroTarget != null)
                {
                    if (mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party == this.heroTarget.PartyBelongedTo.Party))
                    {
                        int current = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                        this.amountGot += current - amountBeforeFight;
                        
                        if (this.amountGot >= this.itemAmount)
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
                    if (mapEventSide.IsMainPartyAmongParties() )
                    {
                        foreach (MapEventParty t in mapEventSide2.Parties)
                        {
                            if (t.Party.Settlement != null)
                            {
                                if (t.Party.Settlement == this.settlementTarget)
                                {
                                    int current = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                                    this.amountGot += current - amountBeforeFight;

                                    if (this.amountGot >= this.itemAmount)
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
                }
                else
                {
                    if (mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party.Culture.Name.ToString() == this.Action.param[0].target))
                    {
                        this.amountGot += 0;
                        int current = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                        this.amountGot += current - amountBeforeFight;
                        
                        if (this.amountGot >= this.itemAmount)
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
            
            MapEventSide mapEventSide =mapEvent.AttackerSide;
            MapEventSide mapEventSide2 = mapEvent.DefenderSide;
            if (this.heroTarget != null)
            {
                if ((mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party == this.heroTarget.PartyBelongedTo.Party))
                    || (mapEventSide2.IsMainPartyAmongParties() && mapEventSide.Parties.Any((MapEventParty t) => t.Party == this.heroTarget.PartyBelongedTo.Party)))
                {
                    this.amountBeforeFight = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
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
                            if (t.Party.Settlement == this.settlementTarget)
                            {
                                this.amountBeforeFight = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
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
                            if (t.Party.Settlement == this.settlementTarget)
                            {
                                this.amountBeforeFight = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
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
                    this.amountBeforeFight = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                }
            }

            
        }

        private void takeConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!this.actioncomplete)
            {
                questGen.currentActionIndex++;

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], this.itemAmount);

                if (this.heroTarget != null || this.settlementTarget != null)
                {
                    MakePeaceAction.Apply(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                    //FactionManager.DeclareAlliance(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                }
                this.actioncomplete = true;
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
                    this.heroTarget = targetHero;
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
                    this.itemTarget = targetItem;
                    break;
                }
            }
        }

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
        }
        public override TextObject getDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Steal stuff":
                    if (this.heroTarget != null)
                    {
                        strat = new TextObject("There're some items I need you to steal from {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("There're some items I need you to steal from {SETTLEMENT}. Are you up for the task?", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("There're some items I need you to steal from a group of {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }
                    
                    break;

                case "Gather raw materials":
                    strat = new TextObject("I need you to gather {ITEM}. Can you do that for me?", null);
                    strat.SetTextVariable("HERO", this.itemTarget.Name);
                    break;

                case "Steal valuables for resale":
                    if (this.heroTarget != null)
                    {
                        strat = new TextObject("There're some items I need you to steal from {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("There're some items I need you to steal from {SETTLEMENT}. Are you up for the task?", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("There're some items I need you to steal from a group of {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
                case "Steal supplies":
                    if (this.heroTarget != null)
                    {
                        strat = new TextObject("There're some items I need you to steal from {HERO}. Are you up for the task?", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("There're some items I need you to steal from {SETTLEMENT}. Are you up for the task?", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
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
                    if (this.heroTarget != null)
                    {
                        strat = new TextObject("Steal from {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("Steal from {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("Steal from a group of {HERO}.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;

                case "Gather raw materials":
                    strat = new TextObject("Gather {ITEM}.", null);
                    strat.SetTextVariable("HERO", this.itemTarget.Name);
                    break;

                case "Steal valuables for resale":
                    if (this.heroTarget != null)
                    {
                        strat = new TextObject("Steal from {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("Steal from {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("Steal from a group of {HERO}.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
                case "Steal supplies":
                    if (this.heroTarget != null)
                    {
                        strat = new TextObject("Steal from {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("Steal from {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
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
                    if (this.heroTarget != null)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;

                case "Gather raw materials":
                    strat = new TextObject("{ITEM} is a type of {TYPE}, belonging to the category of {CATEGORY} and is part of the {CULTURE} culture.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    strat.SetTextVariable("TYPE", this.itemTarget.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", this.itemTarget.ItemCategory.ToString());
                    strat.SetTextVariable("CULTURE", this.itemTarget.Culture.ToString());
                    break;

                case "Steal valuables for resale":
                    if (this.heroTarget != null)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
                case "Steal supplies":
                    if (this.heroTarget != null)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
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

    }
}
