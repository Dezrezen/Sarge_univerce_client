using Enums;

namespace Data
{
    [System.Serializable]
    public class TrainingQueue
    {
        public UnitID unitId;
        public int amount;
        public float trainingTime;
        public float timeLeft;

        public TrainingQueue(UnitID id, float time)
        {
            unitId = id;
            amount = 1;
            trainingTime = time;
            timeLeft = time;
        }

        public void IncreaseUnitsAmount()
        {
            amount += 1;
            timeLeft += trainingTime;
        }

        public void DecreaseUnitsAmount()
        {
            amount -= 1;
            timeLeft -= trainingTime;
        }
    }
}