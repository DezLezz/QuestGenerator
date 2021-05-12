using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Craft.Refinement;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static QuestGenerator.QuestGenTestCampaignBehavior;

namespace QuestGenerator
{
    [Serializable]
    class gatherAction : actionTarget
    {
        static Random rnd = new Random();

        [NonSerialized]
        public ItemObject itemTarget;

        [NonSerialized]
        public Settlement settlementTarget;

        public int itemAmount;

        public string settlementStringTarget;

        public gatherAction(string action, string target) : base(action, target)
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
                var setName = this.target;
                InformationManager.DisplayMessage(new InformationMessage(setName));

                ItemObject[] array = (from x in ItemObject.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB gather"));
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
                    InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB gather"));
                }
                if (array.Length == 1)
                {
                    this.settlementTarget = array[0];
                }
            }
        }

        public override void IssueQ(List<actionTarget> list, Settlement issueSettlement, Hero issueGiver)
        {
            if (this.target.Contains("item"))
            {
                var npcNumb = this.target;
                int amount = 0;
                int i = list.IndexOf(this);
                if (i > 0)
                {
                    if (list[i - 1].action == "goto")
                    {

                        var itemList = ItemObject.AllTradeGoods;

                        int r = rnd.Next(itemList.Count());

                        this.SetItemTarget(itemList.ElementAt(r));

                        this.target = itemList.ElementAt(r).Name.ToString();

                        if (this.itemTarget.IsAnimal || this.itemTarget.IsFood)
                        {
                            amount = rnd.Next(1, 10);
                            this.SetItemAmount(amount);
                        }
                        else
                        {
                            this.SetItemAmount(2);
                        }

                        Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == list[i - 1].target) select x).ToArray<Settlement>();

                        if (array.Length > 1 || array.Length == 0)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Everything is on fire Issue1"));
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

                            return Campaign.Current.Models.MapDistanceModel.GetDistance(x, array[0], 1500f, out num) && flag;
                        });

                        if (settlement == null)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Everything is on fire Issue2"));
                        }
                        else
                        {
                            this.settlementStringTarget = settlement.Name.ToString();
                        }
                        
                        this.SetSettlementTarget(settlement);
                    }

                    else
                    {

                        var itemList = ItemObject.AllTradeGoods;

                        int r = rnd.Next(itemList.Count());

                        this.SetItemTarget(itemList.ElementAt(r));

                        this.target = itemList.ElementAt(r).Name.ToString();

                        if (this.itemTarget.IsAnimal || this.itemTarget.IsFood)
                        {
                            amount = rnd.Next(1, 10);
                            this.SetItemAmount(amount);
                        }
                        else
                        {
                            this.SetItemAmount(2);
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

                            return Campaign.Current.Models.MapDistanceModel.GetDistance(x, issueSettlement, 1500f, out num) && flag;
                        });

                        if (settlement == null)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Everything is on fire Issue3"));
                        }
                        else
                        {
                            this.settlementStringTarget = settlement.Name.ToString();
                        }

                        this.SetSettlementTarget(settlement);
                    }
                }

                

                else if (i == 0)
                {

                    var itemList = (from x in ItemObject.AllTradeGoods where (x.Name.ToString() == "Charcoal") select x).ToList<ItemObject>();

                    int r = rnd.Next(itemList.Count());

                    this.SetItemTarget(itemList.ElementAt(r));

                    this.target = itemList.ElementAt(r).Name.ToString();

                    if (this.itemTarget.IsCraftedWeapon)
                    {
                        
                        this.SetItemAmount(1);
                    }
                    else
                    {
                        amount = rnd.Next(1, 10);
                        this.SetItemAmount(amount);
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

                        return Campaign.Current.Models.MapDistanceModel.GetDistance(x, issueSettlement, 1500f, out num) && flag;
                    });

                    if (settlement == null)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Everything is on fire Issue3"));
                    }
                    else
                    {
                        this.settlementStringTarget = settlement.Name.ToString();
                    }

                    this.SetSettlementTarget(settlement);
                }

                if (this.GetItemTarget() != null)
                {
                    foreach (actionTarget nextAction in list)
                    {
                        if (nextAction.target == npcNumb)
                        {
                            nextAction.target = this.target;
                            nextAction.SetItemTarget(this.GetItemTarget());
                            break;
                        }
                    }
                }

            }

            else if (this.GetItemAmount() == 0)
            {
                int amount = 0;
                if (this.itemTarget.IsCraftedWeapon)
                {

                    this.SetItemAmount(1);
                }
                else
                {
                    amount = rnd.Next(1, 10);
                    this.SetItemAmount(amount);
                }
            }
        }

        public override void QuestQ(List<actionTarget> list, Hero questGiver, QuestBase questBase, QuestGenTestCampaignBehavior.QuestGenTestQuest questGen, int index)
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
                TextObject textObject = new TextObject("Gather {ITEM_AMOUNT} {ITEM_NAME}.", null);
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

                if (questGen.currentActionIndex < questGen.actionsTargets.Count)
                {
                    questGen.currentAction = questGen.actionsTargets[questGen.currentActionIndex];
                }
                else
                {
                    questGen.SuccessConsequences();
                }
            }
        }

        public override void OnEquipmentSmeltedByHeroEventQuest(Hero hero, EquipmentElement equipmentElement, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (RefinePath.itemRefined)
            {
                bool flag = false;
                int amountRemaining = this.GetItemAmount();
                int amountPurchased = 0;
                InformationManager.DisplayMessage(new InformationMessage("item refined: " + equipmentElement.Item.Name.ToString()));

                var refined = RefinePath.refineFormulaS;


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

                    if (questGen.currentActionIndex < questGen.actionsTargets.Count)
                    {
                        questGen.currentAction = questGen.actionsTargets[questGen.currentActionIndex];
                    }
                    else
                    {
                        questGen.SuccessConsequences();
                    }
                }

                RefinePath.itemRefined = false;
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

                    if (questGen.currentActionIndex < questGen.actionsTargets.Count)
                    {
                        questGen.currentAction = questGen.actionsTargets[questGen.currentActionIndex];
                    }
                    else
                    {
                        questGen.SuccessConsequences();
                    }
                }
            }
            
        }

        public override void OnNewItemCraftedEventQuest(ItemObject item, Crafting.OverrideData crafted, int index, QuestGenTestQuest questGen, QuestBase questBase)
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
                questGen.UpdateQuestTaskS(questGen.journalLogs[index], questGen.journalLogs[index].CurrentProgress + amountPurchased);
            }

            if (amountRemaining <= 0)
            {
                questGen.currentActionIndex++;

                if (questGen.currentActionIndex < questGen.actionsTargets.Count)
                {
                    questGen.currentAction = questGen.actionsTargets[questGen.currentActionIndex];
                }
                else
                {
                    questGen.SuccessConsequences();
                }
            }
        }
    }
}
