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
    public class damageAction : actionTarget
    {
        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        [XmlIgnore]
        public Settlement settlementTarget;

        public string nonHeroTarget = "none";

        public bool heroFlag = false;

        public bool settlementFlag = false;

        public damageAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public damageAction() { }
        public override void bringTargetsBack()
        {
            if (this.heroTarget == null && heroFlag)
            {
                var setName = this.Action.param[1].target;
                this.heroTarget = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
            if (this.settlementTarget == null && settlementFlag)
            {
                var setName = this.Action.param[0].target;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("damage action - line 61"));
                }
                if (array.Length == 1)
                {
                    this.settlementTarget = array[0];
                }
            }
            if (this.questGiver == null)
            {
                var setName = this.questGiverString;
                this.questGiver = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);

            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestCampaignBehavior.QuestGenTestIssue questGen, bool alternative)
        {
            int HorS = rnd.Next(1,3);
            bool nextIsCapture = false;
            if (alternative)
            {
                if (this.index < questGen.alternativeActionsInOrder.Count-1)
                {
                    if (questGen.alternativeActionsInOrder[this.index+1].action == "capture")
                    {
                        nextIsCapture = true;
                    }
                }
            }
            else
            {
                if (this.index < questGen.actionsInOrder.Count-1)
                {
                    if (questGen.actionsInOrder[this.index + 1].action == "capture")
                    {
                        nextIsCapture = true;
                    }
                }
            }
            
            Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
            {
                bool war = false;
                if (x.MapFaction != null && this.questGiver != null)
                {
                    if (x.MapFaction.IsAtWarWith(this.questGiver.MapFaction))
                    {
                        war = true;
                    }
                    
                }
                return war && x.IsVillage;
            });

            if (HorS == 1 && !nextIsCapture && settlement != null)
            {
                if (this.Action.param[0].target.Contains("place"))
                {
                    string placeNumb = this.Action.param[0].target;
                    this.settlementFlag = true;
                    if (alternative)
                    {
                        questGen.alternativeMission.updateSettlementTargets(placeNumb, settlement);
                    }
                    else
                    {
                        questGen.chosenMission.updateSettlementTargets(placeNumb, settlement);
                    }
                }
                else if (this.settlementTarget != null)
                {
                    string placeNumb = this.Action.param[0].target;
                    this.settlementFlag = true;
                    if (alternative)
                    {
                        questGen.alternativeMission.updateSettlementTargets(placeNumb, settlement);
                    }
                    else
                    {
                        questGen.chosenMission.updateSettlementTargets(placeNumb, settlement);
                    }
                }

            }
            else
            {
                if (this.Action.param[1].target == null)
                {
                    this.Action.param[1].target = "enemy123";
                }
                if (this.Action.param[1].target.Contains("enemy"))
                {
                    var npcNumb = this.Action.param[1].target;
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
                            closeParties = mp.GetPartiesAroundPosition(this.questGiver.CurrentSettlement.Position2D, 150);
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
                    this.nonHeroTarget = this.Action.param[1].target;
                }
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
                        TextObject textObject = new TextObject("Attack {HERO}'s party.", null);
                        textObject.SetTextVariable("HERO", this.heroTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    }
                    else if (settlementFlag)
                    {
                        questBase.AddTrackedObject(this.settlementTarget);
                        TextObject textObject = new TextObject("Completly raid the Village of {SETTLEMENT}.", null);
                        textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    }
                    else
                    {
                        TextObject textObject = new TextObject("Damage a party of " + this.nonHeroTarget + ".", null);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[this.index - 1].actioncomplete)
                    {
                        this.actionInLog = true;
                        if (heroFlag)
                        {
                            questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                            TextObject textObject = new TextObject("Attack {HERO}'s party.", null);
                            textObject.SetTextVariable("HERO", this.heroTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        }
                        else if (settlementFlag)
                        {
                            questBase.AddTrackedObject(this.settlementTarget);
                            TextObject textObject = new TextObject("Completly raid the Village of {SETTLEMENT}.", null);
                            textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        }
                        else
                        {
                            TextObject textObject = new TextObject("Damage a party of " + this.nonHeroTarget + ".", null);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        }
                    }
                }
            }
            
        }

        public override void RaidCompletedEvent(BattleSideEnum winnerSide, MapEvent mapEvent, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (winnerSide == BattleSideEnum.Attacker && this.settlementFlag)
            {
                if (mapEvent.GetMapEventSide(mapEvent.WinningSide).IsMainPartyAmongParties() && mapEvent.DefenderSide.LeaderParty.Settlement.Name.ToString() == this.settlementTarget.Name.ToString())
                {
                    this.damageConsequences(index, questBase, questGen);
                }
            } 
        }

        public override void HeroKilledEvent(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.heroTarget != null)
            {
                if (victim == this.heroTarget)
                {
                    this.damageConsequences(index, questBase, questGen);
                }
            }
        }

        public override void MapEventEnded(MapEvent mapEvent, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (mapEvent.WinningSide != BattleSideEnum.None && mapEvent.DefeatedSide != BattleSideEnum.None && !this.settlementFlag)
            {
                MapEventSide mapEventSide = mapEvent.GetMapEventSide(mapEvent.WinningSide);
                MapEventSide mapEventSide2 = mapEvent.GetMapEventSide(mapEvent.DefeatedSide);
                if (mapEventSide.IsMainPartyAmongParties() && mapEventSide2.Parties.Any((MapEventParty t) => t.Party.Culture.Name.ToString() == this.nonHeroTarget))
                {
                    this.damageConsequences(index, questBase, questGen);
                }
            }
        }

        private void damageConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                questGen.currentActionIndex++;

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);
                actioncomplete = true;
                questGen.chosenMission.run(CustomBTStep.questQ, questBase, questGen);
                if (this.heroFlag || this.settlementFlag)
                {
                    MakePeaceAction.Apply(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                    //FactionManager.DeclareAlliance(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
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
            foreach (Parameter p in this.Action.param)
            {
                if (p.target == targetString)
                {
                    p.target = targetSettlement.Name.ToString();
                    this.settlementTarget = targetSettlement;
                    break;
                }
            }
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
                    else if (settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} has become a nuisance lately. I need you to get rid of it.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
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
                    else if (settlementFlag)
                    {
                        strat = new TextObject("A few of my enemies have been more active recently. I need you to teach the people at {SETTLEMENT} a lesson.", null);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.Name);
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
                    else if (settlementFlag)
                    {
                        strat = new TextObject("The village of {SETTLEMENT} has wronged me. I need you to take care of them so that justice can be served.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
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
                    else if (settlementFlag)
                    {
                        strat = new TextObject("The people at {SETTLEMENT} have been threatening the well-being of our settlement. I need you to deal with them.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("A group of {HERO} have been threatening the well-being of our settlement. I need you to deal with them.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
                case "Create Diversion":
                    if (heroFlag)
                    {
                        strat = new TextObject("I'm planning a surprise attack on {FACTION} and I need you to distract {HERO} for me. Do you think you could create a diversion?", null);
                        strat.SetTextVariable("FACTION", this.heroTarget.MapFaction.Name);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("I'm planning a surprise attack on {FACTION} and I need you to distract the village of {SETTLEMENT} for me. Do you think you could create a diversion?", null);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("I'm planning a surprise attack on the {HERO} and I need you to distract a group of them. Do you think you could create a diversion?", null);
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
                    else if (settlementFlag)
                    {
                        strat = new TextObject("We're planning an attack on one of our enemies. Will you join us and attack {SETTLEMENT}?", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("We're planning an attack on one of our enemies. Will you join us and attack a group of {HERO}?", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }

                case "Practice combat":
                    if (heroFlag)
                    {
                        strat = new TextObject("I advise you to train and polish your combat skills by fighting against {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("I advise you to train and polish your combat skills by fighting against the village {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("I advise you to train and polish your combat skills by fighting against a group of {HERO}.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
            }
            return strat;
        }

        public override TextObject getTitle(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Kill pests":
                    if (heroFlag)
                    {
                        strat = new TextObject("Kill {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Destroy {Settlement}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("Kill {HERO}.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;

                    }
                case "Kill enemies":
                    if (heroFlag)
                    {
                        strat = new TextObject("Kill {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Destroy {Settlement}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("Kill {HERO}.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
                case "Revenge, Justice":
                    if (heroFlag)
                    {
                        strat = new TextObject("Take revenge on {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Take revenge on {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("Take revenge on a group of {HERO}.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
                case "Attack threatening entities":
                    if (heroFlag)
                    {
                        strat = new TextObject("Attack {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Attack {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("Attack {HERO}.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
                case "Create Diversion":
                    if (heroFlag)
                    {
                        strat = new TextObject("Create a Diversion against {HERO}.", null);
                        strat.SetTextVariable("FACTION", this.heroTarget.MapFaction.Name);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Create a Diversion against {SETTLEMENT}.", null);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("Create a Diversion against {HERO}.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
                case "Attack enemy":
                    if (heroFlag)
                    {
                        strat = new TextObject("Attack {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Attack {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("Attack {HERO}.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }

                case "Practice combat":
                    if (heroFlag)
                    {
                        strat = new TextObject("Pratice combat against {HERO}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        break;
                    }
                    else if (settlementFlag)
                    {
                        strat = new TextObject("Pratice combat against {SETTLEMENT}.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        break;
                    }
                    else
                    {
                        strat = new TextObject("Pratice combat against a group of {HERO}.", null);
                        strat.SetTextVariable("HERO", this.nonHeroTarget);
                        break;
                    }
            }
            return strat;
        }

        public override string getListenString(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Kill pests":
                    if (this.heroFlag)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }
                    break;
                case "Kill enemies":
                    if (this.heroFlag)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }
                    break;
                case "Revenge, Justice":
                    if (this.heroFlag)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }
                    break;
                case "Attack threatening entities":
                    if (this.heroFlag)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }
                    break;
                case "Create Diversion":
                    if (this.heroFlag)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }
                    break;
                case "Attack enemy":
                    if (this.heroFlag)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
                    }
                    else
                    {
                        strat = new TextObject("Unfortunately I can't pinpoint the exact location of a group of {HERO}, however, there're bound to be somewhere nearby, so keep your eyes open.", null);
                        strat.SetTextVariable("HERO", this.Action.param[0].target);
                    }
                    break;

                case "Practice combat":
                    if (this.heroFlag)
                    {
                        strat = new TextObject("If you're looking for {HERO}, you can probably find him near {SETTLEMENT}.", null);
                        strat.SetTextVariable("HERO", this.heroTarget.Name);
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    else if (this.settlementFlag)
                    {
                        strat = new TextObject("{SETTLEMENT} is a village located not far from here. It belongs to one of our enemies the {FACTION} faction.", null);
                        strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        strat.SetTextVariable("FACTION", this.settlementTarget.MapFaction.Name);
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
