//Stathis Georgiou ©2021
using StaGeGames.SmartUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Viones.TekongPulau {

	public class ScreenOrientationChecker : MonoBehaviour
	{
		
		public enum OrientMode { NULL, LANDSCAPE, PORTRAIT }
		public OrientMode orientMode = OrientMode.NULL;
		public OrientMode orientModeNow = OrientMode.NULL;
		[Space]
		public bool allowSmartResizesToInit;

		bool isEditor;

		[SerializeField]
		List<SmartResize> smartResises = new List<SmartResize>();
		[SerializeField]
		private int width, height;

		public UnityEvent OnrOrientationHasChanged;

		private void Awake()
        {
			Canvas[] canvas = FindObjectsOfType<Canvas>();

			foreach(Canvas v in canvas)
            {
				smartResises.AddRange(v.GetComponentsInChildren<SmartResize>(true));
            }

			isEditor = Application.platform == RuntimePlatform.WindowsEditor;
			
			//if (isEditor) { GetEditorOrientation(); } else { GetDeviceOrientation(); }
		}

        private void Start()
        {
#if UNITY_EDITOR
			GetEditorOrientation();
#else
			GetDeviceOrientation(); 
#endif
			ApplyNewOrientation();
		}

        void Update()
		{
#if UNITY_EDITOR
			if (isEditor)
            {
                if (EditorOrientationHasChanged())
                {
					Debug.Log("dfjndsigfsdihnsidl");
					GetEditorOrientation();
					//ListenerManager.OnScreenRotationOccured?.Invoke();
					if (allowSmartResizesToInit)
					{
						//ApplyNewOrientation();
						Invoke("ApplyNewOrientation", 0.5f);
					}
				}
				//Debug.Log("no orientation occured");
				return;
            }
#else

			if(OrientationHasChanged())
            {
				GetDeviceOrientation();
				//ListenerManager.OnScreenRoationOccured?.Invoke();
				if (allowSmartResizesToInit)
				{
					//ApplyNewOrientation();
					Invoke("ApplyNewOrientation", 0.5f);
				}
			}
#endif

		}

		void ApplyNewOrientation()
        {
			if (isEditor) Debug.LogWarning("ApplyNewOrientation");

			foreach (SmartResize sm in smartResises) { sm.Init(); }

			if (OnrOrientationHasChanged != null) OnrOrientationHasChanged?.Invoke();

			CancelInvoke();
        }

		bool OrientationHasChanged()
        {
			if ((width == Screen.width && height == Screen.height)) return false;

			//       switch (orientModeNow)
			//       {
			//           case OrientMode.NULL:
			//orientMode = OrientMode.NULL;
			//return true;
			//           case OrientMode.LANDSCAPE:
			//return Screen.orientation != ScreenOrientation.Landscape;
			//           case OrientMode.PORTRAIT:
			//return Screen.orientation != ScreenOrientation.Portrait;
			//           default:
			//return false;
			//       }

			return true;
        }

        void GetDeviceOrientation()
		{
			width = Screen.width;
			height = Screen.height;
			if (Screen.orientation == ScreenOrientation.Landscape)
			{
				orientMode = OrientMode.LANDSCAPE;
				orientModeNow = OrientMode.LANDSCAPE;
			}
			else if (Screen.orientation == ScreenOrientation.Portrait)
			{
				orientMode = OrientMode.PORTRAIT;
				orientModeNow = OrientMode.PORTRAIT;
			}
			else
			{
				orientMode = OrientMode.LANDSCAPE;
				orientModeNow = OrientMode.LANDSCAPE;
			}
		}


#region Editor

#if UNITY_EDITOR

		bool EditorOrientationHasChanged()
		{
			Vector2 screensize = GetMainGameViewSize(); // Debug.Log(screensize);
			if ((width == (int)screensize.x && height == (int)screensize.y)) { return false; }
			return true;
		}

		void GetEditorOrientation()
		{
			Vector2 screensize = GetMainGameViewSize();
			width = (int)screensize.x;
			height = (int)screensize.y;

			if (Screen.width > Screen.height)
			{
				orientMode = OrientMode.LANDSCAPE;
				orientModeNow = OrientMode.LANDSCAPE;
			}
			else
			{
				orientMode = OrientMode.PORTRAIT;
				orientModeNow = OrientMode.PORTRAIT;
			}
		}

		Vector2 GetMainGameViewSize()
		{
			System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
			System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
			return (Vector2)Res;
		}

#endif

#endregion

	}

}
