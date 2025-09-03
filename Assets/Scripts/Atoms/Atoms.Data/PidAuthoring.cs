using Unity.Entities;
using UnityEngine;

namespace AtomicSimulation.Core
{
    public class PidAuthoring : MonoBehaviour
    {
        public float kp;
        public float ki;
        public float kd;
        
        // PID state
        public float integral;
        public float previousError;

        public class PidBaker : Baker<PidAuthoring>
        {
            public override void Bake(PidAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PID
                {
                    Kp = authoring.kp, Ki = authoring.ki, Kd = authoring.kd ,
                    Integral = authoring.integral,
                    PreviousError = authoring.previousError
                });
            }
        }
    }
}