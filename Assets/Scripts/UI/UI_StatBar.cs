using UnityEngine;
using UnityEngine.UI;

namespace CFS
{
    [RequireComponent(typeof(Slider))]
    public class UI_StatBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private RectTransform rectTransform;

        [Header("Bar Options")]
        [SerializeField] protected bool scaleBarLengthWithStats = true;
        [SerializeField] protected float widthScaleMultiplier = 1;

        // SCALE BAR DEPENDING ON STAT
        // SECONDARY BAR TO SHOW ACTION COST ( YELLOW FLASH )

        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
            Debug.LogWarning(slider == null ? "No Slider" : null, gameObject);
        }

        protected virtual void Start()
        {
            slider.value = slider.maxValue;
        }

        public virtual void SetStat(int newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;

            if (scaleBarLengthWithStats)
            {
                rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
                
                // Resets position of the bars
                PlayerUIManager.Instance.playerHudManager.RefreshHUD();
            }
        }

    }
}