using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EntitiesAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public int m_Row;
    public int m_Col;
    public int totalNum;
}

public class EntitiesAuthoringBaker : Baker<EntitiesAuthoring>
{
    private int instanceId = 10000;
    public override void Bake(EntitiesAuthoring authoring)
    {
        var data = new EntitiesComponentData
        {
            m_PrefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            m_Row = authoring.m_Row,
            m_Col = authoring.m_Col,
            totalNum = authoring.totalNum
        };
        AddComponent(GetEntity(TransformUsageFlags.Dynamic),data);
    }
}
public struct EntitiesComponentData : IComponentData {
    public Entity m_PrefabEntity;
    public int m_Row;
    public int m_Col;
    public int totalNum;
} 