//Stathis Georgiou ©2021
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StaGeGames.SmartUI
{

    [CustomEditor(typeof(SmartManager))]
    public class SmartManagerEditor : Editor
    {
        static SmartManager myTarget;
        static Vector2 screenSize;

        private void OnEnable()
        {
            myTarget = (SmartManager)target;
            if (myTarget == null) return;
            screenSize = GuiUtilities.GetMainGameViewSize();
            EditorApplication.update = CheckSize;
            //CheckSize();
        }

        static void CheckSize() {

            if (myTarget == null) return;

            if (myTarget.checkScreenChanged)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                {
                    // Ensure continuous Update calls.
                    if (!Application.isPlaying)
                    {
                        EditorApplication.QueuePlayerLoopUpdate();
                        SceneView.RepaintAll();
                        if (GuiUtilities.GetMainGameViewSize() != screenSize)
                        {
                            screenSize = GuiUtilities.GetMainGameViewSize();
                            myTarget.InvokeDelayApply();
                            Debug.Log("[SmartManager] Game View changed to " + screenSize.x + " X " + screenSize.y);
                        }
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (myTarget == null) return;

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = GuiUtilities.HexColor("#C2C2C2", Color.gray);

            GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
            TextFieldStyles.richText = true;

            string s = "©<color=cyan>Sta</color><color=red>Ge</color> 2020 - <color=#006DAB>stagegames.eu</color>";
            if (GUILayout.Button(s, TextFieldStyles)) { Application.OpenURL("http://stagegames.eu"); }

            TextFieldStyles.fontSize = 14;
            TextFieldStyles.fontStyle = FontStyle.Bold;
            TextFieldStyles.normal.textColor = Color.yellow;
            TextFieldStyles.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.Space(30);
            myTarget.checkScreenChanged = EditorGUILayout.Toggle("Auto Apply On Screen Size Changed?", myTarget.checkScreenChanged);
            EditorGUILayout.Space();

            GUI.backgroundColor = Color.gray;

            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            if (GUILayout.Button("Apply Resize [ALL]")) { myTarget.ApplyResizeToAll(); }
            EditorGUILayout.Space();
            GUI.color = Color.cyan;
            if (GUILayout.Button("Apply Resize [Active Only]")) { myTarget.ApplyResizeToActive(); }
            EditorGUILayout.Space(30);

        }

    }

}
