using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

public partial class GD_XmlData : MonoBehaviour
{
    public abstract class GD_XmlBase
    {
        public virtual string GetID { get { return ""; } }
    }
}