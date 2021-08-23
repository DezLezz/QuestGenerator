using System.Xml.Serialization;

namespace ThePlotLords.QuestBuilder.CustomBT
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
