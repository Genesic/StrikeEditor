using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

public partial class GD_XmlData : MonoBehaviour
{
    [XmlRoot("Root")]
    public class csFormation
    {
        [XmlElement("陣型")]
        public Row<csFormationAttribute> element = new Row<csFormationAttribute>();
    }

    [Serializable]
    public class csFormationAttribute : GD_XmlBase
    {
        [XmlAttribute("陣型編號")]
        public string ID;
        [XmlAttribute("適用人數PM用")]
        public string PeopleNum;
        [XmlAttribute("座標1x")]
        public string X1;
        [XmlAttribute("座標1y")]
        public string Y1;
        [XmlAttribute("座標2x")]
        public string X2;
        [XmlAttribute("座標2y")]
        public string Y2;
        [XmlAttribute("座標3x")]
        public string X3;
        [XmlAttribute("座標3y")]
        public string Y3;
        [XmlAttribute("座標4x")]
        public string X4;
        [XmlAttribute("座標4y")]
        public string Y4;
        [XmlAttribute("座標5x")]
        public string X5;
        [XmlAttribute("座標5y")]
        public string Y5;
        [XmlAttribute("座標6x")]
        public string X6;
        [XmlAttribute("座標6y")]
        public string Y6;
        [XmlAttribute("座標7x")]
        public string X7;
        [XmlAttribute("座標7y")]
        public string Y7;
        [XmlAttribute("座標8x")]
        public string X8;
        [XmlAttribute("座標8y")]
        public string Y8;
        [XmlAttribute("座標9x")]
        public string X9;
        [XmlAttribute("座標9y")]
        public string Y9;

        public override string GetID { get { return ID; } }
    }
}