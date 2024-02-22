using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Entities;
using Unity.Mathematics;

public class test_MainHandler : MonoBehaviour
{
    EntityArchetype tempAchetype;
    private Entity swordEntity;
    private void Awake()
    {
        
 //       tempAchetype = _manager.CreateArchetype(
 //typeof(Translation),
 //typeof(Target)
 //);
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