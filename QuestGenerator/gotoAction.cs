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
                    InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB goto"));
                }
                if (array.Length == 1)
                {
                    this.settlementTarget = array[0];
                }
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative)
        {

            if (this.Action.param[0].target.Contains("place"))
            {
                string placeNumb = this.Action.param[0].target;
                Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                {
                    float num;
                    return x != questBase.IssueSettlement && x.Notables.Any<Hero>() && Campaign.Current.Models.MapDistanceModel.GetDistance(x, questBase.IssueSettlement, 100f, out num);
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
            
            if (this.settlementTarget != null)
            {
                questBase.AddTrackedObject(this.settlementTarget);
                TextObject textObject = new TextObject("Go to {SETTLEMENT}", null);
                textObject.SetTextVariable("SETTLEMENT", this.settlementTarget.Name);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
            }
        }

        public override void OnSettlementEnteredQuest(MobileParty party, Settlement settlement, Hero hero, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (settlement.Name == this.settlementTarget.Name)
            {
                InformationManager.DisplayMessage(new InformationMessage("Settlement Reached"));
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

    }
}
