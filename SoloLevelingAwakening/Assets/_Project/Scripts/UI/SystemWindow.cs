using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SLA.UI
{
    /// <summary>
    /// One world-space System window. Pair it with XRI's Lazy Follow so it trails the head,
    /// never locked to it. Text must stay 18 px or larger for combat reading.
    /// </summary>
    public class SystemWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _body;
        [SerializeField] private Graphic _border;
        [SerializeField] private Color _infoColor = new Color(0.247f, 0.722f, 1f);    // #3FB8FF
        [SerializeField] private Color _rewardColor = new Color(1f, 0.788f, 0.290f);  // #FFC94A
        [SerializeField] private Color _warningColor = new Color(0.898f, 0.220f, 0.231f); // #E5383B

        private float _hideAt = -1f;

        public void Show(in SystemMessage message)
        {
            if (_title != null) _title.text = message.Title;
            if (_body != null) _body.text = message.Body;
            if (_border != null) _border.color = ColorFor(message.Kind);

            gameObject.SetActive(true);
            _hideAt = message.Seconds > 0f ? Time.unscaledTime + message.Seconds : -1f;
        }

        public void Hide()
        {
            _hideAt = -1f;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_hideAt > 0f && Time.unscaledTime >= _hideAt)
            {
                Hide();
            }
        }

        private Color ColorFor(SystemMessageKind kind)
        {
            switch (kind)
            {
                case SystemMessageKind.Reward: return _rewardColor;
                case SystemMessageKind.Warning: return _warningColor;
                default: return _infoColor;
            }
        }
    }
}
