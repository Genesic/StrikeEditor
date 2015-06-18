using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GD_MonsterEditor))]
public class GD_MonsterEditorInspector : GD_EditorBase<GD_MonsterEditor>
{
	public override void OnInspectorGUI ()
	{
		GUIStyle my_style = new GUIStyle();
		GUIStyle title_style = new GUIStyle();
		my_style.fontSize = 13;
		title_style.fontSize = 15;
		EditorGUILayout.LabelField("【基本資料】", title_style);

		EditorGUILayout.SelectableLabel ("編號:" + this.Target.monster_info.id, my_style);
		EditorGUILayout.SelectableLabel ("怪物ID:" + this.Target.monster_info.monster_id, my_style);
		EditorGUILayout.SelectableLabel ("名稱:" + this.Target.monster_info.name, my_style);
		EditorGUILayout.SelectableLabel ("星等:" + this.Target.monster_info.star, my_style);
		EditorGUILayout.SelectableLabel ("難度:" + this.Target.monster_info.hard, my_style);
		EditorGUILayout.SelectableLabel ("HP:" + this.Target.monster_info.hp, my_style);
		EditorGUILayout.SelectableLabel ("盾牌HP:" + this.Target.monster_info.shield_hp, my_style);
		EditorGUILayout.SelectableLabel ("重力場:" + this.Target.monster_info.gravity, my_style);
		EditorGUILayout.SelectableLabel ("本體掉落率:" + this.Target.monster_info.drop_prob, my_style);
		EditorGUILayout.SelectableLabel ("掉落金錢:" + this.Target.monster_info.money, my_style);
		EditorGUILayout.SelectableLabel ("DEF:" + this.Target.monster_info.def, my_style);

		EditorGUILayout.LabelField("【攻擊點】", title_style);

		for (int i=0; i < this.Target.monster_info.atk_point.Length ; i++) {
			EditorGUILayout.LabelField("攻擊點"+(i+1), my_style);
			GUILayout.BeginHorizontal();
			EditorGUILayout.SelectableLabel ("技能ID:" + this.Target.monster_info.atk_point[i].skill_id, my_style, GUILayout.Width(100f));
			EditorGUILayout.SelectableLabel (" 技能名稱:" + this.Target.monster_info.atk_point[i].name, my_style, GUILayout.Width(200f));
			EditorGUILayout.SelectableLabel (" CD:" + this.Target.monster_info.atk_point[i].cd, my_style, GUILayout.Width(50f));
			EditorGUILayout.SelectableLabel (" ATK:" + this.Target.monster_info.atk_point[i].atk, my_style, GUILayout.Width(50f));
			GUILayout.EndHorizontal();
		}
	}
}
