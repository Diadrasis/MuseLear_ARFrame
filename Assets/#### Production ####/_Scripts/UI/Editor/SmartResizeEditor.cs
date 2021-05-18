using UnityEditor;
using UnityEngine;

namespace StaGeGames.SmartUI
{

    [CustomEditor(typeof(SmartResize))]
    public class SmartResizeEditor : Editor
    {
        bool showVariables = true;
        string btnLabel = "Show Script Variables";
        float myStaticW, myStaticH;
        float responsiveScale = 1f;
        
        SmartResize myTarget;

        bool hasNoNeededMotionScript = false;

        private void OnEnable()
        {
            myTarget = (SmartResize)target;
            if (myTarget == null) return;
            myStaticW = myTarget.staticWidth;
            myStaticH = myTarget.staticHeight;

            if (!myTarget.isMovable)
            {
                if (myTarget.GetComponent<SmartMotion>() != null)
                {
                    hasNoNeededMotionScript = true;
                    myTarget.GetComponent<SmartMotion>().isInEditorAlive = false;
                }
            }
            else
            {
                if (myTarget.GetComponent<SmartMotion>() != null)
                {
                    myTarget.GetComponent<SmartMotion>().isInEditorAlive = true;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (myTarget == null) return;
            if (myTarget.target == null) myTarget.target = myTarget.GetComponent<RectTransform>();
            if (myTarget.targetParent == null)
            {
                if (myTarget.transform.parent.GetComponent<SmartResize>() != null)
                {
                    myTarget.targetParent = myTarget.transform.parent.GetComponent<RectTransform>();
                }
                else
                {
                    myTarget.targetParent = myTarget.transform.root.GetComponent<RectTransform>();
                }
            }

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

            GUI.backgroundColor = Color.black;

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                if (Application.isPlaying)
                {
                    GUI.color = Color.yellow;
                    GUILayout.Label("Editing won't be saved while in play mode");
                    //return;
                }
            }

            if (GUILayout.Button(btnLabel, TextFieldStyles))
            {
                showVariables = !showVariables;
                btnLabel = showVariables ? "Hide Script Variables" : "Show Script Variables";
            }

            GUI.backgroundColor = oldColor; GUI.color = Color.white;
            if (showVariables) DrawDefaultInspector();

            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            myTarget.isMovable = EditorGUILayout.Toggle("Is Panel Movable?", myTarget.isMovable);

            if (EditorGUI.EndChangeCheck())
            {
                SmartMotion sm = myTarget.GetComponent<SmartMotion>();
                if (!myTarget.isMovable)
                {
                    if (sm != null)
                    {
                        hasNoNeededMotionScript = true;
                        sm.isInEditorAlive = false;
                    }
                   
                }
                else {
                    if (sm != null) { sm.isInEditorAlive = true; }
                    hasNoNeededMotionScript = false;
                }
            }

            if (hasNoNeededMotionScript)
            {
                GUI.backgroundColor = Color.black; GUI.color = Color.cyan;
                if (GUILayout.Button("Remove Smart Motion?")) { DestroyImmediate(myTarget.GetComponent<SmartMotion>()); hasNoNeededMotionScript = false; }
                GUI.color = Color.white; GUI.backgroundColor = oldColor;
            }

            GUILayout.EndHorizontal();

            if (!myTarget.isMovable)
            {
                EditorGUILayout.Space();
                myTarget.initPosWithPercentage = EditorGUILayout.Toggle("Set initial pos with percentage?", myTarget.initPosWithPercentage);
                EditorGUILayout.Space();
                if (myTarget.initPosWithPercentage)
                {
                    myTarget.initPosXpercent = EditorGUILayout.Slider("Start Pos X", myTarget.initPosXpercent, 0f, 100f);
                    myTarget.initPosYpercent = EditorGUILayout.Slider("Start Pos Y", myTarget.initPosYpercent, 0f, 100f);
                }
                else
                {
                    myTarget.initPosX = EditorGUILayout.Slider("Start Pos X", myTarget.initPosX, -5000f, 5000f);
                    myTarget.initPosY = EditorGUILayout.Slider("Start Pos Y", myTarget.initPosY, -5000f, 5000f);
                }
                
                EditorGUILayout.Space();
            }

            if (myTarget.isMovable) myTarget.isVisibleOnStart = EditorGUILayout.Toggle("Visible on Start?", myTarget.isVisibleOnStart);

            EditorGUILayout.Space();

            #region Resize Setings

            myTarget.keepAspectRatio = EditorGUILayout.Toggle("Preserve Aspect", myTarget.keepAspectRatio);
            if (myTarget.keepAspectRatio)
            {
                EditorGUI.BeginChangeCheck();
                myTarget.responsivePercentageScale = EditorGUILayout.Slider("Percentage Scale", myTarget.responsivePercentageScale, 1f, 200f);
                if (EditorGUI.EndChangeCheck()) {  myTarget.ResponsivePercentageScale(); }
            }
            else
            {
                EditorGUILayout.Space();

                #region Static - Percent TABS

                myTarget.staticMode = GUILayout.Toolbar(myTarget.staticMode, new string[] { "Static", "Percent" });
                
                switch (myTarget.staticMode)
                {
                    case 0:
                        myTarget.staticWidth = EditorGUILayout.FloatField("Width Static", myTarget.staticWidth);
                        myTarget.staticHeight = EditorGUILayout.FloatField("Height Static", myTarget.staticHeight);     
                        break;
                    case 1:

                        EditorGUI.BeginChangeCheck();

                        if (myTarget.HasImageTexure())
                        {
                            myTarget.lockApsectOnPercentResize = EditorGUILayout.Toggle("Lock Aspect", myTarget.lockApsectOnPercentResize);
                        }

                        myTarget.widthPercent = EditorGUILayout.Slider("Width %", myTarget.widthPercent, 1, 100);
                        EditorGUILayout.LabelField("Final Width", myTarget.WidthPercent().ToString());

                        if (EditorGUI.EndChangeCheck()) {
                            if (myTarget.lockApsectOnPercentResize)
                            {
                                myTarget.ResponsiveScaleWidth();
                            }
                            
                        }

                        EditorGUI.BeginChangeCheck();
                        myTarget.heightPercent = EditorGUILayout.Slider("Height %", myTarget.heightPercent, 1, 100);
                        EditorGUILayout.LabelField("Final Height", myTarget.HeightPercent().ToString());
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (myTarget.lockApsectOnPercentResize)
                            {
                                myTarget.ResponsiveScaleHeight();
                            }
                        }
                        break;
                    default:
                        EditorGUI.BeginChangeCheck();

                        if (myTarget.HasImageTexure())
                        {
                            myTarget.lockApsectOnPercentResize = EditorGUILayout.Toggle("Lock Aspect", myTarget.lockApsectOnPercentResize);
                        }

                        myTarget.widthPercent = EditorGUILayout.Slider("Width %", myTarget.widthPercent, 1, 100);
                        EditorGUILayout.LabelField("Final Width", myTarget.WidthPercent().ToString());

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (myTarget.lockApsectOnPercentResize)
                            {
                                myTarget.ResponsiveScaleWidth();
                            }

                        }

                        EditorGUI.BeginChangeCheck();
                        myTarget.heightPercent = EditorGUILayout.Slider("Height %", myTarget.heightPercent, 1, 100);
                        EditorGUILayout.LabelField("Final Height", myTarget.HeightPercent().ToString());
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (myTarget.lockApsectOnPercentResize)
                            {
                                myTarget.ResponsiveScaleHeight();
                            }
                        }
                        break;
                }

                #endregion
            }

            #endregion

            EditorGUILayout.Space();

            GUI.color = Color.cyan;
            if (GUILayout.Button("Set Panel Size")) { myTarget.Init(); }

            GUI.color = oldColor;
            SerializedProperty OnResizeComplete = serializedObject.FindProperty("OnResizeComplete"); // <-- UnityEvent
            EditorGUILayout.PropertyField(OnResizeComplete);

            if (GUI.changed)
            {
                if (myTarget == null) return;
                myTarget.Init();

                serializedObject.ApplyModifiedProperties();
            }

        }
    }

}
