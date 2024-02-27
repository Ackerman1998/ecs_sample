using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class test_MainHandler : MonoBehaviour
{
    EntityArchetype tempAchetype;
    private Entity swordEntity;
    EntityManager entityManager;
    public Text numTotal;
    private void Awake()
    {
         entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
             
        //       tempAchetype = _manager.CreateArchetype(
        //typeof(Translation),
        //typeof(Target)
        //);
    }
    public void ButtonStart() {
        GameObject.Find("Canvas/START").gameObject.SetActive(false);
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<SpawnEntitiesSystem>();
        SpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<SpawnEntitiesSystem>(ssh);
        spawnEntitiesSystem.Create();
    }
    private void Update()
    {
        numTotal.text = entityManager.GetAllEntities().Length.ToString();
        if (Input.GetKeyDown(KeyCode.W)) {
            //var archetype = entityManager.CreateArchetype(
            //   typeof(LocalToWorld),
            //   typeof(Translation),
            //   typeof(RenderMesh),
            //   typeof(RenderBounds));
            //var entity = entityManager.CreateEntity(archetype);
            //entityManager.SetComponentData(entity, new Translation
            //{
            //    Value = new float3(0, 100, 0)
            //});


            //EntitiesComponentData entitiesComponentData = entityManager.GetComponentData<EntitiesComponentData>(entity);
            //var ee = entityManager.Instantiate(entitiesComponentData.m_PrefabEntity);
            //LocalTransform localParam = LocalTransform.FromPosition(new float3(1, 0, 1));
            //localParam.Scale = 1;
            //entityManager.SetComponentData(ee, localParam);
            // SpawnEntitiesSystem spawnEntitiesSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<SpawnEntitiesSystem>();
            SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<SpawnEntitiesSystem>();
            SpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<SpawnEntitiesSystem>(ssh);
            for (int i=1;i<100;i++) {
                for (int j=1;j<100;j++) {
                    spawnEntitiesSystem.testCreate(i,j);
                }
            }

        }
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        GenerateCube();
    //    }
    //}
    //private EntityManager _manager;
    //private void GenerateCube() {
    //    for (int i = 0; i < GetPixel.Instance.posList.Count; i++)
    //    {
    //        Entity temp = SpawnTempEntity(GetPixel.Instance.posList[i]);
    //        SpawnNewSword(GetPixel.Instance.posList[i], temp);
    //    }
    //}
    //public void SpawnNewSword(float2 pos, Entity prefabEntity)
    //{
    //    Entity newSword = _manager.Instantiate(swordEntity);
    //    Translation ballTrans = new Translation
    //    {
    //        Value = new float3(pos.x, 0f, pos.y)
    //    };

    //    float3 temp;
    //    float randomSpeed = UnityEngine.Random.Range(4f, 7f);
    //    temp = float3.zero;

    //    Target target = new Target
    //    {
    //        isGo = false,
    //        Tpos = temp,
    //        randomSpeed = randomSpeed,
    //        targetTempentity = prefabEntity
    //    };

    //    _manager.AddComponentData(newSword, ballTrans);
    //    _manager.AddComponentData(newSword, target);
    //}

    //private Entity SpawnTempEntity(float2 aa)
    //{

    //    Entity tempEntity = _manager.CreateEntity(tempAchetype);

    //    Target target2 = new Target
    //    {
    //        isGo = false,
    //        Tpos = float3.zero,
    //    };

    //    Translation tempTrans = new Translation
    //    {
    //        Value = new float3(aa.x, 0f, aa.y)
    //    };

    //    _manager.SetComponentData(tempEntity, target2);
    //    _manager.SetComponentData(tempEntity, tempTrans);

    //    return tempEntity;

    //}
}
//[CustomEditor(typeof(test_MainHandler))]
//public class test_MainHandlerEditor : Editor {
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        if (GUILayout.Button("test create")) { 

//        }
//    }
//}
public struct Target : IComponentData
{
    public bool isGo;
    public float3 Tpos;
    public float randomSpeed;
    public Entity targetTempentity;
}
public struct Translation : IComponentData
{
    public float3 Value;
}