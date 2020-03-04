using System;
using System.Collections.Generic;
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
    //public Dictionary<string,GameObject> prefabs;

    public GameObject sketchPlane;

    public List<GameObject> prefabs;
    public float3 range;
    public int count;
    public uint seed=10;
    BlobAssetReference<Collider>  sourceCollider;


    private float _planeStartZ;
    Entity sourceEntity;
    public List<Entity> sourceEntitys;
    public List<BlobAssetReference<Collider>> sourceColliders;
    Unity.Mathematics.Random random;
    public int current_type;
    void OnEnable()
    {
        if (this.enabled)
        {
            random = new Unity.Mathematics.Random();
            random.InitState(seed);
            sourceEntitys = new List<Entity>();
            sourceColliders = new List<BlobAssetReference<Collider>>();
            //prefabs = new Dictionary<string, GameObject>();
            float3 gravity = new float3(0, 0.0f, 0);//float3.zero;

            base.init(gravity, false /* don't add the control script from the base physics */);

            for (int i=0; i< prefabs.Count;i++)
            {
                var prefab = prefabs[i];
                var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
                // Create entity prefab from the game object hierarchy once
                var sourceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
                var entityManager = BasePhysicsDemo.DefaultWorld.EntityManager;
                sourceEntitys.Add(sourceEntity);
                //var positions = new NativeArray<float3>(count, Allocator.Temp);
                //var rotations = new NativeArray<quaternion>(count, Allocator.Temp);
                //RandomPointsOnCircle(transform.position, range, ref positions, ref rotations);
                var sourceCollider = entityManager.GetComponentData<PhysicsCollider>(sourceEntity).Value;
                sourceColliders.Add(sourceCollider);
            }

            _planeStartZ = sketchPlane.transform.position.z;

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

    public void RandomPointsOnCircle(float3 center, float3 range, ref NativeArray<float3> positions, ref NativeArray<quaternion> rotations)
    {
        var count = positions.Length;
        // Initialize the seed of the random number generator
        
        
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

    void OnGUI()
    {
        for (int i = 0; i < sourceEntitys.Count;i++){
            if (GUI.Button(new Rect(10, 10+i*100, 150, 100), "Protein Type "+i.ToString()))
            {
                print("You clicked the button! "+i.ToString());
                current_type = i;
            }
        }


        // float scrollZ = GUI.VerticalSlider(new Rect(25, 25, 100, 30), scrollZ, 10.0f, 0.0f);

        Vector3 sketchPosition = sketchPlane.transform.position;
        sketchPosition.z = GUI.VerticalSlider(_sketchPlaneSliderRect(), sketchPosition.z, 0.0f, 30.0f);       
        sketchPlane.transform.position = sketchPosition;
    }

    Rect _sketchPlaneSliderRect()
    {
        int width = Screen.width;
        return new Rect(width - 50 - 10, 10, 50, 500);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {   
            Vector2 mousePosition = Input.mousePosition;     

            int width = Screen.width;

            if (new Rect(10, 10, 150, 100*sourceEntitys.Count).Contains(mousePosition)) return;
            if (_sketchPlaneSliderRect().Contains(mousePosition)) return;

            // Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x,mousePosition.y,10.0f));
            // Debug.Log(p);   //new float3(0, 1, 0)
            // ///CreateDynamicBody(new float3(p.x,p.y,p.z), quaternion.identity, sourceCollider, float3.zero, float3.zero, 1.0f);
            // int i = current_type;//random.NextInt(0,sourceEntitys.Count);
            // var instance = BasePhysicsDemo.DefaultWorld.EntityManager.Instantiate(sourceEntitys[i]);
            // BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Translation { Value = new float3(p.x,p.y,p.z) });
            // BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Rotation { Value = quaternion.identity });
            // BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new PhysicsCollider { Value = sourceColliders[i] });
       

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // Physics.Raycast(ray.origin, ray.direction, 100000f);
            
            UnityEngine.RaycastHit hitInfo;

            if( sketchPlane.GetComponent<UnityEngine.Collider>().Raycast(ray, out hitInfo, 10000f) )
            {
                addBrushPoint(hitInfo.point, current_type);
            }
        }  




    }

    void addBrushPoint(Vector3 mousePosition, int prefabIndex)
    {
        // CreateDynamicBody(new float3(mousePosition), quaternion.identity, sourceCollider, float3.zero, float3.zero, 1.0f);
        
        var instance = BasePhysicsDemo.DefaultWorld.EntityManager.Instantiate(sourceEntitys[prefabIndex]);

        BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Translation { Value = mousePosition });
        BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Rotation { Value = quaternion.identity });
        BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new PhysicsCollider { Value = sourceColliders[prefabIndex] });
    }

}
