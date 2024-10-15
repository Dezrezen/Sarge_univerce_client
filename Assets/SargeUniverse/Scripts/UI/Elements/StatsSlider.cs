using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI.Elements
{
    public class StatsSlider : MonoBehaviour
    {
        [SerializeField] private Slider _barSlider;
        [SerializeField] private TMP_Text _barSliderValueText;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetValues(int baseValue, int difValue = 0)
        {
            _barSlider.value = 1f * baseValue / (baseValue + difValue);
            _barSliderValueText.text = baseValue.ToString();
            if (difValue > 0)
            {
                _barSliderValueText.text += "+" + difValue;
            }
        }
        
        public void SetValues(float baseValue, float difValue = 0)
        {
            _barSlider.value = (baseValue + difValue) / baseValue;
            _barSliderValueText.text = baseValue.ToString(CultureInfo.InvariantCulture);
            if (difValue > 0)
            {
                _barSliderValueText.text += "+" + difValue.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}