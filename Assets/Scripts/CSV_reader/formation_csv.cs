using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class formation_csv : CSV_reader {
	public struct csv_row {
		public int id;
		public int num;
		public Vector2[] enemy_point;
		public Vector2[] hero_point;
	};

	private List<string> keys;
	public Dictionary <int, csv_row> csv_table = new Dictionary<int, csv_row>();

	public void init(){
		keys = new List<string>(new string[]{"陣型編號","適用人數"});
		for (int i = 1; i <= 9; i++) {
			string point_x = "座標"+i+"x";
			string point_y = "座標"+i+"y";
			keys.Add (point_x);
			keys.Add (point_y);
		}

		for (int i = 1; i <= 4; i++) {
			string point_x = i+"p出場X";
			string point_y = i+"p出場Y";
			keys.Add (point_x);
			keys.Add (point_y);
		}

		SplitCsv (keys.ToArray (), csv_path);
		Debug.Log ("formation_csv done");
	}

	public override void setCsvData(string[] row)
	{
		csv_row data = new csv_row();
		int formation_id = 0;
		Vector2[] enemy_point = new Vector2[9];
		Vector2[] hero_point = new Vector2[4];
		foreach( KeyValuePair<int, string> item in key_list){
			switch( item.Value )
			{
			case "陣型編號":
				data.id = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				formation_id = data.id;
				break;
			case "適用人數":
				data.num = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
				break;
			default:
				for( int i = 1; i<=9 ; i++ ){
					if( item.Value.Equals( "座標"+i+"x" ) ){
						enemy_point[i-1].x = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
					} else if ( item.Value.Equals( "座標"+i+"y" ) ){
						enemy_point[i-1].y = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
					}
				}
				for( int i = 1; i<=4 ; i++ ){
					if( item.Value.Equals( i+"p出場X" ) ){
						hero_point[i-1].x = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
					} else if ( item.Value.Equals( i+"p出場Y" ) ){
						if( item.Key >= row.Length ){
							hero_point[i-1].y = 0;
						} else {
							hero_point[i-1].y = string.IsNullOrEmpty(row[item.Key])? 0 : int.Parse(row[item.Key]);
						}
					}
				}
				break;
			}
		}
		data.enemy_point = enemy_point;
		data.hero_point = hero_point;
		csv_table.Add (formation_id, data);
	}

}
