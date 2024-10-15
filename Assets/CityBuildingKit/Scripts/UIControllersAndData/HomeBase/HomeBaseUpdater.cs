using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBaseUpdater : MonoBehaviour
{
    [SerializeField] private StructureSelector _structureSelector;
    
    // Start is called before the first frame update
    void Awake()
    {
        _structureSelector.UpdateStructureLevelEvent.AddListener(UpdateStructureLevelHandler);
    }
    private void UpdateStructureLevelHandler()
    {
        Stats.Instance.CurrentHqLevel = _structureSelector.Level;
    }
}
