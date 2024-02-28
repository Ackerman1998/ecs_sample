using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Extensions
{
    //[BurstCompile]
    //public partial struct AddBulletEntity : IJobEntity
    //{
    //    public EntityCommandBuffer.ParallelWriter ECB;
    //    public BufferLookup<PoolBuffer> PoolBufferLookAt;
    //    public ComponentLookup<ToAfterTransform> PoolEntityLookAt;
    //    [NativeDisableParallelForRestriction]
    //    public EntityManager entityManager;
    //    [ReadOnly]
    //    public float deltaTime;
    //    public void Execute(Entity entity, in ToAfterTransform after)
    //    {
    //        var tPointData = entityManager.GetComponentData<ToAfterTransform>(entity);
    //        tPointData.currentLife += deltaTime;
    //        entityManager.SetComponentData<ToAfterTransform>(entity, tPointData);
    //        if (tPointData.currentLife >= tPointData.timeLife)
    //        {
    //            var hasInit = PoolEntityLookAt.TryGetComponent(entity, out var tag);
    //            if (!hasInit)
    //            {
    //                Debug.LogError("是不是忘记初始化了1");
    //            }

    //            if (!PoolBufferLookAt.TryGetBuffer(tag.Tag, out var buffer))
    //            {
    //                Debug.LogError("是不是忘记初始化了2");
    //            }
    //            buffer.Add(new PoolBuffer(entity));
    //            ECB.RemoveComponent<ToAfterTransform>(0, entity);
    //            ECB.SetEnabled(0, entity, false);
    //            entity.Despawn(ECB);
    //        } 
    //    }
    //}

    //public partial struct RemoveToPool : ISystem
    //{
    //    private BufferLookup<PoolBuffer> _poolBufferLookAt;
    //    private ComponentLookup<ToAfterTransform> _poolEntityLookAt;

    //    [BurstCompile]
    //    public void OnCreate(ref SystemState state)
    //    {
    //        state.RequireForUpdate<ToAfterTransform>();
    //        _poolEntityLookAt = state.GetComponentLookup<ToAfterTransform>();
    //        _poolBufferLookAt = state.GetBufferLookup<PoolBuffer>();
    //        Debug.LogError("RemoveToPool CREATE");
    //    }
    //    public void Init()
    //    {
    //        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
    //        var ecb = ecbSingleton.CreateCommandBuffer(entityManager.WorldUnmanaged);
    //        GameEntitiesComponentData data = SystemAPI.GetSingleton<GameEntitiesComponentData>();
    //        using var ecc = new EntityCommandBuffer(Allocator.TempJob);
    //        Debug.LogError("RemoveToPool Init "+ (data.m_EffectBoomPrefabEntity==Entity.Null));
    //        data.m_EffectBoomPrefabEntity.InitPool(ecc.AsParallelWriter());
    //    }
    //    public void CreateBullet(float x, float y) {
    //        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
    //        var ecb = ecbSingleton.CreateCommandBuffer(entityManager.WorldUnmanaged);
    //        GameEntitiesComponentData data = SystemAPI.GetSingleton<GameEntitiesComponentData>();
    //        using var ecc = new EntityCommandBuffer(Allocator.TempJob);
    //        var ee = data.m_EffectBoomPrefabEntity.Spawn(ecc.AsParallelWriter(), _poolEntityLookAt, _poolBufferLookAt);
    //        LocalTransform localParam = LocalTransform.FromPosition(new float3(x, 1, y));
    //        localParam.Scale = 1f;
    //        ecb.SetComponent(ee, localParam);
    //        ecb.AddComponent<ToAfterTransform>(ee, new ToAfterTransform()
    //        {
    //            timeLife = 0.2f,
    //            currentLife = 0f,
    //            direction = new float3(1, 0, 0),
    //            isStart = true,
    //            isStatic = true
    //        });
    //    }
    //    [BurstCompile]
    //    public void OnUpdate(ref SystemState state)
    //    {
    //        _poolBufferLookAt.Update(ref state);
    //        _poolEntityLookAt.Update(ref state);
    //        var entityQuery = SystemAPI.QueryBuilder().WithAll<ToAfterTransform>().Build();
    //        EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);
    //        var tempEntities = entityQuery.ToEntityArray(Allocator.TempJob);
    //        var job = new AddBulletEntity
    //        {
    //            PoolBufferLookAt = _poolBufferLookAt,
    //            PoolEntityLookAt = _poolEntityLookAt,
    //            ECB = ecb,
    //            entityManager = state.EntityManager,
    //            deltaTime = SystemAPI.Time.DeltaTime,
    //        };
    //        state.Dependency = job.Schedule(state.Dependency);
    //        state.Dependency.Complete();
    //        //ecb.Playback(state.EntityManager);
    //    }
    //    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    //    {
    //        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
    //        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    //        return ecb.AsParallelWriter();
    //    }
    //}
}


