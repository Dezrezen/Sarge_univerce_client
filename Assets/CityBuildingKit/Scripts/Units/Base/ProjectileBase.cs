using Units.Interface;
using UnityEngine;

namespace CityBuildingKit.Scripts.Units.Base
{
    public class ProjectileBase : MonoBehaviour, IProjectile
    {
        [SerializeField]
        protected float ProjectileSpeed;
        [SerializeField]
        protected int Damage;

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        ~ProjectileBase()
        {
            Dispose();
        }
    }
}
