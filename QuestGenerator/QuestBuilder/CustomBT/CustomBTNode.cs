using System.Collections.Generic;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static ThePlotLords.QuestGenTestCampaignBehavior;

namespace ThePlotLords.QuestBuilder.CustomBT
{
    [XmlInclude(typeof(CustomBTMotivation)), XmlInclude(typeof(CustomBTSelector)), XmlInclude(typeof(CustomBTSequence)), XmlInclude(typeof(CustomBTAction))]
    public abstract class CustomBTNode
    {
        public actionTarget ActionTarget { get; set; }

        public Action Action { get; set; }

        public List<CustomBTNode> Children { get; set; }

        public string name { get; set; }

        public string info { get; set; }

        public string subquest_info { get; set; }

        public string origin_quest_motiv { get; set; }

        public string origin_quest_hero { get; set; }

        public CustomBTType nodeType { get; set; }

        [XmlIgnore]
        public CustomBTNode parent { get; set; }

        public CustomBTNode() { }

        public CustomBTNode(Action nodeAction)
        {
            this.Action = nodeAction;
            this.Children = new List<CustomBTNode>();
        }

        public CustomBTNode(Action nodeAction, CustomBTType type, CustomBTNode nodeParent = null)
        {
            this.Action = nodeAction;
            this.Children = new List<CustomBTNode>();
            this.nodeType = type;
            this.parent = nodeParent;
            this.subquest_info = "none";
        }

        public abstract CustomBTState run(CustomBTStep step, QuestBase questBase, QuestGenTestQuest questGen);

        public abstract CustomBTState run(CustomBTStep step, IssueBase questBase, QuestGenTestIssue questGen, bool alternative);

        public abstract CustomBTState bringTargetsBack(QuestBase questBase, QuestGenTestQuest questGen);
        public abstract CustomBTState bringTargetsBack(IssueBase questBase, QuestGenTestIssue questGen, bool alternative);

        public abstract void updateHeroTargets(string targetString, Hero targetHero);
        public abstract void updateSettlementTargets(string targetString, Settlement targetSettlement);
        public abstract void updateItemTargets(string targetString, ItemObject targetItem);

        public virtual void addNode(CustomBTNode newNode)
        {
            this.Children.Add(newNode);
        }

        public virtual void removeNode(CustomBTNode nodeToRemove)
        {
            this.Children.Remove(nodeToRemove);
        }

    }
}
