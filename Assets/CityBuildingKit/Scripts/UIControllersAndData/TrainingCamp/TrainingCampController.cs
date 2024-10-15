using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainingCampController : MonoBehaviour
{
    public static TrainingCampController Instance;

    public List<TrainingCamp> ListOfTrainingCamp { get; set; } = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddToList(TrainingCamp trainingCamp)
    {
        if (!ListOfTrainingCamp.Contains(trainingCamp)) ListOfTrainingCamp.Add(trainingCamp);
    }

    public void RemoveFromList(TrainingCamp trainingCamp)
    {
        ListOfTrainingCamp.Remove(trainingCamp);
    }

    public TrainingCamp GetTrainingCampByUniqId(string id)
    {
        if (ListOfTrainingCamp.Count > 0) return ListOfTrainingCamp.FirstOrDefault(x => x.UniqGuid == id);
        return null;
    }
}