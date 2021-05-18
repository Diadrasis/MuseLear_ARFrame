//Stathis Georgiou ©2021
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaGeGames.SmartUI
{
    [ExecuteInEditMode]
    public class SmartManager : MonoBehaviour
    {
        [HideInInspector]
        public List<SmartResize> smartResizesInScene = new List<SmartResize>();
        [HideInInspector]
        public bool checkScreenChanged;

        public void Init()
        {
            Debug.Log("[SmartManager] Init");
        }
        
        public void ApplyResizeToActive()
        {
            //Debug.Log("[SmartManager] ApplyResizeToActive");
            Canvas[] canvases = FindObjectsOfType<Canvas>();

            foreach (Canvas canv in canvases)
            {
                SmartUI.SmartResize[] smarts = canv.GetComponentsInChildren<SmartUI.SmartResize>(false);
                foreach (SmartUI.SmartResize b in smarts) {  b.Init(); }
                //Debug.LogWarning("Action completed for " + smarts.Length + " ui smart elements");
            }
        }

        public void ApplyResizeToAll()
        {
            //Debug.Log("[SmartManager] ApplyResizeToAll");
            Canvas[] canvases = FindObjectsOfType<Canvas>();

            foreach (Canvas canv in canvases)
            {
                SmartUI.SmartResize[] smarts = canv.GetComponentsInChildren<SmartUI.SmartResize>(true);
                foreach (SmartUI.SmartResize b in smarts) { b.Init(); }
                //Debug.LogWarning("Action completed for " + smarts.Length + " ui smart elements");
            }
        }

        public void InvokeDelayApply()
        {
            Invoke("ApplyResizeToAll", 0.15f);
        }
     
    }
}
