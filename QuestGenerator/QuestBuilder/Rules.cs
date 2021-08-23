using System.Collections.Generic;

namespace ThePlotLords.QuestBuilder
{
    public class Rules
    {

        public string name { get; set; }
        public string type { get; set; }
        public string strategy { get; set; }
        public string explanation { get; set; }

        public List<float> weights {

            get { return _weights; }

            set { _weights = value; }

        }

        private List<float> _weights;

        public List<List<Action>> actions { get; set; }

        public Rules(string name, string type, int index, List<List<Action>> actions, List<float> weights)
        {
            this.name = name;
            this.type = type;
            this.actions = actions;
            this.weights = weights;
        }

        public List<Action> getNewAction(int index)
        {
            var list = this.actions[index];
            List<Action> newList = new List<Action>();

            foreach (Action a in list)
            {
                List<Parameter> newParamList = new List<Parameter>();
                foreach (Parameter p in a.param)
                {
                    Parameter newP = new Parameter(p.type, p.flag, p.sibling_ref);
                    newParamList.Add(newP);
                }
                Action newA = new Action(a.name, a.type, a.index, a.type_of_Target, newParamList);
                newList.Add(newA);
            }

            return newList;
        }

        public Rules() { }
    }
}
