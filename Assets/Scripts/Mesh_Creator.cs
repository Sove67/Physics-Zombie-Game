using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh_Creator : MonoBehaviour
{
    public Grid_Generator Grid_Generator;
    public GameObject meshPrefab;
    private List<List<Grid_Generator.Sector>> sectorGrid = new List<List<Grid_Generator.Sector>> { };

    public List<MeshContainer> meshList = new List<MeshContainer> { };
    public MeshContainer target;
    public Material baseMaterial;

    public void Start()
    {
        sectorGrid = Grid_Generator.sectorGrid;
    }

    public class MeshContainer
    {
        public GameObject gameObject { get; set; }
        public List<Vector3> vertecies { get; set; }
        public List<int> triangles { get; set; }
        public MeshContainer(GameObject gameObject, List<Vector3> vertecies, List<int> triangles)
        {
            this.gameObject = gameObject;
            this.vertecies = vertecies;
            this.triangles = triangles;
        }
    }

    public void CreateMeshObject()
    {
        for (int a = 0; a < Grid_Generator.sectionCount; a++)
        {
            // Create the mesh and target it
            meshList.Add(new MeshContainer(Instantiate(meshPrefab, this.transform), new List<Vector3> { }, new List<int> { }));
            target = meshList[meshList.Count -1];

            // Set Name
            target.gameObject.name = ("Mesh (" + a + ")");

            AssignValues(a);
        }
    }

    public void AssignValues(int id)
    {
        // for each section, check id. if ID matches, add the vertecies to a list.
        for (int a = 0; a < sectorGrid.Count; a++)
        {
            for (int b = 0; b < sectorGrid[0].Count; b++)
            {
                
                if (sectorGrid[a][b].id == id)
                {
                    for (int c = 0; c < sectorGrid[a][b].vertexPosition.Count; c++)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[a][b].vertexPosition[c].y, 0, -sectorGrid[a][b].vertexPosition[c].x));
                    }
                }
            }
        }

        // for each section, check connections. if connected, add vertecies to the list.
        for (int a = 0; a < sectorGrid.Count; a++)
        {
            for (int b = 0; b < sectorGrid[0].Count; b++)
            {

                if (sectorGrid[a][b].id == id)
                {

                    if (sectorGrid[a][b].connected.left == true)
                    {
                        /*  [1] [0][1]
                            [3] [2][3]  */
                        target.vertecies.Add(new Vector3(sectorGrid[a - 1][b].vertexPosition[1].y, 0, -sectorGrid[a - 1][b].vertexPosition[1].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a][b].vertexPosition[0].y, 0, -sectorGrid[a][b].vertexPosition[0].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a - 1][b].vertexPosition[3].y, 0, -sectorGrid[a - 1][b].vertexPosition[3].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a][b].vertexPosition[2].y, 0, -sectorGrid[a][b].vertexPosition[2].x));
                    }

                    if (sectorGrid[a][b].connected.right == true)
                    {
                        /*  [0][1] [0]
                            [2][3] [2]  */
                        target.vertecies.Add(new Vector3(sectorGrid[a][b].vertexPosition[1].y, 0, -sectorGrid[a][b].vertexPosition[1].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a + 1][b].vertexPosition[0].y, 0, -sectorGrid[a + 1][b].vertexPosition[0].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a][b].vertexPosition[3].y, 0, -sectorGrid[a][b].vertexPosition[3].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a + 1][b].vertexPosition[2].y, 0, -sectorGrid[a + 1][b].vertexPosition[2].x));
                    }

                    if (sectorGrid[a][b].connected.up == true)
                    {
                        /*  [2][3]
                     
                            [0][1]
                            [2][3]  */
                        target.vertecies.Add(new Vector3(sectorGrid[a][b + 1].vertexPosition[2].y, 0, -sectorGrid[a][b + 1].vertexPosition[2].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a][b + 1].vertexPosition[3].y, 0, -sectorGrid[a][b + 1].vertexPosition[3].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a][b].vertexPosition[0].y, 0, -sectorGrid[a][b].vertexPosition[0].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a][b].vertexPosition[1].y, 0, -sectorGrid[a][b].vertexPosition[1].x));
                    }

                    if (sectorGrid[a][b].connected.down == true)
                    {
                        /*  [0][1]
                            [2][3]

                            [0][1]  */
                        target.vertecies.Add(new Vector3(sectorGrid[a][b].vertexPosition[2].y, 0, -sectorGrid[a][b].vertexPosition[2].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a][b].vertexPosition[3].y, 0, -sectorGrid[a][b].vertexPosition[3].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a][b - 1].vertexPosition[0].y, 0, -sectorGrid[a][b - 1].vertexPosition[0].x));
                        target.vertecies.Add(new Vector3(sectorGrid[a][b - 1].vertexPosition[1].y, 0, -sectorGrid[a][b - 1].vertexPosition[1].x));
                    }
                }
            }
        }

        //Create triangles out of vertecies
        for (int a = 0; a < target.vertecies.Count; a += 4)
        {
            /*  [0][1]
                [2][3]  */

            target.triangles.Add(a);
            target.triangles.Add(a + 1);
            target.triangles.Add(a + 2);

            target.triangles.Add(a + 1);
            target.triangles.Add(a + 3);
            target.triangles.Add(a + 2);
        }

        SendMesh();
    }

    public void SendMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = target.vertecies.ToArray();
        mesh.triangles = target.triangles.ToArray();

        target.gameObject.GetComponent<MeshFilter>().mesh = mesh;
        target.gameObject.GetComponent<MeshRenderer>().material = baseMaterial;
        target.gameObject.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0f, 1f, .5f, .5f, .25f, .75f, 1f, 1f);
    }
}
