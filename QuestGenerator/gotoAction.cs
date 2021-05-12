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

namespace QuestGenerator
{
    [Serializable]
    class gotoAction : actionTarget
    {
        [NonSerialized]
        public Settlement settlementTarget;
        public gotoAction(string action, string target) : base(action, target)
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
                var setName = this.target;

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

        public override void IssueQ(List<actionTarget> list, Settlement issueSettlement, Hero issueGiver)
        {
            if (this.target.Contains("place"))
            {
                string placeNumb = this.target;
                Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                {
                    float num;
                    return x != issueSettlement && x.Notables.Any<Hero>() && Campaign.Current.Models.MapDistanceModel.GetDistance(x, issueSettlement, 100f, out num);
                });
                foreach (actionTarget nextAction in list)
                {
                    if (nextAction.target == placeNumb)
                    {
                        nextAction.target = settlement.Name.ToString();
                        nextAction.SetSettlementTarget(settlement);
                    }
                }
            }
        }

        public override void QuestQ(List<actionTarget> list, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen, int index)
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
            if (settlement.Name.ToString() == questGen.currentAction.target)
            {
                InformationManager.DisplayMessage(new InformationMessage("Settlement Reached"));
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
