using Helpers;
using Newtonsoft.Json;
using QuestGenerator.QuestBuilder;
using QuestGenerator.QuestBuilder.CustomBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static QuestGenerator.QuestGenTestCampaignBehavior;


namespace QuestGenerator
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

        public exploreAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }

        public exploreAction() { }

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
                this.settlementsToVisit = new Dictionary<Settlement, string>();
                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("explore action - line 58"));
                }
                if (array.Length == 1)
                {
                    this.settlementTarget = array[0];
                }

                for (int i = 0; i < this.settlementsToVisitNames.Count; i++)
                {
                    string settlementName = this.settlementsToVisitNames[i];
                    
                    Settlement[] array1 = (from x in Settlement.All where (x.Name.ToString() == settlementName) select x).ToArray<Settlement>();

                    if (array1.Length > 1 || array1.Length == 0)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("explore action - line 72"));
                    }
                    if (array1.Length == 1)
                    {
                        this.settlementsToVisit.Add(array1[0], this.settlementsToVisitTags[i]);
                    }
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
            settlementsToVisit = new Dictionary<Settlement, string>();
            settlementsToVisitNames = new List<string>();
            settlementsToVisitTags = new List<string>();

            if (this.Action.param[0].target.Contains("place"))
            {
                this.NumberOfSettlementsToVisit = 3;
                this.settlementsVisited = 0;
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

                this.settlementsToVisitNames.Add(this.settlementTarget.Name.ToString());
                this.settlementsToVisitTags.Add("no");
                this.settlementsToVisit.Add(this.settlementTarget, "no");

                for (int i = 0; i < 2; i++)
                {
                    Settlement newSettlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        return x.OwnerClan == this.settlementTarget.OwnerClan && !(this.settlementsToVisit.Keys.Contains(x));
                    });
                    this.settlementsToVisitNames.Add(newSettlement.Name.ToString());
                    this.settlementsToVisitTags.Add("no");
                    this.settlementsToVisit.Add(newSettlement, "no");
                }
            }
            else if (this.settlementsToVisit.IsEmpty())
            {
                if (this.settlementTarget == null)
                {
                    var setName = this.Action.param[0].target;
                    Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                    if (array.Length > 1 || array.Length == 0)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("explore action - line 58"));
                    }
                    if (array.Length == 1)
                    {
                        this.settlementTarget = array[0];
                    }
                }
                this.settlementsToVisitNames.Add(this.settlementTarget.Name.ToString());
                this.settlementsToVisitTags.Add("no");
                this.settlementsToVisit.Add(this.settlementTarget, "no");

                for (int i = 0; i < 2; i++)
                {
                    Settlement newSettlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        return x.OwnerClan == this.settlementTarget.OwnerClan && !this.settlementsToVisit.ContainsKey(x);
                    });
                    this.settlementsToVisitNames.Add(newSettlement.Name.ToString());
                    this.settlementsToVisitTags.Add("no");
                    this.settlementsToVisit.Add(newSettlement, "no");
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
                    if (this.settlementTarget != null && !(this.settlementsToVisit.IsEmpty()))
                    {
                        List<Settlement> tempList = new List<Settlement>();
                        foreach (Settlement s in this.settlementsToVisit.Keys)
                        {
                            questBase.AddTrackedObject(s);
                            tempList.Add(s);
                        }

                        TextObject textObject = new TextObject("Explore the area and visit {SETTLEMENT1}, {SETTLEMENT2}, {SETTLEMENT3}.", null);
                        textObject.SetTextVariable("SETTLEMENT1", tempList[0].Name);
                        textObject.SetTextVariable("SETTLEMENT2", tempList[1].Name);
                        textObject.SetTextVariable("SETTLEMENT3", tempList[2].Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, this.settlementsVisited, this.NumberOfSettlementsToVisit, null, false);
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[this.index - 1].actioncomplete)
                    {
                        this.actionInLog = true;
                        if (this.settlementTarget != null && !(this.settlementsToVisit.IsEmpty()))
                        {
                            List<Settlement> tempList = new List<Settlement>();
                            foreach (Settlement s in this.settlementsToVisit.Keys)
                            {
                                questBase.AddTrackedObject(s);
                                tempList.Add(s);
                            }

                            TextObject textObject = new TextObject("Explore the area and visit {SETTLEMENT1}, {SETTLEMENT2}, {SETTLEMENT3}.", null);
                            textObject.SetTextVariable("SETTLEMENT1", tempList[0].Name);
                            textObject.SetTextVariable("SETTLEMENT2", tempList[1].Name);
                            textObject.SetTextVariable("SETTLEMENT3", tempList[2].Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, this.settlementsVisited, this.NumberOfSettlementsToVisit, null, false);
                        }
                    }
                }
            }
            
        }

        public override void OnSettlementEnteredQuest(MobileParty party, Settlement settlement, Hero hero, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.settlementsToVisit.Keys.Contains(settlement) && this.settlementsToVisit[settlement] == "no")
            {
                this.settlementsToVisit[settlement] = "yes";
                int i = this.settlementsToVisitNames.IndexOf(settlement.Name.ToString());
                this.settlementsToVisitTags[i] = "yes";
                this.settlementsVisited++;

                questBase.RemoveTrackedObject(settlement);
                if (!actioncomplete)
                {
                    questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], this.settlementsVisited);
                    
                    if (this.settlementsVisited == this.NumberOfSettlementsToVisit)
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
                    strat = new TextObject("I need you to scout around {SETTLEMENT}. Report back to me if you find anything worth considering.", null);
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
                    strat = new TextObject("{SETTLEMENT} is a {TYPE} located not far from here. It belongs to one of our allies and I've heard rumors that something dangerous has been seen nearby. Be careful while exploring the nearby area.", null);
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
