using Unity.Burst;
using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Units
{
    // Система для обнуления InverseInertia у игрока (через инспектор никак не смог сделать)
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct UnitsFreezeRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (RefRW<PhysicsMass> physicsMass in SystemAPI.Query<RefRW<PhysicsMass>>())
                physicsMass.ValueRW.InverseInertia = float3.zero;

            state.Enabled = false;
        }
    }
}