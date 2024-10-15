using SargeUniverse.Scripts.Camera;
using TMPro;
using UnityEngine;

namespace SargeUniverse.Scripts.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIWarBaseLabel : MonoBehaviour
    {
        public TMP_Text labelText = null;
        public RectTransform rect = null;
        
        [SerializeField] private float height = 0.08f;
        [SerializeField] private float aspect = 1f;
        private Vector2 _size = Vector2.one;
        
        private void Awake()
        {
            rect ??= GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
        }
        
        private void Start()
        {
            if (rect != null)
            {
                _size = new Vector2(Screen.height * height * aspect, Screen.height * height);
                rect.sizeDelta = _size * WarMapCamera.zoomScale;
            }
        }
    }
}