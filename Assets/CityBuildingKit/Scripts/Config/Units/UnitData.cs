using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Units;
using UnityEngine;

namespace Config.Units
{
    [System.Serializable]
    public class UnitData
    {
        public string name;
        public UnitID id;
        public Sprite image;
        public Unit unitPrefab;

        public string attackAudioTrack = "";
        public string moveAudioTrack = "";
    }
}