namespace Assets.Scripts.World
{
    public enum EdgeType { Hull, Internal, Brace }

    public class PhysicsEdge
    {
        public int A;
        public int B;
        public EdgeType Type;

        public float RestLength;
        public float Stiffness = 1f;
        public float YieldStrain = 0.15f;
        public float BreakStrain = 0.6f;
        public float PlasticRate = 0.1f;
    }
}
