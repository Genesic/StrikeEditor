using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

public partial class GD_XmlData : MonoBehaviour
{
    [XmlRoot("Root")]
    public class csSection
    {
        [XmlElement("節")]
        public Row<csSectionAttribute> element = new Row<csSectionAttribute>();
    }

    [Serializable]
    public class csSectionAttribute : GD_XmlBase
    {
        [XmlAttribute("編號")]
        public string ID;
        [XmlAttribute("對應章編號")]
        public string ChapterID;

        public override string GetID { get { return ID; } }
    }
}