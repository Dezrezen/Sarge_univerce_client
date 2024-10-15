using EasyUI.Tabs;
using UnityEngine ;
using UnityEngine.UI ;

public class TabButtonUI : MonoBehaviour {
   
   [SerializeField]
   private int normalHeight = 70;
   
   [SerializeField]
   private int maxHeight = 170;

   [SerializeField] private TabsUI tabsController;
   [SerializeField] private bool isScaleVertically = false;
   [SerializeField] private Graphic[] showHideGraphicList;
   
   public Button uiButton ;
   public Image uiImage ;
   public LayoutElement uiLayoutElement;

   private int _tabIndex;
   
   void Awake()
   {
      tabsController.OnTabChange.AddListener(OnActive);
   }

   private void OnActive(int index = 0)
   {
      if (isScaleVertically)
      {
         ScaleVertically(index);   
      }

      if (showHideGraphicList.Length > 0)
      {
         ShowHide(index);
      }
   }

   private void ShowHide(int index)
   {
      foreach (var graphic in showHideGraphicList)
      {
         graphic.enabled = IsActive(index);
      }
   }
   
   private void ScaleVertically(int index)
   {
      if (uiButton)
      {
         if (IsActive(index))
         {
            uiLayoutElement.preferredHeight = maxHeight;
         }
         else
         {
            uiLayoutElement.preferredHeight = normalHeight;
            
         }
      }
   }
   private bool IsActive(int index)
   {
      return _tabIndex == index;
   }
   
   public void SetTabIndex(int index)
   {
      _tabIndex = index;
   }
}
