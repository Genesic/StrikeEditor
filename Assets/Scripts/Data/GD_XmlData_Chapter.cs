using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

public partial class GD_XmlData : MonoBehaviour
{
    [XmlRoot("Root")]
    public class csChapter
    {
        [XmlElement("章")]
        public Row<csChapterAttribute> element = new Row<csChapterAttribute>();
    }

    [Serializable]
    public class csChapterAttribute : GD_XmlBase
    {
        [XmlAttribute("編號")]
        public string ID;
        [XmlAttribute("前置章編號")]
        public string ChapterID;

        public override string GetID { get { return ID; } }
    }
}