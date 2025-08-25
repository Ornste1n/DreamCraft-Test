using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using Unity.Collections;
using Unity.Mathematics;
using Authoring.Character;
using InvincibilityFrame = Components.InvincibilityFrame;

namespace Systems.Units.Rendering
{
    // Эффект неуязвимости
    [BurstCompile]
    public partial struct InvincibilityEffectApplySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InvincibilityFrame>();
            state.RequireForUpdate<InvincibilityEffectComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityManager em = state.EntityManager;
            EntityCommandBuffer ecb = new(Allocator.Temp);
            InvincibilityEffectComponent effectData = SystemAPI.GetSingleton<InvincibilityEffectComponent>();
            
            foreach ((RefRW<InvincibilityFrame> inv, Entity ent) in SystemAPI.Query<RefRW<InvincibilityFrame>>().WithEntityAccess())
            {
                float blink = math.abs(math.sin((float)SystemAPI.Time.ElapsedTime * effectData.BlinkSpeed)) * 0.5f + 0.5f;
                float intensity = math.clamp(blink, 0f, 1f);

                URPMaterialPropertyBaseColor newColor = new()
                {
                    Value = new float4(effectData.EffectColor.xyz, intensity)
                };

                if (!em.HasComponent<URPMaterialPropertyBaseColor>(ent))
                    ecb.AddComponent(ent, newColor); // накладываю эффект
                else
                    em.SetComponentData(ent, newColor);
            }

            ecb.Playback(em);
            ecb.Dispose();
        }
    }

    // Убираю эффект с помощью URPMaterialPropertyBaseColor
    [BurstCompile]
    public partial struct InvincibilityEffectCleanupSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<URPMaterialPropertyBaseColor>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);

            foreach ((RefRW<URPMaterialPropertyBaseColor> mat, Entity ent) in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>>().WithEntityAccess())
            {
                if (!SystemAPI.HasComponent<InvincibilityFrame>(ent))
                    ecb.RemoveComponent<URPMaterialPropertyBaseColor>(ent);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}