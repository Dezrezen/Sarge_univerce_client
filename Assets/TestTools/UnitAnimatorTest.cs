using System;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Units;
using SargeUniverse.Scripts.Environment.Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TestTools
{
    public class UnitAnimatorTest : MonoBehaviour
    {
        private enum State
        {
            Idle, Attack
        }

        [SerializeField] private UnitAnimator _animator;
        [SerializeField] private ProjectileSystem _projectileSystem;
        [SerializeField] private int _maxLevel = 1;
        private int _level = 1;
        [SerializeField] private float _projectileSpeed = 10f;
        private State _state = State.Idle;
        private MovementDirection _direction = MovementDirection.North;


        private void Update()
        {
            if (Keyboard.current[Key.Q].wasPressedThisFrame)
            {
                _direction = MovementDirection.NorthWest;
                _animator.SetMovementDirection(_direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation();
                }
                else
                {
                    _animator.PlayAttackAnimation();
                    SpawnProjectile(_direction);
                }
            }

            if (Keyboard.current[Key.W].wasPressedThisFrame)
            {
                _direction = MovementDirection.North;
                _animator.SetMovementDirection(_direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation();
                }
                else
                {
                    _animator.PlayAttackAnimation();
                    SpawnProjectile(_direction);
                }
            }

            if (Keyboard.current[Key.E].wasPressedThisFrame)
            {
                _direction = MovementDirection.NorthEast;
                _animator.SetMovementDirection(_direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation();
                }
                else
                {
                    _animator.PlayAttackAnimation();
                    SpawnProjectile(_direction);
                }
            }

            if (Keyboard.current[Key.A].wasPressedThisFrame)
            {
                _direction = MovementDirection.West;
                _animator.SetMovementDirection(_direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation();
                }
                else
                {
                    _animator.PlayAttackAnimation();
                    SpawnProjectile(_direction);
                }
            }

            if (Keyboard.current[Key.S].wasPressedThisFrame)
            {
                _state = State.Idle;
                _animator.PlayIdleAnimation();
            }

            if (Keyboard.current[Key.D].wasPressedThisFrame)
            {
                _direction = MovementDirection.East;
                _animator.SetMovementDirection(_direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation();
                }
                else
                {
                    _animator.PlayAttackAnimation();
                    SpawnProjectile(_direction);
                }
            }

            if (Keyboard.current[Key.Z].wasPressedThisFrame)
            {
                _direction = MovementDirection.SouthWest;
                _animator.SetMovementDirection(_direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation();
                }
                else
                {
                    _animator.PlayAttackAnimation();
                    SpawnProjectile(_direction);
                }
            }

            if (Keyboard.current[Key.X].wasPressedThisFrame)
            {
                _direction = MovementDirection.South;
                _animator.SetMovementDirection(_direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation();
                }
                else
                {
                    _animator.PlayAttackAnimation();
                    SpawnProjectile(_direction);
                }
            }

            if (Keyboard.current[Key.C].wasPressedThisFrame)
            {
                _direction = MovementDirection.SouthEast;
                _animator.SetMovementDirection(_direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation();
                }
                else
                {
                    _animator.PlayAttackAnimation();
                    SpawnProjectile(_direction);
                }
            }

            if (Keyboard.current[Key.Space].wasPressedThisFrame)
            {
                _state = State.Attack;
                _animator.PlayAttackAnimation();
                SpawnProjectile(_direction);
            }
        }

        private void SpawnProjectile(MovementDirection direction)
        {
            if (_projectileSystem == null)
            {
                return;
            }

            var targetPosition = direction switch
            {
                MovementDirection.North => new Vector3(0, 5, -1),
                MovementDirection.NorthWest => new Vector3(-4, 4.125f, -1),
                MovementDirection.West => new Vector3(-5, 0.95f, -1),
                MovementDirection.SouthWest => new Vector3(-4, -2.25f, -1),
                MovementDirection.South => new Vector3(0, -4, -1),
                MovementDirection.SouthEast => new Vector3(4, -2.25f, -1),
                MovementDirection.East => new Vector3(5, 0.95f, -1),
                MovementDirection.NorthEast => new Vector3(4, 4.125f, -1),
                _ => new Vector3(0, 0)
            };

            targetPosition += Vector3.up;

            _projectileSystem.SpawnProjectile(_projectileSystem.transform.position + targetPosition, _projectileSpeed, _level);
        }
    }
}