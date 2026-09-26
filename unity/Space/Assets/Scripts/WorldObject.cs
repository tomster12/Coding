using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts
{
    public class WorldObject : MonoBehaviour
    {
        [SerializeField] public string ObjectName;
        public GraphBody Body { get; private set; } = null;

        private void Awake()
        {
            Debug.Log("Loading");
            Body = GraphBodySerializer.Load(ObjectName);
        }

        void OnDestroy()
        {
            Debug.Log("Saving");
            GraphBodySerializer.Save(Body, ObjectName);
        }
    }
}
