using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static ThePlotLords.QuestGenTestCampaignBehavior;

namespace ThePlotLords.QuestBuilder.CustomBT
{
    public class CustomBTSelector : CustomBTNode
    {

        public CustomBTSelector(Action a, CustomBTType type, CustomBTNode nodeParent = null) : base(a, type, nodeParent)
        {

        }

        public CustomBTSelector() { }
        public override CustomBTState run(CustomBTStep step, IssueBase issueBase, QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Children.Count() > 0)
            {
                foreach (CustomBTNode node in this.Children)
                {
                    if (node.run(step, issueBase, questGen, alternative) == CustomBTState.fail)
                    {
                        return CustomBTState.fail;
                    }
                }
                return CustomBTState.success;
            }

            else
            {
                return CustomBTState.empty;
            }
        }
        public override CustomBTState run(CustomBTStep step, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (this.Children.Count() > 0)
            {
                foreach (CustomBTNode node in this.Children)
                {
                    if (node.run(step, questBase, questGen) == CustomBTState.fail)
                    {
                        return CustomBTState.fail;
                    }
                }
                return CustomBTState.success;
            }

            else
            {
                return CustomBTState.empty;
            }
        }
        public override CustomBTState bringTargetsBack(IssueBase issueBase, QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Children.Count() > 0)
            {
                foreach (CustomBTNode node in this.Children)
                {
                    if (node.bringTargetsBack(issueBase, questGen, alternative) == CustomBTState.fail)
                    {
                        return CustomBTState.fail;
                    }
                }
                return CustomBTState.success;
            }

            else
            {
                return CustomBTState.empty;
            }
        }

        public override CustomBTState bringTargetsBack(QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (this.Children.Count() > 0)
            {
                foreach (CustomBTNode node in this.Children)
                {
                    if (node.bringTargetsBack(questBase, questGen) == CustomBTState.fail)
                    {
                        return CustomBTState.fail;
                    }
                }
                return CustomBTState.success;
            }

            else
            {
                return CustomBTState.empty;
            }
        }

        public override void updateHeroTargets(string targetString, Hero targetHero)
        {
            if (this.Children.Count() > 0)
            {
                foreach (CustomBTNode node in this.Children)
                {
                    node.updateHeroTargets(targetString, targetHero);

                }
            }

        }
        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
            if (this.Children.Count() > 0)
            {
                foreach (CustomBTNode node in this.Children)
                {
                    node.updateSettlementTargets(targetString, targetSettlement);

                }
            }
        }
        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
            if (this.Children.Count() > 0)
            {
                foreach (CustomBTNode node in this.Children)
                {
                    node.updateItemTargets(targetString, targetItem);

                }
            }
        }
    }
}
