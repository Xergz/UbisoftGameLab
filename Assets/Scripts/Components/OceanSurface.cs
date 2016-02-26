using UnityEngine;
using System.Collections;

public class OceanSurface : MonoBehaviour {
    [Tooltip("Specify the associated wave controller")]
    public WaveController Waves = null;

    [Tooltip("Specify the surface to deform")]
    public MeshFilter surfaceMeshFilter;

    private Mesh surfaceMesh;

	// Use this for initialization
	void Start () {
        surfaceMesh = surfaceMeshFilter.mesh;

        // This is to avoid a buggy beahviour when scaling the y axis
        Vector3 scale = this.transform.localScale;
        if (scale.y > 1 || scale.y < 1) {
            scale.y = 1;
            this.transform.localScale = scale;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Waves != null) {
            Vector3[] vertices = surfaceMesh.vertices;
            Vector3 worldPos;

            for(int i = 0; i < vertices.Length; i++) {
                worldPos = this.transform.TransformPoint(vertices[i]);

                vertices[i].y = Waves.GetOceanHeightAt(worldPos.x, worldPos.z);
            }

            surfaceMesh.vertices = vertices;
        }
	}
}
