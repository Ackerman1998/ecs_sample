using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GameGeneratorAuthoring : MonoBehaviour
{
    public GameObject prefab;
}

public class GameGeneratorBaker : Baker<GameGeneratorAuthoring>
{
    private int instanceId = 10000;
    public override void Bake(GameGeneratorAuthoring authoring)
    {
        var data = new GameGenerator {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
        };
        AddComponent(GetEntity(TransformUsageFlags.Dynamic),data);
    }
}
public struct GameGenerator : IComponentData {
    public Entity prefab;
}