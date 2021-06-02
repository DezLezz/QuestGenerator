using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QuestGenerator.QuestBuilder.CustomBT
{
    public enum CustomBTType
    {
        [XmlEnum(Name = "motivation")]
        motivation,
        [XmlEnum(Name = "action")]
        action,
        [XmlEnum(Name = "sequence")]
        sequence,
        [XmlEnum(Name = "selector")]
        selector
    }
}
