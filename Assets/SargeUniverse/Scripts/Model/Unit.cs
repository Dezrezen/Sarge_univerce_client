using SargeUniverse.Scripts.Data;
using UnityEngine;

namespace SargeUniverse.Scripts.Model
{
    public class Unit : MonoBehaviour
    {
        private Data_Unit _dataUnit;

        public long DatabaseId => _dataUnit.databaseID;

        public void SetPosition(int x, int y)
        {
            
        }
    }
}