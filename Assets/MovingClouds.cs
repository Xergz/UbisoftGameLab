using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Change renderer's material each changeInterval
// seconds from the material array defined in the inspector.
public class MovingClouds : MonoBehaviour
{
    public Material MaterialPrefab;
    [SerializeField] public MovingMaterial[] movingMaterials = new MovingMaterial[0];
    public int numberOfClouds = 0;
    public Renderer rend;

    /// <summary> 
    /// Use to damp down the scrolling speed of the texture since offset is between 0.0f and 1.0f
    /// </summary> 
    public float scrollSpeed;
    public Vector2 tilingRange;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false;

        //Class to hold direction and speed for each material
        movingMaterials = new MovingMaterial[numberOfClouds];

        //Somehow need temporary array to initialize all materials and assign them to renderer
        Material[] createdMaterials = new Material[numberOfClouds];
        for (int i = 0; i < numberOfClouds; i++)
        {
            //Arbitrary random directions
            var cloud = new MovingMaterial(MaterialPrefab, new Vector2(Random.Range(-0.05f, 0.05f),Random.Range(-0.05f, 0.05f)));
            movingMaterials[i] = cloud;
            
            createdMaterials[i] = movingMaterials[i].material;
        }

        rend.materials = createdMaterials;
        //Randomly initalise offsets and tilings
        for (int i = 0; i < rend.materials.Length; i++)
        {
            var tiling = Random.Range(tilingRange.x, tilingRange.y);
            rend.materials[i].mainTextureOffset = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            rend.materials[i].mainTextureScale = new Vector2(tiling,tiling);//Assign same tiling to avoid stretching
        }

        rend.enabled = true;
    }

    void Update()
    {
        if (movingMaterials.Length == 0)
           return;

        for (int i = 0; i < numberOfClouds; i++)
        {
            //Clamp the offset values between 0 and 1 so they don't increase infinitely
            var cloud = movingMaterials[i];
            Vector2 displacement = cloud.deltas * scrollSpeed * Time.deltaTime;
            var offsets = rend.materials[i].mainTextureOffset;
            offsets += displacement;
            offsets.x %= 1.0f;
            offsets.y %= 1.0f;
            rend.materials[i].mainTextureOffset = offsets;
        }
    }
}

public class MovingMaterial
{
    public Material material;
    public Vector2 deltas;

    public MovingMaterial(Material material, Vector2 dir)
    {
        this.material= material;
        this.deltas = dir;
    }
}

