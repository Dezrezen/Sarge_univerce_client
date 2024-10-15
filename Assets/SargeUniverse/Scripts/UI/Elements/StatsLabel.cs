using TMPro;
using UnityEngine;

public class StatsLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _labelText = null;
    [SerializeField] private TMP_Text _infoText = null;

    public void SetLabelText(string label, string text)
    {
        gameObject.name = label.Trim();
        _labelText.text = label;
        _infoText.text = text;
    }
}
