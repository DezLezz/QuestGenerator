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
    public class listenAction : actionTarget
    {
        [XmlIgnore]
        public Hero heroTarget;

        public listenAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public listenAction() { }

        public override Hero GetHeroTarget()
        {
            return this.heroTarget;
        }

        public override void SetHeroTarget(Hero newH)
        {
            this.heroTarget = newH;
        }

        public override void bringTargetsBack()
        {
            if (this.heroTarget == null)
            {
                var setName = this.Action.param[0].target;

                Hero[] array = (from x in Hero.AllAliveHeroes where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("listen action - line 45"));
                }
                if (array.Length == 1)
                {
                    this.heroTarget = array[0];
                }
            }

            if (this.questGiver == null)
            {
                var setName = this.questGiverString;

                Hero[] array = (from x in Hero.AllAliveHeroes where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("listen action - line 61"));
                }
                if (array.Length == 1)
                {
                    this.questGiver = array[0];
                }
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
                    foreach (Hero hero in this.questGiver.CurrentSettlement.Notables)
                    {
                        if (hero != this.questGiver)
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
                    InformationManager.DisplayMessage(new InformationMessage("listen action - line 154"));
                }


            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (this.heroTarget != null)
            {

                questBase.AddTrackedObject(this.heroTarget);
                TextObject textObject = new TextObject("Listen to {HERO}", null);
                textObject.SetTextVariable("HERO", this.heroTarget.Name);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);

                Campaign.Current.ConversationManager.AddDialogFlow(this.GetListenActionDialogFlow(this.heroTarget, index, this.questGiver, questBase, questGen), this);

            }
        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return GetListenActionDialogFlow(this.heroTarget, index, this.questGiver, questBase, questGen);
        }

        private DialogFlow GetListenActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            TextObject npcLine1 = new TextObject("Hello there.", null);
            TextObject playerLine1 = new TextObject("{QUEST_GIVER.LINK} has sent me, do you have any information to share?", null);
            StringHelpers.SetCharacterProperties("QUEST_GIVER", questGiver.CharacterObject, playerLine1);
            TextObject npcLine2 = new TextObject(QuestHelperClass.ListenDialog(questGen.chosenMission.info, questGen.ListenReportPair), null);
            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index == questGen.currentActionIndex).PlayerLine(playerLine1, null).NpcLine(npcLine2, null, null).Consequence(delegate
            {
                this.listenConsequences(index, questBase, questGen);
            }).CloseDialog();
        }

        private void listenConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
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

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
        }

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
        }

        public override TextObject getDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Interview NPC":
                    strat = new TextObject("There's a person with information I require. Go talk to {HERO} and report back if the information is usefull.", null);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    break;
                case "Check on NPC":
                    strat = new TextObject("I need you to check up on {HERO} and report back if there is any problem.", null);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    break;
                case "Recruit":
                    strat = new TextObject("Talk with {HERO} and find out if there are any soldiers worth recruiting nearby. Report back with your findings.", null);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    break;
            }
            return strat;
        }
    }
}
