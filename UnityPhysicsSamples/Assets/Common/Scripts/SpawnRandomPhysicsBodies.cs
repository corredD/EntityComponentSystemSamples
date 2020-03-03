using System;
using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using Collider = Unity.Physics.Collider;

[Serializable]
public class SpawnRandomPhysicsBodies : BasePhysicsDemo
{
    public GameObject prefab;
    public float3 range;
    public int count;
    BlobAssetReference<Collider>  sourceCollider;
    Entity sourceEntity;
    void OnEnable()
    {
        if (this.enabled)
        {
            float3 gravity = float3.zero;
            base.init(gravity);
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
            // Create entity prefab from the game object hierarchy once
            sourceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
            var entityManager = BasePhysicsDemo.DefaultWorld.EntityManager;

            //var positions = new NativeArray<float3>(count, Allocator.Temp);
            //var rotations = new NativeArray<quaternion>(count, Allocator.Temp);
            //RandomPointsOnCircle(transform.position, range, ref positions, ref rotations);

            sourceCollider = entityManager.GetComponentData<PhysicsCollider>(sourceEntity).Value;
            //for (int i = 0; i < count; i++)
           // {
            //    var instance = entityManager.Instantiate(sourceEntity);
            ///    entityManager.SetComponentData(instance, new Translation { Value = positions[i] });
            //    entityManager.SetComponentData(instance, new Rotation { Value = rotations[i] });
            //    entityManager.SetComponentData(instance, new PhysicsCollider { Value = sourceCollider });
            //}

            //positions.Dispose();
            //rotations.Dispose();
        }
    }

    public static void RandomPointsOnCircle(float3 center, float3 range, ref NativeArray<float3> positions, ref NativeArray<quaternion> rotations)
    {
        var count = positions.Length;
        // Initialize the seed of the random number generator
        Unity.Mathematics.Random random = new Unity.Mathematics.Random();
        random.InitState(10);
        for (int i = 0; i < count; i++)
        {
            positions[i] = center + random.NextFloat3(-range, range);
            rotations[i] = random.NextQuaternionRotation();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {   
            Vector2 mousePosition = Input.mousePosition;     
            Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x,mousePosition.y,10.0f));
            Debug.Log(p);   //new float3(0, 1, 0)
            //CreateDynamicBody(new float3(p.x,p.y,p.z), quaternion.identity, sourceCollider, float3.zero, float3.zero, 1.0f);
            
            var instance = BasePhysicsDemo.DefaultWorld.EntityManager.Instantiate(sourceEntity);
            BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Translation { Value = new float3(p.x,p.y,p.z) });
            BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Rotation { Value = quaternion.identity });
            //BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new PhysicsCollider { Value = sourceCollider });
            
        }       
    }
}
