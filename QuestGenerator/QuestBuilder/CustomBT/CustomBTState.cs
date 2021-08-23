using System.Xml.Serialization;

namespace ThePlotLords.QuestBuilder.CustomBT
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
