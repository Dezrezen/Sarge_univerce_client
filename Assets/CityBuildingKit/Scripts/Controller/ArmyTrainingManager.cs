using System.Collections.Generic;
using System.Linq;
using Data;
using Controller;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace CityBuildingKit.Scripts.Controller
{
    public class ArmyTrainingManager
    {
        // private List<TrainingQueue> _trainingQueues = new();
        //
        // public UnityEvent<UnitID> OnTrainingComplete = new();
        // public UnityEvent<UnitID> OnAddToQueue = new();
        // public UnityEvent<int> OnRemoveFromQueue = new();
        // public UnityEvent<int, int> OnUpdateQueue = new();
        //
        // public void AddUnitToQueue(UnitID id)
        // {
        //     var data = GameConfig.instance.GetUnitsData().GetUnitData(id);
        //     if (data == null) return;
        //
        //     Debug.LogWarning("TODO: Calculate training time based on barracks count");
        //     var trainingTime = data.GetTrainingTime(1);
        //     
        //     if (_trainingQueues.Count == 0 || _trainingQueues.Last().unitId != id)
        //     {
        //         _trainingQueues.Add(new TrainingQueue(id, trainingTime));
        //         OnAddToQueue?.Invoke(id);
        //     }
        //     else
        //     {
        //         var queue = _trainingQueues.Last();
        //         queue.IncreaseUnitsAmount();
        //         OnUpdateQueue?.Invoke(_trainingQueues.Count - 1, queue.amount);
        //     }
        // }
        //
        // public void RemoveUnitFromQueue(int index)
        // {
        //     var queue = _trainingQueues[index];
        //     queue.DecreaseUnitsAmount();
        //
        //     if (queue.timeLeft < 0 || queue.amount <= 0)
        //     {
        //         _trainingQueues.RemoveAt(index);
        //         OnRemoveFromQueue?.Invoke(index);
        //         return;
        //     }
        //     OnUpdateQueue?.Invoke(index, queue.amount);
        // }
        //
        // // Update in FixedUpdate
        // public void UpdateTrainingProgress()
        // {
        //     if (_trainingQueues.Count == 0) return;
        //
        //     var queue = _trainingQueues.First();
        //     Debug.Log(queue.timeLeft + "/" + queue.amount);
        //     queue.timeLeft -= Time.fixedDeltaTime;
        //
        //     if (queue.timeLeft <= 0)
        //     {
        //         OnTrainingComplete?.Invoke(queue.unitId);
        //         OnRemoveFromQueue?.Invoke(0);
        //         _trainingQueues.RemoveAt(0);
        //     }
        //     else
        //     {
        //         if (!(queue.timeLeft / queue.trainingTime < queue.amount - 1)) return;
        //         
        //         queue.amount -= 1;
        //         OnUpdateQueue?.Invoke(0, queue.amount);
        //         OnTrainingComplete?.Invoke(queue.unitId);
        //     }
        // }
    }
    
}