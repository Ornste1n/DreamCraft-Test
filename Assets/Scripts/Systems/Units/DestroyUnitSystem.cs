using Components;
using Unity.Entities;

namespace Systems.Units
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial struct DestroyUnitSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var simCommBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = simCommBuffer.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach ((DestroyUnitFlag flag, Entity ent) in SystemAPI.Query<DestroyUnitFlag>().WithEntityAccess())
            {
                ecb.DestroyEntity(ent);
            }
        }
    }
}