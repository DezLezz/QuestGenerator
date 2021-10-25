using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class freeAction : actionTarget
    {
        static Random rnd = new Random();

        [XmlIgnore]
        public Hero heroTarget;

        public freeAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public freeAction() { }

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

        public override void IssueQ(IssueBase questBase, QuestGenTestCampaignBehavior.QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("npc"))
            {
                var npcNumb = this.Action.param[0].target;
                Hero newHero;
                List<Hero> nonEnemies = new List<Hero>();

                int i = index;

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

                                if (defeatPlace == null)
                                {
                                    defeatPlace = questGiver.CurrentSettlement;
                                }

                                Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(defeatPlace.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(questGiver.MapFaction));

                                if (targetSet != null)
                                {
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
                                }
                                else
                                {
                                    targetSet = SettlementHelper.FindNearestSettlementToPoint(questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction != questGiver.MapFaction);
                                }

                                if (targetSet == null)
                                {
                                    targetSet = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
                                }
                                if (heroTarget == null)
                                {
                                    int r = rnd.Next(nonEnemies.Count());
                                    TakePrisonerAction.Apply(targetSet.Party, nonEnemies[r]);
                                    newHero = nonEnemies[r];
                                    questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                                }

                            }
                            else
                            {
                                Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(questGiver.MapFaction));

                                if (targetSet != null)
                                {
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
                                }
                                else
                                {
                                    targetSet = SettlementHelper.FindNearestSettlementToPoint(questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction != questGiver.MapFaction);
                                }

                                if (targetSet == null)
                                {
                                    targetSet = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
                                }
                                if (heroTarget == null)
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
                                if (defeatPlace == null)
                                {
                                    defeatPlace = questGiver.CurrentSettlement;
                                }
                                Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(defeatPlace.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(questGiver.MapFaction));

                                if (targetSet != null)
                                {
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
                                }
                                else
                                {
                                    targetSet = SettlementHelper.FindNearestSettlementToPoint(questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction != questGiver.MapFaction);
                                }

                                if (targetSet == null)
                                {
                                    targetSet = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
                                }
                                if (heroTarget == null)
                                {
                                    int r = rnd.Next(nonEnemies.Count());
                                    TakePrisonerAction.Apply(targetSet.Party, nonEnemies[r]);
                                    newHero = nonEnemies[r];
                                    questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                                }

                            }
                            else
                            {
                                Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(questGiver.MapFaction));

                                if (targetSet != null)
                                {
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
                                }
                                else
                                {
                                    targetSet = SettlementHelper.FindNearestSettlementToPoint(questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction != questGiver.MapFaction);
                                }

                                if (targetSet == null)
                                {
                                    targetSet = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
                                }

                                if (heroTarget == null)
                                {
                                    int r = rnd.Next(nonEnemies.Count());
                                    TakePrisonerAction.Apply(targetSet.Party, nonEnemies[r]);
                                    newHero = nonEnemies[r];
                                    questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                                }

                            }
                        }
                    }
                    else if (i == 0)
                    {
                        Settlement targetSet = SettlementHelper.FindNearestSettlementToPoint(questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(questGiver.MapFaction));

                        if (targetSet != null)
                        {
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
                        }
                        else
                        {
                            targetSet = SettlementHelper.FindNearestSettlementToPoint(questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction != questGiver.MapFaction);
                        }

                        if (targetSet == null)
                        {
                            targetSet = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
                        }


                        if (heroTarget == null)
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
            else if (heroTarget != null)
            {
                if (!heroTarget.IsPrisoner)
                {
                    Settlement targetSet = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsTown && x.MapFaction.IsAtWarWith(heroTarget.MapFaction));
                    if (targetSet == null)
                    {
                        targetSet = SettlementHelper.FindNearestSettlementToPoint(questGiver.CurrentSettlement.Position2D, (Settlement x) => x.IsTown && x.MapFaction != questGiver.MapFaction);
                    }

                    if (targetSet == null)
                    {
                        targetSet = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
                    }
                    TakePrisonerAction.Apply(targetSet.Party, heroTarget);
                }
            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestCampaignBehavior.QuestGenTestQuest questGen)
        {
            if (!actioncomplete && !actionInLog)
            {
                if (index == 0)
                {
                    actionInLog = true;
                    questBase.AddTrackedObject(heroTarget.PartyBelongedTo);
                    TextObject textObject = new TextObject("Find a way to liberate {HERO} from {SETTLEMENT}.", null);
                    textObject.SetTextVariable("HERO", heroTarget.Name);
                    textObject.SetTextVariable("SETTLEMENT", heroTarget.CurrentSettlement.Name);
                    questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                }
                else
                {
                    if (questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index)
                    {
                        actionInLog = true;
                        questBase.AddTrackedObject(heroTarget.PartyBelongedTo);
                        TextObject textObject = new TextObject("Find a way to liberate {HERO} from {SETTLEMENT}.", null);
                        textObject.SetTextVariable("HERO", heroTarget.Name);
                        textObject.SetTextVariable("SETTLEMENT", heroTarget.CurrentSettlement.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                    }
                }
            }

        }

        public override void HeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

            if (prisoner == heroTarget)
            {
                this.freeConsequences(index, questBase, questGen);
            }
        }

        public override void PrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster prisonerRoster, Hero prisonerHero, bool isReleased, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (prisonerHero == null || prisonerHero == heroTarget)
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
                MakePeaceAction.Apply(Hero.MainHero.MapFaction, questGiver.MapFaction);
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

        public override TextObject getDescription(string strategy, int pair)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Rescue NPC":
                    strat = new TextObject("One of my companions, {HERO}, has been arrested. I need you to set him free.", null);
                    strat.SetTextVariable("HERO", heroTarget.Name);
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
                    if (heroTarget != null)
                    {
                        if (heroTarget.Name != null)
                        {
                            strat = new TextObject("Rescue {HERO}.", null);
                            strat.SetTextVariable("HERO", heroTarget.Name);
                        }
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
                case "Rescue NPC":
                    strat = new TextObject("If you're looking for {HERO}, you can probably find him imprisioned in {SETTLEMENT}.", null);
                    strat.SetTextVariable("HERO", heroTarget.Name);
                    strat.SetTextVariable("SETTLEMENT", heroTarget.CurrentSettlement.Name);
                    break;
            }
            return strat.ToString();
        }

        public override TextObject getStepDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Rescue NPC":
                    strat = new TextObject("Free {HERO} from the prison in {SETTLEMENT}.", null);
                    strat.SetTextVariable("HERO", heroTarget.Name);
                    if (this.heroTarget.CurrentSettlement != null)
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.CurrentSettlement.Name);
                    }
                    else
                    {
                        strat.SetTextVariable("SETTLEMENT", this.heroTarget.LastSeenPlace.Name);
                    }
                    break;
            }

            return strat;
        }

    }
}
