using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class AttackRange : MonoBehaviour
    {
        [SerializeField] private float _widthDelta = 0.84f;
        [SerializeField] private float _heightDelta = 0.81f;
        
        private Vector3 _startScale = Vector3.one;
        
        private void Awake()
        {
            _startScale = transform.localScale;
        }

        public void SetRangeSize(float radius)
        {
            transform.localScale = _startScale + new Vector3(radius * _widthDelta, radius * _heightDelta, 0);
        }
    }
}