using UnityEngine;

namespace UnityBase.Command
{
    public interface IMoveEntity
    {
        public Transform MeshTransform { get; }
        public Transform Transform { get; }
        public Vector3 NewPosition { get; }
        public float Speed { get; }
        public bool CanPassNextMovementInstantly { get; }
    }
}