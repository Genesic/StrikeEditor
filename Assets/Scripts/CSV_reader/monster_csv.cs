using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class monster_csv : CSV_reader {	
	public struct ATKpoint {
		public string skill_id;
		public string name;
		public string cd;
		public string atk;
	};
	
	public struct csv_row {
		public int id;
		public int monster_id;
		public string name;
		public int type;
		public int star;
		public string hard;
		public int hp;
		public int shield_hp;
		public int gravity;
		public string drop_prob;
		public int money;
		public int def;
		public ATKpoint[] atk_point;
		public float scale;
		public string chapter;
		public int attr;
		public int cd;
	};
	
	private List<string> keys;
	
	public Dictionary <int, csv_row> csv_table = new Dictionary<int, csv_row>();
	public Dictionary <string, int> monster_chapter = new Dictionary<string, int>();
	public Dictionary <string, int> monster_hard = new Dictionary<string, int>();
	public Dictionary <int, int> monster_attr = new Dictionary<int, int>();
	
	public void init () {
		csv_table = new Dictionary<int, csv_row> ();
		keys = new List<string> (new string[] {
						"編號",
						"怪物id",
						"名稱-PM",
						"屬性-PM",
						"怪物類型",
						"星等-PM",
						"初始CD-PM",
						"難度說明-PM",
						"怪物HP",
						"盾-血量",
						"gravity",
						"擊殺獎勵-1-機率",
						"擊殺獎勵-2-數量",
						"怪物DEF",
						"縮放比例",
						"篇章"
				});
		for (int i = 1; i <= 5; i++) {
			string skill_name = "攻擊點-"+i+"-技能-PM";
			string skill_id = "攻擊點-"+i+"-技能編號";
			string skill_cd = "攻擊點-"+i+"-CD時間";
			string skill_atk = "攻擊點-"+i+"-攻擊力";
			keys.Add (skill_name);
			keys.Add (skill_id);
			keys.Add (skill_cd);
			keys.Add(skill_atk);
		}

		english_line = 0;
		SplitCsv (keys.ToArray (), csv_path);
		Debug.Log ("monster_csv done");
	}
	
	public override void setCsvData(string[] row)
	{
		csv_row data = new csv_row();
		int id = 0;
		ATKpoint[] point = new ATKpoint[5];
		foreach( KeyValuePair<int, string> item in key_list){
			switch( item.Value )
			{
			case "編號":
				data.id = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				id = data.id;
				break;
			case "怪物id":
				data.monster_id = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "怪物類型":
				data.type = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "名稱-PM":
				data.name = row[item.Key];
				break;
			case "屬性-PM":
				data.attr = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "星等-PM":
				data.star = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "初始CD-PM":
				data.cd = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "難度說明-PM":
				data.hard = row[item.Key];
				break;
			case "怪物HP":
				data.hp = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "盾-血量":
				data.shield_hp = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "gravity":
				data.gravity = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "擊殺獎勵-1-機率":
				data.drop_prob = row[item.Key];
				break;
			case "擊殺獎勵-2-數量":
				data.money = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "怪物DEF":
				data.def = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			case "縮放比例":
				data.scale = string.IsNullOrEmpty(row[item.Key])? 0.0f : Convert.ToSingle(row[item.Key]);
				break;
			case "篇章":
				data.chapter = row[item.Key];
				break;
			default : 
				for( int i = 1; i<=5 ; i++ ){
					if( item.Value.Equals( "攻擊點-"+i+"-技能-PM" ) ){
						point[i-1].name = row[item.Key];
					} else if ( item.Value.Equals( "攻擊點-"+i+"-技能編號" ) ){
						point[i-1].skill_id = row[item.Key];
					} else if ( item.Value.Equals( "攻擊點-"+i+"-CD時間" ) ){
						//Debug.Log("monster_id:"+monster_id+" key:"+item.Key+"data=("+row[item.Key]+")");
						point[i-1].cd = row[item.Key];
					} else if ( item.Value.Equals( "攻擊點-"+i+"-攻擊力" ) ){
						point[i-1].atk = row[item.Key];
					}
				}
				break;
			}
		}
		data.atk_point = point;
		csv_table.Add (id, data);

		if ( !monster_chapter.ContainsKey (data.chapter) && data.chapter.Length > 0 )
			monster_chapter.Add( data.chapter, 1 );

		if (!monster_hard.ContainsKey (data.hard) && data.hard.Length > 0) {
			if( data.hard.IndexOf("頭目") == -1 && data.hard.IndexOf("新手教學") == -1 )
				monster_hard.Add (data.hard, 1);
		}

		if ( !monster_attr.ContainsKey (data.attr) && data.attr > 0 )
			monster_attr.Add( data.attr, 1 );

		//monster_type.Add ();
	}
}
