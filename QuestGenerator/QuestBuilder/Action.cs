using System.Collections.Generic;

namespace ThePlotLords.QuestBuilder
{
    public class Action
    {

        public string name { get; set; }
        public string type { get; set; }
        public string pre_condition { get; set; }
        public string post_condition { get; set; }
        public int index { get; set; }

        public string GameObject { get; set; }
        public string type_of_Target { get; set; }

        public List<Parameter> param { get; set; }

        public Action(string name, string type, int index, string type_of_Target)
        {
            this.name = name;
            this.type = type;
            this.index = index;
            this.type_of_Target = type_of_Target;
        }

        public Action(string name, string type, int index, string type_of_Target, List<Parameter> param)
        {
            this.name = name;
            this.type = type;
            this.index = index;
            this.type_of_Target = type_of_Target;
            this.param = param;
        }

        public Action() { }

    }
}
