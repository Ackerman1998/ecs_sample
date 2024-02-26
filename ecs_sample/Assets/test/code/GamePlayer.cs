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
    float shootTimeCurrent =0;
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
        spawnEntitiesSystem.CreateNpc(2, 2);
    }
    public void RecycleBullet(Entity entity) {
        bullets.Add(entity);
    }
    // Update is called once per frame
    void Update()
    {
        if (entityIsCreate==false) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
            GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            float3 curPoint = transform.Position;
            spawnEntitiesSystem.CreateBullet(curPoint.x, curPoint.z);
        }
        shootTimeCurrent += Time.deltaTime;
        if (shootTimeCurrent>=shootTimeSpan) {
            shootTimeCurrent = 0f;
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
            GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            float3 curPoint = transform.Position;
            spawnEntitiesSystem.CreateBullet(curPoint.x, curPoint.z);
            //bullets.Add();
            //for (int i=0;i< bullets.Count;i++)
            //{
            //    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //    if (entityManager.GetComponentData<PlayerBulletData>(bullets[i]).isStart==false) {
            //        entityManager.SetEnabled(bullets[i],false);
            //    }
            //}
        }
       
        //print(Input.GetAxis("Horizontal"));
        //if (Input.GetAxis("Horizontal")!=0) {
        //    var transform = entityManager.GetComponentData<LocalTransform>(entity);
        //    transform.Position += new Unity.Mathematics.float3(0,0, Input.GetAxis("Horizontal")) * Time.deltaTime * 8;
        //}
        //if (Input.GetAxis("Vertical") != 0)
        //{
        //    var transform = entityManager.GetComponentData<LocalTransform>(entity);
        //    transform.Position += new Unity.Mathematics.float3(Input.GetAxis("Vertical"), 0,0) * Time.deltaTime * 8;
        //}
    }
}
