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

                Hero[] array = (from x in Hero.AllAliveHeroes where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("capture action - line 44"));
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
                    InformationManager.DisplayMessage(new InformationMessage("capture action - line 60"));
                }
                if (array.Length == 1)
                {
                    this.questGiver = array[0];
                }
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestCampaignBehavior.QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("enemy"))
            {
                var npcNumb = this.Action.param[0].target;
                Hero newHero;
                int r = rnd.Next(1, 3);
                InformationManager.DisplayMessage(new InformationMessage(r.ToString()));
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
                        Settlement closestHideout = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsHideout());
                        Clan clan = null;
                        if (closestHideout != null)
                        {
                            CultureObject banditCulture = closestHideout.Culture;
                            clan = Clan.BanditFactions.FirstOrDefault((Clan x) => x.Culture == banditCulture);
                        }
                        if (clan == null)
                        {
                            clan = Clan.All.GetRandomElementWithPredicate((Clan x) => x.IsBanditFaction);
                        }
                        this.nonHeroTarget = closestHideout.Culture.Name.ToString();
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
            if (heroFlag)
            {
                questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                TextObject textObject = new TextObject("Capture {HERO}.", null);
                textObject.SetTextVariable("HERO", this.heroTarget.Name);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
            }
            else
            {
                TextObject textObject = new TextObject("Capture a man belonging to the " + this.nonHeroTarget +  ".", null);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
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
                    if (c.Culture.Name.ToString() == this.nonHeroTarget)
                    {
                        this.secondpart = true;
                        questGen.UpdateQuestTaskS(questGen.journalLogs[index], 1);
                        TextObject textObject = new TextObject("Give the prisoner to {HERO} in {QUEST_SETTLEMENT}.", null);
                        textObject.SetTextVariable("HERO", this.questGiver.Name);
                        textObject.SetTextVariable("QUEST_SETTLEMENT", this.questGiver.CurrentSettlement.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("getting dialog flow"));
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
                if (prisoner == this.heroTarget)
                {
                    this.secondpart = true;
                    questGen.UpdateQuestTaskS(questGen.journalLogs[index], 1);
                    TextObject textObject = new TextObject("Give the prisoner to {HERO} in {QUEST_SETTLEMENT}.", null);
                    textObject.SetTextVariable("HERO", this.questGiver.Name);
                    textObject.SetTextVariable("QUEST_SETTLEMENT", this.questGiver.CurrentSettlement.Name);
                    questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    InformationManager.DisplayMessage(new InformationMessage("getting dialog flow"));
                    Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaptureActionDialogFlow(this.questGiver, index, this.questGiver, questBase, questGen), this);
                }
            }
        }

        public override void HeroKilledEvent(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification,int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.heroTarget != null)
            {
                if (victim == this.heroTarget)
                {
                    questGen.CompleteQuestWithFail();
                }
            }
        }

        public override void HeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.heroTarget != null)
            {
                if (prisoner == this.heroTarget)
                {
                    questGen.CompleteQuestWithFail();
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
            InformationManager.DisplayMessage(new InformationMessage("return capture dialog flow"));
            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index == questGen.currentActionIndex).BeginPlayerOptions().PlayerOption(new TextObject("Yes. Here he is.", null), null).ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.ReturnClickableConditions)).NpcLine(textObject, null, null).Consequence(delegate
            {
                this.captureConsequences(index, questBase, questGen);
            }).CloseDialog().PlayerOption(new TextObject("I'm working on it.", null), null).NpcLine(textObject2, null, null).CloseDialog().EndPlayerOptions().CloseDialog();
        }

        private void captureConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!questGen.journalLogs[this.index].HasBeenCompleted())
            {
                questGen.currentActionIndex++;

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);

                if (this.heroFlag)
                {
                    GivePrisonerAction.Apply(this.heroTarget.CharacterObject, PartyBase.MainParty, (this.questGiver.PartyBelongedTo != null) ? this.questGiver.PartyBelongedTo.Party : this.questGiver.CurrentSettlement.Party);
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
    }
}
