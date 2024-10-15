using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class ConstructionProcess : MonoBehaviour
    {
        [SerializeField] private GameObject _craneSprite = null;

        /*private void Awake()
        {
            _craneSprite.SetActive(false);
            gameObject.SetActive(false);
        }*/

        public void StartBuildProcess()
        {
            _craneSprite.SetActive(true);
            gameObject.SetActive(true);
        }

        public void StartUpgradeProcess()
        {
            _craneSprite.SetActive(false);
            gameObject.SetActive(true);
        }

        public void StopProcess()
        {
            gameObject.SetActive(false);
        }
    }
}