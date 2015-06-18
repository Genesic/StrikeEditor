using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Gamesofa
{
    [ExecuteInEditMode]
	public class GD_StrikeEditor : MonoBehaviour
    {
        public const int HeroCount = 4;
        public const int MonsterCount = 8;
        public const int BossCount = 6;

		public UITexture[] TextureArray = null;
		public string jsonStr = "";

        public bool showBtn = false;

        public List<int> Monster = new List<int>(MonsterCount); //小兵編號
        public List<int> Boss = new List<int>(BossCount); //Boss編號
		
        public int FormationIndex = 0; //陣型
        public int ChapterIndex = 0; //章
        public int SectionIndex = 0; //節
        public int BattleIndex = 0; //戰役

		private const float DefaultX = -288f;
		private const float DefaultY = -320f;
		private const float ShowDefaultX = 288f;
		private const float ShowDefaultY = 320f;

        private const int Layer = 0;
        private List<int> MonsterList = new List<int>();
        private List<int> BossList = new List<int>();

        private csUnit[] mUnits; //Node 資料結構

		public monster_csv monster_data;
		public battle_csv battle_data;
		public section_csv section_data;
		public formation_csv formation_data;

		public int select_chapter;
		public int use_soldier_num;

		public bool[] select_ch;
		public bool[] select_hard;
		public bool[] select_attr;

		void Update()
		{
			ShowCoordinate();
			if (select_chapter > 0) {
				Debug.Log (select_chapter);
			}
		}

        //初始化
        public void Init()
        {
            MonsterList.Clear();
            BossList.Clear();

			foreach (KeyValuePair<int, monster_csv.csv_row > monster in monster_data.csv_table )
            {
                switch(monster.Value.type)
                {
                    case 0:
                        MonsterList.Add(monster.Key);
                        break;
                    case 1:
                    case 2:
                    case 3:
                        BossList.Add(monster.Key);
                        break;
                }
            }

			select_ch = new bool[getBattleMonsterChaper().Length];
			select_hard = new bool[getBattleMonsterHard().Length];
			select_attr = new bool[getBattleMonsterAttr().Length];
        }

		public void LoadCsv()
		{
			GameObject csv_reader = GameObject.FindGameObjectWithTag ("csv");
			monster_data = csv_reader.GetComponent<monster_csv> ();
			battle_data = csv_reader.GetComponent<battle_csv>();
			section_data = csv_reader.GetComponent<section_csv>();
			formation_data = csv_reader.GetComponent<formation_csv>();

			monster_data.init ();
			battle_data.init ();
			section_data.init ();
			formation_data.init ();
		}

        //讀檔
        public void LoadJson(string jsonFile)
        {
            Reset();

            string jsonStr = File.ReadAllText(jsonFile);
            Debug.Log("LoadJson ~~~ " + jsonStr);

            Hashtable json = MiniJSON.jsonDecode(jsonStr) as Hashtable;

            ChapterIndex = Convert.ToInt32(json["Chapter"]);
            SectionIndex = Convert.ToInt32(json["Section"]);
            BattleIndex = Convert.ToInt32(json["Battle"]);
            FormationIndex = Convert.ToInt32(json["Formation"]);

            ArrayList monsterArr = MiniJSON.jsonDecode(json["monster"].ToString()) as ArrayList;

            foreach (Hashtable item in monsterArr)
            {
                GameObject obj = Instantiate(Resources.Load("Prefabs/Monster", typeof(GameObject))) as GameObject;

                float x = Convert.ToSingle(item["x"]);
                float y = Convert.ToSingle(item["y"]);

                float sx = Convert.ToSingle(item["sx"]);
                float sy = Convert.ToSingle(item["sy"]);

				int monster_id = Convert.ToInt32(item["id"]);
				obj.GetComponent<UITexture>().mainTexture = Resources.Load("Png/Role/role_" + monster_data.csv_table[monster_id].monster_id) as Texture;
                obj.GetComponent<UIWidget>().MakePixelPerfect();

                obj.layer = Layer;
				obj.name = item["id"].ToString();
                obj.transform.parent = this.gameObject.transform;
                obj.transform.localScale = new Vector3(sx, sy, 1f);
				obj.transform.localPosition = new Vector3(x+DefaultX, y+DefaultY, 0f);
				obj.GetComponent<GD_MonsterEditor>().init ( monster_data.csv_table[monster_id] );

                //如果是boss
				if (monster_data.csv_table[monster_id].type != 0)
                {
					obj.tag = "boss";
					for (int i=0 ; i< BossCount ; i++ ){
						if( Boss[i] == 0 ){
							Boss[i] = monster_id;
							break;
						}
					}
                } else {
					obj.tag = "monster";
					for (int i=0 ; i< MonsterCount ; i++ ){
						if( Monster[i] == 0 ){
							Monster[i] = monster_id;
							break;
						}
					}
				}
            }

            ArrayList heroArr = MiniJSON.jsonDecode(json["hero"].ToString()) as ArrayList;
            int heroCount = 0;
            foreach (Hashtable item in heroArr)
            {
                GameObject obj = Instantiate(Resources.Load("Prefabs/Monster", typeof(GameObject))) as GameObject;

                float x = Convert.ToSingle(item["x"]);
                float y = Convert.ToSingle(item["y"]);

                obj.GetComponent<UITexture>().mainTexture = Resources.Load("Png/Ball/role_ball_0296") as Texture;
                obj.GetComponent<UIWidget>().MakePixelPerfect();

                obj.layer = Layer;
                obj.name = "Hero_" + heroCount;
                obj.transform.parent = this.gameObject.transform;
                obj.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                obj.transform.localPosition = new Vector3(x, y, 0f);

                heroCount++;
            }

        }

        //存檔
		public void SaveJson(UITexture[] textureArr)
        {
			if (textureArr == null || textureArr.Length <= 0)
                return;

            Dictionary<string, string> SceneInfo = new Dictionary<string, string>();
            SceneInfo.Add("Chapter", ChapterIndex.ToString());
            SceneInfo.Add("Section", SectionIndex.ToString());
            SceneInfo.Add("Battle", BattleIndex.ToString());
            SceneInfo.Add("Formation", FormationIndex.ToString());

            ArrayList heroJson = new ArrayList();
            ArrayList monsterJson = new ArrayList();
			this.mUnits = new csUnit[textureArr.Length];

			for (int i = 0; i < textureArr.Length; i++)
            {
				var unit = new csUnit(i, textureArr[i]);
                this.mUnits[i] = unit;

                if (textureArr[i].gameObject.name.IndexOf("Hero_") > -1)
                {
                    //hero
                    Dictionary<string, string> heroElement = new Dictionary<string, string>();
                    heroElement.Add("x", unit.Px.ToString());
                    heroElement.Add("y", unit.Py.ToString());

                    heroJson.Add(heroElement);
                }
                else
                {
                    //monster
                    Dictionary<string, string> monsterElement = new Dictionary<string, string>();

                    monsterElement.Add("id", unit.Name.ToString());
                    monsterElement.Add("x", unit.Px.ToString());
                    monsterElement.Add("y", unit.Py.ToString());
                    monsterElement.Add("sx", unit.Sx.ToString());
                    monsterElement.Add("sy", unit.Sy.ToString());

                    monsterJson.Add(monsterElement);
                }
            }

            string monsterJsonStr = MiniJSON.jsonEncode(monsterJson);
            SceneInfo.Add("monster", monsterJsonStr);

            string heroJsonStr = MiniJSON.jsonEncode(heroJson);
            SceneInfo.Add("hero", heroJsonStr);

            jsonStr = MiniJSON.jsonEncode(SceneInfo);
        }

		//顯示物件座標
		public void ShowCoordinate()
		{
			foreach (UITexture item in this.gameObject.GetComponentsInChildren<UITexture>())
			{
				if (item != null)
				{
					UILabel label = item.GetComponentInChildren<UILabel>();
					label.text = "(" + (item.transform.localPosition.x + ShowDefaultX) + "," + (item.transform.localPosition.y + ShowDefaultY) + ")";
				}
			}
		}

        //建立場景怪物
        public void CreateMonster()
        {
            ClearMonster();

			if ( formation_data.csv_table.ContainsKey(FormationIndex) )
            {
				for (int i = 0; i < MonsterCount; i++)
                {
					if (i < formation_data.csv_table[FormationIndex].num)
                    {                        
						monster_csv.csv_row monster = new monster_csv.csv_row();
	
						if (monster_data.csv_table.ContainsKey(Monster[i]))
							monster = monster_data.csv_table[Monster[i]];
						else 
							continue;

						GameObject obj = Instantiate(Resources.Load("Prefabs/Monster", typeof(GameObject))) as GameObject;
						float x = Convert.ToSingle(formation_data.csv_table[FormationIndex].enemy_point[i].x) + DefaultX;
						float y = Convert.ToSingle(formation_data.csv_table[FormationIndex].enemy_point[i].y) + DefaultY;

						obj.GetComponent<UITexture>().mainTexture = Resources.Load("Png/Role/role_" + monster.monster_id ) as Texture;
                        obj.GetComponent<UIWidget>().MakePixelPerfect();

                        obj.layer = Layer;
						obj.tag = "monster";
						obj.name = Monster[i].ToString();
                        obj.transform.parent = this.gameObject.transform;
						obj.transform.localScale = new Vector3(Convert.ToSingle(monster.scale), Convert.ToSingle(monster.scale), 1f);
                        obj.transform.localPosition = new Vector3(x, y, 0f);
						obj.GetComponent<GD_MonsterEditor>().init ( monster_data.csv_table[Monster[i]] );
                    }
                }
            }
            else
            {
                Debug.LogWarning("Error 沒有對應的陣型編號 無法建立場景!!!");
            }
        }

		//建立場景Boss
		public void CreateBoss()
		{
			ClearBoss();
			
			for (int i = 0; i < BossCount; i++)
			{
				monster_csv.csv_row boss = new monster_csv.csv_row();
											
				//該位置沒有放boss
				if (monster_data.csv_table.ContainsKey(Boss[i]))
					boss = monster_data.csv_table[Boss[i]];
				else
					continue;

				GameObject obj = Instantiate(Resources.Load("Prefabs/Monster", typeof(GameObject))) as GameObject;
				float x = 0 + DefaultX;
				float y = 0 + DefaultY;

				obj.GetComponent<UITexture>().mainTexture = Resources.Load("Png/Role/role_" + boss.monster_id ) as Texture;
				obj.GetComponent<UIWidget>().MakePixelPerfect();
						
				obj.layer = Layer;
				obj.tag = "boss";
				obj.name = Boss[i].ToString();
				obj.transform.parent = this.gameObject.transform;
				obj.transform.localScale = new Vector3(Convert.ToSingle(boss.scale), Convert.ToSingle(boss.scale), 1f);
				obj.transform.localPosition = new Vector3(x, y, 0f);
				obj.GetComponent<GD_MonsterEditor>().init ( monster_data.csv_table[Boss[i]] );
			}
		}

        //建立英雄
        public void CreateHero()
        {
            ClearHero();
            for (int i = 0; i < HeroCount; i++)
            {
                GameObject obj = Instantiate(Resources.Load("Prefabs/Monster", typeof(GameObject))) as GameObject;

                obj.GetComponent<UITexture>().mainTexture = Resources.Load("Png/Ball/role_ball_0296") as Texture;
                obj.GetComponent<UIWidget>().MakePixelPerfect();

                obj.layer = Layer;
                obj.name = "Hero_"+i;
                obj.transform.parent = this.gameObject.transform;
                obj.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                obj.transform.localPosition = new Vector3(i*200, -400, 0f);
            }

        }

        //刪除英雄
        public void ClearHero()
        {
            foreach (UITexture item in this.gameObject.GetComponentsInChildren<UITexture>())
            {
                if (item != null)
                {
                    if (item.gameObject.name.IndexOf("Hero_") > -1)
                    {
                        DestroyImmediate(item.gameObject);
                    }
                }
            }
        }

        //隨機小怪
        public List<int> RandomMonsterPos()
        {
            List<int> list = new List<int>();
            System.Random rnd = new System.Random();

            HashSet<int> generated = new HashSet<int>();

            for (int i = 0; i < MonsterCount; i++)
            {

                int r = 0;

                //bruce 讚!
                do
                { r = rnd.Next(MonsterList.Count); }
                while (generated.Contains(r));

                generated.Add(r);

                list.Add(Convert.ToInt32(MonsterList[r]));
            }
            return list;
        }

		public List<int> CreateRandomMonster ()
		{
			List<int> random_list_cd1 = new List<int>();
			List<int> random_list_cd6 = new List<int>();
			List<int> list = new List<int>();
			System.Random rnd = new System.Random();
			HashSet<int> generated = new HashSet<int>();

			string[] chapter_list = getBattleMonsterChaper ();
			string[] hard_list = getBattleMonsterHard ();
			int[] attr_list = monster_data.monster_attr.Keys.ToArray();

			foreach (KeyValuePair<int, monster_csv.csv_row> keyValue in monster_data.csv_table ) {
				monster_csv.csv_row monster_info = keyValue.Value;
				bool chapter_flag = false;
				bool hard_flag = false;
				bool attr_flag = false;

				// 檢查chapter是否在選擇中
				for( int i =0 ; i< chapter_list.Length ; i ++ ){
					if( chapter_list[i].Equals(monster_info.chapter) ){
						if( select_ch[i] )
							chapter_flag = true;

						break;
					}
				}
				
				if( !chapter_flag )
					continue;

				// 檢查hard是否在選擇中
				for( int i =0 ; i< hard_list.Length ; i++ ){
					if( hard_list[i].Equals( monster_info.hard ) ){
						if( select_hard[i] )
							hard_flag = true;

						break;
					}
				}

				if( !hard_flag )
					continue;

				// 檢查屬性是否在選擇中
				for( int i=0 ; i<attr_list.Length ; i++ ){
					if( attr_list[i] == monster_info.attr ){
						if( select_attr[i] )
							attr_flag = true;

						break;
					}
				}

				if( !attr_flag )
					continue;

				if( monster_info.cd == 1 ){
					random_list_cd1.Add (monster_info.id);
				} else if( monster_info.cd == 6 ){
					random_list_cd6.Add (monster_info.id);
				}
			}

			if (random_list_cd1.Count < use_soldier_num) {
				Debug.Log ("可用怪物數不夠#1");
				return list;
			}

			if (random_list_cd6.Count < MonsterCount - use_soldier_num) {
				Debug.Log ("可用怪物數不夠#6");
				return list;
			}


			for (int i = 0; i < MonsterCount; i++)	{
				int r = 0;
				int selected;
				if( i < use_soldier_num ){
					do
					{ 
						r = rnd.Next(random_list_cd1.Count); 
						selected = random_list_cd1[r];
					}
					while (generated.Contains(selected));
				} else {
					do
					{ 
						r = rnd.Next(random_list_cd6.Count); 
						selected = random_list_cd6[r];
					}
					while (generated.Contains(selected));
				}
				generated.Add(selected);				
				list.Add(selected);
			}

			return list;
		}

        //重置
        public void Reset()
        {
            ClearMonster();
			ClearBoss();

            Monster.Clear();
            for (int i = 0; i < MonsterCount; i++)
                Monster.Add(0);

            Boss.Clear();
            for (int i = 0; i < BossCount; i++)
                Boss.Add(0);

            FormationIndex = 0;
            ChapterIndex = 0;
            SectionIndex = 0;
			BattleIndex = 0;
        }

        //清除場景物件
        public void ClearMonster()
        {
            foreach (UITexture item in this.gameObject.GetComponentsInChildren<UITexture>())
            {         
                if (item != null && item.tag.Equals("monster") )
                {
                    DestroyImmediate(item.gameObject);
                }
            }
        }

		public void ClearBoss()
		{
			foreach (UITexture item in this.gameObject.GetComponentsInChildren<UITexture>())
			{         
				if (item != null && item.tag.Equals("boss") )
				{
					DestroyImmediate(item.gameObject);
				}
			}
		}

		public struct section_info{
			public int section_id;
			public string section_name;
			public int chapter_id;
			public string chapter_name;
		};

		// 用battle_id取得section_id
		public int getSectionByBattleId (int battle_id){
			return battle_data.csv_table [battle_id].section;
		}

		// 用section_id取得 章 跟 節 資料
		public section_info getSectionInfoById( int section_id ){
			section_info data = new section_info ();
			data.chapter_id = section_data.csv_table [section_id].chapter_id;
			data.section_id = section_data.csv_table [section_id].section_id;
			data.chapter_name = section_data.csv_table [section_id].chapter_name;
			data.section_name = section_data.csv_table [section_id].section_name;
			return data;
		}

		// 取得battle_monster中總共有哪些篇章
		public string[] getBattleMonsterChaper(){
			string[] chapter = monster_data.monster_chapter.Keys.ToArray ();
			return chapter;
		}

		// 取得battle_monster中總共有哪些難度
		public string[] getBattleMonsterHard(){
			string[] hard = monster_data.monster_hard.Keys.ToArray ();
			return hard;
		}

		// 取得battle_monster中總共有哪些屬性
		public string[] getBattleMonsterAttr(){
			int[] attr = monster_data.monster_attr.Keys.ToArray();
			string [] attr_list = new string[attr.Length];
			for( int i = 0; i < attr.Length ; i++ ){
				attr_list[i] = attr[i].ToString();
			}
			return attr_list;
		}

		// 取得怪物名稱
		public string getMonsterShow(int id){
			if (monster_data.csv_table.ContainsKey (id)) {
				return monster_data.csv_table [id].name+"("+monster_data.csv_table [id].cd+")"+ " HP:"+monster_data.csv_table [id].hp;
			} else {
				return "";
			}
		}

        //Monster or Hero Node 資料結構
        public class csUnit
        {
            public int Index;
			public UITexture Texture;
			public string Name;

			public float Px;
			public float Py;
            public float Sx;
            public float Sy;

			public csUnit(int _index, UITexture _texture)
            {
                if (_texture.mainTexture == null)
                    Debug.LogWarning("Error textureArr [" + _index + "] -> " + _texture.mainTexture);

                this.Index = _index;
				this.Texture = _texture;

                this.Name = _texture.gameObject.name;
				this.Px = _texture.transform.localPosition.x - DefaultX;
				this.Py = _texture.transform.localPosition.y - DefaultY;
                this.Sx = _texture.transform.localScale.x;
                this.Sy = _texture.transform.localScale.y;
            }
        }

 
    }
}
