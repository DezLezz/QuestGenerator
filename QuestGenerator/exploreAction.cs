using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.Core;
using static QuestGenerator.QuestGenTestCampaignBehavior;
using TaleWorlds.CampaignSystem.Actions;
using System.IO;


namespace QuestGenerator
{
    [Serializable]
    class exploreAction : actionTarget
    {
        
        [NonSerialized]
        public Settlement settlementTarget;

        [NonSerialized]
        public Dictionary<Settlement, string> settlementsToVisit;

        public List<string> settlementsToVisitNames;

        public List<string> settlementsToVisitTags;

        public int NumberOfSettlementsToVisit;

        public int settlementsVisited;

        public exploreAction(string action, string target) : base(action, target)
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
                var setName = this.target;
                this.settlementsToVisit = new Dictionary<Settlement, string>();
                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB goto"));
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
                        InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB goto"));
                    }
                    if (array1.Length == 1)
                    {
                        this.settlementsToVisit.Add(array1[0], this.settlementsToVisitTags[i]);
                    }
                }

            }
        }

        public override void IssueQ(List<actionTarget> list, Settlement issueSettlement, Hero issueGiver)
        {
            settlementsToVisit = new Dictionary<Settlement, string>();
            settlementsToVisitNames = new List<string>();
            settlementsToVisitTags = new List<string>();

            if (this.target.Contains("place"))
            {
                this.NumberOfSettlementsToVisit = 3;
                this.settlementsVisited = 0;
                string placeNumb = this.target;
                Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                {
                    float num;
                    return x != issueSettlement && x.Notables.Any<Hero>() && Campaign.Current.Models.MapDistanceModel.GetDistance(x, issueSettlement, 100f, out num);
                });
                this.target = settlement.Name.ToString();
                this.SetSettlementTarget(settlement);
                foreach (actionTarget nextAction in list)
                {
                    if (nextAction.target == placeNumb)
                    {
                        nextAction.target = settlement.Name.ToString();
                        nextAction.SetSettlementTarget(settlement);
                    }
                }

                this.settlementsToVisitNames.Add(this.settlementTarget.Name.ToString());
                this.settlementsToVisitTags.Add("no");
                this.settlementsToVisit.Add(this.settlementTarget,"no");

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
                this.settlementsToVisitNames.Add(this.settlementTarget.Name.ToString());
                this.settlementsToVisitTags.Add("no");
                this.settlementsToVisit.Add(this.settlementTarget, "no");

                for (int i = 0; i < 2; i++)
                {
                    Settlement newSettlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        return x.OwnerClan == this.settlementTarget.OwnerClan;
                    });
                    this.settlementsToVisitNames.Add(newSettlement.Name.ToString());
                    this.settlementsToVisitTags.Add("no");
                    this.settlementsToVisit.Add(newSettlement, "no");
                }
            }
        }

        public override void QuestQ(List<actionTarget> list, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen, int index)
        {
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

        public override void OnSettlementEnteredQuest(MobileParty party, Settlement settlement, Hero hero, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (this.settlementsToVisit.Keys.Contains(settlement) && this.settlementsToVisit[settlement] == "no")
            {
                this.settlementsToVisit[settlement] = "yes";
                int i = this.settlementsToVisitNames.IndexOf(settlement.Name.ToString());
                this.settlementsToVisitTags[i] = "yes";
                InformationManager.DisplayMessage(new InformationMessage("Settlement Reached"));
                this.settlementsVisited++;
                questGen.UpdateQuestTaskS(questGen.journalLogs[questGen.currentActionIndex], this.settlementsVisited);

                if (this.settlementsVisited == this.NumberOfSettlementsToVisit)
                {
                    questGen.currentActionIndex++;
                }
            }

            if (questGen.currentActionIndex < questGen.actionsTargets.Count)
            {
                questGen.currentAction = questGen.actionsTargets[questGen.currentActionIndex];
            }
            else
            {
                questGen.SuccessConsequences();
            }

        }
    }
}
