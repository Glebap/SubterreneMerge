using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoostersManager))]
public class BoostersManagerEditor : Editor
{
	private SerializedProperty _boostersProperty;
	private SerializedProperty _buttonsProperty;
	private SerializedProperty _tutorialManagerProperty;

	private void OnEnable()
	{
		_boostersProperty = serializedObject.FindProperty("_boosters");
		_buttonsProperty = serializedObject.FindProperty("_buttons");
		_tutorialManagerProperty = serializedObject.FindProperty("_tutorialManager");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
		EditorGUI.EndDisabledGroup();
		
		EditorGUILayout.PropertyField(_tutorialManagerProperty);

		EditorGUILayout.Space();
		
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField("Boosters", GUILayout.Width(135));
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField("Buttons", GUILayout.Width(162));

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		for (int i = 0; i < _boostersProperty.arraySize; i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(_boostersProperty.GetArrayElementAtIndex(i), GUIContent.none);
			EditorGUILayout.PropertyField(_buttonsProperty.GetArrayElementAtIndex(i), GUIContent.none);
			if (GUILayout.Button("-", GUILayout.Width(20)))
			{
				_boostersProperty.DeleteArrayElementAtIndex(i);
				_buttonsProperty.DeleteArrayElementAtIndex(i);
			}
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add Booster/Button"))
		{
			_boostersProperty.InsertArrayElementAtIndex(_boostersProperty.arraySize);
			_buttonsProperty.InsertArrayElementAtIndex(_buttonsProperty.arraySize);
		}
		EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
	}
}