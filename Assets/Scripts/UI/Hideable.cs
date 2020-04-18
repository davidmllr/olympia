using UnityEngine;

namespace UI
{
    /// <summary>
    /// This class is a helper class for UI elements, which makes it easy to hide and show elements.
    /// It uses a CanvasGroup to avoid using SetActive.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Hideable : MonoBehaviour
    {
        [SerializeField] private bool IsVisibleAsDefault;

        private bool visible;
        private CanvasGroup _canvasGroup => GetComponent<CanvasGroup>();

        /// <summary>
        /// If element is visible as default, show it, otherwise hide it.
        /// </summary>
        private void Awake()
        {
            if (IsVisibleAsDefault) Show();
            else Hide();
        }

        /// <summary>
        /// Show the element.
        /// </summary>
        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Hide the element.
        /// </summary>
        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Checks if the associated element is visible.
        /// </summary>
        /// <returns>If the element is visible</returns>
        public bool IsVisible()
        {
            return visible;
        }
    }
}