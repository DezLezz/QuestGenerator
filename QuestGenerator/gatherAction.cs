using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using ThePlotLords.QuestBuilder;
using ThePlotLords.QuestBuilder.CustomBT;
using static ThePlotLords.QuestGenTestCampaignBehavior;

namespace ThePlotLords
{
    public class gatherAction : actionTarget
    {
        static Random rnd = new Random();

        [XmlIgnore]
        public ItemObject itemTarget;

        [XmlIgnore]
        public Settlement settlementTarget;

        public int itemAmount;

        public string settlementStringTarget;

        public gatherAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public gatherAction() { }

        public override int GetItemAmount()
        {
            return itemAmount;
        }

        public override void SetItemAmount(int newIA)
        {
            itemAmount = newIA;
        }

        public override ItemObject GetItemTarget()
        {
            return itemTarget;
        }

        public override void SetItemTarget(ItemObject newI)
        {
            itemTarget = newI;
        }

        public override Settlement GetSettlementTarget()
        {
            return settlementTarget;
        }

        public override void SetSettlementTarget(Settlement newS)
        {
            settlementTarget = newS;
        }

        public override void bringTargetsBack()
        {
            if (this.GetItemTarget() == null)
            {
                var setName = this.Action.param[0].target;

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

            if (settlementStringTarget != null && settlementTarget == null)
            {
                var setName = settlementStringTarget;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("gather action - line 91"));
                }
                if (array.Length == 1)
                {
                    settlementTarget = array[0];
                }
            }
            if (questGiver == null)
            {
                var setName = questGiverString;

                questGiver = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("item"))
            {
                var itemNumb = this.Action.param[0].target;
                int amount = 0;
                ItemObject newItem = new ItemObject();
                int i = index;
                if (i > 0)
                {
                    if (alternative)
                    {
                        if (questGen.alternativeActionsInOrder[i - 1].action == "goto")
                        {

                            var itemList = Items.All;

                            int r = rnd.Next(itemList.Count());

                            newItem = itemList.ElementAt(r);
                            while (newItem.Value > 300)
                            {
                                r = rnd.Next(itemList.Count());

                                newItem = itemList.ElementAt(r);
                            }

                            amount = 300 / newItem.Value;
                            if (amount <= 0)
                            {
                                amount = 1;
                            }
                            this.SetItemAmount(amount);

                            Settlement settlement = questGen.alternativeActionsInOrder[i - 1].GetSettlementTarget();

                            Settlement settlement1 = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {

                                bool flag = false;
                                foreach (ItemRosterElement iR in x.ItemRoster)
                                {
                                    if (iR.EquipmentElement.Item.Name == newItem.Name)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }

                                return flag;
                            });

                            if (settlement != null)
                            {
                                settlementStringTarget = settlement.Name.ToString();
                            }

                            this.SetSettlementTarget(settlement);
                        }

                        else
                        {

                            var itemList = Items.All;

                            int r = rnd.Next(itemList.Count());

                            newItem = itemList.ElementAt(r);
                            while (newItem.Value > 300)
                            {
                                r = rnd.Next(itemList.Count());

                                newItem = itemList.ElementAt(r);
                            }

                            amount = 300 / newItem.Value;
                            if (amount <= 0)
                            {
                                amount = 1;
                            }
                            this.SetItemAmount(amount);

                            Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {

                                bool flag = false;
                                foreach (ItemRosterElement iR in x.ItemRoster)
                                {
                                    if (iR.EquipmentElement.Item.Name == newItem.Name)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }

                                return flag;
                            });

                            if (settlement != null)
                            {
                                settlementStringTarget = settlement.Name.ToString();
                            }

                            this.SetSettlementTarget(settlement);
                        }
                    }
                    else
                    {
                        if (questGen.actionsInOrder[i - 1].action == "goto")
                        {

                            var itemList = Items.All;

                            int r = rnd.Next(itemList.Count());

                            newItem = itemList.ElementAt(r);
                            while (newItem.Value > 300 && newItem == null)
                            {
                                r = rnd.Next(itemList.Count());

                                newItem = itemList.ElementAt(r);
                            }
                            
                            if (newItem.Value <= 0)
                            {
                                amount = 300 / newItem.Value;
                                if (amount <= 0)
                                {
                                    amount = 1;
                                }
                            }
                            else
                            {
                                amount = 300;
                            }
                            this.SetItemAmount(amount);

                            Settlement settlement = questGen.actionsInOrder[i - 1].GetSettlementTarget();

                            Settlement settlement1 = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {

                                bool flag = false;
                                foreach (ItemRosterElement iR in x.ItemRoster)
                                {
                                    if (iR.EquipmentElement.Item.Name == newItem.Name)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }

                                return flag;
                            });

                            if (settlement != null)
                            {
                                settlementStringTarget = settlement.Name.ToString();
                            }

                            this.SetSettlementTarget(settlement);
                        }

