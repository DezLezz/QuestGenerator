using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using System.Xml.Serialization;
using static QuestGenerator.QuestGenTestCampaignBehavior;
using QuestGenerator.QuestBuilder;

namespace QuestGenerator
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

        public gatherAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public gatherAction() { }

        public override int GetItemAmount()
        {
            return this.itemAmount;
        }

        public override void SetItemAmount(int newIA)
        {
            this.itemAmount = newIA;
        }

        public override ItemObject GetItemTarget()
        {
            return this.itemTarget;
        }

        public override void SetItemTarget(ItemObject newI)
        {
            this.itemTarget = newI;
        }

        public override Settlement GetSettlementTarget()
        {
            return this.settlementTarget;
        }

        public override void SetSettlementTarget(Settlement newS)
        {
            this.settlementTarget = newS;
        }

        public override void bringTargetsBack()
        {
            if (this.GetItemTarget() == null)
            {
                var setName = this.Action.param[0].target;
                InformationManager.DisplayMessage(new InformationMessage(setName));

                ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("gather action - 75"));
                }
                if (array.Length == 1)
                {
                    this.itemTarget = array[0];
                }

            }

            if (this.settlementStringTarget != null && this.settlementTarget == null)
            {
                var setName = this.settlementStringTarget;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("gather action - line 92"));
                }
                if (array.Length == 1)
                {
                    this.settlementTarget = array[0];
                }
            }
            if (this.questGiver == null)
            {
                var setName = this.questGiverString;

                Hero[] array = (from x in Hero.AllAliveHeroes where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("gather action - line 107"));
                }
                if (array.Length == 1)
                {
                    this.questGiver = array[0];
                }
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("item"))
            {
                var itemNumb = this.Action.param[0].target;
                int amount = 0;
                ItemObject newItem = new ItemObject();
                int i = this.index;
                if (i > 0)
                {
                    if (alternative)
                    {
                        if (questGen.alternativeActionsInOrder[i - 1].action == "goto")
                        {

                            var itemList = Items.AllTradeGoods;

                            int r = rnd.Next(itemList.Count());

                            newItem = itemList.ElementAt(r);

                            if (this.itemTarget.IsAnimal || this.itemTarget.IsFood)
                            {
                                amount = rnd.Next(1, 10);
                                this.SetItemAmount(amount);
                            }
                            else
                            {
                                this.SetItemAmount(1);
                            }

                            Settlement settlement = questGen.alternativeActionsInOrder[i - 1].GetSettlementTarget();

                            Settlement settlement1 = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {
                                float num;
                                bool flag = false;
                                foreach (ItemRosterElement iR in x.ItemRoster)
                                {
                                    if (iR.EquipmentElement.Item.Name == this.itemTarget.Name)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }

                                return Campaign.Current.Models.MapDistanceModel.GetDistance(x, settlement, 1500f, out num) && flag;
                            });

                            if (settlement != null)
                            {
                                this.settlementStringTarget = settlement.Name.ToString();
                            }

                            this.SetSettlementTarget(settlement);
                        }

                        else
                        {

                            var itemList = Items.AllTradeGoods;

                            int r = rnd.Next(itemList.Count());

                            newItem = itemList.ElementAt(r);

                            if (this.itemTarget.IsAnimal || this.itemTarget.IsFood)
                            {
                                amount = rnd.Next(1, 10);
                                this.SetItemAmount(amount);
                            }
                            else
                            {
                                this.SetItemAmount(1);
                            }

                            Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {
                                float num;
                                bool flag = false;
                                foreach (ItemRosterElement iR in x.ItemRoster)
                                {
                                    if (iR.EquipmentElement.Item.Name == this.itemTarget.Name)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }

                                return Campaign.Current.Models.MapDistanceModel.GetDistance(x, questGen.IssueSettlement, 1500f, out num) && flag;
                            });

                            if (settlement != null)
                            {
                                this.settlementStringTarget = settlement.Name.ToString();
                            }

                            this.SetSettlementTarget(settlement);
                        }
                    }
                    else
                    {
                        if (questGen.actionsInOrder[i - 1].action == "goto")
                        {

                            var itemList = Items.AllTradeGoods;

                            int r = rnd.Next(itemList.Count());

                            newItem = itemList.ElementAt(r);

                            if (this.itemTarget.IsAnimal || this.itemTarget.IsFood)
                            {
                                amount = rnd.Next(1, 10);
                                this.SetItemAmount(amount);
                            }
                            else
                            {
                                this.SetItemAmount(1);
                            }

                            Settlement settlement = questGen.actionsInOrder[i - 1].GetSettlementTarget();

                            Settlement settlement1 = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {
                                float num;
                                bool flag = false;
                                foreach (ItemRosterElement iR in x.ItemRoster)
                                {
                                    if (iR.EquipmentElement.Item.Name == this.itemTarget.Name)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }

                                return Campaign.Current.Models.MapDistanceModel.GetDistance(x, settlement, 1500f, out num) && flag;
                            });

                            if (settlement != null)
                            {
                                this.settlementStringTarget = settlement.Name.ToString();
                            }

                            this.SetSettlementTarget(settlement);
                        }

                        else
                        {

                            var itemList = Items.AllTradeGoods;

                            int r = rnd.Next(itemList.Count());

                            newItem = itemList.ElementAt(r);

                            if (this.itemTarget.IsAnimal || this.itemTarget.IsFood)
                            {
                                amount = rnd.Next(1, 10);
                                this.SetItemAmount(amount);
                            }
                            else
                            {
                                this.SetItemAmount(1);
                            }

                            Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {
                                float num;
                                bool flag = false;
                                foreach (ItemRosterElement iR in x.ItemRoster)
                                {
                                    if (iR.EquipmentElement.Item.Name == this.itemTarget.Name)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }

                                return Campaign.Current.Models.MapDistanceModel.GetDistance(x, questGen.IssueSettlement, 1500f, out num) && flag;
                            });

                            if (settlement != null)
                            {
                                this.settlementStringTarget = settlement.Name.ToString();
                            }

                            this.SetSettlementTarget(settlement);
                        }
                    }
                }



                else if (i == 0)
                {

                    var itemList = Items.AllTradeGoods;

                    int r = rnd.Next(itemList.Count());

                    newItem = itemList.ElementAt(r);

                    if (this.itemTarget.IsAnimal || this.itemTarget.IsFood)
                    {
                        amount = rnd.Next(1, 10);
                        this.SetItemAmount(amount);
                    }
                    else
                    {
                        this.SetItemAmount(1);
                    }

                    Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        float num;
                        bool flag = false;
                        foreach (ItemRosterElement iR in x.ItemRoster)
                        {
                            if (iR.EquipmentElement.Item.Name == this.itemTarget.Name)
                            {
                                flag = true;
                                break;
                            }
                        }

                        return Campaign.Current.Models.MapDistanceModel.GetDistance(x, questGen.IssueSettlement, 1500f, out num) && flag;
                    });

                    if (settlement != null)
                    {
                        this.settlementStringTarget = settlement.Name.ToString();
                    }

                    this.SetSettlementTarget(settlement);
                }

                if (this.GetItemTarget() != null)
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
            else if (this.GetItemAmount() == 0  && this.itemTarget != null)
            {
                int amount = 0;
                if (this.itemTarget.IsCraftedWeapon)
                {
                    this.SetItemAmount(1);
                }
                else
                {
                    amount = rnd.Next(2, 10);
                    this.SetItemAmount(amount);
                }
            }
        }

            
        

        public override void QuestQ(QuestBase questBase, QuestGenTestQuest questGen)
        {

            if (this.settlementTarget != null)
            {
                TextObject textObject = new TextObject("Gather {ITEM_AMOUNT} {ITEM_NAME}. You can maybe find some in {SETTLEMENT}.", null);
                textObject.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
                textObject.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
                textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, this.itemAmount, null, false);
            }
            else
            {
                TextObject textObject = new TextObject("Gather {ITEM_AMOUNT} {ITEM_NAME}. You might need to craft them.", null);
                textObject.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
                textObject.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, this.itemAmount, null, false);
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

            if (amountRemaining <= 0)
            {
                questGen.currentActionIndex++;

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
                InformationManager.DisplayMessage(new InformationMessage("item refined: " + equipmentElement.Item.Name.ToString()));

                var refined = RefinePatch.refineFormulaS;


                if (refined.OutputCount > 0)
                {
                    ItemObject craftingMaterialItem3 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refined.Output);
                    if (craftingMaterialItem3.Name == equipmentElement.Item.Name)
                    {
                        flag = true;
                        amountPurchased += refined.OutputCount;
                    }

                }
                if (refined.Output2Count > 0)
                {
                    ItemObject craftingMaterialItem4 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refined.Output2);
                    if (craftingMaterialItem4.Name == equipmentElement.Item.Name)
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

                if (amountRemaining <= 0)
                {
                    questGen.currentActionIndex++;

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
                InformationManager.DisplayMessage(new InformationMessage("item smelted: " + equipmentElement.Item.Name.ToString()));
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

                if (amountRemaining <= 0)
                {
                    questGen.currentActionIndex++;

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
            InformationManager.DisplayMessage(new InformationMessage("item crafted: " + item.Name.ToString()));

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

            if (amountRemaining <= 0)
            {
                if (!questGen.journalLogs[this.index].HasBeenCompleted())
                {
                    questGen.currentActionIndex++;

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
                    this.itemTarget = targetItem;
                    break;
                }
            }
        }
    }
}
