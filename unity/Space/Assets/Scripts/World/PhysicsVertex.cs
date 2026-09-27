using UnityEngine;

namespace Assets.Scripts.World
{
    public class PhysicsVertex
    {
        public int Id;
        public Vector2 Position;
        public Vector2 OldPosition;
        public float Mass = 1f;
        public bool Pinned;

        public float InverseMass => Pinned || Mass <= 0f ? 0f : 1f / Mass;
    }
}
