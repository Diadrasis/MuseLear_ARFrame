//Stathis Georgiou ©2021
using StaGeGames.SmartUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Viones.TekongPulau {

	public class GridCalculateChildSize : MonoBehaviour
	{

        public RectTransform rectGrid;
		public GridLayoutGroup grid;
        float aspect;
        public SmartResize[] smartResizes;

        private void Awake()
        {
            aspect = 16f / 9f;// grid.cellSize.x / grid.cellSize.y;
           /* if(smartResizes.Length<=0)*/ smartResizes = rectGrid.transform.GetComponentsInChildren<SmartResize>();
        }

        private void OnEnable()
        {
            aspect = 16f / 9f;

            if (grid == null) grid = GetComponent<GridLayoutGroup>();

            if (grid && rectGrid == null) rectGrid = GetComponent<RectTransform>();
        }

        [ContextMenu("Set Text Names")]
        void SetTextNames() {
            //TMPro.TextMeshProUGUI[] txts = rectGrid.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            //foreach (TMPro.TextMeshProUGUI txt in txts) txt.transform.name = txt.transform.parent.name + "_TXT";

            //smartResizes = rectGrid.transform.GetComponentsInChildren<SmartResize>();
        }



        public void OnRectSizeChanged()
        {
            aspect = 16f / 9f;
            Invoke("SetUpChilds", 0.15f);
        }

		void SetUpChilds()
        {
            float width = rectGrid.sizeDelta.x;
            Vector2 space = grid.spacing;
            GridLayoutGroup.Constraint constr = grid.constraint;
            int constrCount = grid.constraintCount;
            switch (constr)
            {
                case GridLayoutGroup.Constraint.Flexible:
                    break;
                case GridLayoutGroup.Constraint.FixedColumnCount://set up cell width
                    //spaces should be 1 less than cells
                    int totalSpaceX = constrCount - 1;
                    float newSpace = totalSpaceX * space.x;
                    //substract space from current width
                    float remainForCells = width - newSpace;
                    //get new cell width size
                    float newCellWidth = remainForCells / constrCount;
                    grid.cellSize = new Vector2(newCellWidth, newCellWidth / aspect);
                    Invoke("InitResizes", 0.15f);
                    //InitResizes();
                    break;
                case GridLayoutGroup.Constraint.FixedRowCount://set up cell height

                    break;
                default:
                    break;
            }
        }


        void InitResizes()
        {
            foreach (SmartResize sm in smartResizes) sm.InitWithDelay();
        }

        void ShowChilds(bool val)
        {
            foreach (SmartResize sm in smartResizes) sm.gameObject.SetActive(val);
        }

    }

}
