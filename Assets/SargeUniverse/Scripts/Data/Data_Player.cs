using System;
using System.Collections.Generic;

namespace SargeUniverse.Scripts.Data
{
    public class Data_Player
    {
        public long id = 0;
        public string name = "Player";
        public int gems = 0;
        public int trophies = 0;
        public bool banned = false;
        public DateTime nowTime;
        public DateTime shield;
        public int xp = 0;
        public int level = 1;
        public DateTime clanTimer;
        public long clanID = 0;
        public int clanRank = 0;
        public long warID = 0;
        public string email = "";
        public int layout = 0;
        public DateTime shield1;
        public DateTime shield2;
        public DateTime shield3;
        public List<Data_Building> buildings = new();
        public List<Data_Unit> units = new();
    }
}