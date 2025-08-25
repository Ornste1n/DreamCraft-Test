using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Components.Aspects;

namespace Systems.Units
{
    public partial struct UnitsProcessDamageSystem : ISystem
    {
        const float DefaultInvincibilitySeconds = 1.0f; // можно вынести
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ComponentLookup<InvincibilityFrame> invLookup = SystemAPI.GetComponentLookup<InvincibilityFrame>(true);
            ComponentLookup<CharacterTag> characterLookup = SystemAPI.GetComponentLookup<CharacterTag>(true);
            
            EntityCommandBuffer ecb = new(Allocator.Temp);

            foreach (UnitAspect unitAspect in SystemAPI.Query<UnitAspect>())
            {
                Entity entity = unitAspect.Entity;
                DynamicBuffer<DamageThisFrame> damageBuffer = unitAspect.DamageBuffer;
                
                if (damageBuffer.IsEmpty) 
                    continue;

                bool isCharacter = characterLookup.HasComponent(entity); // сделал для быстроты так
                
                // считать кадры неуязвимости только для игрока
                if (invLookup.HasComponent(entity) && isCharacter)
                {
                    damageBuffer.Clear();
                    continue;
                }

                float totalDamage = 0;
                foreach (DamageThisFrame damage in damageBuffer)
                    totalDamage += damage.Value;

                damageBuffer.Clear();

                unitAspect.Health -= totalDamage;

                if (unitAspect.Health <= 0 && !isCharacter)
                {
                    ecb.RemoveComponent<ActiveEnemyTag>(entity);
                    ecb.SetEnabled(entity, false);
                }
                else if(unitAspect.Health <= 0 && isCharacter)
                {
                    ecb.AddComponent<DestroyUnitFlag>(unitAspect.Entity);
                }
                
                ecb.AddComponent(entity, new InvincibilityFrame(DefaultInvincibilitySeconds));
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}