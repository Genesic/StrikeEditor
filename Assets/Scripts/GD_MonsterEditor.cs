using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GD_MonsterEditor : MonoBehaviour {

	public monster_csv.csv_row monster_info;

	public void init(monster_csv.csv_row monster)
	{
		monster_info = monster;
	}
}
