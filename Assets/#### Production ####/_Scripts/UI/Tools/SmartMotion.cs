using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace StaGeGames.SmartUI
{
    [AddComponentMenu("UI/#SmartUI/SmartMotion")]
    [RequireComponent(typeof(SmartResize))]
    public class SmartMotion : MonoBehaviour
    {
        [Header("Optional buttons")]
        [Tooltip("Button to show panel. (optional)")]
        public Button ShowButton;
        public Button[] showButtons;

        [Space]
        [Tooltip("Button to hide panel. (optional)")]
        public Button HideButton;
        public Button[] hideButtons;

        [Space]
        [Tooltip("Button to show or hide panel. (optional)")]
        public Button ToggleVisibilityButton;

        [Header("Pivot point")]
        public GuiUtilities.PivotMode sideMode = GuiUtilities.PivotMode.none;
        [Header("Target RectTransform To Move - If empty then nothing happens.")]
        public RectTransform targetRect;
        
        [Header("True: Calculate Speed - False: Use Custom move speed.")]//, order =1)]
        //[Header("False: Use moveSpeedCustom.", order = 2)]
        public bool isAutoSpeed = true;
        public float autoSpeedMultiplier = 1f;

        [HideInInspector]
        [Header("Custom Move Speed")]
        public float moveSpeedCustom = 250f;

        [Header("Custom Space StartPos")]
        public float spaceStartPos = 0f;
        [Header("Custom Space Corner Up or Down Pos")]
        public float spaceUpDown = 0f;

        Vector2 panelInitPosition, panelHiddenPosition;

        [Space]
        public bool isVisible;
        [Space]
        public bool showHideWithAlphaOnly;
        [Space]
        public bool IsAutoHideOn;
        public float hideTime = 5f;

        private bool initCompleted;

        [HideInInspector]
        [SerializeField]
        public UnityEvent OnHideStart, OnHideComplete, OnShowStart, OnShowComplete;

#if UNITY_EDITOR
        [HideInInspector]
        public bool isInEditorAlive;
#endif

        private void Awake()
        {
            if (ShowButton) ShowButton.onClick.AddListener(ShowPanel);
            if (showButtons.Length > 0) { foreach(Button btn in showButtons) btn.onClick.AddListener(ShowPanel); }
            if (HideButton) HideButton.onClick.AddListener(HidePanel);
            if (hideButtons.Length > 0) { foreach (Button btn in hideButtons) btn.onClick.AddListener(HidePanel); }
            if (ToggleVisibilityButton) ToggleVisibilityButton.onClick.AddListener(TogglePanelAppearance);
        }

        /// <summary>
        /// Init panel
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="mode"></param>
        /// <param name="isVisibleAtStart"></param>
        public void Init(RectTransform rect, GuiUtilities.PivotMode mode, bool isVisibleAtStart)
        {
            if (targetRect == null) {
                if (transform.parent)
                {
                    targetRect = transform.GetComponent<RectTransform>();
                }
                else
                {
                    Debug.LogError("Missing target rect !");
                    return;
                }
            }

            panelInitPosition = targetRect.anchoredPosition;

            targetRect = rect;
            sideMode = mode;
            panelHiddenPosition = HidePosition();
            CalculateSpeed();

            isVisible = isVisibleAtStart;

            if (!isVisibleAtStart)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                {
                    //HidePanel();
                    targetRect.anchoredPosition = panelHiddenPosition;
                }
                else
                {
                    targetRect.anchoredPosition = panelHiddenPosition;
                }

                if (OnHideStart != null) OnHideStart?.Invoke();
            }

            initCompleted = true;

#if UNITY_EDITOR
            PrepareEventsForEditorUse();
#endif
        }

        /// <summary>
        /// Show-Hide Panel
        /// </summary>
        public void TogglePanelAppearance()
        {
            CancelInvoke();
            if(targetRect.anchoredPosition == panelHiddenPosition) { ShowPanel(); }
            else { /*if (transform.parent.parent.gameObject.activeSelf)*/ HidePanel(); }
        }

        /// <summary>
        /// Hide panel
        /// </summary>
        public void HidePanel()
        {
            CancelInvoke();
            if (!InitRequest()) return;
            if (!gameObject.activeInHierarchy)
            {
                if (CommonUtilities.IsEditor()) Debug.LogWarning("Panel is inactive!");
                return;
            }
            if (showHideWithAlphaOnly) {
                if (OnHideStart != null) OnHideStart?.Invoke();
                targetRect.anchoredPosition = panelHiddenPosition;
                isVisible = false;
                if (OnHideComplete != null) OnHideComplete?.Invoke();
            }
            else
            {
                StartCoroutine(HideLerpPanel());
            }
        }
        
        IEnumerator HideLerpPanel()
        {
            if (OnHideStart != null) OnHideStart?.Invoke();
            yield return new WaitForEndOfFrame();
            while (Vector2.Distance(targetRect.anchoredPosition, panelHiddenPosition) > 10f)
            {
                targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, panelHiddenPosition, Time.smoothDeltaTime * moveSpeedCustom);
                yield return null;
            }
            targetRect.anchoredPosition = panelHiddenPosition;
            isVisible = false;
            if (OnHideComplete != null) OnHideComplete?.Invoke();
            yield break;
        }

        /// <summary>
        /// show panel
        /// </summary>
        public void ShowPanel()
        {
            if (!InitRequest()) return;
            if (!gameObject.activeInHierarchy)
            {
                if (CommonUtilities.IsEditor()) Debug.LogWarning("Panel is inactive!");
                return;
            }

            if (IsAutoHideOn)
            {
                CancelInvoke();
                Invoke("HidePanel", hideTime);
            }

            if (showHideWithAlphaOnly)
            {
                if (OnShowStart != null) OnShowStart?.Invoke();
                targetRect.anchoredPosition = panelInitPosition;
                isVisible = true;
                if (OnShowComplete != null) OnShowComplete?.Invoke();
            }
            else
            {
                StartCoroutine(ShowLerpPanel());
            }
        }

        public void ResetAutoHideTimeIfUserInteractsWithPanel()
        {
            if (!isVisible) return;
            if (IsAutoHideOn)
            {
                CancelInvoke("HidePanel");
                Invoke("HidePanel", hideTime);
            }
        }

        IEnumerator ShowLerpPanel()
        {
            if(OnShowStart!=null) OnShowStart?.Invoke();

            yield return new WaitForEndOfFrame();
            while (Vector2.Distance(targetRect.anchoredPosition, panelInitPosition) > 10f)
            {
                targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, panelInitPosition, Time.smoothDeltaTime * moveSpeedCustom);
                yield return null;
            }
            targetRect.anchoredPosition = panelInitPosition;
            isVisible = true;
            if (OnShowComplete != null) OnShowComplete?.Invoke();
            yield break;
        }


        /// <summary>
        /// https://gamedev.stackexchange.com/questions/157642/moving-a-2d-object-along-circular-arc-between-two-points
        /// </summary>
        /// <param name="mover"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="radius"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        IEnumerator FollowArc(Transform mover, Vector2 start, Vector2 end,
            float radius, // Set this to negative if you want to flip the arc.
        float duration)
        {

            Vector2 difference = end - start;
            float span = difference.magnitude;

            // Override the radius if it's too small to bridge the points.
            float absRadius = Mathf.Abs(radius);
            if (span > 2f * absRadius)
                radius = absRadius = span / 2f;

            Vector2 perpendicular = new Vector2(difference.y, -difference.x) / span;
            perpendicular *= Mathf.Sign(radius) * Mathf.Sqrt(radius * radius - span * span / 4f);

            Vector2 center = start + difference / 2f + perpendicular;

            Vector2 toStart = start - center;
            float startAngle = Mathf.Atan2(toStart.y, toStart.x);

            Vector2 toEnd = end - center;
            float endAngle = Mathf.Atan2(toEnd.y, toEnd.x);

            // Choose the smaller of two angles separating the start & end
            float travel = (endAngle - startAngle + 5f * Mathf.PI) % (2f * Mathf.PI) - Mathf.PI;

            float progress = 0f;
            do
            {
                float angle = startAngle + progress * travel;
                mover.position = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * absRadius;
                progress += Time.deltaTime / duration;
                yield return null;
            } while (progress < 1f);

            mover.position = end;
        }

        void CalculateSpeed()
        {
            //auto set speed from width
            if (isAutoSpeed)
            {
                Vector2 pos = panelHiddenPosition;// HidePosition();
                if (pos.x != 0)
                {
                    moveSpeedCustom = Mathf.Abs(3f * pos.x);// + (pos.x / 2f));
                }
                else if (pos.y != 0)
                {
                    moveSpeedCustom = Mathf.Abs(3f * pos.y);// + (pos.y / 2f));
                }

                moveSpeedCustom = moveSpeedCustom * autoSpeedMultiplier;
            }
            else
            {
                if (moveSpeedCustom < 0f) moveSpeedCustom = Mathf.Abs(moveSpeedCustom);
            }
        }


        Vector2 HidePosition()
        {
            Vector2 hidePos = targetRect.anchoredPosition;

            switch (sideMode)
            {
                case GuiUtilities.PivotMode.none:
                    break;
                case GuiUtilities.PivotMode.Center:
                    break;
                case GuiUtilities.PivotMode.BottomCenter:
                    hidePos = new Vector2(0f, -targetRect.rect.height);
                    panelInitPosition.y = panelInitPosition.x + spaceStartPos;
                    break;
                case GuiUtilities.PivotMode.BottomLeft:
                    hidePos = new Vector2(-targetRect.rect.width, spaceUpDown);
                    panelInitPosition.x = panelInitPosition.x + spaceStartPos;
                    panelInitPosition.y = panelInitPosition.y + spaceUpDown;
                    break;
                case GuiUtilities.PivotMode.BottomRight:
                    hidePos = new Vector2(targetRect.rect.width, spaceUpDown);
                    panelInitPosition.x = panelInitPosition.x - spaceStartPos;
                    panelInitPosition.y = panelInitPosition.y + spaceUpDown;
                    break;
                case GuiUtilities.PivotMode.TopCenter:
                    hidePos = new Vector2(0f, targetRect.rect.height);
                    panelInitPosition.y = panelInitPosition.x - spaceStartPos;
                    break;
                case GuiUtilities.PivotMode.TopLeft:
                    hidePos = new Vector2(-targetRect.rect.width, -spaceUpDown);
                    panelInitPosition.x = panelInitPosition.x + spaceStartPos;
                    panelInitPosition.y = panelInitPosition.y - spaceUpDown;
                    break;
                case GuiUtilities.PivotMode.TopRight:
                    hidePos = new Vector2(targetRect.rect.width, -spaceUpDown);
                    panelInitPosition.x = panelInitPosition.x - spaceStartPos;
                    panelInitPosition.y = panelInitPosition.y - spaceUpDown;
                    break;
                case GuiUtilities.PivotMode.LeftCenter:
                    hidePos = new Vector2(-targetRect.rect.width, 0f);
                    panelInitPosition.x = panelInitPosition.x + spaceStartPos;
                    break;
                case GuiUtilities.PivotMode.RightCenter:
                    hidePos = new Vector2(targetRect.rect.width, 0f);
                    panelInitPosition.x = panelInitPosition.x - spaceStartPos;
                    break;
                default:
                    break;
            }

            return hidePos;
        }

        private bool InitRequest()
        {
            if (initCompleted) return true;
            SmartResize autoFit = GetComponent<SmartResize>();
            if (!autoFit) return false;
            autoFit.Init();
            return true;
        }

        private void PrepareEventsForEditorUse()
        {
            if (OnHideComplete != null && OnHideComplete.GetPersistentEventCount() > 0) OnHideComplete.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
            if (OnHideStart != null && OnHideStart.GetPersistentEventCount() > 0) OnHideStart.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
            if (OnShowComplete != null && OnShowComplete.GetPersistentEventCount() > 0) OnShowComplete.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
            if (OnShowStart != null && OnShowStart.GetPersistentEventCount() > 0) OnShowStart.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);
        }

        private void Reset()
        {
            //if (!GetComponent<CanvasRenderer>())
            //{
            //    Debug.LogWarning("Can't add \"SmartMotion\" to non-UI objects");
            //    DestroyImmediate(this);
            //    return;
            //}
            if (gameObject.GetComponents<SmartMotion>().Length > 1)
            {
                Debug.LogWarning("Can't add more than one \"SmartMotion\" to " + gameObject.name);
                DestroyImmediate(this);
            }

        }
    }

}
