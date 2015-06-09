using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System;


public partial class GD_XmlData : MonoBehaviour
{
    #region xml公用的部分

    public class Row<T>
    {
        [XmlElement("Row")]
        public List<T> rows = new List<T>();
    }

    #endregion

    #region 加入enum，需和該表格對應的名稱相同。

    public enum XmlTable
    {
        Chapter,
        Section,
        Battle,
        Formation,
        Monster,
    }

    #endregion

    #region 宣告部分

    private static csBattle smBattle;
    private static csChapter smChapter;
    private static csFormation smFormation;
    private static csMonster smMonster;
    private static csSection smSection;

    private static List<csBattleAttribute> BattleList { get { return smBattle.element.rows; } }
    private static List<csChapterAttribute> ChapterList { get { return smChapter.element.rows; } }
    private static List<csFormationAttribute> FormationList { get { return smFormation.element.rows; } }
    private static List<csMonsterAttribute> MonsterList { get { return smMonster.element.rows; } }
    private static List<csSectionAttribute> SectionList { get { return smSection.element.rows; } }

    #endregion

    #region 對外界面

    public static Dictionary<string, csBattleAttribute> Battle = new Dictionary<string, csBattleAttribute>();
    public static Dictionary<string, csChapterAttribute> Chapter = new Dictionary<string, csChapterAttribute>();
    public static Dictionary<string, csFormationAttribute> Formation = new Dictionary<string, csFormationAttribute>();
    public static Dictionary<string, csMonsterAttribute> Monster = new Dictionary<string, csMonsterAttribute>();
    public static Dictionary<string, csSectionAttribute> Section = new Dictionary<string, csSectionAttribute>();

    #endregion

    #region 讀取xml

    private static T GetXml<T>(XmlTable table)
    {
        var xmlPath = string.Format("Assets/Resources/Xml/{0}.xml", Enum.GetName(typeof(XmlTable), table));

        FileStream ReadFileStream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        XmlSerializer serializer = new XmlSerializer(typeof(T));

        T result = (T)serializer.Deserialize(ReadFileStream);

        ReadFileStream.Close();

        return result;
    }

    #endregion

    //對應索引
    private static Dictionary<string, T> SetDic<T>(List<T> list) where T : GD_XmlBase
    {
        Dictionary<string, T> dic = new Dictionary<string, T>();
        foreach (T item in list)
            dic.Add(item.GetID, item);     

        return dic;
    }

    //初始化
    public static void Init()
    {    
        smBattle = GetXml<csBattle>(XmlTable.Battle);
        smChapter = GetXml<csChapter>(XmlTable.Chapter);
        smFormation = GetXml<csFormation>(XmlTable.Formation);
        smMonster = GetXml<csMonster>(XmlTable.Monster);
        smSection = GetXml<csSection>(XmlTable.Section);

        Battle = SetDic<csBattleAttribute>(BattleList);
        Chapter = SetDic<csChapterAttribute>(ChapterList);
        Formation = SetDic<csFormationAttribute>(FormationList);
        Monster = SetDic<csMonsterAttribute>(MonsterList);
        Section = SetDic<csSectionAttribute>(SectionList);

        Debug.LogWarning("< ~~~ GD_XmlData is init finish !! ~~~ >");
    }


}