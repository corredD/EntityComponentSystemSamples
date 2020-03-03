using System.Collections;
using System.Collections.Generic;

using Unity.Entities;

using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;

using UnityEngine;

public class SketchBasedMonoBehaviour : MonoBehaviour
{
    public GameObject template;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            
            
            Physics.Raycast(ray.origin, ray.direction, 100000f);
            
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach( RaycastHit hit in hits )
            {
                var go = hit.collider.gameObject;

                if( go.tag == "canvas" )
                {
                    addBrushPoint(hit.point);
                }
            }

        }
        
    }

    void addBrushPoint(Vector3 mousePosition)
    {
        NativeArray<Entity> entityArray = new NativeArray<Entity>(1, Allocator.Temp);

        // EntityArchetype entityArchetype = entityManager.CreateArchetype(
        //     typeof(Oscillator),
        //     typeof(Translation),
        //     typeof(Rotation),
        //     typeof(Scale),
        //     typeof(RenderMesh),
        //     typeof(RenderBounds),
        //     typeof(WorldRenderBounds),
        //     typeof(EditorRenderData),
        //     typeof(LocalToWorld)
        // );
        // entityManager.CreateEntity(entityArchetype, entityArray);

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(template, settings);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        entityManager.Instantiate(prefab, entityArray);

        for (int i = 0; i < entityArray.Length; i++) {
            Entity entity = entityArray[i];




            entityManager.SetComponentData(entity, new Translation { Value =  mousePosition } );





        }
    }
}
