using System.Xml.Serialization;

namespace ThePlotLords.QuestBuilder.CustomBT
{
    public enum CustomBTStep
    {
        [XmlEnum(Name = "issueQ")]
        issueQ,
        [XmlEnum(Name = "questQ")]
        questQ,
        [XmlEnum(Name = "actionTarget")]
        actionTarget
    }
}
