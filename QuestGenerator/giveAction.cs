using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.Core;
using static QuestGenerator.QuestGenTestCampaignBehavior;
using TaleWorlds.CampaignSystem.Actions;
using System.IO;

namespace QuestGenerator
{
    [Serializable]
    class giveAction : actionTarget
    {
        static Random rnd = new Random();

        [NonSerialized]
        public Hero heroTarget;

        [NonSerialized]
        public ItemObject itemTarget;

        public int itemAmount;

        public giveAction(string action, string target) : base(action, target)
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

            int amount = this.GetItemAmount();

            if (this.heroTarget == null && amount<=0)
            {
                var setName = this.target;
                InformationManager.DisplayMessage(new InformationMessage(setName));

                Hero[] array = (from x in Hero.All where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB give"));
                }
                if (array.Length == 1)
                {
                    this.heroTarget = array[0];
                }
            }

            if (this.GetItemTarget() == null && amount > 0)
            {
                var setName = this.target;
                InformationManager.DisplayMessage(new InformationMessage(setName));

                ItemObject[] array = (from x in ItemObject.All where (x.Name.ToString() == setName) select x).ToArray<ItemObject>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB give"));
                }
                if (array.Length == 1)
                {
                    this.itemTarget = array[0];
                }

            }
        }
        public override void IssueQ(List<actionTarget> list, Settlement issueSettlement, Hero issueGiver)
        {
            if (this.target.Contains("npc"))
            {
                string npcNumb = this.target;
                string targetHero = "none";
                Hero newHero = new Hero();
                int i = list.IndexOf(this);
                if (i > 0)
                {
                    if (list[i - 1].action == "goto")
                    {
                        Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == list[i - 1].target) select x).ToArray<Settlement>();

                        if (array.Length > 1)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Everything is on fire Issue"));
                        }
                        if (array.Length == 1)
                        {
                            newHero = array[0].Notables.GetRandomElement();
                            targetHero = newHero.Name.ToString();

                        }

                        if (targetHero != "none")
                        {
                            foreach (actionTarget nextAction in list)
                            {
                                if (nextAction.target == npcNumb)
                                {
                                    nextAction.target = targetHero;
                                    nextAction.SetHeroTarget(newHero);
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (Hero hero in issueSettlement.Notables)
                        {
                            if (hero != issueGiver)
                            {
                                targetHero = hero.Name.ToString();
                                newHero = hero;
                            }
                        }

                        if (targetHero != "none")
                        {
                            foreach (actionTarget nextAction in list)
                            {
                                if (nextAction.target == npcNumb)
                                {
                                    nextAction.target = targetHero;
                                    nextAction.SetHeroTarget(newHero);
                                }
                            }
                        }
                    }
                }

                else if (i == 0)
                {
                    foreach (Hero hero in issueSettlement.Notables)
                    {
                        if (hero != issueGiver)
                        {
                            targetHero = hero.Name.ToString();
                            newHero = hero;
                            break;
                        }
                    }

                    if (targetHero != "none")
                    {
                        foreach (actionTarget nextAction in list)
                        {
                            if (nextAction.target == npcNumb)
                            {
                                nextAction.target = targetHero;
                                nextAction.SetHeroTarget(newHero);
                                break;
                            }
                        }
                    }
                }

                if (targetHero == "none")
                {
                    InformationManager.DisplayMessage(new InformationMessage("Target Hero is on fire"));
                }


            }

            if (this.target.Contains("item"))
            {
                Hero toGiveHero = list[list.IndexOf(this) - 1].GetHeroTarget();
                int amount = 1;
                string npcNumb = this.target;
                
                while (this.GetItemTarget() == null)
                {
                    foreach (ItemRosterElement itemRosterElement in issueGiver.CurrentSettlement.ItemRoster)
                    {
                        if (itemRosterElement.Amount <= amount)
                        {
                            this.target = itemRosterElement.EquipmentElement.Item.Name.ToString();
                            this.SetItemTarget(itemRosterElement.EquipmentElement.Item);
                            int r = rnd.Next(1,10);
                            this.SetItemAmount(r);
                            break;
                        }
                    }
                    amount += 1;
                }

                if (this.GetItemTarget() != null)
                {
                    foreach (actionTarget nextAction in list)
                    {
                        if (nextAction.target == npcNumb)
                        {
                            nextAction.target = this.target;
                            nextAction.SetItemTarget(this.GetItemTarget());
                            int r2 = rnd.Next(5) + 5;
                            nextAction.SetItemAmount(r2);
                            break;
                        }
                    }
                }
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage("Target Item is on fire"));
                }

            }

            else if (this.GetItemTarget() != null && this.GetItemAmount() == 0)
            {
                int r = rnd.Next(1,10);
                this.SetItemAmount(r);
            }

        }

        public override void QuestQ(List<actionTarget> list, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen, int index)
        {
            if (this.heroTarget != null)
            {
                questBase.AddTrackedObject(this.heroTarget);
                Campaign.Current.ConversationManager.AddDialogFlow(this.GetGiveActionDialogFlow(this.heroTarget, index, questGiver,questBase, questGen), this);

                TextObject textObject = new TextObject("Get {ITEM_AMOUNT} {ITEM_NAME} to give to {HERO}", null);
                textObject.SetTextVariable("HERO", this.heroTarget.Name);
                textObject.SetTextVariable("ITEM_AMOUNT", list[index + 1].GetItemAmount());
                textObject.SetTextVariable("ITEM_NAME", list[index + 1].GetItemTarget().Name);
                int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(list[index + 1].GetItemTarget());
                if (currentItemProgress < list[index + 1].GetItemAmount())
                {
                    TextObject textObject1 = new TextObject("You have enough items to complete the quest.", null);
                    textObject1.SetTextVariable("QUEST_SETTLEMENT", questBase.QuestGiver.CurrentSettlement.Name);
                    InformationManager.AddQuickInformation(textObject1, 0, null, "");
                    questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, currentItemProgress, list[index + 1].GetItemAmount(), null, false);
                }
                else
                {
                    questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, list[index + 1].GetItemAmount(), list[index + 1].GetItemAmount(), null, false);
                }


            }
            else
            {
                TextObject textObject = new TextObject("Give {ITEM_AMOUNT} {ITEM_NAME} to {HERO}", null);
                textObject.SetTextVariable("HERO", list[index - 1].GetHeroTarget().Name);
                textObject.SetTextVariable("ITEM_AMOUNT", this.itemAmount);
                textObject.SetTextVariable("ITEM_NAME", this.itemTarget.Name);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);

            }
        }

        public override DialogFlow getDialogFlows( int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return GetGiveActionDialogFlow(this.heroTarget, index, questGiver, questBase, questGen);
        }

        private DialogFlow GetGiveActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {

            TextObject npcLine1 = new TextObject("Have you brought {ITEM_AMOUNT} of {ITEM_NAME}?", null);
            npcLine1.SetTextVariable("ITEM_AMOUNT", questGen.actionsTargets[index + 1].GetItemAmount());
            npcLine1.SetTextVariable("ITEM_NAME", questGen.actionsTargets[index + 1].GetItemTarget().Name);
            TextObject textObject = new TextObject("Thank you, {?PLAYER.GENDER}milady{?}sir{\\?}! You are a saviour.", null);
            TextObject textObject2 = new TextObject("We await your success, {?PLAYER.GENDER}milady{?}sir{\\?}.", null);
            textObject.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);
            textObject2.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);

            InformationManager.DisplayMessage(new InformationMessage("return give dialog flow"));
            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index == questGen.currentActionIndex).BeginPlayerOptions().PlayerOption(new TextObject("Yes. Here is what you asked for.", null), null).ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(questGen.ReturnItemClickableConditions)).NpcLine(textObject, null, null).Consequence(delegate
            {
                this.giveConsequences(index, questBase, questGen);
            }).CloseDialog().PlayerOption(new TextObject("I'm working on it.", null), null).NpcLine(textObject2, null, null).CloseDialog().EndPlayerOptions().CloseDialog();
        }

        private void giveConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            questGen.currentActionIndex++;
            questGen.currentActionIndex++;
            questGen.UpdateQuestTaskS(questGen.journalLogs[index + 1], 1);
            questGen.UpdateQuestTaskS(questGen.journalLogs[index], questGen.actionsTargets[index + 1].GetItemAmount());

            GiveItemAction.ApplyForParties(PartyBase.MainParty, Settlement.CurrentSettlement.Party, questGen.actionsTargets[index + 1].GetItemTarget(), questGen.actionsTargets[index + 1].GetItemAmount());

            if (questGen.currentActionIndex < questGen.actionsTargets.Count)
            {
                questGen.currentAction = questGen.actionsTargets[questGen.currentActionIndex];
            }
            else
            {
                questGen.SuccessConsequences();
            }
        }

        public override void OnPlayerInventoryExchangeQuest(List<(ItemRosterElement, int)> purchasedItems, List<(ItemRosterElement, int)> soldItems, bool isTrading, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            bool flag = false;

            foreach (ValueTuple<ItemRosterElement, int> valueTuple in purchasedItems)
            {
                ItemRosterElement item = valueTuple.Item1;
                if (item.EquipmentElement.Item == this.GetItemTarget())
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                foreach (ValueTuple<ItemRosterElement, int> valueTuple2 in soldItems)
                {
                    ItemRosterElement item = valueTuple2.Item1;
                    if (item.EquipmentElement.Item == this.GetItemTarget())
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.GetItemTarget());
                questGen.UpdateQuestTaskS(questGen.journalLogs[index - 1], currentItemProgress);
            }
        }

        public override void OnPartyConsumedFoodQuest(MobileParty party, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.GetItemTarget() != null)
            {
                int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.GetItemTarget());
                questGen.UpdateQuestTaskS(questGen.journalLogs[index - 1], currentItemProgress);
            }
        }

        public override void OnHeroSharedFoodWithAnotherHeroQuest(Hero supporterHero, Hero supportedHero, float influence, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.GetItemTarget() != null)
            {
                int currentItemProgress = PartyBase.MainParty.ItemRoster.GetItemNumber(this.GetItemTarget());
                questGen.UpdateQuestTaskS(questGen.journalLogs[index - 1], currentItemProgress);
            }
        }

    }
}
