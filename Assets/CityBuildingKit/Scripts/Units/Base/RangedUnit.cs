using UnityEngine;

namespace CityBuildingKit.Scripts.Units.Base
{
    public class RangedUnit : Unit
    {
        [SerializeField]
        private ProjectileBase projectile;

        public override void Attack()
        {
            base.Attack();
        }

        private void Shoot()
        {

        }
    }
}
