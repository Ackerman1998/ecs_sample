//using UnityEngine;
//using Unity.Entities;
//using Unity.Burst;
//using Unity.Physics;

//public partial struct ChangeColliderSystem : ISystem
//{
//    [BurstCompile]
//    public partial struct ChangeColliderJob : IJobEntity
//    {
//        [WithAll(typeof(ChangeColliderFilterJob))]
//        public void Execute(ref PhysicsCollider collider)
//        {
//            collider.Value.Value.SetCollisionFilter(CollisionFilter.Zero);
//        }
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        state.Dependency = new ChangeColliderJob().Schedule(state.Dependency);
//    }
//}