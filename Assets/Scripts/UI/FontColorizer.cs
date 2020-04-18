using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// This class is used to colorize text used in the Text UI component.
    /// </summary>
    public class FontColorizer : MonoBehaviour
    {
        private Text _text => GetComponent<Text>();

        /// <summary>
        /// Every frame, the color of the text is changed.
        /// </summary>
        private void Update()
        {
            _text.color = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );
        }
    }
}