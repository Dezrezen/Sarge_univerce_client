using System;
using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.Data
{
    public class Data_Unit
    {
        public UnitID id = UnitID.rifleman;
        public int level = 0;
        public long databaseID = 0;
        public int hosing = 1;
        public bool trained = false;
        public bool ready = false;
        public int health = 0;
        public int trainTime_1 = 0;
        public int trainTime_2 = 0;
        public int trainTime_3 = 0;
        public int trainTime_4 = 0;
        public DateTime trainStartTime;
        public DateTime trainEndTime;
        public float moveSpeed = 1;
        public float attackSpeed = 1;
        public float attackRange = 1;
        public float damage = 1;
        public float splashRange = 0;
        public float rangedSpeed = 5;
        public AttackMode attackMode = AttackMode.All;
        public TargetPriority priority = TargetPriority.None;
        public UnitMovementType movement = UnitMovementType.Ground;
        public float priorityMultiplier = 1;
    }
}