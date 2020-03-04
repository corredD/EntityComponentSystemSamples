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
    public float sketchPlaneSpawningOffset = -1.0f;
    public float sketchPlaneStepSize = 3.0f;
    public int sketchPlaneSteps = 3; // i.e. the number of layers
    public int sketchPlaneCurrentStep = 0;
    
    const float _guiInset = 40;

    public List<GameObject> prefabs;
    public float3 range;
    public int count;
    public uint seed=10;
    BlobAssetReference<Collider>  sourceCollider;


    private float _sketchPlaneStartZ;
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

                var sourceCollider = entityManager.GetComponentData<PhysicsCollider>(sourceEntity).Value;
                sourceColliders.Add(sourceCollider);
            }

            _sketchPlaneStartZ = sketchPlane.transform.position.z;
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
        // palette
        for (int i = 0; i < sourceEntitys.Count;i++){
            if (GUI.Button(new Rect(_guiInset, _guiInset+i*100, 150, 100), "Protein Type "+i.ToString()))
            {
                print("You clicked the button! "+i.ToString());
                current_type = i;
            }
        }

        // sketch plane slider
        {
            Vector3 sketchPosition = sketchPlane.transform.position;

            sketchPlaneCurrentStep = (int)GUI.VerticalSlider(_sketchPlaneSliderRect(), sketchPlaneCurrentStep, 0, sketchPlaneSteps);

            sketchPosition.z = _sketchPlaneStartZ + sketchPlaneCurrentStep * sketchPlaneStepSize;
  
            sketchPlane.transform.position = sketchPosition;
        }

        // entity count
        {


            GUI.TextArea( _entityCountTextRect(), "entityCount: " + _entityCount());
        }
    }

    

    int _entityCount()
    {
        return BasePhysicsDemo.DefaultWorld.EntityManager.Debug.EntityCount;
    }

    Rect _entityCountTextRect()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        const int width = 200;
        const int height = 50;

        return new Rect(screenWidth - width, screenHeight - height, width, height);
    }

    Rect _sketchPlaneSliderRect()
    {
        int width = Screen.width;
        
        return new Rect(width - 50 - _guiInset, _guiInset, 150, 500);
    }


    Rect _paletteRect()
    {
        return new Rect(_guiInset, _guiInset, 150, 100*sourceEntitys.Count);
    }

    void update_pen()
    {
        update_mouseOrTouch();
    }

    void update_mouseOrTouch()
    {
        if (Input.GetMouseButton(0))
        {   
            Vector2 mousePosition = Input.mousePosition;     


            Rect debug = _sketchPlaneSliderRect();

            mousePosition.y= Screen.height - mousePosition.y;

            if (_paletteRect().Contains(mousePosition)) return;
            if (_sketchPlaneSliderRect().Contains(mousePosition)) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            UnityEngine.RaycastHit hitInfo;

            if( sketchPlane.GetComponent<UnityEngine.Collider>().Raycast(ray, out hitInfo, 10000f) )
            {
                // lift it off the surface
                hitInfo.point += new Vector3(0f,0f,sketchPlaneSpawningOffset);

                addBrushPoint(hitInfo.point, current_type);
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.stylusTouchSupported )
            update_pen();
        else
            update_mouseOrTouch();
    }

    void addBrushPoint(Vector3 mousePosition, int prefabIndex)
    {
        var instance = BasePhysicsDemo.DefaultWorld.EntityManager.Instantiate(sourceEntitys[prefabIndex]);

        BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Translation { Value = mousePosition });
        BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new Rotation { Value = random.NextQuaternionRotation() });
        BasePhysicsDemo.DefaultWorld.EntityManager.SetComponentData(instance, new PhysicsCollider { Value = sourceColliders[prefabIndex] });
    }

}
