using System.Collections.Generic;

namespace ThePlotLords.QuestBuilder
{
    public class QuestNode
    {

        public string nodeName { get; set; }
        public Quest parentQuest { get; set; }
        public QuestNode parentNode { get; set; }
        public List<QuestNode> childNodes { get; set; }

        public Action action { get; set; }
        public Motivation motivation { get; set; }
        public Strategy strategy { get; set; }
        public Rules rule { get; set; }

        public List<QuestNode> Depth {
            get {
                List<QuestNode> path = new List<QuestNode>();
                foreach (QuestNode node in this.childNodes)
                {
                    List<QuestNode> tmp = node.Depth;
                    if (tmp.Count > path.Count)
                        path = tmp;
                }
                path.Insert(0, this);
                return path;
            }
        }


        public QuestNode(string nodeName, Quest parentQuest, QuestNode parentNode)
        {
            this.nodeName = nodeName;
            this.parentQuest = parentQuest;
            this.parentNode = parentNode;
            this.childNodes = new List<QuestNode>();
        }

    }
}
