using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaGeGames.SmartUI
{

    public class CommonUtilities : MonoBehaviour
    {
        public static bool IsEditor()
        {
            return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor;
        }
    }

}
