using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class ScaleOverTime : MonoBehaviour
    {
        [SerializeField] private float _startScale = 0.01f;
        [SerializeField] private float _finishScale = 1f;
        [SerializeField] private float _stepScale = 0.002f;
        private float _scale;

        private void Awake()
        {
            _scale = _startScale;
            transform.localScale = Vector3.one * _scale;
        }

        private void Update()
        {
            if (_scale > _finishScale)
            {
                return;
            }

            transform.localScale = Vector3.one * _scale;
            _scale += _stepScale;
        }
    }
}