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

        public int currentAmount = 0;

        public bool settlementFlag = false;

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
            if (this.heroTarget == null && !this.settlementFlag)
            {
                var setName = this.Action.param[0].target;

                Hero[] array = (from x in Hero.AllAliveHeroes where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    //InformationManager.DisplayMessage(new InformationMessage("take action - line 75"));
                }
                if (array.Length == 1)
                {
                    this.heroTarget = array[0];
                }
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
                    InformationManager.DisplayMessage(new InformationMessage("take action - line 106"));
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
                    InformationManager.DisplayMessage(new InformationMessage("listen action - line 123"));
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
                if (this.heroTarget != null)
                {
                    Hero toGiveHero = this.heroTarget;

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

                                int r = rnd.Next(1, amount);
                                this.itemAmount = r;
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
                            TroopRoster troop = m.MemberRoster;
                            int t = rnd.Next(troop.GetTroopRoster().Count);

                            TroopRosterElement troopRosterElement = troop.GetTroopRoster()[t];
                            Equipment equipment = troopRosterElement.Character.FirstBattleEquipment;

                            for (EquipmentIndex equipmentIndex = 0; equipmentIndex < EquipmentIndex.ArmorItemEndSlot; equipmentIndex++)
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
            if (this.heroTarget != null)
            {
                questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {HERO}.", null);
                textObject.SetTextVariable("HERO", this.heroTarget.Name);
                textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                textObject.SetTextVariable("AMOUNT", this.itemAmount);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                this.currentAmount = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
            }
            else if (this.settlementFlag)
            {
                questBase.AddTrackedObject(this.settlementTarget);
                TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from {SETTLEMENT}.", null);
                textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                textObject.SetTextVariable("AMOUNT", this.itemAmount);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                this.currentAmount = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
            }
            else
            {
                TextObject textObject = new TextObject("Take {AMOUNT} {ITEM} from a party of " + this.Action.param[0].target + ".", null);
                textObject.SetTextVariable("ITEM", this.itemTarget.Name);
                textObject.SetTextVariable("AMOUNT", this.itemAmount);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                this.currentAmount = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
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
                else if (settlementFlag)
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
                    if (this.heroTarget != null)
                    {
                        FactionManager.DeclareAlliance(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                    }
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
    }
}
