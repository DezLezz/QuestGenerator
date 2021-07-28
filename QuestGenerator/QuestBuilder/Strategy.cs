using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGenerator.QuestBuilder
{
    public class Strategy
    {

        public string Motivation { get; set; }
        public string name { get; set; }
        public int index { get; set; }

        public List<Action> actions { get; set; }

        public Strategy() { }

        public Strategy(string motivation, string name, int index, List<Action> actions)
        {
            Motivation = motivation;
            this.name = name;
            this.index = index;
            this.actions = actions;
        }
    }
}
