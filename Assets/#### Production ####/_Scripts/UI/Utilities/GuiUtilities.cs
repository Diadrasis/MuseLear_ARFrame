using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StaGeGames.SmartUI
{

    public class GuiUtilities : MonoBehaviour
    {

        public enum PivotMode
        {
            none,
            Center,
            BottomCenter,
            BottomLeft,
            BottomRight,
            TopCenter,
            TopLeft,
            TopRight,
            LeftCenter,
            RightCenter
        };

        public static void SetPivot(RectTransform rt, PivotMode mode)
        {
            float x = 0.5f, y = 0.5f;

            switch (mode)
            {
                case PivotMode.none:
                    return;
                case PivotMode.Center:
                    x = y = 0.5f;
                    break;
                case PivotMode.BottomCenter:
                    x = 0.5f;
                    y = 0f;
                    break;
                case PivotMode.BottomLeft:
                    x = 0.0f;
                    y = 0f;
                    break;
                case PivotMode.BottomRight:
                    x = 1.0f;
                    y = 0.0f;
                    break;
                case PivotMode.TopCenter:
                    x = 0.5f;
                    y = 1f;
                    break;
                case PivotMode.TopLeft:
                    x = 0.0f;
                    y = 1f;
                    break;
                case PivotMode.TopRight:
                    x = 1f;
                    y = 1f;
                    break;
                case PivotMode.LeftCenter:
                    x = 0f;
                    y = 0.5f;
                    break;
                case PivotMode.RightCenter:
                    x = 1f;
                    y = 0.5f;
                    break;
                default:
                    break;
            }
            
            rt.pivot =  rt.anchorMin =  rt.anchorMax = new Vector2(x, y);
        }

        public static void ForceRebuildLayout(RectTransform rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        /// <summary>
		/// return color from hex string or html
		/// eg. hexValue = #696969
		/// eg. hexValue = yellow
		/// if error then keeps the default color hexValue = def
		/// </summary>
		/// <param name="hexValue"></param>
		/// <param name="def"></param>
		/// red, cyan, blue, darkblue, lightblue, purple, yellow, 
		/// lime, fuchsia, white, silver, grey, black, orange, brown, 
		/// maroon, green, olive, navy, teal, aqua, magenta
		/// <returns></returns>
		public static Color HexColor(string hexValue, Color def)
        {
            if (ColorUtility.TryParseHtmlString(hexValue, out Color newCol))
            {
                return newCol;
            }

            return def;
        }

        public static Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }

    }

}
