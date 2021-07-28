﻿using Newtonsoft.Json;
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
    public class freeAction : actionTarget
    {
        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        public freeAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public freeAction() { }

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

        public override void IssueQ(IssueBase questBase, QuestGenTestCampaignBehavior.QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("npc"))
            {
                var npcNumb = this.Action.param[0].target;
                Hero newHero;
                List<Hero> nonEnemies = new List<Hero>();

                int i = this.index;

                foreach (Hero h in Hero.AllAliveHeroes)
                {
                    if (!questBase.IssueOwner.IsEnemy(h) && !questBase.IssueOwner.MapFaction.IsAtWarWith(h.MapFaction)) nonEnemies.Add(h);
                }

                if (nonEnemies.IsEmpty())
                {
                    InformationManager.DisplayMessage(new InformationMessage("Hero has no allies or neutrals"));
                }

                else
                {
                    if (i > 0)
                    {
                        if (alternative)
                        {
                            if (questGen.alternativeActionsInOrder[i - 1].action == "damage" || (questGen.alternativeActionsInOrder[i - 1].action == "kill" && questGen.alternativeActionsInOrder[i - 1].GetHeroTarget() != null) || questGen.alternativeActionsInOrder[i - 1].action == "goto")
                            {
                                Hero defeatTarget = questGen.alternativeActionsInOrder[i - 1].GetHeroTarget();
                                Settlement defeatPlace;
                                if (defeatTarget != null)
                                {
                                    defeatPlace = defeatTarget.CurrentSettlement;
                                }
                                else
                                {
                                    defeatPlace = questGen.alternativeActionsInOrder[i - 1].GetSettlementTarget();
                                }
                                Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(defeatPlace.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(this.questGiver.MapFaction));

                                foreach (CharacterObject c in targetSet.Town.GetPrisonerHeroes())
                                {
                                    foreach (Hero h in nonEnemies)
                                    {
                                        if (h.CharacterObject == c)
                                        {
                                            newHero = h;
                                            questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                                            break;
                                        }
                                    }
                                }

                                if (this.heroTarget == null)
                                {
                                    int r = rnd.Next(nonEnemies.Count());
                                    TakePrisonerAction.Apply(targetSet.Party, nonEnemies[r]);
                                    newHero = nonEnemies[r];
                                    questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                                }

                            }
                            else
                            {
                                Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(this.questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(this.questGiver.MapFaction));

                                foreach (CharacterObject c in targetSet.Town.GetPrisonerHeroes())
                                {
                                    foreach (Hero h in nonEnemies)
                                    {
                                        if (h.CharacterObject == c)
                                        {
                                            newHero = h;
                                            questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                                            break;
                                        }
                                    }
                                }

                                if (this.heroTarget == null)
                                {
                                    int r = rnd.Next(nonEnemies.Count());
                                    TakePrisonerAction.Apply(targetSet.Party, nonEnemies[r]);
                                    newHero = nonEnemies[r];
                                    questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                                }

                            }
                        }
                        else
                        {
                            if (questGen.actionsInOrder[i - 1].action == "damage" || (questGen.actionsInOrder[i - 1].action == "kill" && questGen.actionsInOrder[i - 1].GetHeroTarget() != null) || questGen.actionsInOrder[i - 1].action == "goto")
                            {
                                Settlement defeatPlace;
                                Hero defeatTarget = questGen.actionsInOrder[i - 1].GetHeroTarget();
                                if (defeatTarget != null)
                                {
                                    defeatPlace = defeatTarget.CurrentSettlement;
                                }
                                else
                                {
                                    defeatPlace = questGen.actionsInOrder[i - 1].GetSettlementTarget();
                                }
                                Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(defeatPlace.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(this.questGiver.MapFaction));

                                foreach (CharacterObject c in targetSet.Town.GetPrisonerHeroes())
                                {
                                    foreach (Hero h in nonEnemies)
                                    {
                                        if (h.CharacterObject == c)
                                        {
                                            newHero = h;
                                            questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                                            break;
                                        }
                                    }
                                }

                                if (this.heroTarget == null)
                                {
                                    int r = rnd.Next(nonEnemies.Count());
                                    TakePrisonerAction.Apply(targetSet.Party,nonEnemies[r]);
                                    newHero = nonEnemies[r];
                                    questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                                }

                            }
                            else
                            {
                                Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(this.questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(this.questGiver.MapFaction));

                                foreach (CharacterObject c in targetSet.Town.GetPrisonerHeroes())
                                {
                                    foreach (Hero h in nonEnemies)
                                    {
                                        if (h.CharacterObject == c)
                                        {
                                            newHero = h;
                                            questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                                            break;
                                        }
                                    }
                                }

                                if (this.heroTarget == null)
                                {
                                    int r = rnd.Next(nonEnemies.Count());
                                    TakePrisonerAction.Apply(targetSet.Party,nonEnemies[r]);
                                    newHero = nonEnemies[r];
                                    questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                                }

                            }
                        }
                    }
                    else if (i == 0)
                    {
                        Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(this.questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(this.questGiver.MapFaction));

                        foreach (CharacterObject c in targetSet.Town.GetPrisonerHeroes())
                        {
                            foreach (Hero h in nonEnemies)
                            {
                                if (h.CharacterObject == c)
                                {
                                    newHero = h;
                                    if (alternative)
                                    {
                                        questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                                    }
                                    else
                                    {
                                        questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                                    }
                                    break;
                                }
                            }
                        }

                        if (this.heroTarget == null)
                        {
                            int r = rnd.Next(nonEnemies.Count());
                            TakePrisonerAction.Apply(targetSet.Party, nonEnemies[r]);
                            newHero = nonEnemies[r];
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

            }
            else if (this.heroTarget != null)
            {
                if (!this.heroTarget.IsPrisoner)
                {
                    Settlement targetSet = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(this.heroTarget.MapFaction));
                    TakePrisonerAction.Apply(targetSet.Party, this.heroTarget);
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
                    questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                    TextObject textObject = new TextObject("Find a way to free {HERO} from {SETTLEMENT}.", null);
                    textObject.SetTextVariable("HERO", this.heroTarget.Name);
                    textObject.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                }
                else
                {
                    if (questGen.actionsInOrder[this.index - 1].actioncomplete)
                    {
                        this.actionInLog = true;
                        questBase.AddTrackedObject(this.heroTarget.PartyBelongedTo);
                        TextObject textObject = new TextObject("Find a way to free {HERO} from {SETTLEMENT}.", null);
                        textObject.SetTextVariable("HERO", this.heroTarget.Name);
                        textObject.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    }
                }
            }
            
        }

        public override void HeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

            if (prisoner == this.heroTarget)
            {
                this.freeConsequences(index, questBase, questGen);
            }
        }

        public override void PrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster prisonerRoster, Hero prisonerHero, bool isReleased, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (prisonerHero == null || prisonerHero == this.heroTarget)
            {
                this.freeConsequences(index, questBase, questGen);
            }
        }

        private void freeConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                questGen.currentActionIndex++;

                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);
                actioncomplete = true;
                questGen.chosenMission.run(CustomBTStep.questQ, questBase, questGen);
                MakePeaceAction.Apply(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
                if (questGen.currentActionIndex < questGen.actionsInOrder.Count)
                {
                    questGen.currentAction = questGen.actionsInOrder[questGen.currentActionIndex];
                }
                else
                {
                    
                    //FactionManager.DeclareAlliance(Hero.MainHero.MapFaction, this.questGiver.MapFaction);
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
                case "Rescue NPC":
                    strat = new TextObject("One of my companions, {HERO}, has been arrested. I need you to set him free.", null);
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
                case "Rescue NPC":
                    strat = new TextObject("Rescue {HERO}.", null);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    break;
            }
            return strat;
        }

        public override string getListenString(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Rescue NPC":
                    strat = new TextObject("If you're looking for {HERO}, you can probably find him imprisioned in {SETTLEMENT}.", null);
                    strat.SetTextVariable("HERO", this.heroTarget.Name);
                    strat.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    break;
            }
            return strat.ToString();
        }

    }
}