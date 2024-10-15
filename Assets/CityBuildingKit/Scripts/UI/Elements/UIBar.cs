using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class UIBar : MonoBehaviour
    {
        public Image bar = null;
        public RectTransform rect = null;
        public TMP_Text[] texts = null;
        
        [SerializeField] private float height = 0.04f;
        [SerializeField] private float aspect = 2.5f;

        private Vector2 size = Vector2.one;

        private void Awake()
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
        }

        private void Start()
        {
            if (rect != null)
            {
                size = new Vector2(Screen.height * height * aspect, Screen.height * height);
                rect.sizeDelta = size * SargeUniverse.Scripts.Camera.CameraController.zoomScale;
            }
        }

        private void Update()
        {
            if (rect != null)
            {
                rect.sizeDelta = size / (SargeUniverse.Scripts.Camera.CameraController.zoomScale * 3f);
            }
        }
    }
}