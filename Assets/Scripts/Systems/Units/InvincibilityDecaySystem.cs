using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

namespace Systems.Units
{
    // Считает время для снятия неуязвимости и снимает
    public partial struct InvincibilityDecaySystem  : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            EntityCommandBuffer ecb = new(Allocator.Temp);

            foreach ((RefRW<InvincibilityFrame> inv, Entity ent) in SystemAPI.Query<RefRW<InvincibilityFrame>>().WithEntityAccess())
            {
                inv.ValueRW.Seconds -= dt;
                if (inv.ValueRW.Seconds <= 0f)
                    ecb.RemoveComponent<InvincibilityFrame>(ent);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}