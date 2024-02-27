using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GameEntitiesAuthoring : MonoBehaviour
{
    public GameObject player;
    public GameObject npc;
    public GameObject bullet;
    public GameObject scene;
    public GameObject effect_Boom;
}
public class GameEntitiesAuthoringBaker : Baker<GameEntitiesAuthoring>
{
    private int instanceId = 10000;
    public override void Bake(GameEntitiesAuthoring authoring)
    {
        var data = new GameEntitiesComponentData
        {
            m_PrefabEntity = GetEntity(authoring.player, TransformUsageFlags.Dynamic),
            m_BulletPrefabEntity = GetEntity(authoring.bullet, TransformUsageFlags.Dynamic),
            m_NpcPrefabEntity = GetEntity(authoring.npc, TransformUsageFlags.Dynamic),
            m_ScenePrefabEntity = GetEntity(authoring.scene, TransformUsageFlags.Dynamic),
            m_EffectBoomPrefabEntity = GetEntity(authoring.effect_Boom, TransformUsageFlags.Dynamic),
        };
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), data);
    }
}
public struct GameEntitiesComponentData : IComponentData
{
    public Entity m_PrefabEntity;
    public Entity m_BulletPrefabEntity;
    public Entity m_NpcPrefabEntity;
    public Entity m_ScenePrefabEntity;
    public Entity m_EffectBoomPrefabEntity;
}
struct PlayerBulletData : IComponentData
{
    public float timeLife;
    public float currentLife;
    public float3 direction;
    public bool isStart;
    public bool isStatic;
}
struct RecycleBulletData : IComponentData {
    public bool isRecycle;
}
public struct PoolBuffer : IBufferElementData
{
    public Entity Val;

    public PoolBuffer(Entity val)
    {
        Val = val;
    }
}
public struct PoolEntityTag : IComponentData
{
}

public struct PoolEntity : IComponentData
{
    public Entity Tag;
}