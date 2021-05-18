using UnityEngine;
using UnityEditor;

namespace StaGeGames.SmartUI
{

    [CustomEditor(typeof(SmartMotion))]
    public class SmartMotionEditor : Editor
    {
        bool showVariables=true, isPanelHidden=true, showEvents=false;
        string btnLabel = "Show Script Variables", btnEventsLabel = "Show Events";

        public override void OnInspectorGUI()
        {
            SmartMotion myTarget = (SmartMotion)target;

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = GuiUtilities.HexColor("#C2C2C2", Color.gray);

            GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
            TextFieldStyles.richText = true;
           
            string s = "©<color=cyan>Sta</color><color=red>Ge</color> 2020 - <color=#006DAB>stagegames.eu</color>";
            if (GUILayout.Button(s, TextFieldStyles))
            {
                Application.OpenURL("http://stagegames.eu");
            }

            TextFieldStyles.fontSize = 14;
            TextFieldStyles.fontStyle = FontStyle.Bold;
            TextFieldStyles.normal.textColor = Color.yellow;
            TextFieldStyles.alignment = TextAnchor.MiddleCenter;

            GUI.backgroundColor = Color.black;

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                if (!myTarget.isInEditorAlive)
                {
                    GUI.color = Color.yellow;
                    GUILayout.Label("Not editable because SmartResize do not need to move");
                    return;
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button(btnLabel, TextFieldStyles)) {
                showVariables = !showVariables;
                btnLabel = showVariables ? "Hide Script Variables" : "Show Script Variables";
            }
            GUI.backgroundColor = oldColor;
            GUI.color = Color.white;

            if (showVariables) DrawDefaultInspector();

            if (!myTarget.isAutoSpeed) myTarget.moveSpeedCustom = EditorGUILayout.FloatField("Custom Move Speed", myTarget.moveSpeedCustom);

            TextFieldStyles.fontSize = 18;

            if (!myTarget.gameObject.activeInHierarchy)
            {
                //GUI.color = Color.red;
                TextFieldStyles.fontStyle = FontStyle.Bold;
                TextFieldStyles.normal.textColor = Color.red;
                TextFieldStyles.alignment = TextAnchor.MiddleCenter;
                if (GUILayout.Button("Panel is inactive!", TextFieldStyles)) { Debug.LogWarning("Panel is inactive!"); }
            }
            else
            {
                TextFieldStyles.normal.textColor = Color.white;
                TextFieldStyles.hover.textColor = Color.green;
                GUI.color = Color.cyan;
                isPanelHidden = !myTarget.isVisible;
                if (!isPanelHidden) { if (GUILayout.Button("Hide Target", TextFieldStyles)) { myTarget.HidePanel(); isPanelHidden = true; } }
                else { if (GUILayout.Button("Show Target", TextFieldStyles)) { myTarget.ShowPanel(); isPanelHidden = false; } }
            }

            GUI.color = oldColor;

            TextFieldStyles.fontSize = 18;
            TextFieldStyles.fontStyle = FontStyle.Bold;
            TextFieldStyles.normal.textColor = Color.white;
            TextFieldStyles.alignment = TextAnchor.MiddleCenter;

            SerializedProperty OnShowStart = serializedObject.FindProperty("OnShowStart"); // <-- UnityEvent
            SerializedProperty OnShowComplete = serializedObject.FindProperty("OnShowComplete"); // <-- UnityEvent
            SerializedProperty OnHideStart = serializedObject.FindProperty("OnHideStart"); // <-- UnityEvent
            SerializedProperty OnHideComplete = serializedObject.FindProperty("OnHideComplete"); // <-- UnityEvent

            EditorGUILayout.Space();
            if (GUILayout.Button(btnEventsLabel, TextFieldStyles))
            {
                showEvents = !showEvents;
                btnEventsLabel = showEvents ? "Hide Events" : "Show Events";
            }

            if (showEvents)
            {
                EditorGUILayout.PropertyField(OnShowStart);
                EditorGUILayout.PropertyField(OnShowComplete);
                EditorGUILayout.PropertyField(OnHideStart);
                EditorGUILayout.PropertyField(OnHideComplete);
            }

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                // Ensure continuous Update calls.
                if (!Application.isPlaying)
                {
                    EditorApplication.QueuePlayerLoopUpdate();
                    SceneView.RepaintAll();
                }
            }

            if (GUI.changed)
            {
                //if (myTarget.ShowButton) {Debug.Log("Button exist");}else { Debug.Log("Button Null"); }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

}
