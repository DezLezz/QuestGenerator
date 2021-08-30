using System;
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
    public class exchangeAction : actionTarget
    {

        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        [XmlIgnore]
        public ItemObject itemTargetGive;

        [XmlIgnore]
        public ItemObject itemTargetReceive;

        public int itemAmountGive = 0;

        public int itemAmountReceive = 0;

        public exchangeAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }

        public exchangeAction() { }


        public override Hero GetHeroTarget()
        {
            return heroTarget;
        }

        public override void SetHeroTarget(Hero newH)
        {
            heroTarget = newH;
        }

        public override void bringTargetsBack()
        {

            if (heroTarget == null)
            {
                var setName = this.Action.param[0].target;

                heroTarget = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }

            if (itemTargetReceive == null)
            {
                var setName = this.Action.param[1].target;

                ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("exchange action - line 78"));
                }
                if (array.Length == 1)
                {
                    itemTargetReceive = array[0];
                }
            }

            if (itemTargetGive == null)
            {
                var setName = this.Action.param[2].target;

                ItemObject[] array = (from x in Items.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("exchange action - line 94"));
                }
                if (array.Length == 1)
                {
                    itemTargetGive = array[0];
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
            if (this.Action.param[0].target.Contains("npc"))
            {
                string npcNumb = this.Action.param[0].target;
                string targetHero = "none";
                Hero newHero = new Hero();
                int i = index;
                if (i > 0)
                {
                    if (alternative)
                    {
                        if (questGen.alternativeActionsInOrder[i - 1].action == "goto")
                        {
                            Settlement settlement = questGen.alternativeActionsInOrder[i - 1].GetSettlementTarget();

                            newHero = settlement.Notables.GetRandomElement();
                            targetHero = newHero.Name.ToString();
                        }
                        else
                        {
                            foreach (Hero hero in questBase.IssueSettlement.Notables)
                            {
                                if (hero != questGen.IssueOwner)
                                {
                                    targetHero = hero.Name.ToString();
                                    newHero = hero;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (questGen.actionsInOrder[i - 1].action == "goto")
                        {
                            Settlement settlement = questGen.actionsInOrder[i - 1].GetSettlementTarget();

                            newHero = settlement.Notables.GetRandomElement();
                            targetHero = newHero.Name.ToString();
                        }
                        else
                        {
                            foreach (Hero hero in questBase.IssueSettlement.Notables)
                            {
                                if (hero != questGen.IssueOwner)
                                {
                                    targetHero = hero.Name.ToString();
                                    newHero = hero;
                                }
                            }
                        }
                    }

                }

                else if (i == 0)
                {
                    foreach (Hero hero in questGiver.CurrentSettlement.Notables)
                    {
                        if (hero != questGen.IssueOwner)
                        {
                            targetHero = hero.Name.ToString();
                            newHero = hero;
                        }
                    }
                }

                if (targetHero != "none")
                {
                    if (alternative)
                    {
                        questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                    }
                    else
                    {
                        questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                    }

                }

                if (targetHero == "none")
                {
                    InformationManager.DisplayMessage(new InformationMessage("exchange action - line 201"));
                }

            }



            if (this.Action.param[2].target.Contains("item"))
            {
                int i = index;

                Hero toGiveHero = heroTarget;

                int amount = 50;
                string itemNumb = this.Action.param[2].target;

                while (itemTargetReceive == null && amount > 0)
                {
                    foreach (ItemRosterElement itemRosterElement in toGiveHero.CurrentSettlement.ItemRoster)
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

                            int amount2 = 300 / itemRosterElement.EquipmentElement.Item.Value;
                            if (amount2 <= 0)
                            {
                                amount2 = 1;
                            }
                            if (amount2 >= amount)
                            {
                                amount2 = amount;
                            }
                            itemAmountReceive = amount2;
                            break;
                        }
                    }
                    amount -= 1;
                }

                if (amount <= 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("amount is zero or less"));
                }


            }

            if (this.Action.param[1].target.Contains("item"))
            {
                ItemObject newItem = new ItemObject();
                string itemNumb = this.Action.param[1].target;
                var itemList = Items.All;

                int r = rnd.Next(itemList.Count());

                newItem = itemList.ElementAt(r);
                while (newItem.Value > 300)
                {
                    r = rnd.Next(itemList.Count());

                    newItem = itemList.ElementAt(r);
                }

                int r2 = 1;
                if (itemTargetReceive != null)
                {
                    r2 = (itemTargetReceive.Value * itemAmountReceive) / newItem.Value;
                }
                else if (r2 <= 0)
                {
                    r2 = rnd.Next(1, 10);
                }

                itemAmountGive = r2;

                if (alternative)
                {
                    questGen.alternativeMission.updateItemTargets(itemNumb, newItem);
                }
                else
                {
                    questGen.chosenMission.updateItemTargets(itemNumb, newItem);
                }

            }

            else if ((itemTargetGive != null && itemAmountGive == 0) && (itemTargetReceive != null && itemAmountReceive == 0))
            {
                if (itemAmountGive == 0)
                {
                    int amount = 300 / itemTargetGive.Value;
                    if (amount <= 0)
                    {
                        amount = 1;
                    }
                    itemAmountGive = amount;
                }
                if (itemAmountReceive == 0)
                {
                    int amount = 300 / itemTargetReceive.Value;
                    if (amount <= 0)
                    {
                        amount = 1;
                    }
                    itemAmountReceive = amount;
                }

            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete && !actionInLog)
            {
                if (index == 0)
                {
                    actionInLog = true;
                    if (heroTarget != null)
                    {
                        questBase.AddTrackedObject(heroTarget);
                        Campaign.Current.ConversationManager.AddDialogFlow(this.GetExchangeActionDialogFlow(heroTarget, index, questGiver, questBase, questGen), this);

                        TextObject textObject2 = new TextObject("Exhange {ITEM_AMOUNT1} {ITEM_NAME1} for {ITEM_AMOUNT2} {ITEM_NAME2} with {HERO}", null);
                        textObject2.SetTextVariable("HERO", heroTarget.Name);
                        textObject2.SetTextVariable("ITEM_AMOUNT1", itemAmountGive);
                        textObject2.SetTextVariable("ITEM_NAME1", itemTargetGive.Name);
                        textObject2.SetTextVariable("ITEM_AMOUNT2", itemAmountReceive);
                        textObject2.SetTextVariable("ITEM_NAME2", itemTargetReceive.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject2));

                    }
                }
                else
                {
                    if (questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index)
                    {
                        actionInLog = true;
                        if (heroTarget != null)
                        {
                            questBase.AddTrackedObject(heroTarget);
                            Campaign.Current.ConversationManager.AddDialogFlow(this.GetExchangeActionDialogFlow(heroTarget, index, questGiver, questBase, questGen), this);

                            TextObject textObject2 = new TextObject("Exhange {ITEM_AMOUNT1} {ITEM_NAME1} for {ITEM_AMOUNT2} {ITEM_NAME2} with {HERO}", null);
                            textObject2.SetTextVariable("HERO", heroTarget.Name);
                            textObject2.SetTextVariable("ITEM_AMOUNT1", itemAmountGive);
                            textObject2.SetTextVariable("ITEM_NAME1", itemTargetGive.Name);
                            textObject2.SetTextVariable("ITEM_AMOUNT2", itemAmountReceive);
                            textObject2.SetTextVariable("ITEM_NAME2", itemTargetReceive.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject2));

                        }
                    }
                }
            }


        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return this.GetExchangeActionDialogFlow(heroTarget, index, this.questGiver, questBase, questGen);
        }

        private DialogFlow GetExchangeActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            TextObject npcLine1 = new TextObject("I have {ITEM_AMOUNT2} {ITEM_NAME2}, have you brought {ITEM_AMOUNT1} of {ITEM_NAME1} so we can trade?", null);
            npcLine1.SetTextVariable("ITEM_AMOUNT1", itemAmountGive);
            npcLine1.SetTextVariable("ITEM_NAME1", itemTargetGive.Name);
            npcLine1.SetTextVariable("ITEM_AMOUNT2", itemAmountReceive);
            npcLine1.SetTextVariable("ITEM_NAME2", itemTargetReceive.Name);
            TextObject textObject = new TextObject("Pleasure doing business with you {?PLAYER.GENDER}milady{?}sir{\\?}, safe travels.", null);
            TextObject textObject2 = new TextObject("We await your return, {?PLAYER.GENDER}milady{?}sir{\\?}.", null);
            textObject.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);
            textObject2.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);

            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index == questGen.currentActionIndex).BeginPlayerOptions().PlayerOption(new TextObject("Yes. Here you go.", null), null).ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(questGen.ReturnItemClickableConditionsExchange)).NpcLine(textObject, null, null).Consequence(delegate
            {
                this.exchangeConsequences(index, questBase, questGen);
            }).CloseDialog().PlayerOption(new TextObject("I'm working on it.", null), null).NpcLine(textObject2, null, null).CloseDialog().EndPlayerOptions().CloseDialog();
        }

        private void exchangeConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                questGen.currentActionIndex++;
                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);

                GiveItemAction.ApplyForParties(PartyBase.MainParty, Settlement.CurrentSettlement.Party, itemTargetGive, itemAmountGive);
                GiveItemAction.ApplyForParties(Settlement.CurrentSettlement.Party, PartyBase.MainParty, itemTargetReceive, itemAmountReceive);
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

        //public override void OnPlayerInventoryExchangeQuest(List<(ItemRosterElement, int)> purchasedItems, List<(ItemRosterElement, int)> soldItems, bool isTrading, int index, QuestGenTestQuest questGen, QuestBase questBase)
        //{
        //    bool flag = false;

        //    foreach (ValueTuple<ItemRosterElement, int> valueTuple in purchasedItems)
        //    {
        //        ItemRosterElement item = valueTuple.Item1;
        //        if (item.EquipmentElement.Item == this.itemTargetGive)
        //        {
        //            flag = true;
        //            break;
        //        }
        //    }
        //    if (!flag)
        //    {
        //        foreach (ValueTuple<ItemRosterElement, int> valueTuple2 in soldItems)
        //        {
        //            ItemRosterElement item = valueTuple2.Item1;
        //            if (item.EquipmentElement.Item == this.itemTargetGive)
        //            {
        //                flag = true;
        //                break;
        //            }
        //        }
        //    }
        //    if (flag)
        //    {
        //        int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTargetGive);
        //        questGen.UpdateQuestTaskS(questGen.journalLogs[index], currentItemProgress);
        //        if (currentItemProgress >= this.itemAmountGive)
        //        {
        //            TextObject textObject2 = new TextObject("Exhange {ITEM_AMOUNT1} {ITEM_NAME1} for {ITEM_AMOUNT2} {ITEM_NAME2} with {HERO}", null);
        //            textObject2.SetTextVariable("HERO", this.heroTarget.Name);
        //            textObject2.SetTextVariable("ITEM_AMOUNT1", this.itemAmountGive);
        //            textObject2.SetTextVariable("ITEM_NAME1", this.itemTargetGive.Name);
        //            textObject2.SetTextVariable("ITEM_AMOUNT2", this.itemAmountReceive);
        //            textObject2.SetTextVariable("ITEM_NAME2", this.itemTargetReceive.Name);
        //            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);
        //        }
        //    }
        //}

        //public override void OnPartyConsumedFoodQuest(MobileParty party, int index, QuestGenTestQuest questGen, QuestBase questBase)
        //{
        //    if (this.GetHeroTarget() != null)
        //    {
        //        int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTargetGive);
        //        questGen.UpdateQuestTaskS(questGen.journalLogs[index], currentItemProgress);
        //        if (currentItemProgress >= this.itemAmountGive)
        //        {
        //            TextObject textObject2 = new TextObject("Exhange {ITEM_AMOUNT1} {ITEM_NAME1} for {ITEM_AMOUNT2} {ITEM_NAME2} with {HERO}", null);
        //            textObject2.SetTextVariable("HERO", this.heroTarget.Name);
        //            textObject2.SetTextVariable("ITEM_AMOUNT1", this.itemAmountGive);
        //            textObject2.SetTextVariable("ITEM_NAME1", this.itemTargetGive.Name);
        //            textObject2.SetTextVariable("ITEM_AMOUNT2", this.itemAmountReceive);
        //            textObject2.SetTextVariable("ITEM_NAME2", this.itemTargetReceive.Name);
        //            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);
        //        }
        //    }
        //}

        //public override void OnHeroSharedFoodWithAnotherHeroQuest(Hero supporterHero, Hero supportedHero, float influence, int index, QuestGenTestQuest questGen, QuestBase questBase)
        //{
        //    if (this.GetHeroTarget() != null)
        //    {
        //        int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTargetGive);
        //        questGen.UpdateQuestTaskS(questGen.journalLogs[index], currentItemProgress);
        //        if (currentItemProgress >= this.itemAmountGive)
        //        {
        //            TextObject textObject2 = new TextObject("Exhange {ITEM_AMOUNT1} {ITEM_NAME1} for {ITEM_AMOUNT2} {ITEM_NAME2} with {HERO}", null);
        //            textObject2.SetTextVariable("HERO", this.heroTarget.Name);
        //            textObject2.SetTextVariable("ITEM_AMOUNT1", this.itemAmountGive);
        //            textObject2.SetTextVariable("ITEM_NAME1", this.itemTargetGive.Name);
        //            textObject2.SetTextVariable("ITEM_AMOUNT2", this.itemAmountReceive);
        //            textObject2.SetTextVariable("ITEM_NAME2", this.itemTargetReceive.Name);
        //            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);
        //        }
        //    }
        //}

        //public override void OnEquipmentSmeltedByHeroEventQuest(Hero hero, EquipmentElement equipmentElement, int index, QuestGenTestQuest questGen, QuestBase questBase)
        //{
        //    if (RefinePatch.itemRefined)
        //    {
        //        bool flag = false;
        //        int amountRemaining = this.itemAmountGive;
        //        int amountPurchased = 0;

        //        var refined = RefinePatch.refineFormulaS;


        //        if (refined.OutputCount > 0)
        //        {
        //            ItemObject craftingMaterialItem3 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refined.Output);
        //            if (craftingMaterialItem3.Name == equipmentElement.Item.Name)
        //            {
        //                flag = true;
        //                amountPurchased += refined.OutputCount;
        //            }

        //        }
        //        if (refined.Output2Count > 0)
        //        {
        //            ItemObject craftingMaterialItem4 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refined.Output2);
        //            if (craftingMaterialItem4.Name == equipmentElement.Item.Name)
        //            {
        //                flag = true;
        //                amountPurchased += refined.Output2Count;
        //            }

        //        }

        //        if (flag)
        //        {
        //            int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTargetGive);
        //            questGen.UpdateQuestTaskS(questGen.journalLogs[index], currentItemProgress);
        //            if (currentItemProgress >= this.itemAmountGive)
        //            {
        //                TextObject textObject2 = new TextObject("Exhange {ITEM_AMOUNT1} {ITEM_NAME1} for {ITEM_AMOUNT2} {ITEM_NAME2} with {HERO}", null);
        //                textObject2.SetTextVariable("HERO", this.heroTarget.Name);
        //                textObject2.SetTextVariable("ITEM_AMOUNT1", this.itemAmountGive);
        //                textObject2.SetTextVariable("ITEM_NAME1", this.itemTargetGive.Name);
        //                textObject2.SetTextVariable("ITEM_AMOUNT2", this.itemAmountReceive);
        //                textObject2.SetTextVariable("ITEM_NAME2", this.itemTargetReceive.Name);
        //                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);
        //            }
        //        }

        //        RefinePatch.itemRefined = false;
        //    }
        //    else
        //    {
        //        bool flag = false;
        //        int amountRemaining = itemAmountGive;
        //        int amountPurchased = 0;
        //        int[] smeltingOutputForItem = Campaign.Current.Models.SmithingModel.GetSmeltingOutputForItem(equipmentElement.Item);

        //        Campaign campaign = Campaign.Current;
        //        GameModels models = campaign.Models;
        //        ItemObject resourceItem;

        //        for (int i = 0; i < smeltingOutputForItem.Length; i++)
        //        {
        //            var material = (CraftingMaterials)i;

        //            SmithingModel smithingModel = models.SmithingModel;
        //            resourceItem = ((smithingModel != null) ? smithingModel.GetCraftingMaterialItem(material) : null);

        //            if (resourceItem.Name == this.itemTargetGive.Name)
        //            {
        //                flag = true;
        //                amountPurchased += smeltingOutputForItem[i];
        //                break;
        //            }
        //        }

        //        if (flag)
        //        {
        //            int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTargetGive);
        //            questGen.UpdateQuestTaskS(questGen.journalLogs[index], currentItemProgress);
        //            if (currentItemProgress >= this.itemAmountGive)
        //            {
        //                TextObject textObject2 = new TextObject("Exhange {ITEM_AMOUNT1} {ITEM_NAME1} for {ITEM_AMOUNT2} {ITEM_NAME2} with {HERO}", null);
        //                textObject2.SetTextVariable("HERO", this.heroTarget.Name);
        //                textObject2.SetTextVariable("ITEM_AMOUNT1", this.itemAmountGive);
        //                textObject2.SetTextVariable("ITEM_NAME1", this.itemTargetGive.Name);
        //                textObject2.SetTextVariable("ITEM_AMOUNT2", this.itemAmountReceive);
        //                textObject2.SetTextVariable("ITEM_NAME2", this.itemTargetReceive.Name);
        //                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);
        //            }
        //        }
        //    }

        //}

        //public override void OnNewItemCraftedEventQuest(ItemObject item, Crafting.OverrideData crafted, bool flag2, int index, QuestGenTestQuest questGen, QuestBase questBase)
        //{
        //    bool flag = false;
        //    int amountRemaining = this.itemAmountGive;
        //    int amountPurchased = 0;

        //    if (item == this.itemTargetGive)
        //    {
        //        flag = true;
        //        amountPurchased++;
        //    }

        //    if (flag)
        //    {
        //        int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTargetGive);
        //        questGen.UpdateQuestTaskS(questGen.journalLogs[index], currentItemProgress);
        //        if (currentItemProgress >= this.itemAmountGive)
        //        {
        //            TextObject textObject2 = new TextObject("Exhange {ITEM_AMOUNT1} {ITEM_NAME1} for {ITEM_AMOUNT2} {ITEM_NAME2} with {HERO}", null);
        //            textObject2.SetTextVariable("HERO", this.heroTarget.Name);
        //            textObject2.SetTextVariable("ITEM_AMOUNT1", this.itemAmountGive);
        //            textObject2.SetTextVariable("ITEM_NAME1", this.itemTargetGive.Name);
        //            textObject2.SetTextVariable("ITEM_AMOUNT2", this.itemAmountReceive);
        //            textObject2.SetTextVariable("ITEM_NAME2", this.itemTargetReceive.Name);
        //            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);
        //        }
        //    }

        //}

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

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
        }

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
            for (int i = 0; i < this.Action.param.Count; i++)
            {
                Parameter p = this.Action.param[i];
                if (p.target == targetString && i == 1)
                {
                    p.target = targetItem.Name.ToString();
                    itemTargetGive = targetItem;
                    break;
                }
                if (p.target == targetString && i == 2)
                {
                    p.target = targetItem.Name.ToString();
                    itemTargetReceive = targetItem;
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
                    strat.SetTextVariable("ITEM", itemTargetGive.Name);
                    break;
                case "Trade for supplies":
                    strat = new TextObject("I have some {ITEM} here we can exchange for {ITEM2}. Would you be interested in taking a look ?", null);
                    strat.SetTextVariable("ITEM", itemTargetReceive.Name);
                    strat.SetTextVariable("ITEM2", itemTargetGive.Name);
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
                    strat.SetTextVariable("ITEM", itemTargetGive.Name);
                    break;
                case "Trade for supplies":
                    strat = new TextObject("Trade {ITEM} for {ITEM2}.", null);
                    strat.SetTextVariable("ITEM", itemTargetReceive.Name);
                    strat.SetTextVariable("ITEM2", itemTargetGive.Name);
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
                    strat.SetTextVariable("ITEM", itemTargetGive.Name);
                    strat.SetTextVariable("TYPE", itemTargetGive.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", itemTargetGive.ItemCategory.ToString());
                    strat.SetTextVariable("VALUE", itemTargetGive.Value);
                    break;
                case "Trade for supplies":
                    strat = new TextObject("{ITEM} is a type of {TYPE}, belonging to the category of {CATEGORY} and is valued around {VALUE} gold coins.", null);
                    strat.SetTextVariable("ITEM", itemTargetGive.Name);
                    strat.SetTextVariable("TYPE", itemTargetGive.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", itemTargetGive.ItemCategory.ToString());
                    strat.SetTextVariable("VALUE", itemTargetGive.Value);
                    break;

            }
            return strat.ToString();
        }

        public override TextObject getStepDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Deliver item for study":
                    strat = new TextObject("Exchange {ITEM} for {ITEM2} with {HERO} from {SETTLEMENT}.", null);
                    strat.SetTextVariable("ITEM", itemTargetReceive.Name);
                    strat.SetTextVariable("ITEM2", itemTargetGive.Name);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    if (this.heroTarget.CurrentSettlement != null)
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    break;
                case "Obtain luxuries":
                    strat = new TextObject("Exchange {ITEM} for {ITEM2} with {HERO} from {SETTLEMENT}.", null);
                    strat.SetTextVariable("ITEM", itemTargetReceive.Name);
                    strat.SetTextVariable("ITEM2", itemTargetGive.Name);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    if (this.heroTarget.CurrentSettlement != null)
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    break;
                case "Obtain rare items":
                    strat = new TextObject("Exchange {ITEM} for {ITEM2} with {HERO} from {SETTLEMENT}.", null);
                    strat.SetTextVariable("ITEM", itemTargetReceive.Name);
                    strat.SetTextVariable("ITEM2", itemTargetGive.Name);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    if (this.heroTarget.CurrentSettlement != null)
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    break;
                case "Recover lost/stolen item":
                    strat = new TextObject("Exchange {ITEM} for {ITEM2} with {HERO} from {SETTLEMENT}.", null);
                    strat.SetTextVariable("ITEM", itemTargetReceive.Name);
                    strat.SetTextVariable("ITEM2", itemTargetGive.Name);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    if (this.heroTarget.CurrentSettlement != null)
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    break;
                case "Gather raw materials":
                    strat = new TextObject("Exchange {ITEM} for {ITEM2} with {HERO} from {SETTLEMENT}.", null);
                    strat.SetTextVariable("ITEM", itemTargetReceive.Name);
                    strat.SetTextVariable("ITEM2", itemTargetGive.Name);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    if (this.heroTarget.CurrentSettlement != null)
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    break;
                case "Deliver supplies":
                    strat = new TextObject("Exchange {ITEM} for {ITEM2} with {HERO} from {SETTLEMENT}.", null);
                    strat.SetTextVariable("ITEM", itemTargetReceive.Name);
                    strat.SetTextVariable("ITEM2", itemTargetGive.Name);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    if (this.heroTarget.CurrentSettlement != null)
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    break;
                case "Trade for supplies":
                    strat = new TextObject("Exchange {ITEM} for {ITEM2} with {HERO} from {SETTLEMENT}.", null);
                    strat.SetTextVariable("ITEM", itemTargetReceive.Name);
                    strat.SetTextVariable("ITEM2", itemTargetGive.Name);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    if (this.heroTarget.CurrentSettlement != null)
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    break;

            }
            return strat;
        }

    }
}
