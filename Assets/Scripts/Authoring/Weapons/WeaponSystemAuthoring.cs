using Components;
using UnityEngine;
using Unity.Entities;
using ScriptableObjects;
using Unity.Collections;
using System.Collections.Generic;

namespace Authoring.Weapons
{
    public class WeaponSystemAuthoring : MonoBehaviour
    {
        [SerializeField] private List<WeaponDataSo> WeaponDataList;
        
        public class WeaponSystemAuthoringBaker : Baker<WeaponSystemAuthoring>
        {
            public override void Bake(WeaponSystemAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                List<WeaponDataSo> weaponsData = authoring.WeaponDataList;
                
                BlobBuilder builder = new BlobBuilder(Allocator.Temp);
                ref ProjectileBlobRoot root = ref builder.ConstructRoot<ProjectileBlobRoot>();
                BlobBuilderArray<ProjectileBlob> projectiles = builder.Allocate(ref root.Array, weaponsData.Count);
                
                var weaponSprites = AddBuffer<WeaponMaterialsElement>(entity);
                
                if(authoring.WeaponDataList.Count == 0) return;
                
                for (int i = 0; i < weaponsData.Count; i++)
                {
                    WeaponDataSo dataSo = weaponsData[i];
                    projectiles[i] = new ProjectileBlob
                    {
                        Damage = dataSo.Damage,
                        Speed = dataSo.Speed,
                        Lifetime = dataSo.Lifetime,
                        Count = dataSo.Count,
                        SpreadAngle = dataSo.SpreadAngle,
                        SineAmplitude = dataSo.SineAmplitude,
                        SineFrequency = dataSo.SineFrequency,
                    };

                    weaponSprites.Add(new WeaponMaterialsElement
                    {
                        Material = dataSo.WeaponMaterial
                    });
                }

                BlobAssetReference<ProjectileBlobRoot> blobRef =
                    builder.CreateBlobAssetReference<ProjectileBlobRoot>(Allocator.Persistent);
                
                AddBlobAsset(ref blobRef, out _);
                builder.Dispose();
                
                AddComponent(entity, new ProjectileBlobRef()
                {
                    Catalog = blobRef
                });
            }
        }
    }
}