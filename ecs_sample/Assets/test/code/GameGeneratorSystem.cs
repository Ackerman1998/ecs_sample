using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[BurstCompile(CompileSynchronously = true)]
public partial struct GameGeneratorSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var dt = SystemAPI.Time.DeltaTime;
        //UnityEngine.Debug.Log("start ************ ");
    
        foreach (var gameGen in SystemAPI.Query<RefRW<GameGenerator>>())
        {
            var gen = state.EntityManager.Instantiate(gameGen.ValueRO.prefab);
            var cx = SystemAPI.GetComponentRW<LocalTransform>(gen);
            cx.ValueRW.Position = new float3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10), 0);
        }
    }
    public void testFunction() {
        //GameGeneratorSystem ecsFacade = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameGeneratorSystem>();
    }
}
