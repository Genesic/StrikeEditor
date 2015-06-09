using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

public partial class GD_XmlData : MonoBehaviour
{
    [XmlRoot("Root")]
    public class csBattle
    {
        [XmlElement("戰役")]
        public Row<csBattleAttribute> element = new Row<csBattleAttribute>();
    }

    [Serializable]
    public class csBattleAttribute : GD_XmlBase
    {
        [XmlAttribute("編號")]
        public string ID;
        [XmlAttribute("對應節編號")]
        public string SectionID;
        [XmlAttribute("戰役類型")]
        public string BattleType;

        public override string GetID { get { return ID; } }
    }
}