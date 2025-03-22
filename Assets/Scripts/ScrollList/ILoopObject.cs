using UnityEngine;

namespace Game
{
    public interface ILoopObject
    {
        RectTransform objTrans { get; }
        void DespawnObject();
        void LoopIndexRefresh(int index);
        void SetLoopObjectLocalPosition(Vector3 localPos);
    }
}