using System;
using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.BattleSystem.Data;
using SargeUniverse.Scripts.BattleSystem.PathFinding;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem
{
    public class Battle
    {
        public long id = 0;
		public DateTime baseTime = DateTime.Now;
		public int frameCount = 0;
		public long defender = 0;
		public long attacker = 0;
		private List<BattleBuildingData> _buildings = new();
		private List<BattleUnitData> _defenceUnits = new();
		private List<BattleUnitData> _attackUnits = new();
		private List<UnitToAdd> _unitsToAdd = new();
		private BattleGrid _battleGrid = null;
		private BattleGrid _unlimitedBattleGrid = null;
		private PathBuilder _pathBuilder = null;
		private PathBuilder _unlimitedPathBuilder = null;
		private List<Tile> _blockedTiles = new();
		private readonly List<Data.ProjectileData> _projectiles = new();

		public double percentage = 0;
		public bool battleEnd = false;
		public bool surrender = false;
		public int surrenderFrame = 0;
		public float duration = 0;

		public int unitsDeployed = 0;
		public bool hqDestroyed = false;
		public bool fiftyPercentDestroyed = false;
		public bool completelyDestroyed = false;

		public int winTrophies = 0;
		public int loseTrophies = 0;
		private int _projectilesCount = 0;

		public List<BattleUnitData> DefenceUnits => _defenceUnits;
		public List<BattleUnitData> AttackUnits => _attackUnits;
		
		public Callbacks.ProjectileCallback projectileCallback = null;

		public BattleReward GetBattleReward()
		{
			var reward = new BattleReward();
			foreach (var building in _buildings)
			{
				switch(building.GetId())
				{
					case BuildingID.hq:
						reward.totalSupplies += building.lootSuppliesStorage;
						reward.totalPower += building.lootPowerStorage;
						reward.totalEnergy += building.lootEnergyStorage;
						reward.lootedSupplies += building.lootedSupplies;
						reward.lootedPower += building.lootedPower;
						reward.lootedEnergy += building.lootedEnergy;
						break;
					case BuildingID.supplydrop:
					case BuildingID.supplyvault:
						reward.totalSupplies += building.lootSuppliesStorage;
						reward.lootedSupplies += building.lootedSupplies;
						break;
					case BuildingID.powerplant:
					case BuildingID.powerstorage:
						reward.totalPower += building.lootPowerStorage;
						reward.lootedPower += building.lootedPower;
						break;
				}
			}
			return reward;
		}

		public int GetStars()
		{
			var stars = 0;
			if(hqDestroyed)
			{
				stars++;
			}

			if(fiftyPercentDestroyed)
			{
				stars++;
			}

			if(completelyDestroyed)
			{
				stars++;
			}
			return stars;
		}

		public int GetTrophies()
		{
			var stars = GetStars();
			if (stars > 0)
			{
				if (stars >= 3)
				{
					return winTrophies;
				}
				
				var t = (int)Math.Floor(winTrophies / (double)stars);
				return t * stars;
			}
			return loseTrophies * -1;
		}

		public void Init(
			List<BattleBuildingData> buildings,
			List<BattleUnitData> defenceUnits,
			DateTime time,
			Callbacks.AttackCallback buildingAttackCallback = null,
			Callbacks.DoubleCallback buildingDestroyCallback = null,
			Callbacks.FloatCallback buildingDamageCallback = null,
			Callbacks.ProjectileCallback buildingProjectileCallback = null,
			Callbacks.AttackCallback defUnitAttackCallback = null,
			Callbacks.IndexCallback defUnitDieCallback = null, 
			Callbacks.FloatCallback defUnitDamageCallback = null, 
			Callbacks.FloatCallback defUnitHealCallback = null,
			Callbacks.IndexCallback defUnitTargetCallback = null,
			Callbacks.IndexCallback defUnitIdleCallback = null,
			Callbacks.BlankCallback starGained = null)
		{
			duration = Constants.BattleDuration;
			frameCount = 0;
			percentage = 0;
			unitsDeployed = 0;
			fiftyPercentDestroyed = false;
			hqDestroyed = false;
			completelyDestroyed = false;
			battleEnd = false;
			_projectilesCount = 0;
			surrender = false;

			_battleGrid = new BattleGrid(
				Constants.GridSize + Constants.BattleGridOffset * 2, 
				Constants.GridSize + Constants.BattleGridOffset * 2
			);
			_unlimitedBattleGrid = new BattleGrid(
				Constants.GridSize + Constants.BattleGridOffset * 2, 
				Constants.GridSize + Constants.BattleGridOffset * 2
			);
			_pathBuilder = new PathBuilder(_battleGrid);
			_unlimitedPathBuilder = new PathBuilder(_unlimitedBattleGrid);
			
			_buildings = buildings;
			_defenceUnits = defenceUnits;
			baseTime = time;
			
			projectileCallback = buildingProjectileCallback;
			
			for(var i = 0; i < _buildings.Count; i++)
			{
				_buildings[i].attackCallback = buildingAttackCallback;
				_buildings[i].destroyCallback = buildingDestroyCallback;
				_buildings[i].damageCallback = buildingDamageCallback;
				_buildings[i].starCallback = starGained;

				_buildings[i].Initialize();
				_buildings[i].worldCenterPosition = new BattleVector2(
					(_buildings[i].buildingData.x + (_buildings[i].buildingData.columns / 2f)) * Constants.GridCellSize, 
					(_buildings[i].buildingData.y + (_buildings[i].buildingData.rows / 2f)) * Constants.GridCellSize
				);

				var startX = _buildings[i].buildingData.x;
				var endX = _buildings[i].buildingData.x + _buildings[i].buildingData.columns;

				var startY = _buildings[i].buildingData.y;
				var endY = _buildings[i].buildingData.y + _buildings[i].buildingData.rows;

				if(_buildings[i].GetId() != BuildingID.wall && _buildings[i].buildingData.columns > 1 && _buildings[i].buildingData.rows > 1)
				{
					startX++;
					startY++;
					endX--;
					endY--;
					if(endX <= startX || endY <= startY)
					{
						continue;
					}
				}

				for(var x = startX; x < endX; x++)
				{
					for(var y = startY; y < endY; y++)
					{
						_battleGrid[x, y].blocked = true;
						_blockedTiles.Add(new Tile(_buildings[i].buildingData.id, new Vector2Int(x, y), i));
					}
				}
			}

			foreach (var defenceUnit in _defenceUnits)
			{
				defenceUnit.attackCallback = defUnitAttackCallback;
				defenceUnit.dieCallback = defUnitDieCallback;
				defenceUnit.damageCallback = defUnitDamageCallback;
				defenceUnit.healCallback = defUnitHealCallback;
				defenceUnit.targetCallback = defUnitTargetCallback;
				defenceUnit.idleCallback = defUnitIdleCallback;
			}
		}

		public bool IsAliveUnitsOnGrid()
		{
			return _attackUnits.FindAll(unit => unit.health > 0).Count > 0;
		}

		private bool CanBattleGoOn()
		{
			if (Math.Abs(percentage - 1d) > Mathf.Epsilon && IsAliveUnitsOnGrid())
			{
				double time = frameCount * Constants.BattleFrameRate;
				if(time < duration)
				{
					return true;
				}
			}
			return false;
		}

		public bool CanAddUnit(int x, int y)
		{
			x += Constants.BattleGridOffset;
			y += Constants.BattleGridOffset;

			foreach (var b in _buildings)
			{
				if(b.health <= 0)
				{
					continue;
				}

				var startX = b.buildingData.x;
				var endX = b.buildingData.x + b.buildingData.columns;

				var startY = b.buildingData.y;
				var endY = b.buildingData.y + b.buildingData.rows;

				for(var x2 = startX; x2 < endX; x2++)
				{
					for(var y2 = startY; y2 < endY; y2++)
					{
						if(x == x2 && y == y2)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public void AddUnit(
			Data_Unit dataUnit, 
			int x, 
			int y, 
			Callbacks.UnitSpawned spawnCallback = null, 
			Callbacks.AttackCallback attackCallback = null, 
			Callbacks.IndexCallback dieCallback = null, 
			Callbacks.FloatCallback damageCallback = null, 
			Callbacks.FloatCallback healCallback = null,
			Callbacks.IndexCallback targetCallback = null,
			Callbacks.IndexCallback idleCallback = null)
		{
			if(battleEnd)
			{
				return;
			}
			
			x += Constants.BattleGridOffset;
			y += Constants.BattleGridOffset;
			var unitToAdd = new UnitToAdd()
			{
				callback = spawnCallback
			};
			var battleUnit = new BattleUnitData
			{
				attackCallback = attackCallback,
				dieCallback = dieCallback,
				damageCallback = damageCallback,
				healCallback = healCallback,
				targetCallback = targetCallback,
				idleCallback = idleCallback,
				dataUnit = dataUnit
			};
			battleUnit.Initialize(x, y);
			battleUnit.health = dataUnit.health;
			unitToAdd.unitData = battleUnit;
			unitToAdd.x = x;
			unitToAdd.y = y;
			_unitsToAdd.Add(unitToAdd);
		}

		public void ExecuteFrame()
		{
			var addIndex = _attackUnits.Count;
			for(var i = _unitsToAdd.Count - 1; i >= 0; i--)
			{
				unitsDeployed += _unitsToAdd[i].unitData.dataUnit.hosing;
				_unitsToAdd[i].x += Constants.BattleGridOffset;
				_unitsToAdd[i].y += Constants.BattleGridOffset;
				_attackUnits.Insert(addIndex, _unitsToAdd[i].unitData);

				addIndex++;
				if(_unitsToAdd[i].callback != null)
				{
					_unitsToAdd[i].callback.Invoke(
						_unitsToAdd[i].unitData.dataUnit.databaseID,
						_unitsToAdd[i].x,
						_unitsToAdd[i].y);
				}
				_unitsToAdd.RemoveAt(i);
			}

			for (var i = 0; i < _buildings.Count; i++)
			{
				if(_buildings[i].buildingData.attackMode != AttackMode.None && _buildings[i].health > 0)
				{
					HandleBuilding(i, Constants.BattleFrameRate);
				}
			}

			foreach (var defenceUnit in _defenceUnits)
			{
				if (defenceUnit.health > 0)
				{
					HandleUnit(defenceUnit);
				}
			}

			for (var i = 0; i < _attackUnits.Count; i++)
			{
				if(_attackUnits[i].health > 0)
				{
					HandleUnit(_attackUnits[i]);
					// HandleUnit(i, Constants.BattleFrameRate);
				}
			}

			if(_projectiles.Count > 0)
			{
				for (var i = _projectiles.Count - 1; i >= 0; i--)
				{
					_projectiles[i].timer -= Constants.BattleFrameRate;
					if (_projectiles[i].timer <= 0)
					{
						if(_projectiles[i].type == TargetType.Unit)
						{
							_attackUnits[_projectiles[i].target]?.TakeDamage(_projectiles[i].damage);
							if(_projectiles[i].splash > 0)
							{
								for (int j = 0; j < _attackUnits.Count; j++)
								{
									if (j != _projectiles[i].target)
									{
										var distance = BattleVector2.Distance(_attackUnits[j].position, _attackUnits[_projectiles[i].target].position);
										if(distance < _projectiles[i].splash * Constants.GridCellSize)
										{
											_attackUnits[j].TakeDamage(_projectiles[i].damage * (1f - (distance / _projectiles[i].splash * Constants.GridCellSize)));
										}
									}
								}
							}
						}
						else
						{
							_buildings[_projectiles[i].target].TakeDamage(_projectiles[i].damage, ref _battleGrid, ref _blockedTiles, ref percentage, ref fiftyPercentDestroyed, ref hqDestroyed, ref completelyDestroyed);
						}
						_projectiles.RemoveAt(i);
					}
				}
			}

			frameCount++;
		}

		private void HandleBuilding(int index, double deltaTime)
		{
			if(_buildings[index].target >= 0)
			{
				if(_attackUnits[_buildings[index].target].health <= 0 || !IsUnitInRange(_buildings[index].target, index))
				{
					_buildings[index].target = -1;
				}
				else
				{
					if(IsUnitCanBeSeen(_buildings[index].target, index))
					{
						_buildings[index].attackTimer += deltaTime;
						var attacksCount = (int)Math.Floor(_buildings[index].attackTimer / _buildings[index].buildingData.speed);
						if(attacksCount > 0)
						{
							_buildings[index].attackTimer -= (attacksCount * _buildings[index].buildingData.speed);
							for (var i = 1; i <= attacksCount; i++)
							{
								if(_buildings[index].buildingData.radius > 0 && _buildings[index].buildingData.rangedSpeed > 0)
								{
									var distance = BattleVector2.Distance(_attackUnits[_buildings[index].target].position, _buildings[index].worldCenterPosition);
									var projectile = new Data.ProjectileData
									{
										type = TargetType.Unit,
										target = _buildings[index].target,
										timer = distance / (_buildings[index].buildingData.rangedSpeed * Constants.GridCellSize),
										damage = _buildings[index].buildingData.damage,
										splash = _buildings[index].buildingData.splashRange,
										follow = true,
										position = _buildings[index].worldCenterPosition
									};
									
									_projectilesCount++;
									projectile.id = _projectilesCount;
									_projectiles.Add(projectile);
									projectileCallback?.Invoke(
										projectile.id, 
										_buildings[index].worldCenterPosition, 
										_attackUnits[_buildings[index].target].position
									);
								}
								else
								{
									_attackUnits[_buildings[index].target].TakeDamage(_buildings[index].buildingData.damage);
									if (_buildings[index].buildingData.splashRange > 0)
									{
										for (var j = 0; j < _attackUnits.Count; j++)
										{
											if (j != _buildings[index].target)
											{
												var distance = BattleVector2.Distance(_attackUnits[j].position, _attackUnits[_buildings[index].target].position);
												if (distance < _buildings[index].buildingData.splashRange * Constants.GridCellSize)
												{
													_attackUnits[j].TakeDamage(_buildings[index].buildingData.damage * (1f - distance / _buildings[index].buildingData.splashRange * Constants.GridCellSize));
												}
											}
										}
									}
								}
								
								if (_buildings[index].attackCallback != null)
								{
									_buildings[index].attackCallback.Invoke(
										_buildings[index].buildingData.databaseID, 
										_attackUnits[_buildings[index].target].dataUnit.databaseID
									);
								}
							}
						}
					}
					else
					{
						_buildings[index].target = -1;
					}
				}
			}
			if (_buildings[index].target < 0)
			{
				if(FindTargetForBuilding(index))
				{
					HandleBuilding(index, deltaTime);
				}
			}
		}

		private bool FindTargetForBuilding(int index)
		{
			for (var i = 0; i < _attackUnits.Count; i++)
			{
				if (_attackUnits[i].health <= 0)
				{
					continue;
				}

				if (_buildings[index].buildingData.attackMode == AttackMode.Ground && _attackUnits[i].dataUnit.movement == UnitMovementType.Fly)
				{
					continue;
				}

				if (_buildings[index].buildingData.attackMode == AttackMode.Air && _attackUnits[i].dataUnit.movement != UnitMovementType.Fly)
				{
					continue;
				}

				if (IsUnitInRange(i, index) && IsUnitCanBeSeen(i, index))
				{
					_buildings[index].attackTimer = _buildings[index].buildingData.speed;
					_buildings[index].target = i;
					return true;
				}
			}
			return false;
		}

		private bool IsUnitInRange(int unitIndex, int buildingIndex)
		{
			var distance = BattleVector2.Distance(_buildings[buildingIndex].worldCenterPosition, _attackUnits[unitIndex].position);
			if (distance <= (_buildings[buildingIndex].buildingData.radius * Constants.GridCellSize))
			{
				if (_buildings[buildingIndex].buildingData.blindRange > 0 && distance <= _buildings[buildingIndex].buildingData.blindRange)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		private bool IsUnitCanBeSeen(int unitIndex, int buildingIndex)
		{
			// TODO: For abilities
			return true;
		}

		private void HandleUnit(BattleUnitData battleUnitData)
		{
			if (battleUnitData.defMode)
			{
				HandleDefenceUnit(battleUnitData, Constants.BattleFrameRate);
			}
			else
			{
				HandleAttackUnit(battleUnitData, Constants.BattleFrameRate);
			}
		}

		private void HandleDefenceUnit(BattleUnitData unit, double deltaTime)
		{
			if (unit.targetUnit == null || unit.targetUnit.health <= 0)
			{
				FindTargetForUnit(unit);
				if (unit.targetUnit == null)
				{
					unit.Stop();
					return;
				}
			}
			
			if (unit.path != null)
			{
				if (unit.targetUnit == null || unit.targetUnit.health <= 0)
				{
					unit.Stop();
				}
				else
				{
					unit.moving = true;
					var remainedTime = unit.pathTime - unit.pathTraveledTime;
					if (remainedTime >= deltaTime)
					{
						double moveExtra = 1;
						double s = unit.dataUnit.moveSpeed / 10f;
						if (s != (unit.dataUnit.moveSpeed / 10f))
						{
							moveExtra = s / (unit.dataUnit.moveSpeed / 10f);
						}

						unit.pathTraveledTime += (deltaTime * moveExtra);
						if (unit.pathTraveledTime > unit.pathTime)
						{
							unit.pathTraveledTime = unit.pathTime;
						}

						if (unit.pathTraveledTime < 0)
						{
							unit.pathTraveledTime = 0;
						}

						deltaTime = 0;
					}
					else
					{
						unit.pathTraveledTime = unit.pathTime;
						deltaTime -= remainedTime;
					}

					/*unit.position = GetPathPosition(unit.path.points,
						(float)(unit.pathTraveledTime / unit.pathTime));*/
					
					unit.position = BattleVector2.LerpUnclamped(unit.position,
						BattleMapUtils.GridToWorldPosition(unit.path.points.Last().location), (float)(unit.pathTraveledTime / unit.pathTime));

					if (unit.dataUnit.attackRange > 0 && IsUnitInRange(unit))
					{
						unit.path = null;
					}
					else
					{
						var targetPosition = BattleMapUtils.GridToWorldPosition(new Vector2Int(
							unit.path.points.Last().location.x,
							unit.path.points.Last().location.y)
						);
						var distance = BattleVector2.Distance(unit.position, targetPosition);
						if (distance <= Constants.GridCellSize * 0.05f)
						{
							unit.position = targetPosition;
							unit.path = null;
							unit.moving = false;
						}
					}
				}
			}

			if (unit.targetUnit != null)
			{
				if (unit.targetUnit.health > 0)
				{
					if (unit.path == null)
					{
						var distance = BattleVector2.Distance(unit.position, unit.targetUnit.position);
						if (distance < unit.dataUnit.attackRange)
						{

							unit.moving = false;
							unit.attackTimer += deltaTime;

							if (unit.attackTimer >= unit.dataUnit.attackSpeed)
							{
								float multiplier = 1;
								/*if (unit.dataUnit.priority != TargetPriority.All ||
								    unit.dataUnit.priority != TargetPriority.None)
								{
									switch (_buildings[unit.target].buildingData.id)
									{
										case BuildingID.hq:
										case BuildingID.supplydrop:
										case BuildingID.supplyvault:
										case BuildingID.powerplant:
										case BuildingID.powerstorage:
											if (_attackUnits[index].dataUnit.priority == TargetPriority.Resources)
											{
												multiplier = _attackUnits[index].dataUnit.priorityMultiplier;
											}

											break;
										case BuildingID.wall:
											if (_attackUnits[index].dataUnit.priority == TargetPriority.Walls)
											{
												multiplier = _attackUnits[index].dataUnit.priorityMultiplier;
											}

											break;
										case BuildingID.watchtower:
										case BuildingID.rocketturret:
										case BuildingID.motor:
											if (_attackUnits[index].dataUnit.priority == TargetPriority.Defenses)
											{
												multiplier = _attackUnits[index].dataUnit.priorityMultiplier;
											}

											break;
									}
								}*/



								if (unit.dataUnit.attackRange > 0 && unit.dataUnit.rangedSpeed > 0)
								{
									var projectile = new Data.ProjectileData
									{
										type = TargetType.Unit,
										target = _attackUnits.FindIndex(u => u.dataUnit.databaseID == unit.targetUnit.dataUnit.databaseID),
										timer = distance / (unit.dataUnit.rangedSpeed *
										                    Constants.GridCellSize),
										damage = unit.dataUnit.damage * multiplier,
										follow = true,
										position = unit.position
									};
									_projectilesCount++;
									projectile.id = _projectilesCount;
									_projectiles.Add(projectile);
									projectileCallback?.Invoke(
										projectile.id,
										unit.position,
										unit.targetUnit.position
									);
								}
								else
								{
									unit.targetUnit.TakeDamage(unit.dataUnit.damage * multiplier);
								}

								unit.attackTimer -= unit.dataUnit.attackSpeed;
								unit.attackCallback?.Invoke(
									unit.dataUnit.databaseID,
									unit.targetUnit.dataUnit.databaseID
								);
							}
						}
					}
				}
				else
				{
					unit.Stop();
				}
			}
			
			if (_attackUnits.Count > 0)
			{
				FindTargetForUnit(unit);
			}
		}

		private void HandleAttackUnit(BattleUnitData unit, double deltaTime)
		{
			if ((unit.targetUnit == null || unit.targetUnit.health <= 0) && (unit.targetBuilding == null || unit.targetBuilding.health <= 0))
			{
				FindTargetForUnit(unit);
				if (unit.targetUnit == null && unit.targetBuilding == null)
				{
					unit.Stop();
					return;
				}
			}
			
			if (unit.path != null)
			{
				if (unit.targetUnit == null || unit.targetUnit.health <= 0)
				{
					unit.Stop();
				}
				else
				{
					unit.moving = true;
					var remainedTime = unit.pathTime - unit.pathTraveledTime;
					if (remainedTime >= deltaTime)
					{
						double moveExtra = 1;
						double s = unit.dataUnit.moveSpeed / 10f;
						if (s != (unit.dataUnit.moveSpeed / 10f))
						{
							moveExtra = s / (unit.dataUnit.moveSpeed / 10f);
						}

						unit.pathTraveledTime += (deltaTime * moveExtra);
						if (unit.pathTraveledTime > unit.pathTime)
						{
							unit.pathTraveledTime = unit.pathTime;
						}

						if (unit.pathTraveledTime < 0)
						{
							unit.pathTraveledTime = 0;
						}

						deltaTime = 0;
					}
					else
					{
						unit.pathTraveledTime = unit.pathTime;
						deltaTime -= remainedTime;
					}

					/*unit.position = GetPathPosition(unit.path.points,
						(float)(unit.pathTraveledTime / unit.pathTime));*/
					
					unit.position = BattleVector2.LerpUnclamped(unit.position,
						BattleMapUtils.GridToWorldPosition(unit.path.points.Last().location), (float)(unit.pathTraveledTime / unit.pathTime));

					if (unit.dataUnit.attackRange > 0 && IsUnitInRange(unit))
					{
						unit.path = null;
					}
					else
					{
						var targetPosition = BattleMapUtils.GridToWorldPosition(new Vector2Int(
							unit.path.points.Last().location.x,
							unit.path.points.Last().location.y)
						);
						var distance = BattleVector2.Distance(unit.position, targetPosition);
						if (distance <= Constants.GridCellSize * 0.05f)
						{
							unit.position = targetPosition;
							unit.path = null;
							unit.moving = false;
						}
					}
				}
			}
			
			if (unit.targetBuilding != null)
			{
				if (unit.targetBuilding.health > 0)
				{
					if (unit.targetBuilding.buildingData.id == BuildingID.wall && unit.mainTarget >= 0 &&
					    unit.targetBuilding.health <= 0)
					{
						unit.moving = false;
						unit.target = -1;
					}
					else
					{
						if (unit.path == null)
						{
							unit.moving = false;
							unit.attackTimer += deltaTime;
							if (unit.attackTimer >= unit.dataUnit.attackSpeed)
							{
								float multiplier = 1;
								if (unit.dataUnit.priority != TargetPriority.All ||
								    unit.dataUnit.priority != TargetPriority.None)
								{
									switch (unit.targetBuilding.buildingData.id)
									{
										case BuildingID.hq:
										case BuildingID.supplydrop:
										case BuildingID.supplyvault:
										case BuildingID.powerplant:
										case BuildingID.powerstorage:
											if (unit.dataUnit.priority == TargetPriority.Resources)
											{
												multiplier = unit.dataUnit.priorityMultiplier;
											}

											break;
										case BuildingID.wall:
											if (unit.dataUnit.priority == TargetPriority.Walls)
											{
												multiplier = unit.dataUnit.priorityMultiplier;
											}

											break;
										case BuildingID.watchtower:
										case BuildingID.rocketturret:
										case BuildingID.motor:
											if (unit.dataUnit.priority == TargetPriority.Defenses)
											{
												multiplier = unit.dataUnit.priorityMultiplier;
											}

											break;
									}
								}

								var distance = BattleVector2.Distance(
									unit.position, unit.targetBuilding.worldCenterPosition
								);

								if (unit.dataUnit.attackRange > 0 && unit.dataUnit.rangedSpeed > 0)
								{
									var projectile = new Data.ProjectileData
									{
										type = TargetType.Building,
										target = _buildings.FindIndex(b => b.buildingData.databaseID == unit.targetBuilding.buildingData.databaseID),
										timer = distance / (unit.dataUnit.rangedSpeed * Constants.GridCellSize),
										damage = unit.dataUnit.damage * multiplier,
										follow = true,
										position = unit.position
									};
									_projectilesCount++;
									projectile.id = _projectilesCount;
									_projectiles.Add(projectile);
									projectileCallback?.Invoke(
										projectile.id,
										unit.position,
										unit.targetBuilding.worldCenterPosition
									);
								}
								else
								{
									unit.targetBuilding.TakeDamage(
										unit.dataUnit.damage * multiplier,
										ref _battleGrid,
										ref _blockedTiles,
										ref percentage,
										ref fiftyPercentDestroyed,
										ref hqDestroyed,
										ref completelyDestroyed
									);
								}

								unit.attackTimer -= unit.dataUnit.attackSpeed;
								unit.attackCallback?.Invoke(
									unit.dataUnit.databaseID,
									unit.targetBuilding.buildingData.databaseID
								);
							}
						}
					}
				}
				else
				{
					unit.Stop();
				}
			}
			else if (unit.targetUnit != null)
			{
				if (unit.targetUnit.health > 0)
				{
					if (unit.path == null)
					{
						var distance = BattleVector2.Distance(unit.position, unit.targetUnit.position);
						if (distance < unit.dataUnit.attackRange)
						{

							unit.moving = false;
							unit.attackTimer += deltaTime;

							if (unit.attackTimer >= unit.dataUnit.attackSpeed)
							{
								float multiplier = 1;
								/*if (unit.dataUnit.priority != TargetPriority.All ||
								    unit.dataUnit.priority != TargetPriority.None)
								{
									switch (_buildings[unit.target].buildingData.id)
									{
										case BuildingID.hq:
										case BuildingID.supplydrop:
										case BuildingID.supplyvault:
										case BuildingID.powerplant:
										case BuildingID.powerstorage:
											if (_attackUnits[index].dataUnit.priority == TargetPriority.Resources)
											{
												multiplier = _attackUnits[index].dataUnit.priorityMultiplier;
											}

											break;
										case BuildingID.wall:
											if (_attackUnits[index].dataUnit.priority == TargetPriority.Walls)
											{
												multiplier = _attackUnits[index].dataUnit.priorityMultiplier;
											}

											break;
										case BuildingID.watchtower:
										case BuildingID.rocketturret:
										case BuildingID.motor:
											if (_attackUnits[index].dataUnit.priority == TargetPriority.Defenses)
											{
												multiplier = _attackUnits[index].dataUnit.priorityMultiplier;
											}

											break;
									}
								}*/



								if (unit.dataUnit.attackRange > 0 && unit.dataUnit.rangedSpeed > 0)
								{
									var projectile = new Data.ProjectileData
									{
										type = TargetType.Unit,
										target = _defenceUnits.FindIndex(u => u.dataUnit.databaseID == unit.targetUnit.dataUnit.databaseID),
										timer = distance / (unit.dataUnit.rangedSpeed *
										                    Constants.GridCellSize),
										damage = unit.dataUnit.damage * multiplier,
										follow = true,
										position = unit.position
									};
									_projectilesCount++;
									projectile.id = _projectilesCount;
									_projectiles.Add(projectile);
									projectileCallback?.Invoke(
										projectile.id,
										unit.position,
										unit.targetUnit.position
									);
								}
								else
								{
									unit.targetUnit.TakeDamage(unit.dataUnit.damage * multiplier);
								}

								unit.attackTimer -= unit.dataUnit.attackSpeed;
								unit.attackCallback?.Invoke(
									unit.dataUnit.databaseID,
									unit.targetUnit.dataUnit.databaseID
								);
							}
						}
					}
				}
				else
				{
					unit.Stop();
				}
			}
			
			if (_defenceUnits.Count > 0 || _buildings.Count > 0)
			{
				FindTargetForUnit(unit);
			}
		}

		private void FindTargetForUnit()
		{
			
		}
		
		private void HandleUnit(int index, double deltaTime)
		{
			if (_attackUnits[index].path != null)
			{
				if (_attackUnits[index].target < 0 || (_attackUnits[index].target >= 0 && _buildings[_attackUnits[index].target].health <= 0))
				{
					_attackUnits[index].moving = false;
					_attackUnits[index].path = null;
					_attackUnits[index].target = -1;
				}
				else
				{
					_attackUnits[index].moving = true;
					var remainedTime = _attackUnits[index].pathTime - _attackUnits[index].pathTraveledTime;
					if (remainedTime >= deltaTime)
					{
						double moveExtra = 1;
						double s = GetUnitMoveSpeed(index);
						if (s != (_attackUnits[index].dataUnit.moveSpeed / 10f))
						{
							moveExtra = s / (_attackUnits[index].dataUnit.moveSpeed / 10f);
						}
						_attackUnits[index].pathTraveledTime += (deltaTime * moveExtra);
						if (_attackUnits[index].pathTraveledTime > _attackUnits[index].pathTime)
						{
							_attackUnits[index].pathTraveledTime = _attackUnits[index].pathTime;
						}
						if (_attackUnits[index].pathTraveledTime < 0)
						{
							_attackUnits[index].pathTraveledTime = 0;
						}
						deltaTime = 0;
					}
					else
					{
						_attackUnits[index].pathTraveledTime = _attackUnits[index].pathTime;
						deltaTime -= remainedTime;
					}

					_attackUnits[index].position = GetPathPosition(_attackUnits[index].path.points, (float)(_attackUnits[index].pathTraveledTime / _attackUnits[index].pathTime));

					if (_attackUnits[index].dataUnit.attackRange > 0 && IsBuildingInRange(index, _attackUnits[index].target))
					{
						_attackUnits[index].path = null;
					}
					else
					{
						var targetPosition = BattleMapUtils.GridToWorldPosition(new Vector2Int(
							_attackUnits[index].path.points.Last().location.x, 
							_attackUnits[index].path.points.Last().location.y)
						);
						var distance = BattleVector2.Distance(_attackUnits[index].position, targetPosition);
						if (distance <= Constants.GridCellSize * 0.05f)
						{
							_attackUnits[index].position = targetPosition;
							_attackUnits[index].path = null;
							_attackUnits[index].moving = false;
						}
					}
				}
			}

			if (_attackUnits[index].target >= 0)
			{
				if (_buildings[_attackUnits[index].target].health > 0)
				{
					if (_buildings[_attackUnits[index].target].buildingData.id == BuildingID.wall && _attackUnits[index].mainTarget >= 0 && _buildings[_attackUnits[index].mainTarget].health <= 0)
					{
						_attackUnits[index].moving = false;
						_attackUnits[index].target = -1;
					}
					else
					{
						if (_attackUnits[index].path == null)
						{
							_attackUnits[index].moving = false;
							_attackUnits[index].attackTimer += deltaTime;
							if (_attackUnits[index].attackTimer >= _attackUnits[index].dataUnit.attackSpeed)
							{
								float multiplier = 1;
								if (_attackUnits[index].dataUnit.priority != TargetPriority.All || _attackUnits[index].dataUnit.priority != TargetPriority.None)
								{
									switch (_buildings[_attackUnits[index].target].buildingData.id)
									{
										case BuildingID.hq:
										case BuildingID.supplydrop:
										case BuildingID.supplyvault:
										case BuildingID.powerplant:
										case BuildingID.powerstorage:
											if(_attackUnits[index].dataUnit.priority == TargetPriority.Resources)
											{
												multiplier = _attackUnits[index].dataUnit.priorityMultiplier;
											}
											break;
										case BuildingID.wall:
											if(_attackUnits[index].dataUnit.priority == TargetPriority.Walls)
											{
												multiplier = _attackUnits[index].dataUnit.priorityMultiplier;
											}
											break;
										case BuildingID.watchtower:
										case BuildingID.rocketturret:
										case BuildingID.motor:
											if(_attackUnits[index].dataUnit.priority == TargetPriority.Defenses)
											{
												multiplier = _attackUnits[index].dataUnit.priorityMultiplier;
											}
											break;
									}
								}

								var distance = BattleVector2.Distance(
									_attackUnits[index].position, 
									_buildings[_attackUnits[index].target].worldCenterPosition
								);
								
								if (_attackUnits[index].dataUnit.attackRange > 0 && _attackUnits[index].dataUnit.rangedSpeed > 0)
								{
									var projectile = new Data.ProjectileData
									{
										type = TargetType.Building,
										target = _attackUnits[index].target,
										timer = distance / (_attackUnits[index].dataUnit.rangedSpeed * Constants.GridCellSize),
										damage = GetUnitDamage(index) * multiplier,
										follow = true,
										position = _attackUnits[index].position
									};
									_projectilesCount++;
									projectile.id = _projectilesCount;
									_projectiles.Add(projectile);
									projectileCallback?.Invoke(
										projectile.id, 
										_attackUnits[index].position, 
										_buildings[_attackUnits[index].target].worldCenterPosition
									);
								}
								else
								{
									_buildings[_attackUnits[index].target].TakeDamage(
										GetUnitDamage(index) * multiplier, 
										ref _battleGrid, 
										ref _blockedTiles, 
										ref percentage, 
										ref fiftyPercentDestroyed, 
										ref hqDestroyed, 
										ref completelyDestroyed
									);
								}
								_attackUnits[index].attackTimer -= _attackUnits[index].dataUnit.attackSpeed;
								if (_attackUnits[index].attackCallback != null)
								{
									_attackUnits[index].attackCallback.Invoke(
										_attackUnits[index].dataUnit.databaseID, 
										_buildings[_attackUnits[index].target].buildingData.databaseID
									);
								}
							}
						}
					}
				}
				else
				{
					_attackUnits[index].moving = false;
					_attackUnits[index].target = -1;
				}
			}

			if (_attackUnits[index].target < 0)
			{
				_attackUnits[index].moving = false;
				FindTargets(index, _attackUnits[index].dataUnit.priority);
				if(deltaTime > 0 && _attackUnits[index].target >= 0)
				{
					HandleUnit(index, deltaTime);
				}
			}
		}

		private void ListUnitTargets(int index, TargetPriority priority)
		{
			_attackUnits[index].resourceTargets.Clear();
			_attackUnits[index].defenceTargets.Clear();
			_attackUnits[index].otherTargets.Clear();
			if (priority == TargetPriority.Walls)
			{
				priority = TargetPriority.All;
			}
			for (var i = 0; i < _buildings.Count; i++)
			{
				if (_buildings[i].health <= 0 || _buildings[i].GetId() == BuildingID.wall || !IsBuildingCanBeAttacked(_buildings[i].GetId()))
				{
					continue;
				}
				var distance = BattleVector2.Distance(_buildings[i].worldCenterPosition, _attackUnits[index].position);
				switch (_buildings[i].buildingData.id)
				{
					case BuildingID.hq:
					case BuildingID.supplydrop:
					case BuildingID.supplyvault:
					case BuildingID.powerplant:
					case BuildingID.powerstorage:
						_attackUnits[index].resourceTargets.Add(i, distance);
						break;
					case BuildingID.watchtower:
					case BuildingID.rocketturret:
					case BuildingID.motor:
						_attackUnits[index].defenceTargets.Add(i, distance);
						break;
					case BuildingID.wall:
						// Don't include
						break;
					default:
						_attackUnits[index].otherTargets.Add(i, distance);
						break;
				}
			}
		}

		private void FindTargets(int index, TargetPriority priority)
		{
			ListUnitTargets(index, priority);
			if (priority == TargetPriority.Defenses)
			{
				if(_attackUnits[index].defenceTargets.Count > 0)
				{
					 AssignTarget(index, ref _attackUnits[index].defenceTargets);
				}
				else
				{
					FindTargets(index, TargetPriority.All);
					return;
				}
			}
			else if(priority == TargetPriority.Resources)
			{
				if(_attackUnits[index].resourceTargets.Count > 0)
				{
					AssignTarget(index, ref _attackUnits[index].resourceTargets);
				}
				else
				{
					FindTargets(index, TargetPriority.All);
					return;
				}
			}
			else if(priority == TargetPriority.All || priority == TargetPriority.Walls)
			{
				var temp = _attackUnits[index].GetAllTargets();
				if(temp.Count > 0)
				{
					AssignTarget(index, ref temp, priority == TargetPriority.Walls);
				}
			} 
		}

		private bool FindTargetForUnit(BattleUnitData unit)
		{
			var attackMode = unit.dataUnit.attackMode;
			var unitTargets = new List<BattleUnitData>();
			var buildingTargets = new List<BattleBuildingData>();
			
			if (unit.defMode)
			{
				foreach (var attackUnit in _attackUnits)
				{
					if (attackUnit.health <= 0)
					{
						continue;
					}

					if (unit.dataUnit.attackMode == AttackMode.Ground &&
					    attackUnit.dataUnit.movement == UnitMovementType.Fly)
					{
						continue;
					}
					
					unitTargets.Add(attackUnit);
				}

				if (unitTargets.Count == 0)
				{
					return false;
				}
				
				unit.attackTimer = unit.dataUnit.attackSpeed;
				unit.targetUnit = GetClosestUnitTarget(unit, unitTargets);
				AssignUnitTarget(unit);
				return true;
			}

			foreach (var defenceUnit in _defenceUnits)
			{
				if (defenceUnit.health <= 0)
				{
					continue;
				}

				if (unit.dataUnit.attackMode == AttackMode.Ground &&
				    defenceUnit.dataUnit.movement == UnitMovementType.Fly)
				{
					continue;
				}

				unitTargets.Add(defenceUnit);
			}

			foreach (var building in _buildings)
			{
				if (building.health <= 0)
				{
					continue;
				}
				buildingTargets.Add(building);
			}

			GetClosestTarget(unit, buildingTargets, unitTargets);
			AssignUnitTarget(unit);
			return true;
		}

		private BattleUnitData GetClosestUnitTarget(BattleUnitData unit, List<BattleUnitData> targets)
		{
			var closestDistance = 999f;
			BattleUnitData closestTarget = null;
			foreach (var target in targets)
			{
				var distance = BattleVector2.Distance(unit.position, target.position);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestTarget = target;
				}
			}
			return closestTarget;
		}

		private void GetClosestTarget(BattleUnitData unit, List<BattleBuildingData> buildings,
			List<BattleUnitData> units)
		{
			var closestDistance = 999f;
			foreach (var building in buildings)
			{
				var distance = BattleVector2.Distance(unit.position, building.worldCenterPosition);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					unit.targetBuilding = building;
					unit.targetUnit = null;
				}
			}
			
			foreach (var u in units)
			{
				var distance = BattleVector2.Distance(unit.position, u.position);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					unit.targetBuilding = null;
					unit.targetUnit = u;
				}
			}
		}

		private void AssignUnitTarget(BattleUnitData unit)
		{
			Path path = null;
			if (unit.defMode)
			{
				path = GetPathToUnit(unit, true);
			}
			else
			{
				if (unit.targetUnit != null && unit.targetUnit.health > 0)
				{
					path = GetPathToUnit(unit);
				}
				else if (unit.targetBuilding != null && unit.targetBuilding.health > 0)
				{
					path = GetPathToBuilding(unit);
				}
			}
			
			unit.AssignTarget(path);
		}
		
		private Path GetPathToUnit(BattleUnitData unit, bool ignoreWalls = false)
		{
			if (unit.targetUnit == null || unit.targetUnit.health <= 0)
			{
				return null;
			}
			
			var unitGridPosition = BattleMapUtils.WorldToGridPosition(unit.position);
			var targetGridPosition = BattleMapUtils.WorldToGridPosition(unit.targetUnit.position);
			
			var tiles = new List<Path>();
			if(unit.dataUnit.movement == UnitMovementType.Ground)
			{
				#region With Walls Effect
				var closest = -1;
				float distance = 99999;
				var blocks = 999;
				
				var path1 = new Path();
				var path2 = new Path();
				path1.Create(ref _pathBuilder, targetGridPosition, unitGridPosition);
				path2.Create(ref _unlimitedPathBuilder, targetGridPosition, unitGridPosition);
				
				if (path1.points != null && path1.points.Count > 0)
				{
					path1.length = GetPathLength(path1.points);
					var lengthToBlocks = (int)Math.Floor(path1.length / (Constants.BattleTilesWorthOfOneWall * Constants.GridCellSize));
					if (path1.length < distance && lengthToBlocks <= blocks)
					{
						closest = tiles.Count;
						distance = path1.length;
						blocks = lengthToBlocks;
					}
					tiles.Add(path1);
				}


				if (path2.points != null && path2.points.Count > 0)
				{
					path2.length = GetPathLength(path2.points);
					for (var i = 0; i < path2.points.Count; i++)
					{
						for (var j = 0; j < _blockedTiles.Count; j++)
						{
							if (_blockedTiles[j].position.x == path2.points[i].location.x &&
							    _blockedTiles[j].position.y == path2.points[i].location.y)
							{
								if (_blockedTiles[j].id == BuildingID.wall &&
								    _buildings[_blockedTiles[j].index].health > 0)
								{
									path2.blocks.Add(_blockedTiles[j]);
								}

								break;
							}
						}
					}

					if (path2.length < distance && path2.blocks.Count <= blocks)
					{
						closest = tiles.Count;
						distance = path1.length;
						blocks = path2.blocks.Count;
					}

					tiles.Add(path2);
				}

				tiles[closest].points.Reverse();
				if (tiles[closest].blocks.Count > 0)
				{
					var last = tiles[closest].blocks.Last();
					for (var i = tiles[closest].points.Count - 1; i >= 0; i--)
					{
						var x = tiles[closest].points[i].location.x;
						var y = tiles[closest].points[i].location.y;
						tiles[closest].points.RemoveAt(i);
						if(x == last.position.x && y == last.position.y)
						{
							break;
						}
					}
					return tiles[closest];
				}
				else
				{
					return tiles[closest];
				}
				#endregion
			}
			else
			{
				#region Without Walls Effect
				var closest = -1;
				float distance = 99999;
				
				var path = new Path();
				if (path.Create(ref _unlimitedPathBuilder, targetGridPosition, unitGridPosition))
				{
					path.length = GetPathLength(path.points);
					if(path.length < distance)
					{
						closest = tiles.Count;
						distance = path.length;
					}
					tiles.Add(path);
				}
				
				if (closest >= 0)
				{
					tiles[closest].points.Reverse();
					return tiles[closest];
				}
				#endregion
			}
			return null;
		}

		private Path GetPathToBuilding(BattleUnitData unit)
		{
			if(unit.targetBuilding.buildingData.id == BuildingID.wall || unit.targetBuilding.buildingData.id == BuildingID.decoration || unit.targetBuilding.buildingData.id == BuildingID.obstacle)
			{
				return null;
			}

			var unitGridPosition = BattleMapUtils.WorldToGridPosition(unit.position);

			// Get the x and y list of the building's surrounding tiles
			List<int> columns = new();
			List<int> rows = new();
			var startX = unit.targetBuilding.buildingData.x;
			var endX = unit.targetBuilding.buildingData.x + unit.targetBuilding.buildingData.columns - 1;
			var startY = unit.targetBuilding.buildingData.y;
			var endY = unit.targetBuilding.buildingData.y + unit.targetBuilding.buildingData.rows - 1;
			if (unit.dataUnit.movement == UnitMovementType.Ground && unit.targetBuilding.GetId() == BuildingID.wall)
			{
				startX--;
				startY--;
				endX++;
				endY++;
			}
			columns.Add(startX);
			columns.Add(endX);
			rows.Add(startY);
			rows.Add(endY);

			var tiles = new List<Path>();
			if(unit.dataUnit.movement == UnitMovementType.Ground)
			{
				#region With Walls Effect
				var closest = -1;
				float distance = 99999;
				var blocks = 999;
				for (var x = 0; x < columns.Count; x++)
				{
					for (var y = 0; y < rows.Count; y++)
					{
						if (x >= 0 && y >= 0 && x < Constants.GridSize && y < Constants.GridSize)
						{
							var path1 = new Path();
							var path2 = new Path();
							path1.Create(ref _pathBuilder, new Vector2Int(columns[x], rows[y]), unitGridPosition);
							path2.Create(ref _unlimitedPathBuilder, new Vector2Int(columns[x], rows[y]), unitGridPosition);
							if (path1.points != null && path1.points.Count > 0)
							{
								path1.length = GetPathLength(path1.points);
								var lengthToBlocks = (int)Math.Floor(path1.length / (Constants.BattleTilesWorthOfOneWall * Constants.GridCellSize));
								if (path1.length < distance && lengthToBlocks <= blocks)
								{
									closest = tiles.Count;
									distance = path1.length;
									blocks = lengthToBlocks;
								}
								tiles.Add(path1);
							}
							if (path2.points != null && path2.points.Count > 0)
							{
								path2.length = GetPathLength(path2.points);
								for (var i = 0; i < path2.points.Count; i++)
								{
									for (var j = 0; j < _blockedTiles.Count; j++)
									{
										if (_blockedTiles[j].position.x == path2.points[i].location.x && _blockedTiles[j].position.y == path2.points[i].location.y)
										{
											if (_blockedTiles[j].id == BuildingID.wall && _buildings[_blockedTiles[j].index].health > 0)
											{
												path2.blocks.Add(_blockedTiles[j]);
											}
											break;
										}
									}
								}
								if (path2.length < distance && path2.blocks.Count <= blocks)
								{
									closest = tiles.Count;
									distance = path1.length;
									blocks = path2.blocks.Count;
								}
								tiles.Add(path2);
							}
						}
					}
				}
				tiles[closest].points.Reverse();
				if (tiles[closest].blocks.Count > 0)
				{
					for (var i = 0; i < _attackUnits.Count; i++)
					{
						if (_attackUnits[i].health <= 0 || _attackUnits[i].dataUnit.movement != UnitMovementType.Ground || i != _attackUnits.IndexOf(unit) || _attackUnits[i].target < 0 || _attackUnits[i].mainTarget != unit.mainTarget || _attackUnits[i].mainTarget < 0 || _buildings[_attackUnits[i].mainTarget].buildingData.id != BuildingID.wall || _buildings[_attackUnits[i].mainTarget].health <= 0)
						{
							continue;
						}
						var pos = BattleMapUtils.WorldToGridPosition(_attackUnits[i].position);
						var points = _pathBuilder.BuildPath(new Vector2Int(pos.x, pos.y), new Vector2Int(unitGridPosition.x, unitGridPosition.y)).ToList();
						if (!Path.IsValid(ref points, new Vector2Int(pos.x, pos.y), new Vector2Int(unitGridPosition.x, unitGridPosition.y)))
						{
							continue;
						}
						// float dis = GetPathLength(points, false);
						if (id <= Constants.BattleGroupWallAttackRadius)
						{
							var end = _attackUnits[i].path.points.Last().location;
							var path = new Path();
							if (path.Create(ref _pathBuilder, pos, new Vector2Int(end.x, end.y)))
							{
								unit.mainTarget = _buildings.IndexOf(unit.targetBuilding);
								path.blocks = _attackUnits[i].path.blocks;
								path.length = GetPathLength(path.points);
								return path;
							}
						}
					}

					var last = tiles[closest].blocks.Last();
					for (var i = tiles[closest].points.Count - 1; i >= 0; i--)
					{
						var x = tiles[closest].points[i].location.x;
						var y = tiles[closest].points[i].location.y;
						tiles[closest].points.RemoveAt(i);
						if(x == last.position.x && y == last.position.y)
						{
							break;
						}
					}
					unit.mainTarget = _buildings.IndexOf(unit.targetBuilding);
					return tiles[closest];
				}
				else
				{
					return tiles[closest];
				}
				#endregion
			}
			else
			{
				#region Without Walls Effect
				var closest = -1;
				float distance = 99999;
				for (var x = 0; x < columns.Count; x++)
				{
					for (var y = 0; y < rows.Count; y++)
					{
						if(columns[x] >= 0 && rows[y] >= 0 && columns[x] < Constants.GridSize && rows[y] < Constants.GridSize)
						{
							var path = new Path();
							if (path.Create(ref _unlimitedPathBuilder, new Vector2Int(columns[x], rows[y]), unitGridPosition))
							{
								path.length = GetPathLength(path.points);
								if(path.length < distance)
								{
									closest = tiles.Count;
									distance = path.length;
								}
								tiles.Add(path);
							}
						}
					}
				}
				if (closest >= 0)
				{
					tiles[closest].points.Reverse();
					return tiles[closest];
				}
				#endregion
			}
			return null;
		}
		
		private void AssignTarget(int index, ref Dictionary<int, float> targets, bool wallsPriority = false)
		{
			if(wallsPriority)
			{
				var wallPath = GetPathToWall(index, ref targets);
				if(wallPath.Item1 >= 0)
				{
					_attackUnits[index].AssignTarget(wallPath.Item1, wallPath.Item2);
					return;
				}
			}

			var min = targets.Aggregate((a, b) => a.Value < b.Value ? a : b).Key;
			var path = GetPathToBuilding(min, index);
			if(path.Item1 >= 0)
			{
				_attackUnits[index].AssignTarget(path.Item1, path.Item2);
			}
		}

		private (int, Path) GetPathToWall(int unitIndex, ref Dictionary<int, float> targets)
		{
			var unitGridPosition = BattleMapUtils.WorldToGridPosition(_attackUnits[unitIndex].position);
			var tiles = new List<Path>();
			foreach(var target in (targets.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value)))
			{
				var points = _pathBuilder.BuildPath(
					new Vector2Int(_buildings[target.Key].buildingData.x, _buildings[target.Key].buildingData.y), 
					new Vector2Int(unitGridPosition.x, unitGridPosition.y)
				);
				if(Path.IsValid(
					ref points, 
					new Vector2Int(_buildings[target.Key].buildingData.x, _buildings[target.Key].buildingData.y), 
					new Vector2Int(unitGridPosition.x, unitGridPosition.y))
					)
				{
					continue;
				}

				for (var i = 0; i < _attackUnits.Count; i++)
				{
					if(_attackUnits[i].health <= 0 || _attackUnits[i].dataUnit.movement != UnitMovementType.Ground || i != unitIndex || _attackUnits[i].target < 0 || _attackUnits[i].mainTarget != target.Key || _attackUnits[i].mainTarget < 0 || _buildings[_attackUnits[i].mainTarget].buildingData.id != BuildingID.wall || _buildings[_attackUnits[i].mainTarget].health <= 0)
					{
						continue;
					}
					var pos = BattleMapUtils.WorldToGridPosition(_attackUnits[i].position);
					var pts = _pathBuilder.BuildPath(new Vector2Int(pos.x, pos.y), new Vector2Int(unitGridPosition.x, unitGridPosition.y)).ToList();
					if (Path.IsValid(ref pts, new Vector2Int(pos.x, pos.y), new Vector2Int(unitGridPosition.x, unitGridPosition.y)))
					{
						var dis = GetPathLength(pts, false);
						if (id <= Constants.BattleGroupWallAttackRadius)
						{
							var end = _attackUnits[i].path.points.Last().location;
							var p = new Path();
							if (p.Create(ref _pathBuilder, pos, new Vector2Int(end.x, end.y)))
							{
								_attackUnits[unitIndex].mainTarget = target.Key;
								p.blocks = _attackUnits[i].path.blocks;
								p.length = GetPathLength(p.points);
								return (_attackUnits[i].target, p);
							}
						}
					}
				}
				var path = new Path();
				if (path.Create(ref _unlimitedPathBuilder, unitGridPosition, new Vector2Int(_buildings[target.Key].buildingData.x, _buildings[target.Key].buildingData.y)))
				{
					path.length = GetPathLength(path.points);
					for (var i = 0; i < path.points.Count; i++)
					{
						for (var j = 0; j < _blockedTiles.Count; j++)
						{
							if (_blockedTiles[j].position.x == path.points[i].location.x && _blockedTiles[j].position.y == path.points[i].location.y)
							{
								if(_blockedTiles[j].id == BuildingID.wall && _buildings[_blockedTiles[j].index].health > 0)
								{
									var t = _blockedTiles[j].index;
									for (var k = path.points.Count - 1; k >= j; k--)
									{
										path.points.RemoveAt(k);
									}
									path.length = GetPathLength(path.points);
									return (t, path);
								}
								break;
							}
						}
					}
					break;
				}
			}
			return (-1, null);
		}

		private (int, Path) GetPathToBuilding(int buildingIndex, int unitIndex)
		{
			if(_buildings[buildingIndex].buildingData.id == BuildingID.wall || _buildings[buildingIndex].buildingData.id == BuildingID.decoration || _buildings[buildingIndex].buildingData.id == BuildingID.obstacle)
			{
				return (-1, null);
			}

			var unitGridPosition = BattleMapUtils.WorldToGridPosition(_attackUnits[unitIndex].position);

			// Get the x and y list of the building's surrounding tiles
			List<int> columns = new();
			List<int> rows = new();
			var startX = _buildings[buildingIndex].buildingData.x;
			var endX = _buildings[buildingIndex].buildingData.x + _buildings[buildingIndex].buildingData.columns - 1;
			var startY = _buildings[buildingIndex].buildingData.y;
			var endY = _buildings[buildingIndex].buildingData.y + _buildings[buildingIndex].buildingData.rows - 1;
			if (_attackUnits[unitIndex].dataUnit.movement == UnitMovementType.Ground && _buildings[buildingIndex].GetId() == BuildingID.wall)
			{
				startX--;
				startY--;
				endX++;
				endY++;
			}
			columns.Add(startX);
			columns.Add(endX);
			rows.Add(startY);
			rows.Add(endY);

			var tiles = new List<Path>();
			if(_attackUnits[unitIndex].dataUnit.movement == UnitMovementType.Ground)
			{
				#region With Walls Effect
				var closest = -1;
				float distance = 99999;
				var blocks = 999;
				for (var x = 0; x < columns.Count; x++)
				{
					for (var y = 0; y < rows.Count; y++)
					{
						if (x >= 0 && y >= 0 && x < Constants.GridSize && y < Constants.GridSize)
						{
							var path1 = new Path();
							var path2 = new Path();
							path1.Create(ref _pathBuilder, new Vector2Int(columns[x], rows[y]), unitGridPosition);
							path2.Create(ref _unlimitedPathBuilder, new Vector2Int(columns[x], rows[y]), unitGridPosition);
							if (path1.points != null && path1.points.Count > 0)
							{
								path1.length = GetPathLength(path1.points);
								var lengthToBlocks = (int)Math.Floor(path1.length / (Constants.BattleTilesWorthOfOneWall * Constants.GridCellSize));
								if (path1.length < distance && lengthToBlocks <= blocks)
								{
									closest = tiles.Count;
									distance = path1.length;
									blocks = lengthToBlocks;
								}
								tiles.Add(path1);
							}
							if (path2.points != null && path2.points.Count > 0)
							{
								path2.length = GetPathLength(path2.points);
								for (var i = 0; i < path2.points.Count; i++)
								{
									for (var j = 0; j < _blockedTiles.Count; j++)
									{
										if (_blockedTiles[j].position.x == path2.points[i].location.x && _blockedTiles[j].position.y == path2.points[i].location.y)
										{
											if (_blockedTiles[j].id == BuildingID.wall && _buildings[_blockedTiles[j].index].health > 0)
											{
												path2.blocks.Add(_blockedTiles[j]);
											}
											break;
										}
									}
								}
								if (path2.length < distance && path2.blocks.Count <= blocks)
								{
									closest = tiles.Count;
									distance = path1.length;
									blocks = path2.blocks.Count;
								}
								tiles.Add(path2);
							}
						}
					}
				}
				tiles[closest].points.Reverse();
				if (tiles[closest].blocks.Count > 0)
				{
					for (var i = 0; i < _attackUnits.Count; i++)
					{
						if (_attackUnits[i].health <= 0 || _attackUnits[i].dataUnit.movement != UnitMovementType.Ground || i != unitIndex || _attackUnits[i].target < 0 || _attackUnits[i].mainTarget != buildingIndex || _attackUnits[i].mainTarget < 0 || _buildings[_attackUnits[i].mainTarget].buildingData.id != BuildingID.wall || _buildings[_attackUnits[i].mainTarget].health <= 0)
						{
							continue;
						}
						var pos = BattleMapUtils.WorldToGridPosition(_attackUnits[i].position);
						var points = _pathBuilder.BuildPath(new Vector2Int(pos.x, pos.y), new Vector2Int(unitGridPosition.x, unitGridPosition.y)).ToList();
						if (!Path.IsValid(ref points, new Vector2Int(pos.x, pos.y), new Vector2Int(unitGridPosition.x, unitGridPosition.y)))
						{
							continue;
						}
						// float dis = GetPathLength(points, false);
						if (id <= Constants.BattleGroupWallAttackRadius)
						{
							var end = _attackUnits[i].path.points.Last().location;
							var path = new Path();
							if (path.Create(ref _pathBuilder, pos, new Vector2Int(end.x, end.y)))
							{
								_attackUnits[unitIndex].mainTarget = buildingIndex;
								path.blocks = _attackUnits[i].path.blocks;
								path.length = GetPathLength(path.points);
								return (_attackUnits[i].target, path);
							}
						}
					}

					var last = tiles[closest].blocks.Last();
					for (var i = tiles[closest].points.Count - 1; i >= 0; i--)
					{
						var x = tiles[closest].points[i].location.x;
						var y = tiles[closest].points[i].location.y;
						tiles[closest].points.RemoveAt(i);
						if(x == last.position.x && y == last.position.y)
						{
							break;
						}
					}
					_attackUnits[unitIndex].mainTarget = buildingIndex;
					return (last.index, tiles[closest]);
				}
				else
				{
					return (buildingIndex, tiles[closest]);
				}
				#endregion
			}
			else
			{
				#region Without Walls Effect
				var closest = -1;
				float distance = 99999;
				for (var x = 0; x < columns.Count; x++)
				{
					for (var y = 0; y < rows.Count; y++)
					{
						if(columns[x] >= 0 && rows[y] >= 0 && columns[x] < Constants.GridSize && rows[y] < Constants.GridSize)
						{
							var path = new Path();
							if (path.Create(ref _unlimitedPathBuilder, new Vector2Int(columns[x], rows[y]), unitGridPosition))
							{
								path.length = GetPathLength(path.points);
								if(path.length < distance)
								{
									closest = tiles.Count;
									distance = path.length;
								}
								tiles.Add(path);
							}
						}
					}
				}
				if (closest >= 0)
				{
					tiles[closest].points.Reverse();
					return (buildingIndex, tiles[closest]);
				}
				#endregion
			}
			return (-1, null);
		}

		private bool IsBuildingInRange(int unitIndex, int buildingIndex)
		{
			for(var x = _buildings[buildingIndex].buildingData.x; x < _buildings[buildingIndex].buildingData.x + _buildings[buildingIndex].buildingData.columns; x++)
			{
				for(var y = _buildings[buildingIndex].buildingData.y; y < _buildings[buildingIndex].buildingData.y + _buildings[buildingIndex].buildingData.columns; y++)
				{
					var distance = BattleVector2.Distance(BattleMapUtils.GridToWorldPosition(new Vector2Int(x, y)), _attackUnits[unitIndex].position);
					if(distance <= _attackUnits[unitIndex].dataUnit.attackRange)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsUnitInRange(BattleUnitData unit)
		{
			var unitPosition = unit.PositionOnGrid();
			var targetPosition = unit.targetUnit.PositionOnGrid();
			var distance = BattleVector2.Distance(unitPosition, targetPosition);
			return distance <= unit.dataUnit.attackRange;
		}

		private static float GetPathLength(IList<Cell> path, bool includeCellSize = true)
		{
			float length = 0;
			if (path?.Count > 1)
			{
				for (var i = 1; i < path.Count; i++)
				{
					length += BattleVector2.Distance(
						new BattleVector2(path[i - 1].location.x, path[i - 1].location.y), 
						new BattleVector2(path[i].location.x, path[i].location.y)
					);
				}
			}
			
			if (includeCellSize)
			{
				length *= Constants.GridCellSize;
			}
			return length;
		}

		private float GetUnitDamage(int index)
		{
			return _attackUnits[index].dataUnit.damage;
		}

		private float GetUnitMoveSpeed(int index)
		{
			return _attackUnits[index].dataUnit.moveSpeed / 10f;
		}

		private BattleVector2 GetPathPosition(IList<Cell> path, float t)
		{
			t = t < 0 ? 0 : t;
			t = t > 1 ? 1 : t;
			
			var totalLength = GetPathLength(path);
			var length = 0f;
			
			if (path?.Count > 1)
			{
				for(var i = 1; i < path.Count; i++)
				{
					var a = new Vector2Int(path[i - 1].location.x, path[i - 1].location.y);
					var b = new Vector2Int(path[i].location.x, path[i].location.y);
					var pathLength = BattleVector2.Distance(a, b) * Constants.GridCellSize;
					var p = (length + pathLength) / totalLength;
					if(p >= t)
					{
						t = (t - (length / totalLength)) / (p - (length / totalLength));
						return BattleVector2.LerpUnclamped(
							BattleMapUtils.GridToWorldPosition(a), 
							BattleMapUtils.GridToWorldPosition(b), 
							t
						);
					}
					length += pathLength;
				}
			}
			return BattleMapUtils.GridToWorldPosition(new Vector2Int(path[0].location.x, path[0].location.y));
		}

		private bool IsBuildingCanBeAttacked(BuildingID buildingId)
		{
			return buildingId switch
			{
				BuildingID.obstacle => false,
				_ => true
			};
		}
    }
}