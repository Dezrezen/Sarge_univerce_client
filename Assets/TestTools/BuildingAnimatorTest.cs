using System;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Buildings.Animators;
using SargeUniverse.Scripts.Environment.Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TestTools
{
    public class BuildingAnimatorTest : MonoBehaviour
    {
        private enum State
        {
            Idle, Attack
        }
        
        [SerializeField] private BuildingAnimator _animator;
        [SerializeField] private ProjectileSystem _projectileSystem;
        [SerializeField] private int _maxLevel = 1;
        private int _level = 1;
        [SerializeField] private float _projectileSpeed = 10f;
        private State _state = State.Idle;
        private MovementDirection _direction = MovementDirection.North;

        [SerializeField]private Transform _mouseTransform;

        private void Update()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var mousePosOnPlane = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x , mousePos.y, 0));
            _mouseTransform.position = new Vector3(mousePosOnPlane.x, mousePosOnPlane.y, -1);
            
            
            if (Keyboard.current[Key.Q].wasPressedThisFrame)
            {
                _direction = MovementDirection.NorthWest;
                _animator.SetDirection(_level, _direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation(_level);
                }
                else
                {
                    _animator.PlayAttackAnimation(_level, _direction, () => SpawnProjectile(_direction));
                }
            }
            
            if (Keyboard.current[Key.W].wasPressedThisFrame)
            {
                _direction = MovementDirection.North;
                _animator.SetDirection(_level, _direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation(_level);
                }
                else
                {
                    _animator.PlayAttackAnimation(_level, _direction, () => SpawnProjectile(_direction));
                }
            }
            
            if (Keyboard.current[Key.E].wasPressedThisFrame)
            {
                _direction = MovementDirection.NorthEast;
                _animator.SetDirection(_level, _direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation(_level);
                }
                else
                {
                    _animator.PlayAttackAnimation(_level, _direction, () => SpawnProjectile(_direction));
                }
            }
            
            if (Keyboard.current[Key.A].wasPressedThisFrame)
            {
                _direction = MovementDirection.West;
                _animator.SetDirection(_level, _direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation(_level);
                }
                else
                {
                    _animator.PlayAttackAnimation(_level, _direction, () => SpawnProjectile(_direction));
                }
            }
            
            if (Keyboard.current[Key.S].wasPressedThisFrame)
            {
                _state = State.Idle;
                _animator.PlayIdleAnimation(_level);
            }
            
            if (Keyboard.current[Key.D].wasPressedThisFrame)
            {
                _direction = MovementDirection.East;
                _animator.SetDirection(_level, _direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation(_level);
                }
                else
                {
                    _animator.PlayAttackAnimation(_level, _direction, () => SpawnProjectile(_direction));
                }
            }
            
            if (Keyboard.current[Key.Z].wasPressedThisFrame)
            {
                _direction = MovementDirection.SouthWest;
                _animator.SetDirection(_level, _direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation(_level);
                }
                else
                {
                    _animator.PlayAttackAnimation(_level, _direction, () => SpawnProjectile(_direction));
                }
            }
            
            if (Keyboard.current[Key.X].wasPressedThisFrame)
            {
                _direction = MovementDirection.South;
                _animator.SetDirection(_level, _direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation(_level);
                }
                else
                {
                    _animator.PlayAttackAnimation(_level, _direction, () => SpawnProjectile(_direction));
                }
            }
            
            if (Keyboard.current[Key.C].wasPressedThisFrame)
            {
                _direction = MovementDirection.SouthEast;
                _animator.SetDirection(_level, _direction);
                if (_state == State.Idle)
                {
                    _animator.PlayIdleAnimation(_level);
                }
                else
                {
                    _animator.PlayAttackAnimation(_level, _direction, () => SpawnProjectile(_direction));
                }
            }
            
            if (Keyboard.current[Key.Space].wasPressedThisFrame)
            {
                _state = State.Attack;
                _animator.PlayAttackAnimation(_level, () => SpawnProjectile(_direction));
            }
            
            if (Keyboard.current[Key.T].wasPressedThisFrame)
            {
                _state = State.Attack;
                _animator.PlayAttackAnimation(_level, () => SpawnProjectile());
            }
            
            if (Keyboard.current[Key.NumpadPlus].wasPressedThisFrame)
            {
                if (_level < _maxLevel)
                {
                    _level++;
                    if (_state == State.Idle)
                    {
                        _animator.PlayIdleAnimation(_level);
                    }
                    else
                    {
                        _animator.PlayAttackAnimation(_level);
                    }
                }
            }
            
            if (Keyboard.current[Key.NumpadMinus].wasPressedThisFrame)
            {
                if (_level > 1)
                {
                    _level--;
                    if (_state == State.Idle)
                    {
                        _animator.PlayIdleAnimation(_level);
                    }
                    else
                    {
                        _animator.PlayAttackAnimation(_level);
                    }
                }
            }
            
            if (Keyboard.current[Key.Backspace].wasPressedThisFrame)
            {
                _animator.PlayDestroyAnimation();
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

        private void SpawnProjectile()
        {
            _projectileSystem.SpawnProjectile(_mouseTransform, _projectileSpeed, _level);
        }
    }
}