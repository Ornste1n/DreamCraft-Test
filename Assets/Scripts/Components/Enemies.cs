using Unity.Entities;

namespace Components
{
    public struct EnemyTag : IComponentData { }

    public struct EnemyAttackData : IComponentData
    {
        public float Damage;

        public EnemyAttackData(float value) => Damage = value;
    }
    
    public struct EnemyPoolSettings : IComponentData
    {
        public Entity Prefab;
        public int PoolSize;
        public int MaxPoolSize;
    }
    
    public struct EnemyPoolElement : IBufferElementData
    {
        public Entity Enemy;
    }
    
    public struct ActiveEnemyTag : IComponentData { }
    public struct EnemyPoolInitialized : IComponentData { }
}