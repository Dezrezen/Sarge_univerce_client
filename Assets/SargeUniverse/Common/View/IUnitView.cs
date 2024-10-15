using System;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Common.View
{
    public interface IUnitView : IInitializable, IDisposable
    {
        Transform UnitTransform { get; }
        Vector3 Position  { get; }
        
        void SetPosition(Vector3 position, bool local = false);
        void SetRotation(Quaternion rotation);
        void SetParent(Transform parent);
    }
}