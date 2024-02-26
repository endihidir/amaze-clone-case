using UnityEngine;

namespace UnityBase.Command
{
    public interface IMoveEntity
    {
        public Transform Transform { get; }
        public Vector3 NewPosition { get; }
        public float Duration { get; }
        public bool CanPassNextMovementInstantly { get; }
    }
}