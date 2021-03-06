using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static ThePlotLords.QuestGenTestCampaignBehavior;

namespace ThePlotLords.QuestBuilder.CustomBT
{
    public class CustomBTAction : CustomBTNode
    {

        public CustomBTAction(Action a, CustomBTType type, CustomBTNode nodeParent = null) : base(a, type, nodeParent)
        {
        }

        public CustomBTAction() { }

        public override CustomBTState run(CustomBTStep step, IssueBase issueBase, QuestGenTestIssue questGen, bool alternative)
        {
            if (step == CustomBTStep.actionTarget)
            {
                if (this.ActionTarget == null)
                {
                    switch (this.Action.name)
                    {
                        case "goto":
                            this.ActionTarget = new gotoAction(this.Action.name, this.Action);
                            break;
                        case "listen":
                            this.ActionTarget = new listenAction(this.Action.name, this.Action);
                            break;
                        case "report":
                            this.ActionTarget = new reportAction(this.Action.name, this.Action);
                            break;
                        case "give":
                            this.ActionTarget = new giveAction(this.Action.name, this.Action);
                            break;
                        case "gather":
                            this.ActionTarget = new gatherAction(this.Action.name, this.Action);
                            break;
                        case "explore":
                            this.ActionTarget = new exploreAction(this.Action.name, this.Action);
                            break;
                        case "quest":
                            this.ActionTarget = new subquestAction(this.Action.name, this.Action);
                            this.ActionTarget.children = this.Children;
                            break;
                        case "exchange":
                            this.ActionTarget = new exchangeAction(this.Action.name, this.Action);
                            break;
                        case "capture":
                            this.ActionTarget = new captureAction(this.Action.name, this.Action);
                            break;
                        case "kill":
                            this.ActionTarget = new killAction(this.Action.name, this.Action);
                            break;
                        case "free":
                            this.ActionTarget = new freeAction(this.Action.name, this.Action);
                            break;
                        case "take":
                            this.ActionTarget = new takeAction(this.Action.name, this.Action);
                            break;
                        case "use":
                            this.ActionTarget = new useAction(this.Action.name, this.Action);
                            break;
                        case "damage":
                            this.ActionTarget = new damageAction(this.Action.name, this.Action);
                            break;
                    }

                    this.ActionTarget.questGiver = questGen.missionHero;
                    this.ActionTarget.questGiverString = questGen.missionHero.Name.ToString();
                }


                if (alternative)
                {
                    this.ActionTarget.index = questGen.alternativeActionsInOrder.Count;
                    questGen.alternativeActionsInOrder.Add(this.ActionTarget);
                }
                else
                {
                    this.ActionTarget.index = questGen.actionsInOrder.Count;
                    questGen.actionsInOrder.Add(this.ActionTarget);
                }


                return CustomBTState.success;
            }
            if (step == CustomBTStep.issueQ)
            {
                this.ActionTarget.IssueQ(issueBase, questGen, alternative);
            }
            return CustomBTState.success;
        }

        public override CustomBTState run(CustomBTStep step, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (step == CustomBTStep.questQ)
            {

                this.ActionTarget.QuestQ(questBase, questGen);
                return CustomBTState.success;
            }

            return CustomBTState.success;
        }

        public override CustomBTState bringTargetsBack(IssueBase issueBase, QuestGenTestIssue questGen, bool alternative)
        {

            if (this.ActionTarget != null)
            {
                if (alternative)
                {
                    this.ActionTarget.bringTargetsBack();
                    questGen.alternativeActionsInOrder.Add(this.ActionTarget);
                }
                else
                {
                    this.ActionTarget.bringTargetsBack();
                    questGen.actionsInOrder.Add(this.ActionTarget);
                }

            }

            return CustomBTState.success;
        }

        public override CustomBTState bringTargetsBack(QuestBase questBase, QuestGenTestQuest questGen)
        {
            this.ActionTarget.bringTargetsBack();
            questGen.actionsInOrder.Add(this.ActionTarget);

            return CustomBTState.success;
        }

        public override void updateHeroTargets(string targetString, Hero targetHero)
        {
            this.ActionTarget.updateHeroTargets(targetString, targetHero);
        }
        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
            this.ActionTarget.updateSettlementTargets(targetString, targetSettlement);
        }
        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
            this.ActionTarget.updateItemTargets(targetString, targetItem);
        }
    }
}
