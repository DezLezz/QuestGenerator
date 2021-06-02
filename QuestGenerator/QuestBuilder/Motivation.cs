using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGenerator.QuestBuilder
{
    public class Motivation
    {

        public string name { get; set; }
        public List<Strategy> strategies { get; set; }

        public Motivation(string name, List<Strategy> strategies)
        {
            this.name = name;
            this.strategies = strategies;
        }

    }
}
