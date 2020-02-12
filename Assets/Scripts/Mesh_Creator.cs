using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh_Creator : MonoBehaviour
{
    public Grid_Generator Grid_Generator;
    public GameObject meshPrefab;
    private List<List<Grid_Generator.Sector>> sectorGrid;

    public void Start()
    {
        sectorGrid = Grid_Generator.sectorGrid;

        CreateMeshObject();
    }

    public void CreateMeshObject()
    {

        AssignValues();
    }

    public void AssignValues()
    {
        DrawMesh();
    }

    public void DrawMesh()
    {

    }

    /*
    
    void CreateShape()
    {
        vertices = new Vector3[]
        {
            new Vector3 (0,0,0),
            new Vector3 (1,0,0),
            new Vector3 (0,0,1),
            new Vector3 (1,0,1)
        };

        triangles = new int[]
        {
            2, 1, 0
        };
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

    }
    
    */
}
