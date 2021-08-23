using System.Collections.Generic;

namespace ThePlotLords.QuestBuilder
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
