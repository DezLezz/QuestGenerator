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

namespace QuestGenerator
{
    public class takeAction : actionTarget
    {
        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        [XmlIgnore]
        public ItemObject itemTarget;

        public int itemAmount = 0;

        public int currentAmount = 0;

        public takeAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
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
            if (this.heroTarget == null)
            {
                var setName = this.Action.param[0].target;

                Hero[] array = (from x in Hero.AllAliveHeroes where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("give action - line 73"));
                }
                if (array.Length == 1)
                {
                    this.heroTarget = array[0];
                }
            }

            if (this.GetItemTarget() == null)
            {
                var setName = this.Action.param[1].target;

                ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("give action - line 89"));
                }
                if (array.Length == 1)
                {
                    this.itemTarget = array[0];
                }

            }

            if (this.questGiver == null)
            {
                var setName = this.questGiverString;

                Hero[] array = (from x in Hero.AllAliveHeroes where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("listen action - line 106"));
                }
                if (array.Length == 1)
                {
                    this.questGiver = array[0];
                }
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestCampaignBehavior.QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("enemy"))
            {
                InformationManager.DisplayMessage(new InformationMessage("take action - line 113"));                
            }

            if (this.Action.param[1].target.Contains("item"))
            {
                string itemNumb = this.Action.param[1].target;
                if (this.heroTarget != null)
                {
                    Hero toGiveHero = this.heroTarget;
                    int amount = 1;

                    TroopRoster troop = toGiveHero.PartyBelongedTo.MemberRoster;
                    int t = rnd.Next(troop.GetTroopRoster().Count);

                    TroopRosterElement troopRosterElement = troop.GetTroopRoster()[t];
                    IEnumerable<Equipment> battleequipment = troopRosterElement.Character.BattleEquipments;

                    int e = rnd.Next(battleequipment.Count());
                    Equipment equipment = battleequipment.ElementAt(e);
                    
                    for (EquipmentIndex equipmentIndex = 0; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
                    {
                        EquipmentElement equipmentElement = equipment[equipmentIndex];
                        if (!equipmentElement.IsEmpty)
                        {
                            if (alternative)
                            {
                                questGen.alternativeMission.updateItemTargets(itemNumb, equipmentElement.Item);
                            }
                            else
                            {
                                questGen.chosenMission.updateItemTargets(itemNumb, equipmentElement.Item);
                            }
                            this.itemAmount = 1;
                            break;
                        }
                    }
                    

                }
                else
                {
                    foreach (MobileParty m in MobileParty.All)
                    {
                        if (m.IsBandit && m.ActualClan.Culture.Name.ToString() == this.Action.param[0].target)
                        {
                            TroopRoster troop = m.MemberRoster;
                            int t = rnd.Next(troop.GetTroopRoster().Count);

                            TroopRosterElement troopRosterElement = troop.GetTroopRoster()[t];
                            Equipment equipment = troopRosterElement.Character.FirstBattleEquipment;

                            for (EquipmentIndex equipmentIndex = 0; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
                            {
                                EquipmentElement equipmentElement = equipment[equipmentIndex];
                                if (!equipmentElement.IsEmpty)
                                {
                                    if (alternative)
                                    {
                                        questGen.alternativeMission.updateItemTargets(itemNumb, equipmentElement.Item);
                                    }
                                    else
                                    {
                                        questGen.chosenMission.updateItemTargets(itemNumb, equipmentElement.Item);
                                    }
                                    this.itemAmount = 1;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (this.itemTarget == null)
                    {
                        this.itemTarget = DefaultItems.Grain;
                        this.itemAmount = 1;
                    }
                    this.currentAmount = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                }

            }

            else if (this.GetItemTarget() != null && this.GetItemAmount() == 0)
            {
                int r = rnd.Next(1, 10);
                this.SetItemAmount(r);
            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestCampaignBehavior.QuestGenTestQuest questGen)
        {
            if (this.heroTarget != null)
            {
                questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {HERO}.", null);
                textObject.SetTextVariable("HERO", this.heroTarget.Name);
                textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                textObject.SetTextVariable("AMOUNT", this.itemAmount);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
            }
            else
            {
                TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from a party of " + this.Action.param[0].target + ".", null);
                textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                textObject.SetTextVariable("AMOUNT", this.itemAmount);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
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
                        if (current - this.currentAmount >= this.itemAmount)
                        {
                            this.takeConsequences(index, questBase, questGen);
                        }
                        else
                        {
                            this.currentAmount += current;
                        }
                    }

                }
                else
                {
                    if (mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party.Culture.Name.ToString() == this.Action.param[0].target))
                    {
                        foreach (ItemRosterElement ir in mapEventSide2.ItemRosterForPlayerLootShare(mapEventSide2.Parties[0].Party))
                        {
                            InformationManager.DisplayMessage(new InformationMessage(ir.EquipmentElement.Item.Name.ToString()));
                        }
                        int current = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                        if (current - this.currentAmount >= this.itemAmount)
                        {
                            this.takeConsequences(index, questBase, questGen);
                        }
                        else
                        {
                            this.currentAmount += current;
                        }
                    }
                }
                
            }
        }

        private void takeConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!questGen.journalLogs[this.index].HasBeenCompleted())
            {
                questGen.currentActionIndex++;

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);

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
    }
}
