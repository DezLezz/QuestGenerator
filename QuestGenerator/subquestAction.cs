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
using static TaleWorlds.CampaignSystem.QuestBase;
using QuestGenerator.QuestBuilder;

namespace QuestGenerator
{
    public class subquestAction : actionTarget
    {

        [XmlIgnore]
        public Hero heroTarget;

        [XmlIgnore]
        public Settlement settlementTarget;

        public string settlementTargetString;

        public subquestAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public subquestAction() { }

        public override Hero GetHeroTarget()
        {
            return this.heroTarget;
        }

        public override void SetHeroTarget(Hero newH)
        {
            this.heroTarget = newH;
        }

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
                var setName = this.settlementTargetString;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB goto"));
                }
                if (array.Length == 1)
                {
                    this.settlementTarget = array[0];
                }
            }

            if (this.heroTarget == null)
            {
                var setName = this.Action.param[0].target;

                Hero[] array = (from x in Hero.All where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB give"));
                }
                if (array.Length == 1)
                {
                    this.heroTarget = array[0];
                }
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("npc"))
            {
                string npcNumb = this.Action.param[0].target;
                string targetHero = "none";
                Hero newHero = new Hero();

                if (this.index > 0)
                {
                    if (questGen.actionsInOrder[this.index -1].action == "goto")
                    {
                        Settlement settlement = questGen.actionsInOrder[this.index - 1].GetSettlementTarget();

                        
                        foreach (Hero h in settlement.Notables)
                        {
                            if (h.Issue == null && !h.IsOccupiedByAnEvent())
                            {
                                newHero = h;
                                targetHero = h.Name.ToString();
                                break;
                            }
                        }
                        settlementTargetString = settlement.Name.ToString();
                        settlementTarget = settlement;
                        
                    }
                    else
                    {
                        Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                        {
                            float num;
                            if (Enumerable.Any<Hero>(x.Notables, (Hero y) => !y.IsOccupiedByAnEvent()))
                            {
                                return Campaign.Current.Models.MapDistanceModel.GetDistance(x, questBase.IssueSettlement, 150f, out num);
                            }
                            return false;
                        });

                        foreach (Hero h in settlement.Notables)
                        {
                            if (h.Issue == null && !h.IsOccupiedByAnEvent())
                            {
                                newHero = h;
                                targetHero = newHero.Name.ToString();
                                break;
                            }
                        }

                        settlementTargetString = settlement.Name.ToString();
                        settlementTarget = settlement;
                    }
                }
                else if (this.index == 0)
                {
                    Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        float num;
                        if (Enumerable.Any<Hero>(x.Notables, (Hero y) => !y.IsOccupiedByAnEvent()))
                        {
                            return Campaign.Current.Models.MapDistanceModel.GetDistance(x, questBase.IssueSettlement, 150f, out num);
                        }
                        return false;
                    });

                    foreach (Hero h in settlement.Notables)
                    {
                        if (h.Issue == null && !h.IsOccupiedByAnEvent())
                        {
                            newHero = h;
                            targetHero = newHero.Name.ToString();
                            break;
                        }
                    }

                    settlementTargetString = settlement.Name.ToString();
                    settlementTarget = settlement;
                }
                if (targetHero != "none")
                {
                    if (alternative)
                    {
                        questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                    }
                    else
                    {
                        questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                    }
                }
                if (targetHero == "none")
                {
                    InformationManager.DisplayMessage(new InformationMessage("Target Hero is on fire"));
                }
            }

            else if (this.settlementTarget == null)
            {
                this.settlementTarget = this.heroTarget.CurrentSettlement;
                this.settlementTargetString = this.settlementTarget.Name.ToString();
            }

            PotentialIssueData potentialIssueData = new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnIssueSelected), typeof(QuestGenTestCampaignBehavior.QuestGenTestIssue), IssueBase.IssueFrequency.Common);
            Campaign.Current.IssueManager.CreateNewIssue(potentialIssueData, this.heroTarget);

        }

        public IssueBase OnIssueSelected(in PotentialIssueData pid, Hero issueOwner)
        {
            return new QuestGenTestCampaignBehavior.QuestGenTestIssue(issueOwner, this.children[0]);
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (this.settlementTarget != null)
            {
                TextObject textObject = new TextObject("Talk to {HERO} in {SETTLEMENT} and perform its task.", null);
                textObject.SetTextVariable("HERO", this.heroTarget.Name);
                textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                questGen.journalLogs[this.index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
            }
            else
            {
                InformationManager.DisplayMessage(new InformationMessage("Something went wrong"));
            }
        }

        public override void OnQuestCompletedEventQuest(QuestBase quest, QuestCompleteDetails questCompleteDetails, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (quest.QuestGiver == this.heroTarget)
            {
                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);

                questGen.currentActionIndex++;
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

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
        }

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
        }
    }
}
