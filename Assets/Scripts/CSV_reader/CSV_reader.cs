using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class CSV_reader : MonoBehaviour {
	public string csv_path;
	public int chinese_line = 0;
	public int english_line = 1;
	public Dictionary <int, string> key_list;

	public void setKeylist(string[] keys, string line)
	{
		key_list = new Dictionary<int, string> ();
		string[] key_column = SplitCsvLine (line);
		for (int x = 0 ; x< key_column.Length ; x++){
			foreach( string key_name in keys ){
				if( key_name.Equals( key_column[x], System.StringComparison.Ordinal ) ){
					if( key_list.ContainsKey(x) ){
						Debug.Log ( "x:"+x+" key_name:"+key_name);
						Debug.Log ( "already:"+key_list[x]);
					}
					key_list.Add( x, key_name );
				}
			}
		}
	}

	// splits a CSV row 
	static public string[] SplitCsvLine(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
		@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)", 
		System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
		select m.Groups[1].Value).ToArray();
	}

	public void SplitCsv(string[] keys, string csv_path){

		using (StreamReader reader = new StreamReader(csv_path, System.Text.Encoding.Default)) 
		{
			string line;
			int index = -1;
			while( (line = reader.ReadLine()) != null ){
				index++;

				// 設定中文key
				if( index == chinese_line ){
					setKeylist (keys, line);
					continue;
				}

				// 跳過英文key
				if( index == english_line ){
					continue;
				}

				string[] row = SplitCsvLine(line);
				if ( row.Length > 0 )
					setCsvData(row);
			}
		}
	}

	public virtual void setCsvData (string[] row){
	}
}
