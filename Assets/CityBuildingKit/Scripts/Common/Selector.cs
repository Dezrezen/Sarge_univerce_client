using UnityEngine;

public class Selector : MonoBehaviour
{
    //many game items need an index and a isSelected flag

    public bool isSelected = true;

    public int
        index = -1,
        iRow,
        jCol;

    public int ID { get; set; }

    public int Level { get; set; }
}