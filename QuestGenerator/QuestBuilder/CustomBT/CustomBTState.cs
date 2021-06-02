using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QuestGenerator.QuestBuilder.CustomBT
{
    public enum CustomBTState
    {
        [XmlEnum(Name = "success")]
        success,
        [XmlEnum(Name = "fail")]
        fail,
        [XmlEnum(Name = "empty")]
        empty
    }
}
