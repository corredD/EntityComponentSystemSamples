using System;
using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using Collider = Unity.Physics.Collider;

using UnityEngine;

[Serializable]
public class SketchBasedMonoBehaviour : BasePhysicsDemo
{
    public GameObject prefab;
    private Entity prefabEntity;

    public GameObject sketchPlane;


    public float sketchMoveRate = 0.1f;

    BlobAssetReference<Collider>  sourceCollider;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (this.enabled)
        {
            float3 gravity = float3.zero;

            init(gravity, false);

            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
            // Create entity prefab from the game object hierarchy once
            prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
            var entityManager = BasePhysicsDemo.DefaultWorld.EntityManager;

            sourceCollider = entityManager.GetComponentData<PhysicsCollider>(prefabEntity).Value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetMouseButton(0) && sketchPlane != null )
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // Physics.Raycast(ray.origin, ray.direction, 100000f);
            
            UnityEngine.RaycastHit hitInfo;

            if( sketchPlane.GetComponent<UnityEngine.Collider>().Raycast(ray, out hitInfo, 10000f) )
            {
                addBrushPoint(hitInfo.point);
            }
            


            // RaycastHit[] hits = Physics.RaycastAll(ray);

            // foreach( RaycastHit hit in hits )
            // {
            //     var go = hit.collider.gameObject;

            //     if( go.tag == "canvas" )
            //     {
            //         addBrushPoint(hit.point);
            //     }
            // }
        }

        float deltaScrolLZ = Input.GetAxis("Mouse ScrollWheel");
        sketchPlane.transform.position += new Vector3(0f,0f, deltaScrolLZ * sketchMoveRate);

    }

    void addBrushPoint(Vector3 mousePosition)
    {
        // CreateDynamicBody(new float3(mousePosition), quaternion.identity, sourceCollider, float3.zero, float3.zero, 1.0f);
        
        var instance = BasePhysicsDemo.DefaultWorld.EntityManager.Instantiate(prefabEntity);

        BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Translation { Value = mousePosition });
        BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Rotation { Value = quaternion.identity });
        BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new PhysicsCollider { Value = sourceCollider });
    }
}
