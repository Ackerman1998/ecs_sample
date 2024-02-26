using ClientComponents.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GamePlayer : MonoBehaviour
{
    public static GamePlayer _instance;
    EntityManager entityManager;
    Entity entity;
    bool entityIsCreate = false;
    public UIJoyist uIJoyist;
    private List<Entity> bullets = new List<Entity>();
    float shootTimeSpan = 0.2f;
    float genNpcTimeSpan = 0.1f;
    float shootTimeCurrent =0;
    float genNpcTimeCurrent = 0;
    private void Awake()
    {
        _instance = this;
        entityIsCreate = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        uIJoyist = GameObject.Find("Canvas/Joystick").GetComponent<UIJoyist>();
    }
    public void StartLaunch() {
        if (entityIsCreate==false) {
            GameObject.Find("Canvas/START").gameObject.SetActive(false);
            CreateScene();
            CreatePlayer();
        }
    }
    public void Init()
    {
      
    }
    private void CreatePlayer() {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
        GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
        entity = spawnEntitiesSystem.CreateInstance(0, 0);
        entityIsCreate = true;
    }
    private void CreateScene()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
        GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
        spawnEntitiesSystem.CreateScene(0, 0);
    }
    private void CreateBullet() {

    }
    private void CreateNpc()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
        GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
        spawnEntitiesSystem.CreateNpc(GetRandomNumByRange(4,20), GetRandomNumByRange(4, 20));
    }
    private int GetRandomNumByRange(int min,int max) {
        int direction = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;
        int rr = UnityEngine.Random.Range(min, max+1)* direction;
        return rr;
    }
    public void RecycleBullet(Entity entity) {
        bullets.Add(entity);
    }
    // Update is called once per frame
    void Update()
    {
        if (entityIsCreate == false)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
            GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            float3 curPoint = transform.Position;
            spawnEntitiesSystem.CreateBullet(curPoint.x, curPoint.z);
        }
        shootTimeCurrent += Time.deltaTime;
        if (shootTimeCurrent >= shootTimeSpan)
        {
            shootTimeCurrent = 0f;
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
            GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            float3 curPoint = transform.Position;
            spawnEntitiesSystem.CreateBullet(curPoint.x, curPoint.z);
        }
        genNpcTimeCurrent += Time.deltaTime;
        if (genNpcTimeCurrent >= genNpcTimeSpan)
        {
            genNpcTimeCurrent = 0f;
            CreateNpc();
        }
    }
}
