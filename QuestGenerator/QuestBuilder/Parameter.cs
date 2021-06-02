using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGenerator.QuestBuilder
{
    public class Parameter
    {
        public string type { get; set; }
        public int flag { get; set; }
        public int sibling_ref { get; set; }

        public string target { get; set; }

        public Parameter(string type, int flag)
        {
            this.type = type;
            this.flag = flag;
            this.sibling_ref = -1;
        }

        public Parameter(string type, int flag, int sibling_ref)
        {
            this.type = type;
            this.flag = flag;
            this.sibling_ref = sibling_ref;
        }

        public Parameter() { }

    }
}
