namespace Common.Model
{
    [System.Serializable]
    public class PlayerDataModel
    {
        public int supplies;
        public int maxSupplies;
        public int energy;
        public int maxEnergy;
        public int gems;
        public int workers;
        public int maxWorkers;

        public PlayerDataModel()
        {
            supplies = 10000;
            maxSupplies = 25000;
            energy = 10000;
            maxEnergy = 25000;
            gems = 100;
            workers = 1;
            maxWorkers = 1;
        }
    }
}