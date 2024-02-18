using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[BurstCompile(CompileSynchronously = true)]
public partial struct SpawnEntitiesSystem : ISystem
{
    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EntitiesComponentData>();
    }

    //public void OnUpdate(ref SystemState state) {
    //    var dt = SystemAPI.Time.DeltaTime;
    //    //UnityEngine.Debug.Log("start ************ ");

    //    foreach (var gameGen in SystemAPI.Query<RefRW<GameGenerator>>())
    //    {
    //        var gen = state.EntityManager.Instantiate(gameGen.ValueRO.prefab);
    //        var cx = SystemAPI.GetComponentRW<LocalTransform>(gen);
    //        cx.ValueRW.Position = new float3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10), 0);
    //    }
    //}
    void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        foreach (var data in SystemAPI.Query<EntitiesComponentData>())
        {
            Unity.Mathematics.Random m_Random = new Unity.Mathematics.Random(1);
            var m_RandomRange = new float4(-data.m_Row * 0.5f, data.m_Row * 0.5f, -data.m_Col * 0.5f, data.m_Col * 0.5f);
            var halfSize = new float2(data.m_Col * 0.5f, data.m_Row * 0.5f);
            for (int i = 0; i < data.m_Row; i++)
            {
                for (int j = 0; j < data.m_Col; j++)
                {
    
                    var entity = state.EntityManager.Instantiate(data.m_PrefabEntity);
                    LocalTransform localParam = LocalTransform.FromPosition(new float3(j - halfSize.x, 0, i - halfSize.y));
                    localParam.Scale = 0.5f;
                    ecb.SetComponent(entity, localParam);
                    ecb.AddComponent<TargetMovePointData>(entity, new TargetMovePointData() {
                        targetPoint = new float3(m_Random.NextFloat(m_RandomRange.x, m_RandomRange.y), 0, m_Random.NextFloat(m_RandomRange.z, m_RandomRange.w)) });
                }
            }
            state.Enabled = false;
        }
    }

    public void testFunction() {
        //GameGeneratorSystem ecsFacade = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameGeneratorSystem>();
    }

    [BurstCompile]
    partial struct MoveEntitiesSystem : ISystem
    {
        Unity.Mathematics.Random m_Random;
        float4 m_RandomRange;
        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TargetMovePointData>();
            if (SystemAPI.TryGetSingleton<EntitiesComponentData>(out var dt))
            {
                //m_RandomRange = new float4(-dt.m_Row * 0.5f, dt.m_Row * 0.5f, -dt.m_Col * 0.5f, dt.m_Col * 0.5f);
                m_RandomRange = new float4(-dt.m_Row * 50f, dt.m_Row * 50f, -dt.m_Col * 50f, dt.m_Col * 50f);
            }
            else
            {
                m_RandomRange = new float4(-100, 100, -100,100);
                //m_RandomRange = new float4(-dt.m_Row * 50f, dt.m_Row * 50f, -dt.m_Col * 50f, dt.m_Col * 50f);
            }
        }
        void OnStartRunning(ref SystemState state)
        {
            if (SystemAPI.TryGetSingleton<EntitiesComponentData>(out var dt))
            {
                //m_RandomRange = new float4(-dt.m_Row * 0.5f, dt.m_Row * 0.5f, -dt.m_Col * 0.5f, dt.m_Col * 0.5f);
                m_RandomRange = new float4(-dt.m_Row * 50f, dt.m_Row * 50f, -dt.m_Col * 50f, dt.m_Col * 50f);
            }
            else
            {
                m_RandomRange = new float4(-50, 50, -50, 50);
                //m_RandomRange = new float4(-dt.m_Row * 50f, dt.m_Row * 50f, -dt.m_Col * 50f, dt.m_Col * 50f);
            }
        }
        void OnDestroy(ref SystemState state)
        {

        }

        void OnUpdate(ref SystemState state)
        {
            //UnityEngine.Debug.LogError("MoveEntitiesSystem entity:111");
            EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);
            m_Random = new Unity.Mathematics.Random((uint)Time.frameCount);
            var entityQuery = SystemAPI.QueryBuilder().WithAll<TargetMovePointData>().Build();
            var tempEntities = entityQuery.ToEntityArray(Allocator.TempJob);
            //Debug.LogError("m_RandomRange = "+ m_RandomRange);
            var moveJob = new MoveEntitiesJob
            {
                random = m_Random,
                moveSpeed = SystemAPI.Time.DeltaTime * 10,
                randomRange = m_RandomRange,
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
    struct TargetMovePointData : IComponentData
    {
        public float3 targetPoint;
    }
    [BurstCompile]
    partial struct MoveEntitiesJob : IJobParallelFor
    {
        [ReadOnly]
        public Unity.Mathematics.Random random;
        [ReadOnly]
        public float4 randomRange;

        [ReadOnly]
        public float moveSpeed;

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
            var tPointData = entityManager.GetComponentData<TargetMovePointData>(entity);
            var tPoint = tPointData.targetPoint;
            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            //UnityEngine.Debug.LogError("MoveEntitiesJob Execute:111");
            float3 curPoint = transform.Position;
            var offset = tPoint - curPoint;
            if (math.lengthsq(offset) < 0.4f)
            {
                tPointData.targetPoint = new float3(random.NextFloat(randomRange.x, randomRange.y), 0, random.NextFloat(randomRange.z, randomRange.w));
                entityWriter.SetComponent(index, entity, tPointData);
            }

            float3 moveDir = math.normalize(tPointData.targetPoint - curPoint);
            transform.Rotation = Quaternion.LookRotation(moveDir);
            transform.Position += moveDir * moveSpeed;
         
            entityWriter.SetComponent(index, entity, transform);
        }
    }
}
