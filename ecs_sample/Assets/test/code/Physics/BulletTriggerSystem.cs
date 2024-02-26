using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Entities;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using UnityEngine;
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StatefulTriggerEventSystem))]
public partial struct BulletTriggerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerBulletData>();
    }
    public void OnUpdate(ref SystemState state) {
        var ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
        .CreateCommandBuffer(state.WorldUnmanaged);

        var nonTriggerQuery = SystemAPI.QueryBuilder().WithNone<StatefulTriggerEvent>().Build();
        var nonTriggerMask = nonTriggerQuery.GetEntityQueryMask();
        //Assert.IsFalse(nonTriggerQuery.HasFilter(),
        //    "The use of EntityQueryMask in this system will not respect the query's active filter settings.");
        foreach (var (triggerEventBuffer, changeMaterial, entity) in
             SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>, RefRW<PlayerBulletData>>()
                 .WithEntityAccess()) {
            for (int i = 0; i < triggerEventBuffer.Length; i++) {
                var triggerEvent = triggerEventBuffer[i];
                var otherEntity = triggerEvent.GetOtherEntity(entity);
                //Debug.LogError("triggerEventBuffer.Length = "+ triggerEventBuffer.Length.ToString());
                // exclude other triggers and processed events
                if (triggerEvent.State == StatefulEventState.Stay ||
                    !nonTriggerMask.MatchesIgnoreFilter(otherEntity))
                {
                    continue;
                }
                if (triggerEvent.State == StatefulEventState.Enter)
                {
                    ecb.DestroyEntity(otherEntity);
                    ecb.DestroyEntity(entity);
                }
                else
                {

                }
            }
        }
    }
}
