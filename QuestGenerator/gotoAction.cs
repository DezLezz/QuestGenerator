using Helpers;
using System.Linq;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using ThePlotLords.QuestBuilder;
using ThePlotLords.QuestBuilder.CustomBT;
using static ThePlotLords.QuestGenTestCampaignBehavior;

namespace ThePlotLords
{
    public class gotoAction : actionTarget
    {
        [XmlIgnore]
        public Settlement settlementTarget;

        public gotoAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public gotoAction() { }

        public override Settlement GetSettlementTarget()
        {
            return settlementTarget;
        }

        public override void SetSettlementTarget(Settlement newS)
        {
            settlementTarget = newS;
        }

        public override void bringTargetsBack()
        {
            if (settlementTarget == null)
            {
                var setName = this.Action.param[0].target;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("goto action - line 45"));
                }
                if (array.Length == 1)
                {
                    settlementTarget = array[0];
                }
            }
            if (questGiver == null)
            {
                var setName = questGiverString;

                questGiver = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative)
        {

            if (this.Action.param[0].target.Contains("place"))
            {
                string placeNumb = this.Action.param[0].target;
                Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                {
                    return x != questGiver.CurrentSettlement && !x.IsHideout() && x.Notables.Count >= 1 && x.MapFaction == questGiver.MapFaction;
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
            if (!actioncomplete && !actionInLog)
            {
                if (index == 0)
                {
                    actionInLog = true;
                    if (settlementTarget != null)
                    {
                        questBase.AddTrackedObject(settlementTarget);
                        TextObject textObject = new TextObject("Visit the settlement of {SETTLEMENT}", null);
                        textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);

                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index)
                    {
                        actionInLog = true;
                        if (settlementTarget != null)
                        {
                            questBase.AddTrackedObject(settlementTarget);
                            TextObject textObject = new TextObject("Visit the settlement of {SETTLEMENT}", null);
                            textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);

                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        }
                    }
                }
            }


        }

        public override void OnSettlementEnteredQuest(MobileParty party, Settlement settlement, Hero hero, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (settlement.Name == settlementTarget.Name)
            {
                if (!actioncomplete)
                {
                    questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);
                    questGen.RemoveTrackedObject(settlementTarget);
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
                    settlementTarget = targetSettlement;
                    break;
                }
            }
        }

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
        }

        public override TextObject getDescription(string strategy, int pair)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Visit dangerous place":
                    strat = new TextObject("I need you to scout {SETTLEMENT}. Report back to me if you find anything worth considering.", null);
                    strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
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
                    strat = new TextObject("Visit the area around {SETTLEMENT}.", null);
                    strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
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
                    if (settlementTarget.IsTown)
                    {
                        strat.SetTextVariable("TYPE", "town");
                    }
                    else if (settlementTarget.IsCastle)
                    {
                        strat.SetTextVariable("TYPE", "castle");
                    }
                    else
                    {
                        strat.SetTextVariable("TYPE", "village");
                    }
                    strat.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                    break;
            }
            return strat.ToString();
        }

    }
}
