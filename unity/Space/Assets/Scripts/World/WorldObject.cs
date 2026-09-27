using UnityEngine;

namespace Assets.Scripts.World
{
    public class WorldObject : MonoBehaviour
    {
        [SerializeField] public string ObjectName;
        [SerializeField] PhysicsWorld world;

        public PhysicsGraph Graph { get; private set; }

        void Awake()
        {
            Graph = PhysicsGraphSerializer.Load(ObjectName);
            if (Graph.Vertices.Count == 0) Graph.AddVertex(Vector2.zero);
        }

        void OnEnable()
        {
            if (world != null) world.Register(this);
        }

        void OnDisable()
        {
            if (world != null) world.Unregister(this);
        }

        void OnDestroy()
        {
            PhysicsGraphSerializer.Save(Graph, ObjectName);
        }
    }
}
