using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace StaGeGames.SmartUI {
    [AddComponentMenu ("UI/#SmartUI/SmartResize")]
    [RequireComponent (typeof (RectTransform))]
    public class SmartResize : MonoBehaviour {

        public RectTransform target, targetParent;
        [HideInInspector]
        [Tooltip ("keep aspect ratio")]
        public bool keepAspectRatio, lockApsectOnPercentResize;
       // [Tooltip ("Keep position x,y as is")]
        //public bool keepRelativePosition;

        [HideInInspector]
        public float widthPercent = 100f, heightPercent = 100f;

        [HideInInspector]
        [Tooltip ("Keeps size ovveriding percentage")]
        public float staticWidth, staticHeight;
        [HideInInspector]
        public int staticMode;

        [HideInInspector]
        public float responsivePercentageScale = 100f;

        [HideInInspector]
        public float initPosX, initPosY, initPosXpercent, initPosYpercent;
        [HideInInspector]
        public bool initPosWithPercentage;

        [HideInInspector]
        public bool isMovable, isVisibleOnStart;

        //the size of canvas during development
        private Vector2 initParentSize;
        private Vector2 textureSize;
        private bool hasLayoutElement;
        private LayoutElement layOutElem;

        [Header ("Pivot point")]
        public GuiUtilities.PivotMode pivotMode = GuiUtilities.PivotMode.LeftCenter;

        [HideInInspector]
        [SerializeField]
        public UnityEvent OnResizeComplete;

        private void Start () { InitAtRuntime(); }

        private void InitAtRuntime(){
            if (!HasParentSmartResize (out float waitTime)) { Init (); } else { Invoke ("Init", waitTime); }
        }

        public void InitWithDelay(){ Invoke("InitAtRuntime", 0.2f); }

        public void Init () {

            //Debug.Log("[SmartResize] Init - staticMode = " + staticMode + " ["+gameObject.name+"]");

            if (target == null) target = GetComponent<RectTransform> ();
            if (targetParent == null)
            {
                if (transform.parent.GetComponent<SmartResize>() != null)
                {
                    targetParent = transform.parent.GetComponent<RectTransform>();
                }
                else
                {
                    targetParent = transform.root.GetComponent<RectTransform>();
                }
            }

            CheckTexture2dSizeChanged();

            //layOutElem = target.gameObject.GetComponent<LayoutElement>();
            //if (layOutElem && !hasLayoutElement) hasLayoutElement = true;

            initParentSize = targetParent.sizeDelta;

            //float fX = initParentSize.x;
            //float fH = initParentSize.y;

            //if (fX <= 0 || fH <= 0 || target.sizeDelta.x <=0 || target.sizeDelta.y <=0)
            //{
            //    Debug.Log(fX + " x " + fH);
            //    return;
            //}
            
            target.sizeDelta = new Vector2(WidthFinal, HeightFinal);

            GuiUtilities.SetPivot (target, pivotMode);

            target.anchoredPosition = Vector3.zero;

            if (!isMovable)
            {
                Vector2 finalPos = target.anchoredPosition;
                if (initPosWithPercentage)
                {
                    finalPos.x = finalPos.x + (targetParent.sizeDelta.x * initPosXpercent) / 100f;
                    finalPos.y = finalPos.y - (targetParent.sizeDelta.y * initPosYpercent) / 100f;
                }
                else
                {
                    finalPos.x = finalPos.x + initPosX;
                    finalPos.y = finalPos.y - initPosY;
                }
                target.anchoredPosition = finalPos;
            }

            GuiUtilities.ForceRebuildLayout (target);

            if (isMovable) {
                SmartMotion transitionClass = target.gameObject.GetComponent<SmartMotion> ();
                if (transitionClass == null) transitionClass = target.gameObject.AddComponent<SmartMotion> ();
                transitionClass.Init (target, pivotMode, isVisibleOnStart);
            }
            
            CancelInvoke();

            if (OnResizeComplete != null) OnResizeComplete?.Invoke();

#if UNITY_EDITOR
            PrepareEventsForEditorUse();
#endif
        }

        #region Width - Height Calculations

        public float WidthFinal
        {
            get
            {
                if (!keepAspectRatio)
                {
                    if (staticMode == 0 && staticWidth > 0)//is static
                    {
                        return staticWidth;
                    }
                    else if (staticMode == 1 && widthPercent >= 1)//is percent
                    {
                        return WidthPercent();
                    }
                    return targetParent.sizeDelta.x;
                }
                return GetResponsivePercentageScaleVector().x;
            }
        }

        public float HeightFinal
        {
            get
            {
                if (!keepAspectRatio)
                {
                    if (staticMode == 0 && staticHeight > 0)//is static
                    {
                        return staticHeight;
                    }
                    else if (staticMode == 1 && heightPercent >= 1)//is percent
                    {
                        return HeightPercent();
                    }
                    return targetParent.sizeDelta.y;
                }
                return GetResponsivePercentageScaleVector().y;
            }
        }

        public float WidthPercent()
        {
            if (lockApsectOnPercentResize) { return (textureSize.x * widthPercent) / 100f; }
            return (targetParent.sizeDelta.x * widthPercent) / 100f;
        }
        public float HeightPercent()
        {
            if (lockApsectOnPercentResize) { return (textureSize.y * heightPercent) / 100f; }
            return (targetParent.sizeDelta.y * heightPercent) / 100f;
        }


        public bool HasImageTexure()
        {
            Image img = target.GetComponent<Image>();
            if (img == null) return false;
            if (img.sprite == null) return false;
            return img.sprite.texture != null;
        }

        private bool CheckTexture2dSizeChanged()
        {
            if (!HasImageTexure()) return false;
            Texture tex = target.GetComponent<Image>().sprite.texture;
            Vector2 texSize = new Vector2((float)tex.width, (float)tex.height);
            if (texSize != textureSize) { textureSize = texSize; return true; }
            return false;
        }

        public void ResponsiveScaleWidth()
        {
            if (!HasImageTexure()) return;
            CheckTexture2dSizeChanged();
            float aspectRatio = textureSize.x / textureSize.y;
            initParentSize = textureSize;
            heightPercent = widthPercent * aspectRatio;
            //Debug.Log("ResponsiveScaleWidth " + aspectRatio);
        }

        public void ResponsiveScaleHeight()
        {
            if (!HasImageTexure()) return;
            CheckTexture2dSizeChanged();
            float aspectRatio = textureSize.x / textureSize.y;
            initParentSize = textureSize;
            widthPercent = heightPercent * aspectRatio;
            //Debug.Log("ResponsiveScaleHeight " + aspectRatio);
        }

        public void ResponsivePercentageScale()
        {
            //Debug.Log("ResponsivePercentageScale");
            initParentSize = targetParent.sizeDelta;
            float x = (initParentSize.x * responsivePercentageScale) / 100f;
            float y = (initParentSize.y * responsivePercentageScale) / 100f;
            target.sizeDelta = new Vector2(x, y);
        }

        Vector2 GetResponsivePercentageScaleVector()
        {
            //Debug.Log("GetResponsivePercentageScaleVector");
            initParentSize = targetParent.sizeDelta;
            float x = (initParentSize.x * responsivePercentageScale) / 100f;
            float y = (initParentSize.y * responsivePercentageScale) / 100f;
            return new Vector2(x, y);
        }

        #endregion
        private void Reset () {
            if (!GetComponent<CanvasRenderer> ()) {
                Debug.LogWarning ("Can't add \"SmartResize\" to non-UI objects");
                DestroyImmediate (this);
                return;
            }
            if (gameObject.GetComponents<SmartResize> ().Length > 1) {
                Debug.LogWarning ("Can't add more than one \"SmartResize\" to " + gameObject.name);
                DestroyImmediate (this);
            }
        }

        [ContextMenu("Find All Parents")]
        void fff()
        {
            if(HasParentSmartResize(out float p)) { Debug.Log(p); }
        }
        private bool HasParentSmartResize (out float val) 
        {
            val = 0f;
            //if (targetParent == null || targetParent.GetComponent<SmartResize>() == null) return false;
            int p = 0;
            Transform tp = target;
            for (int i = 0; i < 50; i++)
            {
                //if (tp == null) i++;
                tp = tp.parent;
                if (tp == null) break;
                if (tp.GetComponent<SmartResize>() != null) { p++; }
            }
            if (p <= 0) return false;
           // Debug.Log("has " + p + " parents");
            val = p * 0.1f; 
            return true;

            #region old code

            //if (targetParent) {
            //    int p = 0;
            //    if (targetParent.GetComponent<SmartResize> () != null) {
            //        p++;
            //        Transform tp = targetParent.parent;
            //        if (tp) {
            //            if (tp.GetComponent<SmartResize> () != null) { p++; }

            //            tp = targetParent.parent.parent;
            //            if (tp) {
            //                if (tp.GetComponent<SmartResize> () != null) { p++; }
            //                tp = targetParent.parent.parent.parent;
            //                if (tp) {
            //                    if (tp.GetComponent<SmartResize> () != null) { p++; }
            //                    tp = targetParent.parent.parent.parent.parent;
            //                    if (tp) {
            //                        if (tp.GetComponent<SmartResize> () != null) { p++; }
            //                        tp = targetParent.parent.parent.parent.parent.parent;
            //                        if (tp) {
            //                            if (tp.GetComponent<SmartResize> () != null) { p++; }
            //                            tp = targetParent.parent.parent.parent.parent.parent.parent;
            //                            if (tp) {
            //                                if (tp.GetComponent<SmartResize> () != null) { p++; }
            //                                tp = targetParent.parent.parent.parent.parent.parent.parent.parent;
            //                                if (tp) {
            //                                    if (tp.GetComponent<SmartResize> () != null) { p++; }
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        val = p * 0.1f;
            //        return true;
            //    }
            //}


            //return false;

            #endregion
        }


        private void PrepareEventsForEditorUse()
        {
            if (OnResizeComplete != null && OnResizeComplete.GetPersistentEventCount() > 0) OnResizeComplete.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
        }

    }

}