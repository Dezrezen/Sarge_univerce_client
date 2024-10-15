using UnityEngine;
using Zenject;

namespace SargeUniverse.Common.View
{
    public class UnitView : MonoBehaviour, IUnitView
    {
        public Transform UnitTransform { get; private set; }

        [Inject]
        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
            Destroy(gameObject);
        }

        private void Awake()
        {
            UnitTransform = transform;
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        public Vector3 Position => UnitTransform == null ? Vector3.zero : UnitTransform.position;

        public virtual void SetPosition(Vector3 position, bool local = false)
        {
            if (local) transform.localPosition = position;
            else transform.position = position;
        }

        public virtual void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public virtual void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public T GetGenericView<T>() where T : UnitView
        {
            return this as T;
        }

        public class Factory : PlaceholderFactory<UnitView, IUnitView>
        {
        }
    }
}