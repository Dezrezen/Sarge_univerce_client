using UnityEngine;

public class TestUnit : MonoBehaviour
{
    [SerializeField] private Selector _selector;

    [SerializeField] private int _id;
    [SerializeField] private int _level;

    // Start is called before the first frame update
    private void Start()
    {
        _selector.ID = _id;
        _selector.Level = _level;
    }
}