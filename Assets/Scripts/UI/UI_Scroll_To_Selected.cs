using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace CFS
{
	public class UI_Scroll_To_Selected: MonoBehaviour
    {
        [SerializeField] private GameObject currentSelected, previousSelected;
        [SerializeField] private RectTransform currentSelectedTransform, contentPanel;
        [SerializeField] private ScrollRect scrollRect;

        private void Update()
        {
            currentSelected = EventSystem.current.currentSelectedGameObject;

            if (currentSelected)
            {
                if (currentSelected != previousSelected)
                {
                    previousSelected = currentSelected;
                    currentSelectedTransform = currentSelected.GetComponent<RectTransform>();
                    SnapTo(currentSelectedTransform);
                }
            }
        }

        private void SnapTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();

            var newPosition = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) -
                                  (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

            newPosition.x = 0;

            contentPanel.anchoredPosition = newPosition;

        }

    }
}