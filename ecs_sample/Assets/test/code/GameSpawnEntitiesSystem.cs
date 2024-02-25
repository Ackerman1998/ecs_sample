using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Entities.EntitiesJournaling;
using static UnityEngine.EventSystems.EventTrigger;
[BurstCompile(CompileSynchronously = true)]
public partial struct GameSpawnEntitiesSystem : ISystem
{
    [WriteOnly]
    NativeList<Entity> bulletsPool;
    void OnCreate(ref SystemState state)
    {
        bulletsPool = new NativeList<Entity>();
        state.RequireForUpdate<GameEntitiesComponentData>();
    }
    //创建实体
    public Entity CreateInstance(int x,int y) {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(entityManager.WorldUnmanaged);
        GameEntitiesComponentData data = SystemAPI.GetSingleton<GameEntitiesComponentData>();
        var ee = entityManager.Instantiate(data.m_PrefabEntity);
        LocalTransform localParam = LocalTransform.FromPosition(new float3(x, 1, y));
        localParam.Scale = 1f;
        ecb.SetComponent(ee, localParam);
        ecb.AddComponent<PlayerMoveData>(ee, new PlayerMoveData()
        {
            direction = new float3(0,0,0)
        }); ;
        return ee;
    }
    public Entity CreateNpc(int x, int y)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(entityManager.WorldUnmanaged);
        GameEntitiesComponentData data = SystemAPI.GetSingleton<GameEntitiesComponentData>();
        var ee = entityManager.Instantiate(data.m_NpcPrefabEntity);
        LocalTransform localParam = LocalTransform.FromPosition(new float3(x, 2, y));
        localParam.Scale = 1f;
        ecb.SetComponent(ee, localParam);
  
