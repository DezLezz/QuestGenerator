using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace ThePlotLords
{
    public class RemoveQuests
    {

        public static void UnnistalQuests()
        {
            Campaign.Current.CampaignBehaviorManager.RemoveBehavior<QuestGenTestCampaignBehavior>();

            foreach (var issue in Campaign.Current.IssueManager.Issues.ToList())
            {
                if (issue.Value.GetType().Namespace.Contains("ThePlotLords"))
                {
                    Campaign.Current.IssueManager.DeactivateIssue(issue.Value);
                }
            }

            InformationManager.DisplayMessage(new InformationMessage("[PL] - Quests removed, you can now save and exit the game. Don't forget to remove/unistall Plot Lords."));
        }

    }
}
