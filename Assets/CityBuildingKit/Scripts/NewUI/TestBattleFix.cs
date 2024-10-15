using System.Linq;
using UnityEngine;

public class TestBattleFix : MonoBehaviour
{
    public void Attack()
    {
        // var units = FindObjectsOfType<FighterController>().ToList();
        // foreach (var unit in units) unit.FindAndAttack();
        // Helios.Instance.surroundIndex[0] = 0;
    }

    public void AttackPreferred()
    {
        var units = FindObjectsOfType<FighterController>().ToList();
        foreach (var unit in units)
            // if (unit.UnitData.GetPreferredTarget() != EntityType.Any)
            unit.FindAndAttackPreferred();
        Helios.Instance.surroundIndex[0] = 0;
    }
}