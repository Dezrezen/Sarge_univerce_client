using UnityEngine;
using UnityEngine.UI;

namespace UI.Base
{
    public class UnitCard : MonoBehaviour
    {
        [SerializeField] protected Image cardImage;

        public void SetCardImage(Sprite sprite)
        {
            cardImage.sprite = sprite;
        }
    }
}