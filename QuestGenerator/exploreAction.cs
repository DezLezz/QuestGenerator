using Helpers;
using System.Collections.Generic;
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
    public class exploreAction : actionTarget
    {
        [XmlIgnore]
        public Settlement settlementTarget;

        [XmlIgnore]
        public Dictionary<Settlement, string> settlementsToVisit;

        public List<string> settlementsToVisitNames;

        public List<string> settlementsToVisitTags;

        public int NumberOfSettlementsToVisit;

        public int settlementsVisited;

        public exploreAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }

        public exploreAction() { }

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
                settlementsToVisit = new Dictionary<Settlement, string>();
                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("explore action - line 58"));
                }
                if (array.Length == 1)
                {
                    settlementTarget = array[0];
                }

                for (int i = 0; i < settlementsToVisitNames.Count; i++)
                {
                    string settlementName = settlementsToVisitNames[i];

                    Settlement[] array1 = (from x in Settlement.All where (x.Name.ToString() == settlementName) select x).ToArray<Settlement>();

                    if (array1.Length > 1 || array1.Length == 0)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("explore action - line 72"));
                    }
                    if (array1.Length == 1)
                    {
                        settlementsToVisit.Add(array1[0], settlementsToVisitTags[i]);
                    }
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
            settlementsToVisit = new Dictionary<Settlement, string>();
            settlementsToVisitNames = new List<string>();
            settlementsToVisitTags = new List<string>();

            if (this.Action.param[0].target.Contains("place"))
            {
                NumberOfSettlementsToVisit = 3;
                settlementsVisited = 0;
                string placeNumb = this.Action.param[0].target;
                Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                {
                    return x != questGiver.CurrentSettlement && !x.IsHideout && x.Notables.Count >= 1 && x.MapFaction == questGiver.MapFaction;
                });

                if (alternative)
                {
                    questGen.alternativeMission.updateSettlementTargets(placeNumb, settlement);
                }
                else
                {
                    questGen.chosenMission.updateSettlementTargets(placeNumb, settlement);
                }

                settlementsToVisitNames.Add(settlementTarget.Name.ToString());
                settlementsToVisitTags.Add("no");
                settlementsToVisit.Add(settlementTarget, "no");

                for (int i = 0; i < 2; i++)
                {
                    Settlement newSettlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        return x.OwnerClan == settlementTarget.OwnerClan && !(settlementsToVisit.Keys.Contains(x));
                    });

                    if (newSettlement != null)
                    {
                        settlementsToVisitNames.Add(newSettlement.Name.ToString());
                        settlementsToVisitTags.Add("no");
                        settlementsToVisit.Add(newSettlement, "no");
                    }
                    else
                    {
                        i--;
                    }
                    
                }
            }
            else if (settlementsToVisit.IsEmpty())
            {
                if (settlementTarget == null)
                {
                    var setName = this.Action.param[0].target;
                    Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                    if (array.Length > 1 || array.Length == 0)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("explore action - line 58"));
                    }
                    if (array.Length == 1)
                    {
                        settlementTarget = array[0];
                    }
                }
                settlementsToVisitNames.Add(settlementTarget.Name.ToString());
                settlementsToVisitTags.Add("no");
                settlementsToVisit.Add(settlementTarget, "no");

                for (int i = 0; i < 2; i++)
                {
                    Settlement newSettlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        return x.OwnerClan == settlementTarget.OwnerClan && !settlementsToVisit.ContainsKey(x);
                    });
                    if (newSettlement != null)
                    {
                        settlementsToVisitNames.Add(newSettlement.Name.ToString());
                        settlementsToVisitTags.Add("no");
                        settlementsToVisit.Add(newSettlement, "no");
                    }
                    else
                    {
                        i--;
                    }

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
                    if (settlementTarget != null && !(settlementsToVisit.IsEmpty()))
                    {
                        List<Settlement> tempList = new List<Settlement>();
                        foreach (Settlement s in settlementsToVisit.Keys)
                        {
                            questBase.AddTrackedObject(s);
                            tempList.Add(s);
                        }

                        TextObject textObject = new TextObject("Explore the area and visit {SETTLEMENT1}, {SETTLEMENT2}, {SETTLEMENT3}.", null);
                        textObject.SetTextVariable("SETTLEMENT1", tempList[0].Name);
                        textObject.SetTextVariable("SETTLEMENT2", tempList[1].Name);
                        textObject.SetTextVariable("SETTLEMENT3", tempList[2].Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, settlementsVisited, NumberOfSettlementsToVisit, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index)
                    {
                        actionInLog = true;
                        if (settlementTarget != null && !(settlementsToVisit.IsEmpty()))
                        {
                            List<Settlement> tempList = new List<Settlement>();
                            foreach (Settlement s in settlementsToVisit.Keys)
                            {
                                questBase.AddTrackedObject(s);
                                tempList.Add(s);
                            }

                            TextObject textObject = new TextObject("Explore the area and visit {SETTLEMENT1}, {SETTLEMENT2}, {SETTLEMENT3}.", null);
                            textObject.SetTextVariable("SETTLEMENT1", tempList[0].Name);
                            textObject.SetTextVariable("SETTLEMENT2", tempList[1].Name);
                            textObject.SetTextVariable("SETTLEMENT3", tempList[2].Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, settlementsVisited, NumberOfSettlementsToVisit, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        }
                    }
                }
            }

        }

        public override void OnSettlementEnteredQuest(MobileParty party, Settlement settlement, Hero hero, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (settlementsToVisit.Keys.Contains(settlement) && settlementsToVisit[settlement] == "no")
            {
                settlementsToVisit[settlement] = "yes";
                int i = settlementsToVisitNames.IndexOf(settlement.Name.ToString());
                settlementsToVisitTags[i] = "yes";
                settlementsVisited++;

                questBase.RemoveTrackedObject(settlement);
                if (!actioncomplete)
                {
                    questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], settlementsVisited);

                    if (settlementsVisited == NumberOfSettlementsToVisit)
                    {
                        questGen.currentActionIndex++;
                        actioncomplete = true;
                        questGen.chosenMission.run(CustomBTStep.questQ, questBase, questGen);

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
                    strat = new TextObject("I need you to scout around {SETTLEMENT}. Report back to me if you find anything worth considering.", null);
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
                    strat = new TextObject("{SETTLEMENT} is a {TYPE} located not far from here. It belongs to one of our allies and I've heard rumors that something dangerous has been seen nearby. Be careful while exploring the nearby area.", null);
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

        public override TextObject getStepDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            List<Settlement> tempList = new List<Settlement>();
            foreach (Settlement s in settlementsToVisit.Keys)
            {
                tempList.Add(s);
            }
            strat = new TextObject("Visit {SETTLEMENT1}, {SETTLEMENT2} and {SETTLEMENT3}.", null);
            strat.SetTextVariable("SETTLEMENT1", tempList[0].Name);
            strat.SetTextVariable("SETTLEMENT2", tempList[1].Name);
            strat.SetTextVariable("SETTLEMENT3", tempList[2].Name);

            return strat;
        }

    }
}
