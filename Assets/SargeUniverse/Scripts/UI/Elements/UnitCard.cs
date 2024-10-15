using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class UnitCard : MonoBehaviour
    {
        [SerializeField] private Image _cardImage = null;
        [SerializeField] private Image _levelCard = null;
        [SerializeField] private TMP_Text _levelText = null;

        public void SetCardImage(Sprite sprite)
        {
            _cardImage.sprite = sprite;
        }

        public void SetLevel(string level)
        {
            _levelText.text = level;
        }
    }
}