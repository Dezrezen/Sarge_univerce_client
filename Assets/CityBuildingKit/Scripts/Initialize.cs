using Assets.Scripts;
using UnityEngine;

namespace CityBuildingKit.Scripts
{
    public class Initialize : MonoBehaviour
    {
        void Awake()
        {
            if (!FindObjectOfType<Loader>())
            {
                Instantiate(Resources.Load("Prefabs/UI/Loader"));
            }
        }
    
        public void OnPlay()
        {
            Loader.instance.loadScene(Constants.GAME);
        } 
    }
}
