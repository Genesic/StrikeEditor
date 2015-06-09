
using Gamesofa;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(GD_StrikeEditor))]
public class GD_StrikeEditorInspector : GD_EditorBase<GD_StrikeEditor>
{
    private static bool isFirst = false;

	public override void OnInspectorGUI ()
	{
        if (!isFirst)
        {
            isFirst = true;
            this.Target.Reset();
			this.Target.LoadCsv();
            //this.Target.LoadXml();
            this.Target.Init();
        }


        EditorGUIUtility.LookLikeControls();
        EditorGUILayout.Separator();

        #region 重置 & 建立

        EditorGUILayout.LabelField("【重置 & 建立】");

        GUILayout.BeginHorizontal();

        Color lastcolor = GUI.backgroundColor;

        GUI.backgroundColor = Color.magenta;

        if (GUILayout.Button("All Reset", GUILayout.Height(30f)))
        {
            this.Target.Reset();
        }

        if (GUILayout.Button("Create", GUILayout.Height(30f)))
        {
            this.Target.Create();
        }

        GUILayout.EndHorizontal();

        GUI.backgroundColor = lastcolor;

        #endregion 重置 & 建立

        EditorGUILayout.Separator();

        #region 讀取Json & 存檔Json

        EditorGUILayout.LabelField("【讀取Json & 存檔Json】");

        GUI.backgroundColor = Color.magenta;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load Json", GUILayout.Height(30f)))
        {
            string json = EditorUtility.OpenFilePanel("Load strike json", "Assets/Resources/Json", "json");
            this.Target.LoadJson(json);
        }

        if (GUILayout.Button("Save Json", GUILayout.Height(30f)))
        {
			this.Target.TextureArray = this.vGetChxBoxesFromNode(this.Target.gameObject);

            if (this.Target.TextureArray.Length == 0)
            {
                Debug.LogWarning("Error 場景上沒有物件  所以不存檔!!!");
                return;
            }

            this.Target.SaveJson(this.Target.TextureArray);

            if (this.Target.BattleIndex == 0)
            {
                Debug.LogWarning("Error 戰役為 預設值:0  所以不存檔!!!");
                return;
            }

            if (this.Target.FormationIndex == 0)
            {
                Debug.LogWarning("Error 陣型編號為 預設值:0  所以不存檔!!!");
                return;
            }

            Debug.LogWarning("SaveJson: " + this.Target.jsonStr);

            string jsonName = this.Target.ChapterIndex.ToString() + "-" + this.Target.SectionIndex.ToString() + "-" + this.Target.BattleIndex.ToString() + "-";
            string jsonPath = Application.dataPath + @"/Resources/Json/" + jsonName + "strikeJson.json";

            if (!File.Exists(jsonPath))
            {
                Directory.CreateDirectory("Assets/Resources/Json");
            }

            FileInfo file = new FileInfo(jsonPath);
            StreamWriter sw = file.CreateText();
            sw.WriteLine(this.Target.jsonStr);
            sw.Close();
            sw.Dispose();

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
                    
        }

        GUILayout.EndHorizontal();

        GUI.backgroundColor = lastcolor;

        #endregion 讀取Json & 存檔Json

        EditorGUILayout.Separator();

        #region 陣型編號

        EditorGUILayout.LabelField("【陣型編號】");

        GUILayout.BeginHorizontal();

        this.Target.FormationIndex = EditorGUILayout.IntField("陣型編號", this.Target.FormationIndex, GUILayout.Width(250));

        GUILayout.EndHorizontal();

        if (GUILayout.Button("陣型編號 Reset", GUILayout.Height(30f)))
        {
            this.Target.FormationIndex = 0;
        }

        #endregion 陣型編號

        EditorGUILayout.Separator();

        #region 章節戰役

        EditorGUILayout.LabelField("【章節戰役】");

        GUILayout.BeginHorizontal();

        GUILayout.Label("章", GUILayout.Width(30f));
        EditorGUILayout.LabelField(this.Target.ChapterIndex.ToString(), GUILayout.MinWidth(40f));

        GUILayout.Label("節", GUILayout.Width(30f));
        EditorGUILayout.LabelField(this.Target.SectionIndex.ToString(), GUILayout.MinWidth(40f));

        if (GD_XmlData.Section.ContainsKey(this.Target.SectionIndex.ToString()))
        {
            this.Target.ChapterIndex = Convert.ToInt32(GD_XmlData.Section[this.Target.SectionIndex.ToString()].ChapterID);
        }

        GUILayout.Label("戰役", GUILayout.Width(30f));
        this.Target.BattleIndex = EditorGUILayout.IntField(this.Target.BattleIndex, GUILayout.MinWidth(40f));

        if (GD_XmlData.Battle.ContainsKey(this.Target.BattleIndex.ToString()))
        {
            this.Target.SectionIndex = Convert.ToInt32(GD_XmlData.Battle[this.Target.BattleIndex.ToString()].SectionID);
        }


        GUILayout.EndHorizontal();

        if (GUILayout.Button("章節戰役 Reset", GUILayout.Height(30f)))
        {
            this.Target.ChapterIndex = 0;
            this.Target.SectionIndex = 0;
            this.Target.BattleIndex = 0;
        }

        #endregion 章節戰役

        EditorGUILayout.Separator();

        #region 小兵編號

        EditorGUILayout.LabelField("【設定 出場小兵編號】");

        for (int i = 0; i < GD_StrikeEditor.MonsterCount; i++)
        {
            this.Target.Monster[i] = EditorGUILayout.IntField("小兵" + (i + 1) + " 編號", this.Target.Monster[i], GUILayout.Width(250));
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("小兵 Reset", GUILayout.Height(30f)))
        {
            for (int i = 0; i < GD_StrikeEditor.MonsterCount; i++)
            {
                this.Target.Monster[i] = 0;
            }
        }
        if (GUILayout.Button("小兵 Random", GUILayout.Height(30f)))
        {
            this.Target.Monster = this.Target.RandomMonsterPos();
        }

        GUILayout.EndHorizontal();

        #endregion 小兵編號

        EditorGUILayout.Separator();

        #region Boss 位置設定

        EditorGUILayout.LabelField("【設定 出場Boss位置】");


        for (int i = 0; i < GD_StrikeEditor.BossCount; i++)
        {
            GUILayout.BeginHorizontal();

            if (GD_XmlData.Formation.ContainsKey(this.Target.FormationIndex.ToString("D4")))
            {

                if (i < Convert.ToInt32(GD_XmlData.Formation[this.Target.FormationIndex.ToString("D4")].PeopleNum))
                {
                    this.Target.Boss[i] = EditorGUILayout.IntField("位置" + (i + 1) + " Boss編號", this.Target.Boss[i], GUILayout.Width(250));

					//string x = this.Target.FormationX[this.Target.FormationIndex.ToString("D4")][i];
					//string y = this.Target.FormationY[this.Target.FormationIndex.ToString("D4")][i];
					string x = this.Target.formation_data.csv_table[this.Target.FormationIndex].enemy_point[i].x.ToString();
					string y = this.Target.formation_data.csv_table[this.Target.FormationIndex].enemy_point[i].y.ToString();
                    EditorGUILayout.LabelField("  X : " + x + "  Y : " + y);
                }
            }       

            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Boss Reset", GUILayout.Height(30f)))
        {
            for (int i = 0; i < GD_StrikeEditor.BossCount; i++)
            {
                this.Target.Boss[i] = 0;
            }
        }

        #endregion Boss 位置設定

        EditorGUILayout.Separator();

        #region 英雄編號

        EditorGUILayout.LabelField("【設定 出場英雄位置】");

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("刪除英雄", GUILayout.Height(30f)))
        {
            this.Target.ClearHero();
        }

        if (GUILayout.Button("建立英雄", GUILayout.Height(30f)))
        {
            this.Target.CreateHero();
        }

        GUILayout.EndHorizontal();

        #endregion 英雄編號

        //base.OnInspectorGUI();
	}

	private UITexture[] vGetChxBoxesFromNode(GameObject _node)
    {
        var cbs = _node.GetComponentsInChildren<UITexture>(true);
        var sorted = cbs.OrderBy( c => c.name);
        return sorted.ToArray();
    }
}
