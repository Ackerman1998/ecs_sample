using ClientComponents.Base;
using DOTS.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using static UnityEngine.EventSystems.EventTrigger;

public class GamePlayer : MonoBehaviour
{
    public static GamePlayer _instance;
    EntityManager entityManager;
    public Entity entity;
    bool entityIsCreate = false;
    public UIJoyist uIJoyist;
    private List<Entity> bullets = new List<Entity>();
    float shootTimeSpan = 0.1f;
    float genNpcTimeSpan = 0.1f;
    float shootTimeCurrent =0;
    float genNpcTimeCurrent = 0;
    public Camera targetCam;
    private Vector3 distance;
    public Text totalNum;
    private void Awake()
    {
        _instance = this;
        entityIsCreate = false;
        Application.targetFrameRate = 60;
        EntityPool.Init();
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
    private void OnDestroy()
    {
        EntityPool.Destroy();
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
        var transform = entityManager.GetComponentData<LocalTransform>(entity);
        float3 curPoint = transform.Position;
        distance =targetCam.transform.position - new Vector3(curPoint.x, curPoint.y, curPoint.z) ;
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
        int direction = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;
        var transform = entityManager.GetComponentData<LocalTransform>(entity);
        float3 curPoint = transform.Position;
        int max = 70;
        int min = 30;
        if (direction == 1)
        {

            spawnEntitiesSystem.CreateNpc(curPoint.x+GetRandomNumByRange(min, max), curPoint.z + GetRandomNumByRange(0, max));
        }
        else {
            spawnEntitiesSystem.CreateNpc(curPoint.x + GetRandomNumByRange(0, max), curPoint.z + GetRandomNumByRange(min, max));
        }
  
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
        //if (Input.GetKeyDown(KeyCode.Y)) {
        //    entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //    SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<RemoveToPool>();
        //    RemoveToPool spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<RemoveToPool>(ssh);
        //    spawnEntitiesSystem.Init();

        //}
        if (Input.GetKey(KeyCode.T))
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
            GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
            spawnEntitiesSystem.CreateEffBoomTest(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50));
            //SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<RemoveToPool>();
            //RemoveToPool spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<RemoveToPool>(ssh);
            //spawnEntitiesSystem.CreateBullet(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50));
            //spawnEntitiesSystem.CreateEffBoomTest(0,0);
            //for (int i=0;i<30;i++) {
            //    for (int j=0;j<30;j++) {
            // spawnEntitiesSystem.CreateEffBoomTest(i, j);
            //    }
            //}
        }


        if (entityIsCreate == false)
        {
            return;
        }
        if (entityManager != null)
        {
            totalNum.text = entityManager.GetAllEntities().Length.ToString();
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
            spawnEntitiesSystem.CreateBulletDirection(curPoint.x, curPoint.z, 1, 0);
            spawnEntitiesSystem.CreateBulletDirection(curPoint.x, curPoint.z, -1, 0);
            spawnEntitiesSystem.CreateBulletDirection(curPoint.x, curPoint.z, 0, 1);
            spawnEntitiesSystem.CreateBulletDirection(curPoint.x, curPoint.z, 0, -1);
            CreateBulletBySystem(curPoint, 0.5f, 0.5f);
            int ee = UnityEngine.Random.Range(1, 100);
            int ff = 100 - ee;
            float numX = (float)ee / 100f;
            float numYY = (float)ff / 100f;
            CreateBulletBySystem(curPoint, numX, numYY);
            CreateBulletBySystem(curPoint, numYY, numX);

        }
        genNpcTimeCurrent += Time.deltaTime;
        if (genNpcTimeCurrent >= genNpcTimeSpan)
        {
            genNpcTimeCurrent = 0f;
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();
            CreateNpc();

            //CreateNpc();
            //CreateNpc();
            //CreateNpc();
            //CreateNpc();
            //CreateNpc();
            //CreateNpc();
            //CreateNpc();
            //CreateNpc();
            //CreateNpc();
            //CreateNpc();
        }
        if (targetCam) {
            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            float3 curPoint = transform.Position;
            targetCam.transform.position = new Vector3(curPoint.x, curPoint.y, curPoint.z) + distance;
        }
    }
    private void CreateBulletBySystem(float3 curPoint,float x,float y) {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        SystemHandle ssh = entityManager.WorldUnmanaged.GetExistingUnmanagedSystem<GameSpawnEntitiesSystem>();
        GameSpawnEntitiesSystem spawnEntitiesSystem = entityManager.WorldUnmanaged.GetUnsafeSystemRef<GameSpawnEntitiesSystem>(ssh);
        spawnEntitiesSystem.CreateBulletDirection(curPoint.x, curPoint.z, x, y);
        spawnEntitiesSystem.CreateBulletDirection(curPoint.x, curPoint.z, -x, y);
        spawnEntitiesSystem.CreateBulletDirection(curPoint.x, curPoint.z, x, -y);
        spawnEntitiesSystem.CreateBulletDirection(curPoint.x, curPoint.z, -x, -y);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(GamePlayer))]
public class GamePlayerEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("test")) {
            Debug.LogError("current num = "+ EntityPool.pools.Count);
        }
    }
}
#endif