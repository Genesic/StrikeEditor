using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

public partial class GD_XmlData : MonoBehaviour
{
    [XmlRoot("Root")]
    public class csMonster
    {
        [XmlElement("出場怪物")]
        public Row<csMonsterAttribute> element = new Row<csMonsterAttribute>();
    }

    [Serializable]
    public class csMonsterAttribute : GD_XmlBase
    {
        [XmlAttribute("編號")]
        public string ID;
        [XmlAttribute("怪物id")]
        public string MonsterID;
        [XmlAttribute("縮放比例")]
        public string Scale;
        [XmlAttribute("怪物類型")]
        public string Type;

        public override string GetID { get { return ID; } }
    }
}