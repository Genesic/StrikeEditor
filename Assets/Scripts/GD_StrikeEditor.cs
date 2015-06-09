using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Gamesofa
{
    [ExecuteInEditMode]
	public class GD_StrikeEditor : MonoBehaviour
    {
        public const int HeroCount = 4;
        public const int MonsterCount = 5;
        public const int BossCount = 10;

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

		void Update()
		{
			ShowCoordinate();
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

            Monster = RandomMonsterPos();
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

        //XML 初始化
		public void LoadXml()
        {
            GD_XmlData.Init();
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

                //如果是boss
				if (monster_data.csv_table[monster_id].type != 0)
                {
                    for (int i = 0; i < GD_StrikeEditor.BossCount; i++)
                    {
						if (formation_data.csv_table.ContainsKey(FormationIndex) )
						{
                            if (i < formation_data.csv_table[FormationIndex].num)
                            {
								float bx = Convert.ToSingle(formation_data.csv_table[FormationIndex].enemy_point[i].x);
								float by = Convert.ToSingle(formation_data.csv_table[FormationIndex].enemy_point[i].y);

                                if (x == bx && y == by)
                                {
                                    Boss[i] = Convert.ToInt32(item["id"]);
                                }
                            }
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
        public void Create()
        {
            Clear();

			if ( formation_data.csv_table.ContainsKey(FormationIndex) )
            {
                for (int i = 0; i < BossCount; i++)
                {
					if (i < formation_data.csv_table[FormationIndex].num)
                    {
                        GameObject obj = Instantiate(Resources.Load("Prefabs/Monster", typeof(GameObject))) as GameObject;
						monster_csv.csv_row monster = new monster_csv.csv_row();

						float x = Convert.ToSingle(formation_data.csv_table[FormationIndex].enemy_point[i].x) + DefaultX;
						float y = Convert.ToSingle(formation_data.csv_table[FormationIndex].enemy_point[i].y) + DefaultY;

                        //該位置沒有放boss
                        if (Boss[i] == 0)
                        {
							if (monster_data.csv_table.ContainsKey(Monster[i]))
								monster = monster_data.csv_table[Monster[i]];
                        }
                        else
                        {
							if (monster_data.csv_table.ContainsKey(Boss[i]))
								monster = monster_data.csv_table[Boss[i]];                                
                        }

						obj.GetComponent<UITexture>().mainTexture = Resources.Load("Png/Role/role_" + monster.monster_id ) as Texture;
                        obj.GetComponent<UIWidget>().MakePixelPerfect();

                        obj.layer = Layer;
                        obj.name = monster.monster_id.ToString();
                        obj.transform.parent = this.gameObject.transform;
						obj.transform.localScale = new Vector3(Convert.ToSingle(monster.scale), Convert.ToSingle(monster.scale), 1f);
                        obj.transform.localPosition = new Vector3(x, y, 0f);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Error 沒有對應的陣型編號 無法建立場景!!!");
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

        //重置
        public void Reset()
        {
            Clear();

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
        private void Clear()
        {
            foreach (UITexture item in this.gameObject.GetComponentsInChildren<UITexture>())
            {         
                if (item != null)
                {
                    DestroyImmediate(item.gameObject);
                }
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
