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
using Helpers;
using QuestGenerator.QuestBuilder.CustomBT;

namespace QuestGenerator
{
    public class captureAction : actionTarget
    {
        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        public string nonHeroTarget = "none";

        public bool heroFlag = false;

        public bool secondpart = false;

        public captureAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public captureAction() { }

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
            if (this.heroTarget == null && heroFlag)
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

        public override void IssueQ(IssueBase questBase, QuestGenTestCampaignBehavior.QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("enemy"))
            {
                var npcNumb = this.Action.param[0].target;
                Hero newHero;
                int r = rnd.Next(1, 3);
                List<Hero> mobileEnemies = new List<Hero>();
                var parties = MobileParty.All;

                int i = this.index;

                foreach (MobileParty mp in parties)
                {
                    if (mp.LeaderHero != null)
                    {
                        if (questBase.IssueOwner.IsEnemy(mp.LeaderHero) && questBase.IssueOwner.MapFaction.IsAtWarWith(mp.LeaderHero.MapFaction)) mobileEnemies.Add(mp.LeaderHero);
                    }
                }

                if (mobileEnemies.IsEmpty() || r == 2)
                {
                    this.heroFlag = false;
                    var mp = new MobilePartiesAroundPositionList(32);
                    List<MobileParty> closeParties = mp.GetPartiesAroundPosition(questBase.IssueSettlement.Position2D, 150);
                    foreach (MobileParty m in closeParties)
                    {
                        if (m.IsBandit)
                        {
                            this.nonHeroTarget = m.ActualClan.Culture.Name.ToString();
                            break;
                        }
                    }

                    if (this.nonHeroTarget == "none")
                    {
                        this.nonHeroTarget = "Looters";
                    }

                    if (alternative)
                    {
                        foreach (actionTarget at in questGen.alternativeActionsInOrder)
                        {
                            foreach (Parameter p in at.Action.param)
                            {
                                if (p.target == npcNumb)
                                {
                                    p.target = this.nonHeroTarget;
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
                                    p.target = this.nonHeroTarget;
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
                                this.heroFlag = true;
                                int e = rnd.Next(mobileEnemies.Count);
                                newHero = mobileEnemies[e];
                                questGen.alternativeActionsInOrder[i - 1].SetSettlementTarget(newHero.CurrentSettlement);
                                questGen.alternativeActionsInOrder[i - 1].Action.param[0].target = newHero.CurrentSettlement.Name.ToString();
                                
                                questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                                
                            }
                            else
                            {
                                this.heroFlag = true;
                                int e = rnd.Next(mobileEnemies.Count);
                                newHero = mobileEnemies[e];
                                
                                questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                                
                            }
                        }
                        else
                        {
                            if (questGen.actionsInOrder[i - 1].action == "goto")
                            {
                                this.heroFlag = true;
                                int e = rnd.Next(mobileEnemies.Count);
                                newHero = mobileEnemies[e];
                                questGen.actionsInOrder[i - 1].SetSettlementTarget(newHero.CurrentSettlement);
                                questGen.actionsInOrder[i - 1].Action.param[0].target = newHero.CurrentSettlement.Name.ToString();
                                
                                questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                                
                            }
                            else
                            {
                                this.heroFlag = true;
                                int e = rnd.Next(mobileEnemies.Count);
                                newHero = mobileEnemies[e];
                                
                                questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                                
                            }
                        }
                    }
                    else if (i==0)
                    {
                        this.heroFlag = true;
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
            else if (this.heroTarget != null)
            {
                this.heroFlag = true;
            }
            else
            {
                this.nonHeroTarget = this.Action.param[0].target;
            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestCampaignBehavior.QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                if (this.index == 0)
                {
                    this.actionInLog = true;
                    if (heroFlag)
                    {
                        questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                        TextObject textObject = new TextObject("Capture {HERO}.", null);
                        textObject.SetTextVariable("HERO", this.heroTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    }
                    else
                    {
                        TextObject textObject = new TextObject("Capture a man belonging to the " + this.nonHeroTarget + ".", null);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[this.index - 1].actioncomplete || ((questGen.actionsInOrder[this.index - 1].action == "damage" || questGen.actionsInOrder[this.index - 1].action == "kill") && questGen.actionsInOrder[this.index - 1].actionInLog))
                    {
                        this.actionInLog = true;
                        if (heroFlag)
                        {
                            questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                            TextObject textObject = new TextObject("Capture {HERO}.", null);
                            textObject.SetTextVariable("HERO", this.heroTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        }
                        else
                        {
                            TextObject textObject = new TextObject("Capture a man belonging to the " + this.nonHeroTarget + ".", null);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
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
                    this.heroTarget = targetHero;
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
            if (this.nonHeroTarget != null)
            {
                foreach (CharacterObject c in rooster.Troops)
                {
                    if (c.Culture.Name.ToString() == this.nonHeroTarget && !secondpart)
                    {
                        this.secondpart = true;
                        questGen.UpdateQuestTaskS(questGen.journalLogs[index], 1);
                        TextObject textObject = new TextObject("Give the prisoner to {HERO} in {QUEST_SETTLEMENT}.", null);
                        textObject.SetTextVariable("HERO", this.questGiver.Name);
                        textObject.SetTextVariable("QUEST_SETTLEMENT", this.questGiver.CurrentSettlement.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaptureActionDialogFlow(this.questGiver, index, this.questGiver, questBase, questGen), this);
                        break;
                    }
                }
            }
        }

        public override void HeroPrisonerTaken(PartyBase capturer, Hero prisoner, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.heroTarget != null)
            {
                if (prisoner == this.heroTarget && !secondpart && capturer == PartyBase.MainParty)
                {
                    this.secondpart = true;
                    questGen.UpdateQuestTaskS(questGen.journalLogs[index], 1);
                    TextObject textObject = new TextObject("Give the prisoner to {HERO} in {QUEST_SETTLEMENT}.", null);
                    textObject.SetTextVariable("HERO", this.questGiver.Name);
                    textObject.SetTextVariable("QUEST_SETTLEMENT", this.questGiver.CurrentSettlement.Name);
                    questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaptureActionDialogFlow(this.questGiver, index, this.questGiver, questBase, questGen), this);
                }
            }
        }

        public override void HeroKilledEvent(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification,int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.heroTarget != null)
            {
                if (victim == this.heroTarget && !secondpart)
                {
                    questGen.FailConsequences();
                }
            }
        }

        public override void HeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.heroTarget != null)
            {
                if (prisoner == this.heroTarget && !secondpart)
                {
                    questGen.FailConsequences();
                }
            }
        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (this.secondpart) return GetCaptureActionDialogFlow(this.questGiver, index, this.questGiver, questBase, questGen);

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
                questGen.currentActionIndex++;

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);
                actioncomplete = true;
                questGen.chosenMission.run(CustomBTStep.questQ, questBase, questGen);
                if (this.heroFlag)
                {
                    GivePrisonerAction.Apply(this.heroTarget.CharacterObject, PartyBase.MainParty, (this.questGiver.PartyBelongedTo != null) ? this.questGiver.PartyBelongedTo.Party : this.questGiver.CurrentSettlement.Party);
                    MakePeaceAction.Apply(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                    //FactionManager.DeclareAlliance(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                }
                else
                {
                    foreach (CharacterObject c in MobileParty.MainParty.PrisonRoster.ToFlattenedRoster().Troops)
                    {
                        if (c.Culture.Name.ToString() == this.nonHeroTarget)
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
                if (MobileParty.MainParty.PrisonRoster.Contains(this.heroTarget.CharacterObject))
                {
                    explanation = TextObject.Empty;
                    return true;
                }
            }
            else
            {
                foreach (CharacterObject c in MobileParty.MainParty.PrisonRoster.ToFlattenedRoster().Troops)
                {
                    if (c.Culture.Name.ToString() == this.nonHeroTarget)
                    {
                        explanation = TextObject.Empty;
                        return true;
                    }
                }
            }

            explanation = new TextObject("You don't have the required prisoner.", null);
            return false;
        }
        public override TextObject getDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Capture Criminal":
                    if (heroFlag)
                    {
                        strat = new TextObject("I need you to capture {HERO}. I need help bringing him to justice.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("I need you to capture someone from {HERO}. I need help bringing them to justice.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
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
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                    }
                    else
                    {
                        strat = new TextObject("Capture a {HERO}", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
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
                    if (this.heroFlag)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
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

    }
}
