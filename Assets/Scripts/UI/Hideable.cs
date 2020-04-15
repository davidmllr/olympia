using UnityEngine;

namespace UI
{
    /// <summary>
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Hideable : MonoBehaviour
    {
        [SerializeField] private bool IsVisibleAsDefault;

        private bool visible;
        private CanvasGroup _canvasGroup => GetComponent<CanvasGroup>();

        /// <summary>
        /// </summary>
        private void Awake()
        {
            if (IsVisibleAsDefault) Show();
            else Hide();
        }

        /// <summary>
        /// </summary>
        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// </summary>
        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
            return visible;
        }
    }
}