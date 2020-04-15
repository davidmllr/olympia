using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
    public class FontColorizerTextMeshPro : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshPro => GetComponent<TextMeshProUGUI>();

        private void Update()
        {
            _textMeshPro.color = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );
        }
    }
}