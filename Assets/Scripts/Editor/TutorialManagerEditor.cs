using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TutorialManager))]
public class TutorialManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_touchHandler"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_tutorialHandPrefab"));

        EditorGUILayout.LabelField("Tutorial Steps Data", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        var tutorialStepsArray = serializedObject.FindProperty("_tutorialStepsData");
        tutorialStepsArray.isExpanded = EditorGUILayout.Foldout(tutorialStepsArray.isExpanded, "Tutorial Steps", true);
        EditorGUILayout.EndHorizontal();

        if (tutorialStepsArray.isExpanded)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < tutorialStepsArray.arraySize; i++)
            {
                var tutorialStepDataProperty = tutorialStepsArray.GetArrayElementAtIndex(i);

                var actionTypeProperty = tutorialStepDataProperty.FindPropertyRelative("_actionType");

                var actionType = (TutorialStepActionType)actionTypeProperty.enumValueIndex;

                var boldLabelStyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
                tutorialStepDataProperty.isExpanded = EditorGUILayout.Foldout(tutorialStepDataProperty.isExpanded, new GUIContent($"Step {i + 1}"), true, boldLabelStyle);

                if (!tutorialStepDataProperty.isExpanded) continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.PropertyField(actionTypeProperty);

                if (actionType == TutorialStepActionType.Tap)
                {
                    EditorGUILayout.PropertyField(tutorialStepDataProperty.FindPropertyRelative("_column"));
                }
                else if (actionType == TutorialStepActionType.Merge)
                {
                    DisplayVector2IntPropertyField(tutorialStepDataProperty, "_firstItem");
                    DisplayVector2IntPropertyField(tutorialStepDataProperty, "_secondItem");
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Add")) tutorialStepsArray.arraySize++;
        if (GUILayout.Button("Remove")) tutorialStepsArray.arraySize--;

        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayVector2IntPropertyField(SerializedProperty serializedProperty, string relativePropertyPath)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(TransformString(relativePropertyPath));
        EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative($"{relativePropertyPath}.x"), GUIContent.none);
        EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative($"{relativePropertyPath}.y"), GUIContent.none);
        EditorGUILayout.EndHorizontal();
    }

    private static string TransformString(string input)
    {
        var result = new StringBuilder();
        var length = input.Length;

        for (var i = 1; i < length; i++)
        {
            if (input[i] == '_') continue;
            result.Append(input[i - 1] == '_' ? char.ToUpper(input[i]) : input[i]);
            if (i + 1 < length && char.IsUpper(input[i + 1])) result.Append(" ");
        }

        return result.ToString();
    }
}
