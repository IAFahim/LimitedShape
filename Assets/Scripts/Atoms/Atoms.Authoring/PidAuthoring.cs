using AtomicSimulation.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Atoms.Atoms.Authoring
{
    // Updated PidAuthoring with proper values and missing fields
    public class PidAuthoring : MonoBehaviour
    {
        [Header("PID Gains")]
        [Tooltip("Proportional gain - higher values = stronger response to current error")]
        public float kp = 10f;
        
        [Tooltip("Integral gain - higher values = stronger response to accumulated error")]
        public float ki = 1f;
        
        [Tooltip("Derivative gain - higher values = stronger response to rate of error change")]
        public float kd = 5f;
        
        [Header("PID Limits")]
        [Tooltip("Maximum force the PID can apply")]
        public float maxForce = 25f;
        
        [Tooltip("Clamp integral to prevent windup")]
        public float integralClamp = 10f;
        
        [Header("PID State (Runtime)")]
        [Tooltip("Integral accumulation - usually leave at zero")]
        public float3 integral = float3.zero;
        
        [Tooltip("Previous error for derivative calculation - usually leave at zero")]
        public float3 previousError = float3.zero;

        public class PidBaker : Baker<PidAuthoring>
        {
            public override void Bake(PidAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PID
                {
                    Kp = authoring.kp,
                    Ki = authoring.ki,
                    Kd = authoring.kd,
                    MaxForce = authoring.maxForce,
                    IntegralClamp = authoring.integralClamp,
                    Integral = authoring.integral,
                    PreviousError = authoring.previousError
                });
            }
        }
    }
}
