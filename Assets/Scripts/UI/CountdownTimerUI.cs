using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CountdownTimerUI : MonoBehaviour
    {
        [SerializeField] private Text _text;

        public void Awake()
        {
            Guard.CheckIsNull(_text, "_text", gameObject);
        }

        public void SetText(string newText)
        {
            _text.text = newText;
        }
    }
}