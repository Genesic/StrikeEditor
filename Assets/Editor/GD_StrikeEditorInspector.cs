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
	private static bool[] select_chapter;
	private static bool[] select_hard;
	private static bool[] select_attr;
	private static string[] bmChapter;
	private static string[] bmHard;
	private static string[] bmAttr;

	public override void OnInspectorGUI ()
	{
        if (!isFirst)
        {
            isFirst = true;
            this.Target.Reset();
			this.Target.LoadCsv();
            //this.Target.LoadXml();
            this.Target.Init();
			bmChapter = this.Target.getBattleMonsterChaper();
			bmHard = this.Target.getBattleMonsterHard();
			bmAttr = this.Target.getBattleMonsterAttr();
			select_chapter = new bool[bmChapter.Length];
			select_hard = new bool[bmHard.Length];
			select_attr = new bool[bmAttr.Length];
        }

        EditorGUIUtility.LookLikeControls();
        EditorGUILayout.Separator();
		Color lastcolor = GUI.backgroundColor;
/*
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
*/
        #region 讀取Json & 存檔Json

		GUIStyle title_style = new GUIStyle();
		title_style.fontSize = 13;
        EditorGUILayout.LabelField("【讀取Json & 存檔Json】", title_style);

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
/*
        EditorGUILayout.Separator();

		#region 重置 & 建立
		
		EditorGUILayout.LabelField("【重置 & 建立】");
		
		GUILayout.BeginHorizontal();
		
		//Color lastcolor = GUI.backgroundColor;
		
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
*/		
		EditorGUILayout.Separator();

        #region 陣型編號


		EditorGUILayout.LabelField("【陣型編號】",title_style);        

		GUILayout.BeginHorizontal();

        this.Target.FormationIndex = EditorGUILayout.IntField("陣型編號", this.Target.FormationIndex, GUILayout.Width(250));

        GUILayout.EndHorizontal();

        //if (GUILayout.Button("陣型編號 Reset", GUILayout.Height(30f)))
        //{
         //   this.Target.FormationIndex = 0;
        //}

        #endregion 陣型編號

        EditorGUILayout.Separator();

        #region 章節戰役

		EditorGUILayout.LabelField("【章節戰役】",title_style);        

		GUILayout.BeginHorizontal();

		GUILayout.Label("戰役", GUILayout.Width(30f));
		this.Target.BattleIndex = EditorGUILayout.IntField(this.Target.BattleIndex, GUILayout.MaxWidth(50f));
		
		GD_StrikeEditor.section_info section_data = new GD_StrikeEditor.section_info();
		if( this.Target.BattleIndex > 0){
			int section_id = this.Target.getSectionByBattleId(this.Target.BattleIndex);
			section_data = this.Target.getSectionInfoById(section_id);
			this.Target.ChapterIndex = section_data.chapter_id;
			this.Target.SectionIndex = section_data.section_id;
		}

        GUILayout.Label("章:", GUILayout.Width(20f));
		EditorGUILayout.LabelField( section_data.chapter_name + "("+this.Target.ChapterIndex+")", GUILayout.MinWidth(40f));

		GUILayout.Label("節:", GUILayout.Width(20f));
        EditorGUILayout.LabelField( section_data.section_name + "("+this.Target.SectionIndex+")", GUILayout.MinWidth(40f));

        GUILayout.EndHorizontal();
		
        #endregion 章節戰役

        EditorGUILayout.Separator();

		#region 小兵篩選
		EditorGUILayout.LabelField("【小兵篩選】", title_style);

		GUIStyle my_style = new GUIStyle();
		my_style.normal.textColor = Color.red;
		my_style.fontSize = 11;
		GUILayout.BeginHorizontal();
		GUILayout.Label("篇章", my_style,GUILayout.Width(30f));
		if (GUILayout.Button("重置", GUILayout.Width(40f))) {
			for (int i = 0; i< bmChapter.Length; i++) {
				this.Target.select_ch[i] = false;
			}
		}
		GUILayout.EndHorizontal();

		int horizon_list = 2;
		
		for (int i = 0; i< bmChapter.Length; i++) {
			if( i % horizon_list == 0 ) {
				if( i != 0 )
					GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			}

			this.Target.select_ch[i] = EditorGUILayout.Toggle( bmChapter[i], this.Target.select_ch[i] );

		}
		GUILayout.EndHorizontal();		

		GUILayout.BeginHorizontal();
		GUILayout.Label("難度", my_style,GUILayout.Width(30f));
		if (GUILayout.Button("重置", GUILayout.Width(40f))) {
			for (int i = 0; i< bmHard.Length; i++) {
				this.Target.select_hard[i] = false;
			}
		}
		GUILayout.EndHorizontal();

		for (int i = 0; i< bmHard.Length; i++) {
			if( i % horizon_list == 0 ){
				if( i != 0 )
					GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			}

			this.Target.select_hard[i] = EditorGUILayout.Toggle( bmHard[i], this.Target.select_hard[i]);

		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("屬性", my_style,GUILayout.Width(30f));
		if (GUILayout.Button("重置", GUILayout.Width(40f))) {
			for (int i = 0; i< bmAttr.Length; i++) {
				this.Target.select_attr[i] = false;
			}
		}
		GUILayout.EndHorizontal();

		for (int i = 0; i< bmAttr.Length; i++) {
			if( i % horizon_list == 0 ){
				if( i != 0 )
					GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			}

			this.Target.select_attr[i] = EditorGUILayout.Toggle( bmAttr[i], this.Target.select_attr[i]);

		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("小兵種類設定", GUILayout.Width(100f));
		this.Target.use_soldier_num = EditorGUILayout.IntField(this.Target.use_soldier_num, GUILayout.MaxWidth(50f));
		if (GUILayout.Button("產生小兵編號", GUILayout.Height(18f))){
			List<int> monster_list = this.Target.CreateRandomMonster();
			if( monster_list.Count > 0 )
				this.Target.Monster = monster_list;
		}
		if (GUILayout.Button("清除小兵編號", GUILayout.Height(18f))){
			for (int i = 0; i < GD_StrikeEditor.MonsterCount; i++)
				this.Target.Monster[i] = 0;
		}

		GUILayout.EndHorizontal();
				        
        for (int i = 0; i < GD_StrikeEditor.MonsterCount; i++)
        {
			GUILayout.BeginHorizontal();
			GUILayout.Label("小兵"+(i+1), GUILayout.Width(40f));
			GUILayout.Label(" 編號", GUILayout.Width(40f) );
			this.Target.Monster[i] = EditorGUILayout.IntField( this.Target.Monster[i], GUILayout.MaxWidth(50));
			GUILayout.Label(" "+this.Target.getMonsterShow(this.Target.Monster[i]), GUILayout.Width(300f));
			GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
/*
        if (GUILayout.Button("小兵 Reset", GUILayout.Height(30f)))
        {
            for (int i = 0; i < GD_StrikeEditor.MonsterCount; i++)
            {
                this.Target.Monster[i] = 0;
            }
        }
*/        
		if (GUILayout.Button("清除小兵", GUILayout.Height(30f)))
		{
			this.Target.ClearMonster();
		}

        if (GUILayout.Button("產生小兵", GUILayout.Height(30f)))
        {
			this.Target.CreateMonster();
        }

        GUILayout.EndHorizontal();

        #endregion 小兵編號

        EditorGUILayout.Separator();
		
        #region Boss 位置設定

        EditorGUILayout.LabelField("【新增陷阱】");

		for (int i = 0; i < GD_StrikeEditor.BossCount; i++) {
			GUILayout.BeginHorizontal();

			GUILayout.Label(" 編號", GUILayout.Width(40f) );
			this.Target.Boss[i] = EditorGUILayout.IntField( this.Target.Boss[i], GUILayout.MaxWidth(50));
			GUILayout.Label(" "+this.Target.getMonsterShow(this.Target.Boss[i]), GUILayout.Width(300f));
			GUILayout.EndHorizontal();
		}

		if (GUILayout.Button("清除小兵", GUILayout.Height(30f)))
		{
			//this.Target.ClearBoss();
		}
		if (GUILayout.Button("產生", GUILayout.Height(30f)))
		{
			//this.Target.CreateBoss();
		}
/*		
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
*/
        #endregion Boss 位置設定
/*
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
*/
        //base.OnInspectorGUI();
	}

	private UITexture[] vGetChxBoxesFromNode(GameObject _node)
    {
        var cbs = _node.GetComponentsInChildren<UITexture>(true);
        var sorted = cbs.OrderBy( c => c.name);
        return sorted.ToArray();
    }
}
