using UnityEngine;

namespace SargeUniverse.Scripts.UI.Shop
{
    public class UIShopTabPanel : MonoBehaviour, IShopTabPanel
    {
        [SerializeField] protected Transform _content = null;
        protected UIShop _uiShop = null;

        public void Init(UIShop uiShop)
        {
            _uiShop = uiShop;
        }
        
        protected virtual void Start()
        {
            SubscribeForEvents();
            InitCards();
        }

        protected void OnDestroy()
        {
            UnSubscribeFromEvents();
        }

        public virtual void ShopPanel()
        {
            gameObject.SetActive(true);
        }

        public virtual void HidePanel()
        {
            gameObject.SetActive(false);
        }
        
        protected virtual void SubscribeForEvents()
        {

        }

        protected virtual void UnSubscribeFromEvents()
        {
            
        }

        protected virtual void InitCards()
        {
            
        }

        protected virtual void UpdateCards()
        {
            
        }
    }
}