using System.Collections.Generic;

namespace Model
{
    [System.Serializable]
    public class SaveDataModel
    {
        public List<UnitDataModel> units = new();
        public List<UnitTrainingDataModel> training = new();
    }
}