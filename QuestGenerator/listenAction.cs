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
using QuestGenerator.QuestBuilder.CustomBT;

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

                this.heroTarget = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
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
                        bool questFlag = false;
                        int questindex = 0;
                        for (int j = 0; j < this.index; j++)
                        {
                            if (questGen.alternativeActionsInOrder[j].action == "quest")
                            {
                                questindex = j;
                                questFlag = true;

                            }
                            else if (questGen.alternativeActionsInOrder[j].action == "listen")
                            {
                                questFlag = false;
                            }
                        }
                        if (questFlag)
                        {
                            newHero = questGen.alternativeActionsInOrder[questindex].GetHeroTarget();
                            targetHero = newHero.Name.ToString();
                        }
                        else if (questGen.alternativeActionsInOrder[i - 1].action == "goto")
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
                        bool questFlag = false;
                        int questindex = 0;
                        for (int j = 0; j < this.index; j++)
                        {
                            if (questGen.actionsInOrder[j].action == "quest")
                            {
                                questindex = j;
                                questFlag = true;

                            }
                            else if (questGen.actionsInOrder[j].action == "listen")
                            {
                                questFlag = false;
                            }
                        }
                        if (questFlag)
                        {
                            newHero = questGen.actionsInOrder[questindex].GetHeroTarget();
                            targetHero = newHero.Name.ToString();
                        }
                        else if (questGen.actionsInOrder[i - 1].action == "goto")
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
            if (!actioncomplete)
            {
                if (this.index == 0)
                {
                    if (this.heroTarget != null)
                    {
                        this.actionInLog = true;
                        questBase.AddTrackedObject(this.heroTarget);
                        TextObject textObject = new TextObject("Listen to {HERO}", null);
                        textObject.SetTextVariable("HERO", this.heroTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);

                        Campaign.Current.ConversationManager.AddDialogFlow(this.GetListenActionDialogFlow(this.heroTarget, index, this.questGiver, questBase, questGen), this);

                    }
                }
                else
                {
                    if (questGen.actionsInOrder[this.index - 1].actioncomplete)
                    {
                        if (this.heroTarget != null)
                        {
                            this.actionInLog = true;
                            questBase.AddTrackedObject(this.heroTarget);
                            TextObject textObject = new TextObject("Listen to {HERO}", null);
                            textObject.SetTextVariable("HERO", this.heroTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);

                            Campaign.Current.ConversationManager.AddDialogFlow(this.GetListenActionDialogFlow(this.heroTarget, index, this.questGiver, questBase, questGen), this);

                        }
                    }
                }
            }
            
            
        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return GetListenActionDialogFlow(this.heroTarget, index, this.questGiver, questBase, questGen);
        }

        private DialogFlow GetListenActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            TextObject npcLine1 = new TextObject("Hello there.", null);
            TextObject playerLine1 ;
            bool questFlag = false;
            for (int i = 0; i < this.index; i++)
            {
                if (questGen.actionsInOrder[i].action == "quest")
                {
                    questFlag = true;
                    
                }
                else if (questGen.actionsInOrder[i].action == "listen")
                {
                    questFlag = false;
                }
            }

            TextObject npcLine2;
            if (this.index == 0)
            {
                npcLine2 = new TextObject(QuestHelperClass.ListenDialog(questGen.chosenMission.info, questGen.ListenReportPair), null);
                playerLine1 = new TextObject("{QUEST_GIVER.LINK} has sent me, do you have any information to share?", null);
                StringHelpers.SetCharacterProperties("QUEST_GIVER", questGiver.CharacterObject, playerLine1);
            }
            else if (questFlag) 
            {
                playerLine1 = new TextObject("Alright, I've completed your task, what kind of information can you share with me?", null);
                npcLine2 = new TextObject("So it seems. Very well, then. " + listenCalculator(questBase, questGen), null);
            } 
            else
            {
                npcLine2 = new TextObject(QuestHelperClass.ListenDialog(questGen.chosenMission.info, questGen.ListenReportPair), null);
                playerLine1 = new TextObject("{QUEST_GIVER.LINK} has sent me, do you have any information to share?", null);
                StringHelpers.SetCharacterProperties("QUEST_GIVER", questGiver.CharacterObject, playerLine1);
            }
            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index == questGen.currentActionIndex).PlayerLine(playerLine1, null).NpcLine(npcLine2, null, null).Consequence(delegate
            {
                this.listenConsequences(index, questBase, questGen);
            }).CloseDialog();
        }

        private void listenConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
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

        public override TextObject getTitle(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Interview NPC":
                    strat = new TextObject("Interview {HERO}.", null);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    break;
                case "Check on NPC":
                    strat = new TextObject("Check on {HERO}.", null);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    break;
                case "Recruit":
                    strat = new TextObject("Find recruits from {HERO}.", null);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    break;
            }
            return strat;
        }

        public string listenCalculator(QuestBase questBase, QuestGenTestQuest questGen)
        {

            string strat = "";
            strat = questGen.chosenMission.info;
            string stratObj = "";
            switch (strat)
            {
                case "Deliver item for study":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "give")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Interview NPC":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "listen")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Obtain luxuries":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "give")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Kill pests":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "damage" || a.action == "kill")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Obtain rare items":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "give")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Kill enemies":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "damage" || a.action == "kill")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Visit dangerous place":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "goto" || a.action == "explore")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Revenge, Justice":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "damage" || a.action == "kill")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Capture Criminal":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "capture")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Check on NPC":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "listen")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Recover lost/stolen item":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "give")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Rescue NPC":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "free")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Attack threatening entities":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "damage" || a.action == "kill")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Create Diversion":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "damage")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Recruit":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "listen")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Attack enemy":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "damage" || a.action == "kill")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Steal stuff":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "take")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Gather raw materials":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Steal valuables for resale":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "take")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Practice combat":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "damage")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Practice skill":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "use")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Deliver supplies":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "give")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Steal supplies":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "take")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
                case "Trade for supplies":
                    foreach (actionTarget a in questGen.actionsInOrder)
                    {
                        if (a.action == "exchange")
                        {
                            stratObj = a.getListenString(strat);
                            break;
                        }
                    }
                    break;
            }

            return stratObj;
        }
    }
}
