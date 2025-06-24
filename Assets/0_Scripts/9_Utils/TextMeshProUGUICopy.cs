using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Badbarbos
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProUGUICopy : MonoBehaviour, IPointerClickHandler
    {
        private TextMeshProUGUI _text;

        void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            string fullText = _text.text;

            if (fullText.Length > 16) GUIUtility.systemCopyBuffer = fullText.Substring(16);

            else GUIUtility.systemCopyBuffer = string.Empty;
        }
    }
}
