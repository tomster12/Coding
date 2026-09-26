using System;
using UnityEngine;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Data
{
    public class GraphBody
    {
        public Graph Graph = new();
        public Vector2 Position;
        public float Angle;
        public Vector2 Velocity;
        public float AngularVel;
        public float Mass = 1f;
        public float Inertia = 1f;
        public float InverseMass => Mass > 0 ? 1f / Mass : 0f;
        public float InverseInertia => Inertia > 0 ? 1f / Inertia : 0f;

        public Vector2 WorldPosition(Vector2 localPos)
        {
            float cos = Mathf.Cos(Angle);
            float sin = Mathf.Sin(Angle);

            return Position + new Vector2(
                localPos.x * cos - localPos.y * sin,
                localPos.x * sin + localPos.y * cos
            );
        }

        public Vector2 WorldPosition(GraphVertex v) => WorldPosition(v.Position);

        public Vector2 LocalPosition(Vector2 worldPos)
        {
            float cos = Mathf.Cos(-Angle);
            float sin = Mathf.Sin(-Angle);
            Vector2 d = worldPos - Position;
            return new Vector2(d.x * cos - d.y * sin, d.x * sin + d.y * cos);
        }

        public void Integrate(float dt)
        {
            Position += Velocity * dt;
            Angle += AngularVel * dt;
        }

        public void ApplyForce(Vector2 force, float dt)
        {
            Velocity += force * InverseMass * dt;
        }

        public void ApplyTorque(float torque, float dt)
        {
            AngularVel += torque * InverseInertia * dt;
        }

        public void ApplyImpulse(Vector2 impulse, Vector2 localPoint)
        {
            Velocity += impulse * InverseMass;
            AngularVel += Geometry.Cross(localPoint, impulse) * InverseInertia;
        }
    }
}
