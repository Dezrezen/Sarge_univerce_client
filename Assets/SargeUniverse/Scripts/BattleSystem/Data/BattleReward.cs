namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public struct BattleReward
    {
        public int lootedSupplies;
        public int lootedPower;
        public int lootedEnergy;
        public int totalSupplies;
        public int totalPower;
        public int totalEnergy;

        public BattleReward(int lSupplies = 0, int lPower = 0, int lEnergy = 0, int tSupplies = 0, int tPower = 0, int tEnergy = 0)
        {
            lootedSupplies = lSupplies;
            lootedPower = lPower;
            lootedEnergy = lEnergy;
            totalSupplies = tSupplies;
            totalPower = tPower;
            totalEnergy = tEnergy;
        }
    }
}