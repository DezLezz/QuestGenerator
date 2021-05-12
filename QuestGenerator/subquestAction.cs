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
using static TaleWorlds.CampaignSystem.QuestBase;

namespace QuestGenerator
{
    [Serializable]
    class subquestAction : actionTarget
    {

        [NonSerialized]
        public Hero heroTarget;

        [NonSerialized]
        public Settlement settlementTarget;

        public string settlementTargetString;

        public subquestAction(string action, string target) : base(action, target)
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
                var setName = this.target;

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

        public override void IssueQ(List<actionTarget> list, Settlement issueSettlement, Hero issueGiver)
        {
            if (this.target.Contains("npc"))
            {
                string npcNumb = this.target;
                string targetHero = "none";
                Hero newHero = new Hero();
                int i = list.IndexOf(this);
                if (i > 0)
                {
                    if (list[i - 1].action == "goto")
                    {
                        Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == list[i - 1].target) select x).ToArray<Settlement>();

                        if (array.Length > 1)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Everything is on fire Issue"));
                        }
                        if (array.Length == 1)
                        {
                            foreach (Hero h in array[0].Notables)
                            {
                                if (h.Issue == null && !h.IsOccupiedByAnEvent())
                                {
                                    newHero = h;
                                    targetHero = newHero.Name.ToString();
                                    break;
                                }
                            }
                            settlementTargetString = array[0].Name.ToString();
                            settlementTarget = array[0];
                        }
                        
                    }
                    else
                    {
                        Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                        {
                            float num;
                            if (Enumerable.Any<Hero>(x.Notables, (Hero y) => !y.IsOccupiedByAnEvent()))
                            {
                                return Campaign.Current.Models.MapDistanceModel.GetDistance(x, issueSettlement, 150f, out num);
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

                else if (i == 0)
                {
                    Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        float num;
                        if (Enumerable.Any<Hero>(x.Notables, (Hero y) => !y.IsOccupiedByAnEvent()))
                        {
                            return Campaign.Current.Models.MapDistanceModel.GetDistance(x, issueSettlement, 150f, out num);
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
                    foreach (actionTarget nextAction in list)
                    {
                        if (nextAction.target == npcNumb)
                        {
                            nextAction.target = targetHero;
                            nextAction.SetHeroTarget(newHero);
                        }
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

        }

        public override void QuestQ(List<actionTarget> list, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen, int index)
        {
            if (this.settlementTarget != null)
            {
                TextObject textObject = new TextObject("Talk to {HERO} in {SETTLEMENT} and perform its task.", null);
                textObject.SetTextVariable("HERO", this.heroTarget.Name);
                textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
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
                questGen.UpdateQuestTaskS(questGen.journalLogs[questGen.currentActionIndex], 1);

                questGen.currentActionIndex++;
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