namespace DOTS.Extensions
{
    //public struct ToAfterTransform : IComponentData
    //{
    //    public float timeLife;
    //    public float currentLife;
    //    public float3 direction;
    //    public bool isStart;
    //    public bool isStatic;
    //    public Entity Tag;
    //}

    //public struct PoolEntityTag : IComponentData
    //{
    //}

    //public struct PoolEntity : IComponentData
    //{
    //    public Entity Tag;
    //}

    //public struct PoolBuffer : IBufferElementData
    //{
    //    public Entity Val;

    //    public PoolBuffer(Entity val)
    //    {
    //        Val = val;
    //    }
    //}

    public static class EntityPool
    {
        public static List<Entity> pools;
        public static void Init() {
            pools = new List<Entity>();
        }
        public static void Destroy() {
            pools.Clear();
        }
        public static void PushEntity(this Entity entity) {
            pools.Add(entity);
        }
        public static Entity GetEntity() {
            if (pools.Count>0) {
                Entity ee = pools[0];
                pools.RemoveAt(0);
                return ee;
            }
            return Entity.Null;
        }
        //public static void InitPool(this Entity entity, EntityCommandBuffer.ParallelWriter ecb)
        //{
        //    var e = ecb.CreateEntity(0);
        //    ecb.AddComponent<PoolEntityTag>(0, e);
        //    ecb.AddBuffer<PoolBuffer>(0, e);
        //    ecb.AddComponent(0, entity, new ToAfterTransform
        //    {
        //        Tag = e
        //    });
        //}

        ///// <summary>
        ///// 创建对象
        ///// </summary>
        ///// <param name="entity">对象</param>
        ///// <param name="ecb">ECB</param>
        ///// <param name="poolLookAt"></param>
        ///// <param name="bufferLookAt">对象池</param>
        ///// <param name="updateTransform">是否自定义LocalTransform</param>
        ///// <param name="transform">自定义的LocalTransform</param>
        ///// <returns></returns>
        //public static Entity Spawn(this Entity entity, EntityCommandBuffer.ParallelWriter ecb,
        //    ComponentLookup<ToAfterTransform> poolLookAt, BufferLookup<PoolBuffer> bufferLookAt, bool updateTransform = false,
        //    LocalTransform transform = default)
        //{
        //    Entity e;
        //    Debug.LogError("是不是忘记初始化了"+ poolLookAt==null);
        //    var hasInit = poolLookAt.TryGetComponent(entity, out var tag);
        //    if (!hasInit)
        //    {
        //        Debug.LogError("是不是忘记初始化了");
        //    }

        //    if (hasInit && bufferLookAt.TryGetBuffer(tag.Tag, out var buffer) &&
        //        buffer.Length > 0)
        //    {
        //        var lastIndex = buffer.Length - 1;
        //        e = buffer[lastIndex].Val;
        //        buffer.RemoveAt(lastIndex);

        //        ecb.SetEnabled(0, e, true);
        //    }
        //    else
        //    {
        //        e = ecb.Instantiate(0, entity);
        //    }

        //    //if (!updateTransform)
        //    //{
        //    //    transform = new LocalTransform
        //    //    {
        //    //        Position = float3.zero,
        //    //        Scale = 1,
        //    //        Rotation = quaternion.identity
        //    //    };
        //    //}

        //    //ecb.SetComponent(0, e, transform);
        //    return e;
        //}

        //private static readonly float3 Float3Out = new(100000, 100000, 100000);

        ///// <summary>
        ///// 回池
        ///// </summary>
        ///// <param name="entity">回池对象</param>
        ///// <param name="ecb">ECB</param>
        ///// <param name="poolLookAt">池子对象</param>
        ///// <param name="bufferLookAt">对象池</param>
        //public static void Despawn(this Entity entity, EntityCommandBuffer.ParallelWriter ecb)
        //{
        //    ecb.SetComponent(0, entity, new LocalTransform
        //    {
        //        Position = Float3Out,
        //        Scale = 100
        //    });
        //    ecb.AddComponent<ToAfterTransform>(0, entity);
        //}
    }
}