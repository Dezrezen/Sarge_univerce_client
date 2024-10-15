using System.Collections.Generic;
using Enums;
using UnityEngine.Events;

namespace Controller
{
    public class ArmyManager
    {
        private Dictionary<UnitID, int> _army = new();

        public UnityEvent OnArmyUpdate = new UnityEvent();

        public void AddUnitToArmy(UnitID id)
        {
            if (!_army.TryAdd(id, 1))
                _army[id] += 1;
            
            OnArmyUpdate?.Invoke();
        }

        public void RemoveUnitFromArmy(UnitID id)
        {
            if (!_army.ContainsKey(id)) return;
            
            if (_army[id] > 1)
                _army[id] -= 1;
            else
                _army.Remove(id);
            
            OnArmyUpdate?.Invoke();
        }
    }
}