        return ee;
    }
    public Entity CreateScene(int x, int y)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(entityManager.WorldUnmanaged);
        GameEntitiesComponentData data = SystemAPI.GetSingleton<GameEntitiesComponentData>();
        var ee = entityManager.Instantiate(data.m_ScenePrefabEntity);
        LocalTransform localParam = LocalTransform.FromPosition(new float3(x, 0, y));
        localParam.Scale = 1f;
        ecb.SetComponent(ee, localParam);

        return ee;
    }
    public Entity CreateBullet(float x,float y)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(entityManager.WorldUnmanaged);
        GameEntitiesComponentData data = SystemAPI.GetSingleton<GameEntitiesComponentData>();
        var ee = entityManager.Instantiate(data.m_BulletPrefabEntity);
        float _x = GamePlayer._instance.uIJoyist.directionTempVec.x;
        float _z = GamePlayer._instance.uIJoyist.directionTempVec.y;
        if (_x==0&& _z==0) {
            _x = 0;
            _z = 1;
        }
        LocalTransform localParam = LocalTransform.FromPosition(new float3(x, 1, y));
        localParam.Scale = 0.33f;
        ecb.SetComponent(ee, localParam);
        ecb.AddComponent<PlayerBulletData>(ee, new PlayerBulletData()
        {
            timeLife = 1f,
            currentLife = 0f,
            direction = new float3(_x, 0, _z),
            isStart = true
        }) ;
        return ee;
    }

    public void RecycleBullet(Entity ee) {
        //Debug.Log("RecycleBullet "+ee.Index);
        bulletsPool.Add(ee);
    }
    void OnUpdate(ref SystemState state) { 
    
    }
    [BurstCompile]
    partial struct MovePlayerSystem : ISystem {
        void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PlayerMoveData>();
        }
        void OnUpdate(ref SystemState state) {
            EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);
            var entityQuery = SystemAPI.QueryBuilder().WithAll<PlayerMoveData>().Build();
            var tempEntities = entityQuery.ToEntityArray(Allocator.TempJob);
            float x = GamePlayer._instance.uIJoyist.directionVec.x;
            float z = GamePlayer._instance.uIJoyist.directionVec.y;
            var moveJob = new MovePlayerJob
            {
                moveSpeed = SystemAPI.Time.DeltaTime * 7.5F,
                m_direction = new float3(x,0,z),
                entities = tempEntities,
                entityManager = state.EntityManager,
                entityWriter = ecb
            };
            var moveJobHandle = moveJob.Schedule(tempEntities.Length, 64);
            moveJobHandle.Complete();
            tempEntities.Dispose();//释放完成的entity的job
        }
        private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }
    }
    [BurstCompile]
    partial struct MoveBulletSystem : ISystem
    {
        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerBulletData>();
        }
        void OnUpdate(ref SystemState state) {
            
            EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);
            var entityQuery = SystemAPI.QueryBuilder().WithAll<PlayerBulletData>().Build();
            var tempEntities = entityQuery.ToEntityArray(Allocator.TempJob);
            var moveJob = new MoveBulletJob
            {
                moveSpeed = SystemAPI.Time.DeltaTime * 10,
                deltaTime = SystemAPI.Time.DeltaTime ,
                entities = tempEntities,
                entityManager = state.EntityManager,
                entityWriter = ecb
            };
            var moveJobHandle = moveJob.Schedule(tempEntities.Length, 64);
            moveJobHandle.Complete();
            tempEntities.Dispose();//释放完成的entity的job
        }
        private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }
    }
    struct PlayerMoveData : IComponentData
    {
        public float3 direction;
    }
  
    [BurstCompile]
    partial struct MovePlayerJob : IJobParallelFor {
        [ReadOnly]
        public float moveSpeed;
        [ReadOnly]
        public float3 m_direction;
        [ReadOnly]
        public NativeArray<Entity> entities;
        [NativeDisableParallelForRestriction]
        public EntityManager entityManager;
        [WriteOnly]
        public EntityCommandBuffer.ParallelWriter entityWriter;
        [BurstCompile]
        public void Execute(int index) {
            var entity = entities[index];
            var tPointData = entityManager.GetComponentData<PlayerMoveData>(entity);
          
            //var direction = tPointData.direction;
            var direction = m_direction;
            if (direction.Equals(float3.zero)) {
                return;
            }
            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            float3 curPoint = transform.Position;
            var offset = direction ;
            transform.Rotation = Quaternion.LookRotation(offset);
            transform.Position += offset * moveSpeed * 1;
            entityWriter.SetComponent(index, entity, transform);
        }
    }
    [BurstCompile]
    partial struct MoveBulletJob : IJobParallelFor
    {
        [ReadOnly]
        public float moveSpeed;
        [ReadOnly]
        public float deltaTime;
        [ReadOnly]
        public NativeArray<Entity> entities;
        [NativeDisableParallelForRestriction]
        public EntityManager entityManager;
        [WriteOnly]
        public EntityCommandBuffer.ParallelWriter entityWriter;
        [BurstCompile]
        public void Execute(int index)
        {
            var entity = entities[index];
            var tPointData = entityManager.GetComponentData<PlayerBulletData>(entity);
            tPointData.currentLife += deltaTime;
            entityManager.SetComponentData<PlayerBulletData>(entity, tPointData);
            if (tPointData.currentLife >= tPointData.timeLife)
            {
                entityWriter.RemoveComponent<PlayerBulletData>(0, entity);
                entityWriter.SetEnabled(index,entity, false);
                entityWriter.AddComponent<RecycleBulletData>(index, entity,new RecycleBulletData()
                {
                    isRecycle = true
                });
                //SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
                //GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
                //var hasInit = PoolEntityLookAt.TryGetComponent(entity, out var tag);
                //if (!PoolBufferLookAt.TryGetBuffer(tag.Tag, out var buffer))
                //{

                //}
                //else {
                //    buffer.Add(new PoolBuffer(entity));
                //}
             
                //spawnEntitiesSystem.RecycleBullet(entity);
                entityWriter.DestroyEntity(index, entity);
                //tPointData.isStart = false;
                return;
            }
            //var direction = tPointData.direction;
            var direction = tPointData.direction;
            if (direction.Equals(float3.zero))
            {
                return;
            }
            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            float3 curPoint = transform.Position;
            var offset = direction;
            transform.Rotation = Quaternion.LookRotation(offset);
            transform.Position += offset * moveSpeed * 2;
            entityWriter.SetComponent(index, entity, transform);
        }
    }
}
