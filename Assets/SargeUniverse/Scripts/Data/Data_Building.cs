using System;
using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.Data
{
    public class Data_Building
    {
        public BuildingID id = BuildingID.hq;
        public int level = 1;
        public long databaseID = 0;
        public int x = 0;
        public int y = 0;
        public int warX = -1;
        public int warY = -1;
        public int columns = 0;
        public int rows = 0;
        public int suppliesStorage = 0;
        public int powerStorage = 0;
        public int energyStorage = 0;
        public DateTime boost;
        public int health = 100;
        public float damage = 0;
        public int capacity = 0;
        public int suppliesCapacity = 0;
        public int powerCapacity = 0;
        public int energyCapacity = 0;
        public float speed = 0;
        public float radius = 0;
        public DateTime constructionTime;
        public bool isConstructing = false;
        public int buildTime = 0;
        public AttackMode attackMode = AttackMode.None;
        public float blindRange = 0;
        public float splashRange = 0;
        public float rangedSpeed = 5;
        public double percentage = 0;
        public bool isPlayerBuilding = false;
    }
}