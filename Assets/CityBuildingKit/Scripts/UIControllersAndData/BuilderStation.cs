using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderStation : MonoBehaviour
{
    [SerializeField] private bool _isAddBuilderAtStartToStats;
    [SerializeField] private int _buildersAmount;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_isAddBuilderAtStartToStats)
        {
            Stats.Instance.builders += _buildersAmount;
        }
    }
}
