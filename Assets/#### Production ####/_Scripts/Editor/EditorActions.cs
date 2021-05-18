//StaGe Games ©2020
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using StaGeGames.SmartUI;

namespace StaGeGames.EditorTools 
{

	public class EditorActions : MonoBehaviour
	{
        [MenuItem("StaGe Games/UI/Set UI Navigation to None")]
        public static void SetNavigationUI()
        {
            Selectable[] buttons = FindObjectsOfType<Selectable>();
            
            foreach (Selectable b in buttons)
            {
                Navigation nav = b.navigation;
                nav.mode = Navigation.Mode.None;
                b.navigation = nav;
                //Debug.Log(b.name);
            }

            Debug.LogWarning("Action completed for "+ buttons.Length + " ui elements");
        }

        [MenuItem("StaGe Games/UI/Set UI Navigation to Auto")]
        public static void SetNavigationUIauto()
        {
            Selectable[] buttons = FindObjectsOfType<Selectable>();

            foreach (Selectable b in buttons)
            {
                Navigation nav = b.navigation;
                nav.mode = Navigation.Mode.Automatic;
                b.navigation = nav;
                //Debug.Log(b.name);
            }

            Debug.LogWarning("Action completed for " + buttons.Length + " ui elements");
        }

        [MenuItem("StaGe Games/UI/Apply Smart Resize to All")]
        public static void ApplySmartReziseAll()
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();

            foreach (Canvas canv in canvases)
            {

                SmartUI.SmartResize[] smarts = canv.GetComponentsInChildren<SmartUI.SmartResize>(true);

                foreach (SmartUI.SmartResize b in smarts)
                {
                    b.Init();
                }

                Debug.LogWarning("Action completed for " + smarts.Length + " ui smart elements");
            }
        }

        [MenuItem("StaGe Games/UI/Apply Smart Resize to active only")]
        public static void ApplySmartRezise()
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();

            foreach (Canvas canv in canvases)
            {

                SmartUI.SmartResize[] smarts = canv.GetComponentsInChildren<SmartUI.SmartResize>(true);

                foreach (SmartUI.SmartResize b in smarts)
                {
                    b.Init();
                }

                Debug.LogWarning("Action completed for " + smarts.Length + " ui smart elements");
            }
        }

        [MenuItem("StaGe Games/UI/Smart Resize Manager (in scene)")]
        public static void AddSmartReziseManager()
        {
            SmartManager smartManager = FindObjectOfType<SmartManager>();

            if (smartManager != null)
            {
                Selection.activeGameObject = smartManager.gameObject;
                SceneView.FrameLastActiveSceneView();
                return;
            }

            GameObject sm = new GameObject("[SmartManager]");
            smartManager = sm.AddComponent<SmartManager>();
            smartManager.Init();
            Selection.activeGameObject = smartManager.gameObject;
            SceneView.FrameLastActiveSceneView();
        }

    }

}
