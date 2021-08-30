using System;
using System.Collections.Generic;
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
    public class captureAction : actionTarget
    {
        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        public string nonHeroTarget = "none";

        public bool heroFlag = false;

        public bool secondpart = false;

        public captureAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public captureAction() { }

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
            if (heroTarget == null && heroFlag)
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

        public override void IssueQ(IssueBase questBase, QuestGenTestCampaignBehavior.QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("enemy"))
            {
                var npcNumb = this.Action.param[0].target;
                Hero newHero;
                int r = rnd.Next(1, 3);
                List<Hero> mobileEnemies = new List<Hero>();
                var parties = MobileParty.All;

                int i = index;

                foreach (MobileParty mp in parties)
                {
                    if (mp.LeaderHero != null)
                    {
                        if (questBase.IssueOwner.IsEnemy(mp.LeaderHero) && questBase.IssueOwner.MapFaction.IsAtWarWith(mp.LeaderHero.MapFaction)) mobileEnemies.Add(mp.LeaderHero);
                    }
                }

                if (mobileEnemies.IsEmpty() || r == 2)
                {
                    heroFlag = false;
                    var mp = new MobilePartiesAroundPositionList(32);
                    List<MobileParty> closeParties = mp.GetPartiesAroundPosition(questGiver.CurrentSettlement.Position2D, 150);
                    foreach (MobileParty m in closeParties)
                    {
                        if (m.IsBandit)
                        {
                            nonHeroTarget = m.ActualClan.Culture.Name.ToString();
                            break;
                        }
                    }

                    if (nonHeroTarget == "none")
                    {
                        nonHeroTarget = "Looters";
                    }

                    if (alternative)
                    {
                        foreach (actionTarget at in questGen.alternativeActionsInOrder)
                        {
                            foreach (Parameter p in at.Action.param)
                            {
                                if (p.target == npcNumb)
                                {
                                    p.target = nonHeroTarget;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (actionTarget at in questGen.actionsInOrder)
                        {
                            foreach (Parameter p in at.Action.param)
                            {
                                if (p.target == npcNumb)
                                {
                                    p.target = nonHeroTarget;
                                }
                            }
                        }
                    }

                }

                else
                {
                    if (i > 0)
                    {
                        if (alternative)
                        {
                            if (questGen.alternativeActionsInOrder[i - 1].action == "goto")
                            {
                                heroFlag = true;
                                int e = rnd.Next(mobileEnemies.Count);
                                newHero = mobileEnemies[e];
                                questGen.alternativeActionsInOrder[i - 1].SetSettlementTarget(newHero.CurrentSettlement);
                                questGen.alternativeActionsInOrder[i - 1].Action.param[0].target = newHero.CurrentSettlement.Name.ToString();

                                questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);

                            }
                            else
                            {
                                heroFlag = true;
                                int e = rnd.Next(mobileEnemies.Count);
                                newHero = mobileEnemies[e];

                                questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);

                            }
                        }
                        else
                        {
                            if (questGen.actionsInOrder[i - 1].action == "goto")
                            {
                                heroFlag = true;
                                int e = rnd.Next(mobileEnemies.Count);
                                newHero = mobileEnemies[e];
                                questGen.actionsInOrder[i - 1].SetSettlementTarget(newHero.CurrentSettlement);
                                questGen.actionsInOrder[i - 1].Action.param[0].target = newHero.CurrentSettlement.Name.ToString();

                                questGen.chosenMission.updateHeroTargets(npcNumb, newHero);

                            }
                            else
                            {
                                heroFlag = true;
                                int e = rnd.Next(mobileEnemies.Count);
                                newHero = mobileEnemies[e];

                                questGen.chosenMission.updateHeroTargets(npcNumb, newHero);

                            }
                        }
                    }
                    else if (i == 0)
                    {
                        heroFlag = true;
                        int e = rnd.Next(mobileEnemies.Count);
                        newHero = mobileEnemies[e];
                        if (alternative)
                        {
                            questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                        }
                        else
                        {
                            questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                        }
                    }

                }

            }
            else if (heroTarget != null)
            {
                heroFlag = true;
            }
            else
            {
                nonHeroTarget = this.Action.param[0].target;
            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestCampaignBehavior.QuestGenTestQuest questGen)
        {
            if (!actioncomplete && !actionInLog)
            {
                if (index == 0)
                {
                    actionInLog = true;
                    if (heroFlag)
                    {
                        questBase.AddTrackedObject(heroTarget.PartyBelongedTo);
                        TextObject textObject = new TextObject("You've been requested to capture {HERO}.", null);
                        textObject.SetTextVariable("HERO", heroTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                    }
                    else
                    {
                        TextObject textObject = new TextObject("You've been requested to capture a man belonging to the " + nonHeroTarget + ".", null);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                    }
                }
                else
                {
                    if ((questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index) || ((questGen.actionsInOrder[index - 1].action == "damage" || questGen.actionsInOrder[index - 1].action == "kill") && questGen.actionsInOrder[index - 1].actionInLog))
                    {
                        actionInLog = true;
                        if (heroFlag)
                        {
                            questBase.AddTrackedObject(heroTarget.PartyBelongedTo);
                            TextObject textObject = new TextObject("You've been requested to capture {HERO}.", null);
                            textObject.SetTextVariable("HERO", heroTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        }
                        else
                        {
                            TextObject textObject = new TextObject("You've been requested to capture a man belonging to the a man belonging to the " + nonHeroTarget + ".", null);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        }
                    }
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

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
        }

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
        }

        public override void OnPrisonerTakenEvent(FlattenedTroopRoster rooster, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (nonHeroTarget != null)
            {
                foreach (CharacterObject c in rooster.Troops)
                {
                    if (c.Culture.Name.ToString() == nonHeroTarget && !secondpart)
                    {
                        secondpart = true;
                        questGen.UpdateQuestTaskS(questGen.journalLogs[index], 1);
                        TextObject textObject = new TextObject("Deliver the prisoner to {HERO} in {QUEST_SETTLEMENT}.", null);
                        textObject.SetTextVariable("HERO", questGiver.Name);
                        textObject.SetTextVariable("QUEST_SETTLEMENT", questGiver.CurrentSettlement.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaptureActionDialogFlow(questGiver, index, questGiver, questBase, questGen), this);
                        break;
                    }
                }
            }
        }

        public override void HeroPrisonerTaken(PartyBase capturer, Hero prisoner, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (heroTarget != null)
            {
                if (prisoner == heroTarget && !secondpart && capturer == PartyBase.MainParty)
                {
                    secondpart = true;
                    questGen.UpdateQuestTaskS(questGen.journalLogs[index], 1);
                    TextObject textObject = new TextObject("Deliver the prisoner to {HERO} in {QUEST_SETTLEMENT}.", null);
                    textObject.SetTextVariable("HERO", questGiver.Name);
                    textObject.SetTextVariable("QUEST_SETTLEMENT", questGiver.CurrentSettlement.Name);
                    questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaptureActionDialogFlow(questGiver, index, questGiver, questBase, questGen), this);
                }
            }
        }

        public override void HeroKilledEvent(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (heroTarget != null)
            {
                if (victim == heroTarget && !secondpart)
                {
                    questGen.FailConsequences();
                }
            }
        }

        public override void HeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (heroTarget != null)
            {
                if (prisoner == heroTarget && !secondpart)
                {
                    questGen.FailConsequences();
                }
            }
        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (secondpart) return this.GetCaptureActionDialogFlow(this.questGiver, index, this.questGiver, questBase, questGen);

            else return null;
        }

        private DialogFlow GetCaptureActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            TextObject npcLine1 = new TextObject("Have you brought the prisoner?", null);
            TextObject textObject = new TextObject("Thank you, {?PLAYER.GENDER}milady{?}sir{\\?}.", null);
            TextObject textObject2 = new TextObject("We await your success, {?PLAYER.GENDER}milady{?}sir{\\?}.", null);
            textObject.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);
            textObject2.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject);
            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index <= questGen.currentActionIndex).BeginPlayerOptions().PlayerOption(new TextObject("Yes. Here he is.", null), null).ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.ReturnClickableConditions)).NpcLine(textObject, null, null).Consequence(delegate
            {
                this.captureConsequences(index, questBase, questGen);
            }).CloseDialog().PlayerOption(new TextObject("I'm working on it.", null), null).NpcLine(textObject2, null, null).CloseDialog().EndPlayerOptions().CloseDialog();
        }

        private void captureConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                if (index > 0)
                {
                    if ((questGen.actionsInOrder[index - 1].action == "damage" || questGen.actionsInOrder[index - 1].action == "kill"))
                    {
                        if (questGen.actionsInOrder[index - 1].actioncomplete)
                        {
                            questGen.currentActionIndex++;
                        }
                    }
                }
                
                else
                {
                    questGen.currentActionIndex++;
                }

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);
                actioncomplete = true;
                questGen.chosenMission.run(CustomBTStep.questQ, questBase, questGen);
                if (heroFlag)
                {
                    GivePrisonerAction.Apply(heroTarget.CharacterObject, PartyBase.MainParty, (questGiver.PartyBelongedTo != null) ? questGiver.PartyBelongedTo.Party : questGiver.CurrentSettlement.Party);
                    MakePeaceAction.Apply(Hero.MainHero.MapFaction, questGiver.MapFaction);
                    //FactionManager.DeclareAlliance(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                }
                else
                {
                    foreach (CharacterObject c in MobileParty.MainParty.PrisonRoster.ToFlattenedRoster().Troops)
                    {
                        if (c.Culture.Name.ToString() == nonHeroTarget)
                        {
                            MobileParty.MainParty.PrisonRoster.RemoveTroop(c);
                            break;
                        }
                    }
                }

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

        public bool ReturnClickableConditions(out TextObject explanation)
        {
            if (heroFlag)
            {
                if (MobileParty.MainParty.PrisonRoster.Contains(heroTarget.CharacterObject))
                {
                    explanation = TextObject.Empty;
                    return true;
                }
            }
            else
            {
                foreach (CharacterObject c in MobileParty.MainParty.PrisonRoster.ToFlattenedRoster().Troops)
                {
                    if (c.Culture.Name.ToString() == nonHeroTarget)
                    {
                        explanation = TextObject.Empty;
                        return true;
                    }
                }
            }

            explanation = new TextObject("You don't have the required prisoner.", null);
            return false;
        }
        public override TextObject getDescription(string strategy, int pair)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Capture Criminal":
                    if (heroFlag)
                    {
                        strat = new TextObject("I need you to capture {HERO}. I need help bringing him to justice.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("I need you to capture someone from the {HERO}. I need help bringing them to justice.", null);
                        strat.SetTextVariable("HERO", nonHeroTarget);
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
                case "Capture Criminal":
                    if (heroFlag)
                    {
                        strat = new TextObject("Capture {HERO}.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("Capture capture someone from the {HERO}", null);
                        strat.SetTextVariable("HERO", nonHeroTarget);
                    }

                    break;
            }
            return strat;
        }

        public override string getListenString(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Capture Criminal":
                    if (heroFlag)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", heroTarget.LastSeenPlace.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }

                    break;
            }
            return strat.ToString();
        }

        public override TextObject getStepDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Capture Criminal":
                    if (heroFlag)
                    {
                        strat = new TextObject("Capture {HERO}. I need help bringing him to justice.", null);
                        strat.SetTextVariable("HERO", heroTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("Capture someone from the {HERO}.", null);
                        strat.SetTextVariable("HERO", nonHeroTarget);
                    }

                    break;
            }
            return strat;
        }


    }
}
