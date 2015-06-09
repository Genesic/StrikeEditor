using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class battle_csv : CSV_reader {		
	public struct csv_row {
		public int battle_id;
		public int section;
	};

	private List<string> keys = new List<string>(new string[] {"編號", "對應節編號"});
	public Dictionary <int, csv_row> csv_table = new Dictionary<int, csv_row>();

	public void init () {
		SplitCsv (keys.ToArray(), csv_path);
		Debug.Log ("battle_csv done");
	}

	public override void setCsvData(string[] row)
	{
		csv_row data = new csv_row();
		int battle_id = 0;
		foreach( KeyValuePair<int, string> item in key_list){
			switch( item.Value )
			{
			case "編號":
				data.battle_id = int.Parse(row[item.Key]);
				battle_id = data.battle_id;
				break;
			case "對應節編號":
				data.section = int.Parse(row[item.Key]);
				break;
			}
		}
		csv_table.Add (battle_id, data);
	}

}
