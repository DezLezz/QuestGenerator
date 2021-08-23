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
    public class listenAction : actionTarget
    {
        [XmlIgnore]
        public Hero heroTarget;

        public listenAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public listenAction() { }

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
                        bool questFlag = false;
                        int questindex = 0;
                        for (int j = 0; j < index; j++)
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
                        bool questFlag = false;
                        int questindex = 0;
                        for (int j = 0; j < index; j++)
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
                    InformationManager.DisplayMessage(new InformationMessage("listen action - line 154"));
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
                        TextObject textObject = new TextObject("{HERO} might have the information you need, listen to what he has to say.", null);
                        textObject.SetTextVariable("HERO", heroTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        Campaign.Current.ConversationManager.AddDialogFlow(this.GetListenActionDialogFlow(heroTarget, index, questGiver, questBase, questGen), this);

                    }
                }
                else
                {
                    if (questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index)
                    {
                        if (heroTarget != null)
                        {
                            actionInLog = true;
                            questBase.AddTrackedObject(heroTarget);
                            TextObject textObject = new TextObject("{HERO} might have the information you need, listen to what he has to say.", null);
                            textObject.SetTextVariable("HERO", heroTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                            Campaign.Current.ConversationManager.AddDialogFlow(this.GetListenActionDialogFlow(heroTarget, index, questGiver, questBase, questGen), this);

                        }
                    }
                }
            }


        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return this.GetListenActionDialogFlow(heroTarget, index, this.questGiver, questBase, questGen);
        }

        private DialogFlow GetListenActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            TextObject npcLine1 = new TextObject("Hello there.", null);
            TextObject playerLine1;
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
                playerLine1 = new TextObject(QuestHelperClass.ListenPlayerDialog(questGen.chosenMission.info, questGen.ListenReportPair), null);
                StringHelpers.SetCharacterProperties("QUEST_GIVER", questGiver.CharacterObject, playerLine1);
            }
            else if (questFlag)
            {
                playerLine1 = new TextObject("Alright, I've completed your task, what kind of information can you share with me?", null);
                npcLine2 = new TextObject("So it seems. Very well, then. " + this.listenCalculator(questBase, questGen), null);
            }
            else
            {
                npcLine2 = new TextObject(QuestHelperClass.ListenDialog(questGen.chosenMission.info, questGen.ListenReportPair), null);
                playerLine1 = new TextObject(QuestHelperClass.ListenPlayerDialog(questGen.chosenMission.info, questGen.ListenReportPair), null);
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

        public override TextObject getDescription(string strategy, int pair)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Interview NPC":
                    if (pair ==1)
                    {
                        strat = new TextObject("I've heard about someone from {SETTLEMENT} that was good at fixing wagons. It might be {HERO}, go and talk to him about coming to fix this settlements wagons.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat = new TextObject("There are rumours of a fairly skilled blacksmith in {SETTLEMENT}. Talk with {HERO} and find out if the blacksmith would be willing to forge some tools for me.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", heroTarget.CurrentSettlement.Name);
                    }
                    
                    break;
                case "Check on NPC":
                    if (pair == 1)
                    {
                        strat = new TextObject("I've heard of a few attacks near {SETTLEMENT}. Talk with {HERO} and find out how everything is.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat = new TextObject("I've sent my son to {SETTLEMENT} a couple days ago, but I haven't heard from him since. Talk with {HERO} and find out if he's ok.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", heroTarget.CurrentSettlement.Name);
                    }
                    break;
                case "Recruit":
                    if (pair == 1)
                    {
                        strat = new TextObject("This area has been a bit dangerous lately. Go talk with {HERO} and see if his settlement has some soldiers to spare.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("Our settlement yielded a plentyful harvest this year. Talk with {HERO} and see if he can spare some workers to help us out.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
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
                    strat.SetTextVariable("HERO", heroTarget.Name);
                    break;
                case "Check on NPC":
                    strat = new TextObject("Check on {HERO}.", null);
                    strat.SetTextVariable("HERO", heroTarget.Name);
                    break;
                case "Recruit":
                    strat = new TextObject("Find recruits from {HERO}.", null);
                    strat.SetTextVariable("HERO", heroTarget.Name);
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
                            for (int i = questGen.actionsInOrder.Count -1; i >= 0; i--)
                            {
                                if (questGen.actionsInOrder[i].action == "listen")
                                {
                                    stratObj = a.getListenString(strat);
                                }
                            }
                            
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
                            for (int i = questGen.actionsInOrder.Count - 1; i >= 0; i--)
                            {
                                if (questGen.actionsInOrder[i].action == "listen")
                                {
                                    stratObj = a.getListenString(strat);
                                }
                            }
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

        public override string getListenString(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Check on NPC":
                    strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                    strat.SetTextVariable("HERO", heroTarget.Name);
                    strat.SetTextVariable("SETTLEMENT", heroTarget.LastSeenPlace.Name);

                    break;
                case "Recruit":
                    Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        return x != questGiver.CurrentSettlement && !x.IsHideout() && x.Notables.Count >= 1 && x.MapFaction == questGiver.MapFaction && x.HasRecruits;
                    });

                    if (settlement != null)
                    {
                        strat = new TextObject("If you're looking for a settlement with some troops to recruit you can probably find some in {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", settlement.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately you're out of luck. I don't know of any settlements nearby that have troops for you to recruit.", null);
                    }
                    break;
                case "Interview NPC":
                    strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                    strat.SetTextVariable("HERO", heroTarget.Name);
                    strat.SetTextVariable("SETTLEMENT", heroTarget.LastSeenPlace.Name);

                    break;
            }
            return strat.ToString();
        }


    }
}
