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
    public class gotoAction : actionTarget
    {
        [XmlIgnore]
        public Settlement settlementTarget;
        
        public gotoAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public gotoAction() { }
        
        public override Settlement GetSettlementTarget()
        {
            return this.settlementTarget;
        }

        public override void SetSettlementTarget(Settlement newS)
        {
            this.settlementTarget = newS;
        }

        public override void bringTargetsBack()
        {
            if (this.settlementTarget == null)
            {
                var setName = this.Action.param[0].target;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("goto action - line 45"));
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

        public override void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative)
        {

            if (this.Action.param[0].target.Contains("place"))
            {
                string placeNumb = this.Action.param[0].target;
                Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                {
                    return x != this.questGiver.CurrentSettlement && !x.IsHideout() && x.Notables.Count >= 1 && x.MapFaction == this.questGiver.MapFaction;
                });
                
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

        public override void QuestQ(QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!actioncomplete)
            {
                if (this.index == 0)
                {
                    this.actionInLog = true;
                    if (this.settlementTarget != null)
                    {
                        questBase.AddTrackedObject(this.settlementTarget);
                        TextObject textObject = new TextObject("Go to {SETTLEMENT}", null);
                        textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[this.index - 1].actioncomplete)
                    {
                        this.actionInLog = true;
                        if (this.settlementTarget != null)
                        {
                            questBase.AddTrackedObject(this.settlementTarget);
                            TextObject textObject = new TextObject("Go to {SETTLEMENT}", null);
                            textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        }
                    }
                }
            }
            
            
        }

        public override void OnSettlementEnteredQuest(MobileParty party, Settlement settlement, Hero hero, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (settlement.Name == this.settlementTarget.Name)
            {
                if (!actioncomplete)
                {
                    questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);
                    questGen.RemoveTrackedObject(this.settlementTarget);
                    questGen.currentActionIndex++;
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

        }

        public override void updateHeroTargets(string targetString, Hero targetHero)
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

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
        }

        public override TextObject getDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Visit dangerous place":
                    strat = new TextObject("I need you to scout {SETTLEMENT}. Report back to me if you find anything worth considering.", null);
                    strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                    break;
            }
            return strat;
        }

        public override TextObject getTitle(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Visit dangerous place":
                    strat = new TextObject("Visit {SETTLEMENT}.", null);
                    strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                    break;
            }
            return strat;
        }

        public override string getListenString(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Visit dangerous place":
                    strat = new TextObject("{SETTLEMENT} is a {TYPE} located not far from here. It belongs to one of our allies and I've heard rumors that something dangerous has been seen nearby.", null);
                    if (this.settlementTarget.IsTown)
                    {
                        strat.SetTextVariable("TYPE", "town");
                    }
                    else if (this.settlementTarget.IsCastle)
                    {
                        strat.SetTextVariable("TYPE", "castle");
                    }
                    else
                    {
                        strat.SetTextVariable("TYPE", "village");
                    }
                    strat.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                    break;
            }
            return strat.ToString();
        }

    }
}
