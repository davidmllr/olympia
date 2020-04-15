using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FontColorizerText : MonoBehaviour
    {
        private Text _text => GetComponent<Text>();

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