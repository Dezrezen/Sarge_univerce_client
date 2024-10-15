using System;

namespace Units.Interface
{
    public interface IUnit : IDisposable
    {
        public void Move();
        public void Attack();
        public void Die();
    }
}
