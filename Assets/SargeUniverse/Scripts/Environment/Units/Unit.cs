using System;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Buildings;
using SargeUniverse.Scripts.Environment.Projectiles;
using SargeUniverse.Scripts.Map;
using SargeUniverse.Scripts.Sound;
using UI.Elements;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace SargeUniverse.Scripts.Environment.Units
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private UnitAnimator _animator = null;
        [SerializeField] private ProjectileSystem _projectileSystem = null;

        private string _attackAudioTrack = string.Empty;
        private string _movementAudioTrack = string.Empty;

        private MapGrid _grid = null;
        private Data_Unit _unitData = null;
        private UnitsPlacementGrid _placementGrid = null;
        private int _placementIndex = -1;
        
        public UnitID Id => _unitData.id;
        public long DatabaseId => _unitData.databaseID;

        public UIBar HealthBar { get; set; } = null;
        private int _health = 0;
        
        
        private Vector3 _position = Vector3.zero;
        private Vector3 _targetPosition = Vector3.zero;

        private bool _moving = false;

        public Vector3 Offset => Vector3.up * 0.25f;

        private void Awake()
        {
            _animator.SetMovementDirection(GetRandomDirection());
            _animator.PlayIdleAnimation();
        }

        private void OnDestroy()
        {
            if (_placementGrid != null && _placementIndex > 0)
            {
                _placementGrid.FreeCoordinates(_placementIndex);
            }
            
            if (HealthBar && HealthBar != null)
            {
                Destroy(HealthBar.gameObject);
            }
        }

        public void UpdateTargetPosition(bool isMoving, float x, float y)
        {
            _targetPosition = BattlePositionToWorldPosition(x, y);
            
            if (!_moving && isMoving)
            {
                SetLookDirection(_targetPosition);
                _animator.PlayMoveAnimation();
            }
            _moving = isMoving;
        }

        private void Update()
        {
            if (_moving && transform.position != _targetPosition)
            {
                SetLookDirection(_targetPosition);
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _unitData.moveSpeed * Time.deltaTime / 10f);
            }

            var showBar = _health < _unitData?.health && _health > 0;
            if (showBar && HealthBar )
            {
                UpdateHealthBar();
            }
            HealthBar?.gameObject.SetActive(showBar);
        }

        public void SetMapGrid(MapGrid grid)
        {
            _grid = grid;
        }

        public virtual void SetUnitData(Data_Unit unitData)
        {
            _unitData = unitData;
            _health = unitData.health;
        }

        public void SetAudioData(string attackAudioTrack = "", string movementAudioTrack = "")
        {
            _attackAudioTrack = attackAudioTrack;
            _movementAudioTrack = movementAudioTrack;
        }

        public void PlaceOnGrid(UnitsPlacementGrid placementGrid)
        {
            _placementGrid = placementGrid;
            _placementIndex = placementGrid.GetFreeCoordinatesIndex();
            transform.SetParent(placementGrid.GetParent());
            SetPosition();
        }

        public void PlaceOnGrid(float x, float y)
        {
            _targetPosition = transform.position = BattlePositionToWorldPosition(x, y);
        }

        public void Attack(Transform target, Vector3 offset)
        {
            _moving = false;
            transform.position = _targetPosition;
            
            SetLookDirection(target.position + offset);
            
            Attack();
            if (_projectileSystem != null)
            {
                SpawnProjectile(target, offset);
            }
        }

        private void SpawnProjectile(Transform target, Vector3 offset)
        {
            _projectileSystem.SpawnProjectile(target, _unitData.rangedSpeed, _unitData.level);
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                _health = 0;
                
            }
        }

        public void Idle()
        {
            _animator.PlayIdleAnimation();
        }

        public void Die()
        {
            HealthBar?.gameObject.SetActive(false);
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        
        private void SetPosition()
        {
            transform.localPosition = _placementGrid.GetCoordinates(_placementIndex);
        }

        private void Attack()
        {
            if (_unitData.id == UnitID.helicopter)
            {
                _animator.PlayCombinedAttackAnimation();
            }
            else
            {
                _animator.PlayAttackAnimation();
                if (!string.IsNullOrEmpty(_attackAudioTrack))
                {
                    SoundSystem.Instance.PlaySound(_attackAudioTrack);
                }
            }
        }
        
        private MovementDirection GetRandomDirection()
        {
            return (MovementDirection)Random.Range(0, Enum.GetValues(typeof(MovementDirection)).Length);
        }

        private void SetLookDirection(Vector3 targetPosition)
        {
            var lookDirection = MovementDirection.South;
            var direction = targetPosition - transform.position;
            
            switch (direction.y)
            {
                case > 0 when direction.y >= 3 * Mathf.Abs(direction.x):
                    lookDirection = MovementDirection.North;
                    break;
                case > 0:
                    if (direction.y < 3 * Mathf.Abs(direction.x) && Mathf.Abs(direction.x) < 3 * direction.y)
                    {
                        lookDirection = direction.x > 0 ? MovementDirection.NorthEast : MovementDirection.NorthWest;
                    }
                    else
                    {
                        lookDirection = direction.x > 0 ? MovementDirection.East : MovementDirection.West;
                    }
                    break;
                case < 0 when Mathf.Abs(direction.y) >= 3 * Mathf.Abs(direction.x):
                    lookDirection = MovementDirection.South;
                    break;
                case < 0:
                    if (Mathf.Abs(direction.y) < 3 * Mathf.Abs(direction.x) &&
                        Mathf.Abs(direction.x) < 3 * Mathf.Abs(direction.y))
                    {
                        lookDirection = direction.x > 0 ? MovementDirection.SouthEast : MovementDirection.SouthWest;
                    }
                    else
                    {
                        lookDirection = direction.x > 0 ? MovementDirection.East : MovementDirection.West;
                    }
                    break;
                default:
                    lookDirection = direction.x > 0 ? MovementDirection.East : MovementDirection.West;
                    break;
            }
            _animator.SetMovementDirection(lookDirection);
        }

        private Vector3 BattlePositionToWorldPosition(float x, float y)
        {
            var position = new Vector3(x * _grid.cellSize, y * _grid.cellSize, 0);
            return _grid.xDirection * position.x + _grid.yDirection * position.y;
        }
        
        private void UpdateHealthBar()
        {
            var showBar = _health != _unitData.health && _health > 0;
            if (showBar)
            {
                HealthBar.bar.fillAmount = Mathf.Abs(_health / (_unitData.health * 1f));

                var plainDownLeft = CameraUtils.plainDownLeft;
                var plainTopRight = CameraUtils.plainTopRight;

                var w = plainTopRight.x - plainDownLeft.x;
                var h = plainTopRight.y - plainDownLeft.y;

                var endW = transform.position.x - plainDownLeft.x;
                var endH = transform.position.y - plainDownLeft.y;

                var screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);
                HealthBar.rect.anchoredPosition = screenPoint;
            }
            
            if (HealthBar)
            {
                HealthBar.gameObject.SetActive(showBar);
            }
        }
    }
}