using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI.Elements
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class SwitchButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text = null;
        
        [Header("Settings")]
        [SerializeField] private Sprite _onSprite = null;
        [SerializeField] private Sprite _offSprite = null;
        [SerializeField] private string _onText = string.Empty;
        [SerializeField] private string _offText = string.Empty;

        private Image _image;
        private Button _button;
        
        private bool _state = false;
        
        [HideInInspector] public UnityEvent<bool> onSwitchState = new();

        private void Awake()
        {
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(() => SetState(!_state, true));
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void SetState(bool state, bool invokeCallback = false)
        {
            _state = state;
            _image.sprite = _state ? _onSprite : _offSprite;
            if (_text != null)
            {
                _text.text = _state ? _onText : _offText;
            }

            if (invokeCallback)
            {
                onSwitchState?.Invoke(_state);
            }
        }
    }
}