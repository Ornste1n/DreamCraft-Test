using Authoring.Units;
using Components;
using UnityEngine;
using Unity.Entities;

namespace Authoring.Enemies
{
    [RequireComponent(typeof(UnitAuthoring))]
    public class EnemyAuthoring : MonoBehaviour
    {
        [SerializeField] private int AttackDamage;
        
        public class EnemyAuthoringBaker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyTag());
                AddComponent(entity, new EnemyAttackData(authoring.AttackDamage));
            }
        }
    }
}