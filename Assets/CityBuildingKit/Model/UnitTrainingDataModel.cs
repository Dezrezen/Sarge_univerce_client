using System;
using Enums;

namespace Model
{
    [System.Serializable]
    public class UnitTrainingDataModel
    {
        public UnitID id;
        public int amount;
        public DateTime trainingStartTime;
    }
}