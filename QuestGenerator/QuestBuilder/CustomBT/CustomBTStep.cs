using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QuestGenerator.QuestBuilder.CustomBT
{
    public enum CustomBTStep
    {
        [XmlEnum(Name = "issueQ")]
        issueQ,
        [XmlEnum(Name = "questQ")]
        questQ
    }
}
