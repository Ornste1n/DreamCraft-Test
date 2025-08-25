using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct ProjectileBlob
    {
        public float Damage;
        
        public int Count; // количество за выстрел
        public float Speed;
        public float Lifetime;
        public float SpreadAngle; // угол для определения кучности выстрела
        public float SineAmplitude; // движение по синусоиде
        public float SineFrequency;
    }
    
    public struct ProjectileBlobRoot
    {
        public BlobArray<ProjectileBlob> Array;
    }
    
    public struct ProjectileBlobRef : IComponentData
    {
        public BlobAssetReference<ProjectileBlobRoot> Catalog;
    }

    public struct ProjectilePoolData : IComponentData
    {
        public int PoolSize;
        public int MaxPoolSize;
        public int GrowPerFrame;
        
        public int CurrentIndex;
        public Entity ProjectilePrefab;
    }
    
    public struct ProjectileBufferElement : IBufferElementData
    {
        public Entity Projectile;
    }

    public struct ProjectileData : IComponentData
    {
        public float Damage;
        
        public float3 Direction;
        
        public float Speed;
        public float Lifetime;
        public float TimeAlive;
        public float SineAmplitude;
        public float SineFrequency;
    }
}