using CityBuildingKit.Scripts.Enums;
using Units.Interface;
using UnityEngine;

namespace CityBuildingKit.Scripts.Units.Base
{
    public class Unit : MonoBehaviour, IUnit
    {
        protected int HitPoints;
        protected int DamagePerAttack;
        protected float AttackSpeed; 
        protected float AttackRange;
        protected float MovementSpeed;
        protected DamageType DamageType;
        protected PreferredTarget PreferredTarget;
        protected AttackType AttackType;

        [SerializeField] protected AnimatorController animatorController;
        [SerializeField] protected MovementController movementController;

        public void Move()
        {
            movementController.Move();
        }

        public virtual void Attack()
        {

        }

        public void Die()
        {

        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        ~Unit()
        {
            Dispose();
        }
    }
}