using Helpers;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using ThePlotLords.QuestBuilder;
using ThePlotLords.QuestBuilder.CustomBT;
using static ThePlotLords.QuestGenTestCampaignBehavior;

namespace ThePlotLords
{
    public class reportAction : actionTarget
    {
        [XmlIgnore]
        public Hero heroTarget;

        public reportAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public reportAction() { }

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
                            foreach (Hero hero in questGiver.CurrentSettlement.Notables)
                            {
                                if (hero != questGiver)
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
                            foreach (Hero hero in questGiver.CurrentSettlement.Notables)
                            {
                                if (hero != questGiver)
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
                        if (hero != questGiver)
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
                    InformationManager.DisplayMessage(new InformationMessage("report action - line 153"));
                }


            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete && !actionInLog)
            {
                if (index == 0)
                {
                    if (heroTarget != null)
                    {
                        actionInLog = true;
                        questBase.AddTrackedObject(heroTarget);
                        TextObject textObject = new TextObject("You've completed your task. Report the events to {HERO}.", null);
                        textObject.SetTextVariable("HERO", heroTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        Campaign.Current.ConversationManager.AddDialogFlow(this.GetReportActionDialogFlow(heroTarget, index, questGiver, questBase, questGen), this);
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
                            TextObject textObject = new TextObject("You've completed your task. Report the events to {HERO}.", null);
                            textObject.SetTextVariable("HERO", heroTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                            Campaign.Current.ConversationManager.AddDialogFlow(this.GetReportActionDialogFlow(heroTarget, index, questGiver, questBase, questGen), this);
                        }
                    }
                }
            }


        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return this.GetReportActionDialogFlow(heroTarget, index, this.questGiver, questBase, questGen);
        }

        private DialogFlow GetReportActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            TextObject npcLine1 = new TextObject("Hello there, have you done the job?", null);
            TextObject playerLine1 = new TextObject("Yes. " + QuestHelperClass.ReportDialog(questGen.chosenMission.info, questGen.ListenReportPair), null);
            StringHelpers.SetCharacterProperties("QUEST_GIVER", questGiver.CharacterObject, playerLine1);
            TextObject npcLine2 = new TextObject("Thank you, you've been a great help and I won't forget this.", null);
            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index == questGen.currentActionIndex).PlayerLine(playerLine1, null).NpcLine(npcLine2, null, null).Consequence(delegate
            {
                this.reportConsequences(index, questBase, questGen);
            }).CloseDialog();
        }

        private void reportConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                questGen.currentActionIndex++;
                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);
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
        }


    }
}
