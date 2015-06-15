using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class section_csv : CSV_reader {

	public struct csv_row {
		public int section_id;
		public int chapter_id;
		public string chapter_name;
		public string section_name;
	};
	
	private List<string> keys = new List<string>(new string[] {"編號", "章節-PM", "名稱", "對應章編號"});
	public Dictionary <int, csv_row> csv_table = new Dictionary<int, csv_row>();
	
	//public Dictionary <string, List<section_row>> section = new Dictionary<string, List<section_row>>();
	public Dictionary <string, Dictionary<string, int>> section_list = new Dictionary<string, Dictionary<string, int>>();
	
	public void init () {
		SplitCsv (keys.ToArray(), csv_path);
		Debug.Log ("section_csv done");
/*
		foreach (section_row data in section["序章"]) {
			Debug.Log ( "name:"+data.section_name+" id:"+data.section_id );
		}
*/
	}

	public override void setCsvData(string[] row)
	{
		csv_row data = new csv_row();
		int section_id = 0;
		foreach( KeyValuePair<int, string> item in key_list){
			switch( item.Value )
			{
			case "編號":
				data.section_id = int.Parse(row[item.Key]);
				section_id = data.section_id;
				break;
			case "名稱":
				data.section_name = row[item.Key];
				break;
			case "章節-PM":
				data.chapter_name = row[item.Key];
				break;			
			case "對應章編號":
				data.chapter_id = int.Parse(row[item.Key]);
				break;
			}
		}
		csv_table.Add (section_id, data);

		if( section_list.ContainsKey(data.chapter_name) ){
			section_list[data.chapter_name][data.section_name] = section_id;
		} else {
			var tmp = new Dictionary<string, int>(){{data.section_name, section_id}};
			section_list.Add ( data.chapter_name, tmp );
		}

	}
}
