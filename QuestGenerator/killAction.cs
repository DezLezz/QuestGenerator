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
    public class killAction : actionTarget
    {

        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        public string nonHeroTarget = "none";

        public bool heroFlag = false;

        public killAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public killAction() { }

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
                    InformationManager.DisplayMessage(new InformationMessage("kill action - line 53"));
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
                    InformationManager.DisplayMessage(new InformationMessage("kill action - line 69"));
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
                    List<MobileParty> closeParties = new List<MobileParty>();
                    if (i > 0)
                    {
                        if (alternative)
                        {
                            if (questGen.alternativeActionsInOrder[i - 1].action == "goto")
                            {
                                closeParties = mp.GetPartiesAroundPosition(questGen.alternativeActionsInOrder[i - 1].GetSettlementTarget().Position2D, 150);
                            }

                            else
                            {
                                closeParties = mp.GetPartiesAroundPosition(questBase.IssueSettlement.Position2D, 150);
                            }
                        }
                        else
                        {
                            if (questGen.actionsInOrder[i - 1].action == "goto")
                            {
                                closeParties = mp.GetPartiesAroundPosition(questGen.actionsInOrder[i - 1].GetSettlementTarget().Position2D, 150);
                            }

                            else
                            {
                                closeParties = mp.GetPartiesAroundPosition(questBase.IssueSettlement.Position2D, 150);
                            }
                        }
                    }
                    else if (i == 0)
                    {
                        closeParties = mp.GetPartiesAroundPosition(questBase.IssueSettlement.Position2D, 150);
                    }
                    
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
                    else if (i == 0)
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
                TextObject textObject = new TextObject("Kill {HERO}.", null);
                textObject.SetTextVariable("HERO", this.heroTarget.Name);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
            }
            else
            {
                TextObject textObject = new TextObject("Defeat a party of " + this.nonHeroTarget + ".", null);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
            }
        }

        public override void HeroKilledEvent(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.heroTarget != null)
            {
                if (victim == this.heroTarget)
                {
                    this.killConsequences(index, questBase, questGen);
                }
            }
        }

        public override void MapEventEnded(MapEvent mapEvent, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (mapEvent.WinningSide != BattleSideEnum.None && mapEvent.DefeatedSide != BattleSideEnum.None)
            {
                MapEventSide mapEventSide = mapEvent.GetMapEventSide(mapEvent.WinningSide);
                MapEventSide mapEventSide2 = mapEvent.GetMapEventSide(mapEvent.DefeatedSide);
                if (mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party.Culture.Name.ToString() == this.nonHeroTarget))
                {
                    this.killConsequences(index, questBase, questGen);
                }
            }
        }

        private void killConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
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
                    if (this.heroFlag)
                    {
                        FactionManager.DeclareAlliance(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                    }
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

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
        }

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
        }

        public override TextObject getDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Kill pests":
                    if (heroFlag)
                    {
                        strat = new TextObject("{HERO} has been wreaking havoc lately. I need you to get rid of them.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("{HERO} have been wreaking havoc lately. I need you to get rid of them.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
                case "Kill enemies":
                    if (heroFlag)
                    {
                        strat = new TextObject("A few of my enemies have been more active recently. I need you to teach {HERO} a lesson.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("A few of my enemies have been more active recently.I need you to teach a group of {HERO} a lesson.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
                case "Revenge, Justice":
                    if (heroFlag)
                    {
                        strat = new TextObject("{HERO} has wronged me. I need you to take care of them so that justice can be served.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("A group of {HERO} has wronged me. I need you to take care of them so that justice can be served.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
                case "Attack threatening entities":
                    if (heroFlag)
                    {
                        strat = new TextObject("{HERO} has been threatening the well-being of our settlement. I need you to deal with him.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("A group of {HERO} have been threatening the well-being of our settlement. I need you to deal with them.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
                case "Attack enemy":
                    if (heroFlag)
                    {
                        strat = new TextObject("We're planning an attack on one of our enemies. Will you join us and attack {HERO}?", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("We're planning an attack on one of our enemies. Will you join us and attack a group of {HERO}?", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }

            }
            return strat;
        }
    }
}
