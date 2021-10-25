using Helpers;
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
    public class gotoAction : actionTarget
    {
        [XmlIgnore]
        public Settlement settlementTarget;

        [XmlIgnore]
        public Hero heroTarget;

        public bool settlementEntered = false;

        public bool nextIsGotoFlag = false;

        public gotoAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public gotoAction() { }

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
            if (settlementTarget == null)
            {
                var setName = this.Action.param[0].target;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("goto action - line 45"));
                }
                if (array.Length == 1)
                {
                    settlementTarget = array[0];
                }
            }

            if (heroTarget == null)
            {
                var setName = this.Action.param[0].target;

                heroTarget = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
            if (questGiver == null)
            {
                var setName = questGiverString;

                questGiver = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative)
        {

            if (this.Action.param[0].target.Contains("place"))
            {
                string placeNumb = this.Action.param[0].target;
                Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                {
                    return x != questGiver.CurrentSettlement && !x.IsHideout && x.Notables.Count >= 1 && x.MapFaction == questGiver.MapFaction;
                });

                if (alternative)
                {
                    questGen.alternativeMission.updateSettlementTargets(placeNumb, settlement);
                }
                else
                {
                    questGen.chosenMission.updateSettlementTargets(placeNumb, settlement);
                }
            }

            if (alternative)
            {

                if (this.index < questGen.alternativeActionsInOrder.Count - 1)
                {
                    if (questGen.alternativeActionsInOrder[this.index + 1].action == "goto" || questGen.alternativeActionsInOrder[this.index + 1].action == "explore")
                    {
                        this.nextIsGotoFlag = true;
                    }
                }
            }
            else
            {
                if (this.index < questGen.actionsInOrder.Count - 1)
                {
                    if (questGen.actionsInOrder[this.index + 1].action == "goto" || questGen.actionsInOrder[this.index + 1].action == "explore")
                    {
                        this.nextIsGotoFlag = true;
                    }
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
                    if (settlementTarget != null)
                    {
                        questBase.AddTrackedObject(settlementTarget);
                        TextObject textObject = new TextObject("Visit the settlement of {SETTLEMENT}", null);
                        textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);

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
                            questBase.AddTrackedObject(settlementTarget);
                            TextObject textObject = new TextObject("Visit the settlement of {SETTLEMENT}", null);
                            textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);

                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        }
                    }
                }
            }


        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return this.GetGotoActionDialogFlow(heroTarget, index, this.questGiver, questBase, questGen);
        }

        private DialogFlow GetGotoActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            string lastActionAfterGoto = "";
            for (int i = index; i < questGen.actionsInOrder.Count; i++)
            {
                if (questGen.actionsInOrder[i].action != "goto")
                {
                    lastActionAfterGoto = questGen.actionsInOrder[i].action;
                    break;
                }
            }
            if (lastActionAfterGoto == "")
            {
                lastActionAfterGoto = "goto";
            }

            TextObject npcLine1 = new TextObject(this.getActionString(lastActionAfterGoto), null);
            TextObject textObject = new TextObject("Safe travels.", null);

            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index == questGen.currentActionIndex).BeginPlayerOptions().PlayerOption(new TextObject("Alright, I'll keep moving then.", null), null).NpcLine(textObject, null, null).Consequence(delegate
            {
                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);
                questGen.RemoveTrackedObject(settlementTarget);
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
            }).CloseDialog().EndPlayerOptions().CloseDialog();
        }

        public override void OnSettlementEnteredQuest(MobileParty party, Settlement settlement, Hero hero, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (settlement.Name == settlementTarget.Name)
            {
                if (!actioncomplete && !settlementEntered)
                {
                    settlementEntered = true;

                    if (this.nextIsGotoFlag)
                    {
                        questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);

                        foreach (Hero h in this.settlementTarget.Notables)
                        {
                            if (h != null)
                            {
                                this.heroTarget = h;
                                break;
                            }
                        }

                        TextObject textObject = new TextObject("Talk with {HERO}", null);
                        textObject.SetTextVariable("HERO", heroTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);

                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        Campaign.Current.ConversationManager.AddDialogFlow(this.GetGotoActionDialogFlow(heroTarget, index, questGiver, questBase, questGen), this);
                    }
                    else
                    {
                        questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);
                        questGen.RemoveTrackedObject(settlementTarget);
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

        }

        public override void updateHeroTargets(string targetString, Hero targetHero)
        {
        }

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
            foreach (Parameter p in this.Action.param)
            {
                if (p.target == targetString)
                {
                    p.target = targetSettlement.Name.ToString();
                    settlementTarget = targetSettlement;
                    break;
                }
            }
        }

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
        }

        public override TextObject getDescription(string strategy, int pair)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Visit dangerous place":
                    strat = new TextObject("I need you to scout {SETTLEMENT}. Report back to me if you find anything worth considering.", null);
                    strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    break;
            }
            return strat;
        }

        public override TextObject getTitle(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Visit dangerous place":
                    strat = new TextObject("Visit the area around {SETTLEMENT}.", null);
                    strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    break;
            }
            return strat;
        }

        public override string getListenString(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Visit dangerous place":
                    strat = new TextObject("{SETTLEMENT} is a {TYPE} located not far from here. It belongs to one of our allies and I've heard rumors that something dangerous has been seen nearby.", null);
                    if (settlementTarget.IsTown)
                    {
                        strat.SetTextVariable("TYPE", "town");
                    }
                    else if (settlementTarget.IsCastle)
                    {
                        strat.SetTextVariable("TYPE", "castle");
                    }
                    else
                    {
                        strat.SetTextVariable("TYPE", "village");
                    }
                    strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    break;
            }
            return strat.ToString();
        }

        public string getActionString(string action)
        {

            switch (action)
            {
                case "capture":
                    return "Whoever you are looking to capture is not in this settlement, you should continue your journey.";
                case "damage":
                    return "Whoever you are looking to attack is not in this settlement, you should continue your journey.";
                case "exchange":
                    return "Whoever you are looking to exchange products with is not in this settlement, you should continue your journey.";
                case "explore":
                    return "Your final destination is not this settlement, you should continue exploring.";
                case "free":
                    return "Whoever you are looking to free is not in this settlement, you should continue your journey.";
                case "gather":
                    return "The items you're looking for don't appear to be on this settlement, you should continue your journey.";
                case "give":
                    return "Whoever you are looking to give products to is not in this settlement, you should continue your journey.";
                case "goto":
                    return "Your final destination is not this settlement, you should continue your journey.";
                case "kill":
                    return "Whoever you are looking to kill is not in this settlement, you should continue your journey.";
                case "listen":
                    return "Whoever you are looking to receive information is not in this settlement, you should continue your journey.";
                case "report":
                    return "Whoever you are looking to transmit information is not in this settlement, you should continue your journey.";
                case "subquest":
                    return "Whoever you are looking to talk with is not in this settlement, you should continue your journey.";
                case "take":
                    return "Whoever you are looking to steal from is not in this settlement, you should continue your journey.";
                case "use":
                    return "This settlement doesn't appear to be the best place to train your skills, you should continue your journey.";
            }

            return "What you're looking for doesn't appear to be in this settlement. You should continue your journey.";
        }

        public override TextObject getStepDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            strat = new TextObject("Visit {SETTLEMENT}.", null);
            strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);

            return strat;
        }

    }
}
