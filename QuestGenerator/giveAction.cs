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
using QuestGenerator.QuestBuilder.CustomBT;

namespace QuestGenerator
{
    public class giveAction : actionTarget
    {
        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        [XmlIgnore]
        public ItemObject itemTarget;

        public int itemAmount = 0;

        public bool partTwo = false;
        public giveAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public giveAction() { }

        public override int GetItemAmount()
        {
            return this.itemAmount;
        }

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

                this.heroTarget = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
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

                this.questGiver = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
        }
        public override void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("npc"))
            {
                string npcNumb = this.Action.param[0].target;
                string targetHero = "none";
                Hero newHero = new Hero();
                int i = this.index;
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
                            foreach (Hero hero in this.questGiver.CurrentSettlement.Notables)
                            {
                                if (hero != this.questGiver)
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
                            foreach (Hero hero in this.questGiver.CurrentSettlement.Notables)
                            {
                                if (hero != this.questGiver)
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
                    foreach (Hero hero in questBase.IssueSettlement.Notables)
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
                    InformationManager.DisplayMessage(new InformationMessage("give action - line 196"));
                }

            }

            if (this.Action.param[1].target.Contains("item"))
            {
                Hero toGiveHero = this.heroTarget;
                int amount = 0;
                string itemNumb = this.Action.param[1].target;
                ItemObject newItem;
                var itemList = Items.All;

                int r = rnd.Next(itemList.Count());

                newItem = itemList.ElementAt(r);
                while (newItem.Value > 300)
                {
                    r = rnd.Next(itemList.Count());

                    newItem = itemList.ElementAt(r);
                }

                amount = 300/newItem.Value;
                if (amount <= 0)
                {
                    amount = 1;
                }
                this.SetItemAmount(amount);

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

        public override void QuestQ(QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                if (this.index == 0)
                {
                    this.actionInLog = true;
                    if (this.heroTarget != null)
                    {
                        questBase.AddTrackedObject(this.heroTarget);
                        Campaign.Current.ConversationManager.AddDialogFlow(this.GetGiveActionDialogFlow(this.heroTarget, index, this.questGiver, questBase, questGen), this);

                        TextObject textObject = new TextObject("Get {ITEM_AMOUNT} {ITEM_NAME} to give to {HERO}", null);
                        textObject.SetTextVariable("HERO", this.heroTarget.Name);
                        textObject.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
                        textObject.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
                        int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);
                        if (currentItemProgress < this.itemAmount)
                        {
                            TextObject textObject1 = new TextObject("You have enough items to complete the quest.", null);
                            textObject1.SetTextVariable("QUEST_SETTLEMENT", this.questGiver.CurrentSettlement.Name);
                            InformationManager.AddQuickInformation(textObject1, 0, null, "");
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, currentItemProgress, this.itemAmount, null, false);
                        }
                        else
                        {
                            partTwo = true;
                            TextObject textObject2 = new TextObject("Give {ITEM_AMOUNT} {ITEM_NAME} to {HERO}", null);
                            textObject2.SetTextVariable("HERO", this.heroTarget.Name);
                            textObject2.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
                            textObject2.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
                            questGen.journalLogs[this.index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);
                        }

                    }
                }
                else
                {
                    if (questGen.actionsInOrder[this.index - 1].actioncomplete)
                    {
                        this.actionInLog = true;
                        if (this.heroTarget != null)
                        {
                            questBase.AddTrackedObject(this.heroTarget);
                            Campaign.Current.ConversationManager.AddDialogFlow(this.GetGiveActionDialogFlow(this.heroTarget, index, this.questGiver, questBase, questGen), this);

                            int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.itemTarget);

                            TextObject textObject2 = new TextObject("Give {ITEM_AMOUNT} {ITEM_NAME} to {HERO}", null);
                            textObject2.SetTextVariable("HERO", this.heroTarget.Name);
                            textObject2.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
                            textObject2.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
                            questGen.journalLogs[this.index] = questGen.getDiscreteLog(textObject2, textObject2, 0, 1, null, false);


                        }
                    }
                }
            }
            
            
        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return GetGiveActionDialogFlow(this.heroTarget, index, this.questGiver, questBase, questGen);
        }

        private DialogFlow GetGiveActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            TextObject npcLine1 = new TextObject("Have you brought {ITEM_AMOUNT} of {ITEM_NAME}?", null);
            npcLine1.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
            npcLine1.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
            TextObject textObject = new TextObject("Thank you, {?PLAYER.GENDER}milady{?}sir{\\?}! You are a saint.", null);
            TextObject textObject2 = new TextObject("We await your success, {?PLAYER.GENDER}milady{?}sir{\\?}.", null);
            textObject.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);
            textObject2.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);

            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index == questGen.currentActionIndex).BeginPlayerOptions().PlayerOption(new TextObject("Yes. Here is what you asked for.", null), null).ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(questGen.ReturnItemClickableConditions)).NpcLine(textObject, null, null).Consequence(delegate
            {
                this.giveConsequences(index, questBase, questGen);
            }).CloseDialog().PlayerOption(new TextObject("I'm working on it.", null), null).NpcLine(textObject2, null, null).CloseDialog().EndPlayerOptions().CloseDialog();
        }

        private void giveConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                questGen.currentActionIndex++;

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);

                GiveItemAction.ApplyForParties(PartyBase.MainParty, Settlement.CurrentSettlement.Party, this.itemTarget, this.itemAmount);
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
        //        if (item.EquipmentElement.Item == this.GetItemTarget())
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
        //            if (item.EquipmentElement.Item == this.GetItemTarget())
        //            {
        //                flag = true;
        //                break;
        //            }
        //        }
        //    }
        //    if (flag)
        //    {
        //        int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.GetItemTarget());
        //        questGen.UpdateQuestTaskS(questGen.journalLogs[index], currentItemProgress);
        //        if (currentItemProgress >= this.itemAmount && !partTwo)
        //        {
        //            partTwo = true;
        //            TextObject textObject = new TextObject("Give {ITEM_AMOUNT} {ITEM_NAME} to {HERO}", null);
        //            textObject.SetTextVariable("HERO", this.heroTarget.Name);
        //            textObject.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
        //            textObject.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
        //            questGen.journalLogs[this.index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
        //        }
        //    }
        //}

        //public override void OnPartyConsumedFoodQuest(MobileParty party, int index, QuestGenTestQuest questGen, QuestBase questBase)
        //{
        //    if (this.GetItemTarget() != null)
        //    {
        //        int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.GetItemTarget());
        //        questGen.UpdateQuestTaskS(questGen.journalLogs[index - 1], currentItemProgress);

        //        if (currentItemProgress >= this.itemAmount && !partTwo)
        //        {
        //            partTwo = true;
        //            TextObject textObject = new TextObject("Give {ITEM_AMOUNT} {ITEM_NAME} to {HERO}", null);
        //            textObject.SetTextVariable("HERO", this.heroTarget.Name);
        //            textObject.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
        //            textObject.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
        //            questGen.journalLogs[this.index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
        //        }
        //    }
        //}

        //public override void OnHeroSharedFoodWithAnotherHeroQuest(Hero supporterHero, Hero supportedHero, float influence, int index, QuestGenTestQuest questGen, QuestBase questBase)
        //{
        //    if (this.GetItemTarget() != null)
        //    {
        //        int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.GetItemTarget());
        //        questGen.UpdateQuestTaskS(questGen.journalLogs[index - 1], currentItemProgress);

        //        if (currentItemProgress >= this.itemAmount && !partTwo)
        //        {
        //            partTwo = true;
        //            TextObject textObject = new TextObject("Give {ITEM_AMOUNT} {ITEM_NAME} to {HERO}", null);
        //            textObject.SetTextVariable("HERO", this.heroTarget.Name);
        //            textObject.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
        //            textObject.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
        //            questGen.journalLogs[this.index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
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
                    this.heroTarget = targetHero;
                    break;
                }
            }
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

        public override TextObject getDescription(string strategy)
        {
            TextObject strat = new TextObject("empty",null);
            switch (strategy)
            {
                case "Deliver item for study":
                    strat = new TextObject("I need you to deliver {ITEM} to me, think you could do that?", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
                case "Obtain luxuries":
                    strat = new TextObject("There are some things I've been craving. Do you think you could bring {ITEM} to me?", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
                case "Obtain rare items":
                    strat = new TextObject("{ITEM} has been lacking and is now rare. Find it and bring it to me.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
                case "Recover lost/stolen item":
                    strat = new TextObject("I've lost {ITEM}, think you could get it back for me?", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
                case "Deliver supplies":
                    strat = new TextObject("We are in need some supplies in our settlement. Can you get {ITEM} for us?", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
            }
            return strat;
        }

        public override TextObject getTitle(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Deliver item for study":
                    strat = new TextObject("Deliver {ITEM} for study.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
                case "Obtain luxuries":
                    strat = new TextObject("Obtain {ITEM}.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
                case "Obtain rare items":
                    strat = new TextObject("Obtain {ITEM}.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
                case "Recover lost/stolen item":
                    strat = new TextObject("Recover {ITEM}.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
                case "Deliver supplies":
                    strat = new TextObject("Deliver {ITEM}.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    break;
            }
            return strat;
        }

        public override string getListenString(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Deliver item for study":
                    strat = new TextObject("{ITEM} is a type of {TYPE}, belonging to the category of {CATEGORY} and is part of the {CULTURE} culture.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    strat.SetTextVariable("TYPE", this.itemTarget.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", this.itemTarget.ItemCategory.ToString());
                    strat.SetTextVariable("CULTURE", this.itemTarget.Culture.ToString());
                    break;
                case "Obtain luxuries":
                    strat = new TextObject("{ITEM} is a type of {TYPE}, belonging to the category of {CATEGORY} and is part of the {CULTURE} culture.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    strat.SetTextVariable("TYPE", this.itemTarget.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", this.itemTarget.ItemCategory.ToString());
                    strat.SetTextVariable("CULTURE", this.itemTarget.Culture.ToString());
                    break;
                case "Obtain rare items":
                    strat = new TextObject("{ITEM} is a type of {TYPE}, belonging to the category of {CATEGORY} and is part of the {CULTURE} culture.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    strat.SetTextVariable("TYPE", this.itemTarget.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", this.itemTarget.ItemCategory.ToString());
                    strat.SetTextVariable("CULTURE", this.itemTarget.Culture.ToString());
                    break;
                case "Recover lost/stolen item":
                    strat = new TextObject("{ITEM} is a type of {TYPE}, belonging to the category of {CATEGORY} and is part of the {CULTURE} culture.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    strat.SetTextVariable("TYPE", this.itemTarget.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", this.itemTarget.ItemCategory.ToString());
                    strat.SetTextVariable("CULTURE", this.itemTarget.Culture.ToString());
                    break;
                case "Deliver supplies":
                    strat = new TextObject("{ITEM} is a type of {TYPE}, belonging to the category of {CATEGORY} and is part of the {CULTURE} culture.", null);
                    strat.SetTextVariable("ITEM", this.itemTarget.Name);
                    strat.SetTextVariable("TYPE", this.itemTarget.ItemType.ToString());
                    strat.SetTextVariable("CATEGORY", this.itemTarget.ItemCategory.ToString());
                    strat.SetTextVariable("CULTURE", this.itemTarget.Culture.ToString());
                    break;
            }
            return strat.ToString();
        }

    }
}
