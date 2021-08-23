using System.Collections.Generic;

namespace ThePlotLords.QuestBuilder
{
    public class Quest
    {

        public string QuestName { get; set; }
        public int QuestNumb { get; set; }
        public QuestGiver QuestGiver { get; set; }
        public List<Strategy> Strategies { get; set; }
        public List<Rules> Rules { get; set; }

        public QuestNode root { get; set; }
        public List<Action> steps { get; set; }

        public int charCounter { get; set; }
        public int itemCounter { get; set; }
        public int locCounter { get; set; }
        public int enemyCounter { get; set; }

        public Quest(int questNumb, QuestGiver questGiver, List<Strategy> strategies, List<Rules> rules)
        {
            this.QuestNumb = questNumb;
            this.QuestGiver = questGiver;
            this.Strategies = strategies;
            this.Rules = rules;
            this.steps = new List<Action>();
            this.charCounter = 0;
            this.itemCounter = 0;
            this.locCounter = 0;
            this.enemyCounter = 0;
        }
    }
}
