using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Environment.Projectiles;
using SargeUniverse.Scripts.Sound;
using SargeUniverse.Scripts.Utils;
using UI.Elements;
using UnityEngine;
using Utils;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class BattleBuilding : ConstructionBuilding
    {
        [SerializeField] private ProjectileSystem _projectileSystem;
        
        public UIBar HealthBar { get; set; } = null;

        private int _maxHealth = 0;
        private int _health = 0;

        public Vector3 Offset => new(_buildingData.columns / 2f, _buildingData.rows / 2f, 0);
        
        protected override void OnDestroy()
        {
            if (HealthBar && HealthBar != null)
            {
                Destroy(HealthBar.gameObject);
            }
        }

        protected override void Update()
        {
            base.Update();
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            var showBar = _health != _buildingData.health && _health > 0;
            if (showBar)
            {
                HealthBar.bar.fillAmount = Mathf.Abs(_health / (_maxHealth * 1f));
                
                var end = _grid.GetEndPosition(currentX, currentY, Width, Height);

                var plainDownLeft = CameraUtils.plainDownLeft;
                var plainTopRight = CameraUtils.plainTopRight;

                var w = plainTopRight.x - plainDownLeft.x;
                var h = plainTopRight.y - plainDownLeft.y;

                var endW = end.x - plainDownLeft.x;
                var endH = end.y - plainDownLeft.y;

                var screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);
                HealthBar.rect.anchoredPosition = screenPoint;
            }
            
            if (HealthBar)
            {
                HealthBar.gameObject.SetActive(showBar);
            }
        }
        
        public override void SetBuildingData(Data_Building buildingData, bool updateSprite = false)
        {
            base.SetBuildingData(buildingData, updateSprite);
            _maxHealth = _buildingData.health;
            _health = _buildingData.health;
        }

        public virtual void Attack(Transform targetTransform)
        {
            var lookDirection = DirectionUtils.GetLookDirection(transform.position, targetTransform.position);
            _animator.SetDirection(_buildingData.level, lookDirection);
            _animator.PlayAttackAnimation(
                _buildingData.level, 
                lookDirection, 
                () => ShootProjectile(targetTransform)
            );
        }
        
        public virtual void Attack(Vector3 targetPosition)
        {
            var lookDirection = DirectionUtils.GetLookDirection(transform.position, targetPosition);
            _animator.SetDirection(_buildingData.level, lookDirection);
            _animator.PlayAttackAnimation(
                _buildingData.level, 
                lookDirection, 
                () => ShootProjectile(targetPosition)
            );
        }

        private void ShootProjectile(Transform targetTransform)
        {
            _projectileSystem?.SpawnProjectile(targetTransform, _buildingData.rangedSpeed, _buildingData.level);
        }
        
        private void ShootProjectile(Vector3 targetPosition)
        {
            _projectileSystem?.SpawnProjectile(targetPosition, _buildingData.rangedSpeed, _buildingData.level);
        }

        public void TakeDamage(int damage)
        {
            if (_health <= 0)
            {
                return;
            }
            
            _health = _health >= damage ? _health - damage : 0;
            UpdateHealthBar();

            if (_health == 0)
            {
                HealthBar?.gameObject.SetActive(false);
            }
        }
        
        public virtual void DestroyBuilding()
        {
            SoundSystem.Instance.PlaySound("building_explosion");
            _animator.PlayDestroyAnimation();
            HealthBar?.gameObject.SetActive(false);
        }
        
        protected override void SetBuildingSprite()
        {
            if (_buildingData.isPlayerBuilding)
            {
                base.SetBuildingSprite();
            }
            else
            {
                _buildingSprite.sprite =
                    _buildingsConfig.GetBuildingData(_buildingData.id).GetOpponentBuildingSprite(_buildingData.level);
            }
        }
    }
}