                        else
                        {

                            var itemList = Items.All;

                            int r = rnd.Next(itemList.Count());

                            newItem = itemList.ElementAt(r);
                            while (newItem.Value > 300)
                            {
                                r = rnd.Next(itemList.Count());

                                newItem = itemList.ElementAt(r);
                            }

                            amount = 300 / newItem.Value;
                            if (amount <= 0)
                            {
                                amount = 1;
                            }
                            this.SetItemAmount(amount);

                            Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {

                                bool flag = false;
                                foreach (ItemRosterElement iR in x.ItemRoster)
                                {
                                    if (iR.EquipmentElement.Item.Name == newItem.Name)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }

                                return flag;
                            });

                            if (settlement != null)
                            {
                                settlementStringTarget = settlement.Name.ToString();
                            }

                            this.SetSettlementTarget(settlement);
                        }
                    }
                }

                else if (i == 0)
                {

                    var itemList = Items.All;
                    int r = rnd.Next(itemList.Count());

                    newItem = itemList.ElementAt(r);
                    while (newItem.Value > 300)
                    {
                        r = rnd.Next(itemList.Count());

                        newItem = itemList.ElementAt(r);
                    }

                    amount = 300 / newItem.Value;
                    if (amount <= 0)
                    {
                        amount = 1;
                    }
                    this.SetItemAmount(amount);

                    Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {

                        bool flag = false;
                        foreach (ItemRosterElement iR in x.ItemRoster)
                        {
                            if (iR.EquipmentElement.Item.Name == newItem.Name)
                            {
                                flag = true;
                                break;
                            }
                        }

                        return flag;
                    });

                    if (settlement != null)
                    {
                        settlementStringTarget = settlement.Name.ToString();
                    }

                    this.SetSettlementTarget(settlement);
                }

                if (newItem != null)
                {
                    if (alternative)
                    {
                        questGen.alternativeMission.updateItemTargets(itemNumb, newItem);
                    }
                    else
                    {
                        questGen.chosenMission.updateItemTargets(itemNumb, newItem);
                    }

                }

            }
            else if (this.GetItemAmount() == 0 && itemTarget != null)
            {
                int amount = 300 / itemTarget.Value;
                if (amount <= 0)
                {
                    amount = 1;
                }
                this.SetItemAmount(amount);
            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete && !actionInLog)
            {
                if (index == 0)
                {
                    actionInLog = true;
                    if (settlementTarget != null)
                    {
                        TextObject textObject = new TextObject("Gather {ITEM_AMOUNT} {ITEM_NAME}. You can maybe find some in {SETTLEMENT}.", null);
                        textObject.SetTextVariable("ITEM_AMOUNT", itemAmount);
                        textObject.SetTextVariable("ITEM_NAME", itemTarget.Name);
                        textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                    }
                    else
                    {
                        TextObject textObject = new TextObject();
                        if (itemTarget.IsCraftedByPlayer || itemTarget.IsCraftedWeapon) textObject = new TextObject("Gather {ITEM_AMOUNT} {ITEM_NAME}. You might need to craft them.", null);
                        else textObject = new TextObject("Gather {ITEM_AMOUNT} {ITEM_NAME}.", null);
                        textObject.SetTextVariable("ITEM_AMOUNT", itemAmount);
                        textObject.SetTextVariable("ITEM_NAME", itemTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index)
                    {
                        actionInLog = true;
                        if (settlementTarget != null)
                        {
                            TextObject textObject = new TextObject("Gather {ITEM_AMOUNT} {ITEM_NAME}. You can maybe find some in {SETTLEMENT}.", null);
                            textObject.SetTextVariable("ITEM_AMOUNT", itemAmount);
                            textObject.SetTextVariable("ITEM_NAME", itemTarget.Name);
                            textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        }
                        else
                        {
                            TextObject textObject = new TextObject();
                            if (itemTarget.IsCraftedByPlayer || itemTarget.IsCraftedWeapon) textObject = new TextObject("Gather {ITEM_AMOUNT} {ITEM_NAME}. You might need to craft them.", null);
                            else textObject = new TextObject("Gather {ITEM_AMOUNT} {ITEM_NAME}.", null);
                            textObject.SetTextVariable("ITEM_AMOUNT", itemAmount);
                            textObject.SetTextVariable("ITEM_NAME", itemTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, itemAmount, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        }
                    }
                }
            }

        }

        public override void OnPlayerInventoryExchangeQuest(List<(ItemRosterElement, int)> purchasedItems, List<(ItemRosterElement, int)> soldItems, bool isTrading, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            bool flag = false;
            int amountRemaining = this.GetItemAmount();
            int amountPurchased = 0;
            foreach (ValueTuple<ItemRosterElement, int> valueTuple in purchasedItems)
            {
                ItemRosterElement item = valueTuple.Item1;
                amountPurchased = item.Amount;
                if (item.EquipmentElement.Item == this.GetItemTarget())
                {
                    flag = true;
                    break;
                }
            }

            if (flag)
            {
                amountRemaining -= amountPurchased;
                this.SetItemAmount(amountRemaining);
                questGen.UpdateQuestTaskS(questGen.journalLogs[index], questGen.journalLogs[index].CurrentProgress + amountPurchased);
            }

            if (amountRemaining <= 0 && !actioncomplete)
            {
                questGen.currentActionIndex++;
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

        public override void OnEquipmentSmeltedByHeroEventQuest(Hero hero, EquipmentElement equipmentElement, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (RefinePatch.itemRefined)
            {
                bool flag = false;
                int amountRemaining = this.GetItemAmount();
                int amountPurchased = 0;

                var refined = RefinePatch.refineFormulaS;


                if (refined.OutputCount > 0)
                {
                    ItemObject craftingMaterialItem3 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refined.Output);
                    if (craftingMaterialItem3.Name == this.itemTarget.Name)
                    {
                        flag = true;
                        amountPurchased += refined.OutputCount;
                    }

                }
                if (refined.Output2Count > 0)
                {
                    ItemObject craftingMaterialItem4 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refined.Output2);
                    if (craftingMaterialItem4.Name == this.itemTarget.Name)
                    {
                        flag = true;
                        amountPurchased += refined.Output2Count;
                    }

                }

                if (flag)
                {
                    amountRemaining -= amountPurchased;
                    this.SetItemAmount(amountRemaining);
                    questGen.UpdateQuestTaskS(questGen.journalLogs[index], questGen.journalLogs[index].CurrentProgress + amountPurchased);
                }

                if (amountRemaining <= 0 && !actioncomplete)
                {
                    questGen.currentActionIndex++;
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

                RefinePatch.itemRefined = false;
            }
            else
            {
                bool flag = false;
                int amountRemaining = this.GetItemAmount();
                int amountPurchased = 0;
                int[] smeltingOutputForItem = Campaign.Current.Models.SmithingModel.GetSmeltingOutputForItem(equipmentElement.Item);

                Campaign campaign = Campaign.Current;
                GameModels models = campaign.Models;
                ItemObject resourceItem;

                for (int i = 0; i < smeltingOutputForItem.Length; i++)
                {
                    var material = (CraftingMaterials)i;

                    SmithingModel smithingModel = models.SmithingModel;
                    resourceItem = ((smithingModel != null) ? smithingModel.GetCraftingMaterialItem(material) : null);

                    if (resourceItem.Name == this.GetItemTarget().Name)
                    {
                        flag = true;
                        amountPurchased += smeltingOutputForItem[i];
                        break;
                    }
                }

                if (flag)
                {
                    amountRemaining -= amountPurchased;
                    this.SetItemAmount(amountRemaining);
                    questGen.UpdateQuestTaskS(questGen.journalLogs[index], questGen.journalLogs[index].CurrentProgress + amountPurchased);
                }

                if (amountRemaining <= 0 && !actioncomplete)
                {
                    questGen.currentActionIndex++;
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

        }

        public override void OnNewItemCraftedEventQuest(ItemObject item, Crafting.OverrideData crafted, bool flag2, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            bool flag = false;
            int amountRemaining = this.GetItemAmount();
            int amountPurchased = 0;

            if (item == this.GetItemTarget())
            {
                flag = true;
                amountPurchased++;
            }

            if (flag)
            {
                amountRemaining -= amountPurchased;
                this.SetItemAmount(amountRemaining);

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], questGen.journalLogs[this.index].CurrentProgress + amountPurchased);

            }

            if (amountRemaining <= 0 && !actioncomplete)
            {
                questGen.currentActionIndex++;
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

        public override void OnItemProducedEventQuest(ItemObject itemObject, Settlement settlement, int count, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            bool flag = false;
            int amountRemaining = this.GetItemAmount();
            int amountPurchased = 0;

            if (itemObject == this.GetItemTarget())
            {
                flag = true;
                amountPurchased++;
            }

            if (flag)
            {
                amountRemaining -= amountPurchased;
                this.SetItemAmount(amountRemaining);

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], questGen.journalLogs[this.index].CurrentProgress + amountPurchased);

            }

            if (amountRemaining <= 0 && !actioncomplete)
            {
                questGen.currentActionIndex++;
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

        public override void ItemsLooted(ItemRoster items, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            foreach (ItemRosterElement item in items)
            {
                if (item.EquipmentElement.Item == itemTarget)
                {
                    itemAmount -= item.Amount;
                    questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], questGen.journalLogs[this.index].CurrentProgress + item.Amount);
                }
            }

            if (itemAmount <= 0 && !actioncomplete)
            {
                questGen.currentActionIndex++;
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
        }

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
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

        public override TextObject getDescription(string strategy, int pair)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Gather raw materials":
                    strat = new TextObject("I need you to gather {ITEM}. Can you do that for me?", null);
                    strat.SetTextVariable("ITEM", itemTarget.Name);
                    break;
            }
            return strat;
        }

        public override TextObject getTitle(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Gather raw materials":
                    strat = new TextObject("Gather {ITEM}.", null);
                    strat.SetTextVariable("ITEM", itemTarget.Name);
                    break;
            }
            return strat;
        }

        public override string getListenString(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Gather raw materials":
                    strat = new TextObject("{ITEM} is a type of {TYPE}, belonging to the category of {CATEGORY} and is valued around {VALUE} gold coins.", null);
                    strat.SetTextVariable("ITEM", itemTarget.Name);
                    strat.SetTextVariable("TYPE", itemTarget.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", itemTarget.ItemCategory.ToString());
                    strat.SetTextVariable("VALUE", itemTarget.Value);
                    break;
            }
            return strat.ToString();
        }

        public override TextObject getStepDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            strat = new TextObject("Gather {AMOUNT} {ITEM}.", null);
            strat.SetTextVariable("AMOUNT", itemAmount);
            strat.SetTextVariable("ITEM", itemTarget.Name);

            return strat;
        }

    }
}
