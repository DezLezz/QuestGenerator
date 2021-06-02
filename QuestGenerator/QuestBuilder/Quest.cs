using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGenerator.QuestBuilder
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
            QuestNumb = questNumb;
            QuestGiver = questGiver;
            Strategies = strategies;
            Rules = rules;
            this.steps = new List<Action>();
            charCounter = 0;
            itemCounter = 0;
            locCounter = 0;
            enemyCounter = 0;
        }
    }
}
