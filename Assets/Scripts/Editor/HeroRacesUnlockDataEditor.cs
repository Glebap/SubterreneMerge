using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(HeroRacesUnlockData))]
public class HeroRacesUnlockDataEditor : Editor
{
	private SerializedProperty _data;

	private void OnEnable()
	{
		_data = serializedObject.FindProperty("_data");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.LabelField("Heroes Races Unlock Data", EditorStyles.boldLabel);
		
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField("HeroTierList", GUILayout.Width(135));
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField("UnlockLevel", GUILayout.Width(115));

		EditorGUILayout.EndHorizontal();

		for (var index = 0; index < _data.arraySize; index++)
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			EditorGUILayout.BeginHorizontal();

			SerializedProperty heroRaceUnlockData = _data.GetArrayElementAtIndex(index);
			SerializedProperty heroesTierList = heroRaceUnlockData.FindPropertyRelative("_heroesTierList");
			SerializedProperty levelToUnlock = heroRaceUnlockData.FindPropertyRelative("_unlockLevel");

			EditorGUILayout.PropertyField(heroesTierList, GUIContent.none, GUILayout.MinWidth(135));

			levelToUnlock.intValue = Mathf.Max(1, levelToUnlock.intValue);
			EditorGUILayout.PropertyField(levelToUnlock, GUIContent.none, GUILayout.Width(80));
			
			if (GUILayout.Button("-", GUILayout.Width(30)))
			{
				_data.DeleteArrayElementAtIndex(index);
				serializedObject.ApplyModifiedProperties();
				return;
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}

		if (GUILayout.Button("+ Add New Hero Race Unlock Data"))
			_data.arraySize++;

		serializedObject.ApplyModifiedProperties();
	}
}