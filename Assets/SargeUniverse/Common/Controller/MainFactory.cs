using UnityEngine;
using Zenject;

namespace SargeUniverse.Common.Controller
{
    public class MainFactory<T, V> : IFactory<T, V> where T : Component {
        protected readonly DiContainer _container;
		
        public MainFactory(DiContainer container) {
            _container = container;
        }
		
        public V Create(T param) {
            return _container.InstantiatePrefabForComponent<V>(param);
        }
    }
}