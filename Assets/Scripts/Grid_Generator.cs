using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Generator : MonoBehaviour
{
    /*
        Note To Self:
        While the 2d arrays are constructed x first, then y,
        some debug functions read y then x, and also decrease y but increase x
        in order to output grids to console right way up
    */

    // Variables

    // Num Grid
    public Vector2Int gridDimensions;
    public int randomizerRange;
    private int[,] numGrid;

    // Vertex Grid
    public int sectionSideLength;
    public int streetWidth;

    // Sector Grid
    public Sector[,] sectorGrid;
    public int sectionCount;

    public GameObject meshPrefab;

    public List<MeshContainer> meshList = new List<MeshContainer> { };
    public MeshContainer target;
    public Material baseMaterial;

    // Debugging
    public GameObject marker;



    // Classes
    public class Connection // This class holds data on which edges of a sector border another sector of the same ID.
    {
        public bool left { get; set; }
        public bool right { get; set; }
        public bool up { get; set; }
        public bool down { get; set; }
        public Connection(bool left, bool right, bool up, bool down)
        {
            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
        }
    }

    public class Sector // This class holds data on ID, Vertexes, and Connections for one section within the sectorGrid.
    {
        public int? id { get; set; }
        public Vector2[] vertexPosition { get; set; }
        public Connection connected { get; set; }
        public Sector(int? id, Vector2[] vertexPosition, Connection connected)
        {
            this.id = id;
            this.vertexPosition = vertexPosition;
            this.connected = connected;
        }
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

    public class VertexToggle
    {
        public bool topLeft { get; set; }
        public bool topRight { get; set; }
        public bool bottomLeft { get; set; }
        public bool bottomRight { get; set; }
        public VertexToggle(bool topLeft, bool topRight, bool bottomLeft, bool bottomRight)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
        }
    }



    // Functions
    public void Start()
    {
        NumberGrid();
        SectionGrid();
        CreateMeshObject();

        // Debugging
        string text = "Connections: \n\n";
        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                text = text + "Sector: (" + x + ", " + y + ")  Left: " + sectorGrid[x,y].connected.left + "  Right: " + sectorGrid[x, y].connected.right + "  Up: " + sectorGrid[x, y].connected.up + "  Down: " + sectorGrid[x, y].connected.down + "\n\n";
            }
        }

        Debug.Log(text);
    }

    public void NumberGrid() // Creates a populated grid with random numbers within the "randomizerRange"
    {
        numGrid = new int[gridDimensions.x, gridDimensions.y];
        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            { numGrid[x, y] = (Random.Range(0, (0 + randomizerRange))); }
        }

        // Debugging
        string text = "Numbers: \n\n";
        for (int y = gridDimensions.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                text = text + numGrid[x, y] + ", ";
            }
            text = text + "\n\n";
        }
        Debug.Log(text);
    }

    public void SectionGrid() // Creates a "sectorGrid" with four points on a 2D plane in the corners of each sector.
    {
        sectorGrid = new Sector[gridDimensions.x, gridDimensions.y];
        int id = 0;

        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                sectorGrid[x, y] = new Sector(null, new Vector2[4], new Connection(false, false, false, false));

                float xMod = x * (sectionSideLength + streetWidth);
                float yMod = y * (sectionSideLength + streetWidth);
                float shift1 = (streetWidth / 2);
                float shift2 = -(streetWidth / 2) + sectionSideLength;

                // 2 3
                // 0 1

                sectorGrid[x,y].vertexPosition[0] = (new Vector2(xMod + shift1,     yMod + shift1));
                sectorGrid[x,y].vertexPosition[1] = (new Vector2(xMod + shift2,     yMod + shift1));
                sectorGrid[x,y].vertexPosition[2] = (new Vector2(xMod + shift1,     yMod + shift2));
                sectorGrid[x,y].vertexPosition[3] = (new Vector2(xMod + shift2,     yMod + shift2));

            }
        }

        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                if (sectorGrid[x, y].id == null)
                {
                    sectorGrid[x, y].id = id;
                    GridCrawler(x, y, id);
                    id++;
                }
            }
        }
        sectionCount = id;

        // Debugging
        string text = "Sectors: \n\n";
        for (int y = gridDimensions.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                text = text + sectorGrid[x,y].id + ", ";
            }
            text = text + "\n\n";
        }
        Debug.Log(text);
    }

    public void GridCrawler(int x, int y, int id) // Assign any cardinally connected numbers the same ID as the first, then repeats the check.
    {
        // Left
        if (x - 1 >= 0                  && numGrid[x,y] == numGrid[x - 1,y]     && sectorGrid[x - 1,y].id == null)
        {
            sectorGrid[x - 1,y].id = id;
            GridCrawler(x - 1, y, id);
        }

        // Right
        if (x + 1 < gridDimensions.x    && numGrid[x,y] == numGrid[x + 1,y]     && sectorGrid[x + 1,y].id == null)
        {
            sectorGrid[x + 1,y].id = id;
            GridCrawler(x + 1, y, id);
        }

        // Up
        if (y + 1 < gridDimensions.y && numGrid[x, y] == numGrid[x, y + 1] && sectorGrid[x, y + 1].id == null)
        {
            sectorGrid[x, y + 1].id = id;
            GridCrawler(x, y + 1, id);
        }

        // Down
        if (y - 1 >= 0                  && numGrid[x, y] == numGrid[x, y - 1]   && sectorGrid[x, y - 1].id == null)
        {
            sectorGrid[x, y - 1].id = id;
            GridCrawler(x, y - 1, id);
        }

        ConnectionMarker(x, y, id);
    }

    public void ConnectionMarker(int x, int y, int id) // Marks the connection of any cardinally connected sectors with the same ID. UP/DOWN INVERTED
    {
        // Left
        if (x - 1 >= 0                  && numGrid[x,y] == numGrid[x - 1,y])
        {
            sectorGrid[x,y].connected.left = true;
        }

        // Right
        if (x + 1 < gridDimensions.x    && numGrid[x,y] == numGrid[x + 1,y])
        {
            sectorGrid[x,y].connected.right = true;
        }

        // Up
        if (y + 1 < gridDimensions.y && numGrid[x, y] == numGrid[x, y + 1])
        {
            sectorGrid[x, y].connected.up = true;
        }

        // Down
        if (y - 1 >= 0                  && numGrid[x, y] == numGrid[x, y - 1])
        {
            sectorGrid[x, y].connected.down = true;
        }

    }

    public void CreateMeshObject()
    {
        for (int a = 0; a < sectionCount; a++)
        {
            // Create the mesh and target it
            meshList.Add(new MeshContainer(Instantiate(meshPrefab, this.transform), new List<Vector3> { }, new List<int> { }));
            target = meshList[meshList.Count - 1];

            // Set Name
            target.gameObject.name = ("Mesh (" + a + ")");

            AssignValues(a);
            SendMesh();

            for (int b = 0; b < target.vertecies.Count; b++)
            {
                GameObject newMarker = Instantiate(marker, new Vector3(target.vertecies[b].x, 5, target.vertecies[b].z), new Quaternion(), this.transform);
                newMarker.name = ("Mesh: " + a + " Marker: " + b);
            }
        }
    }
    
    public void AssignValues(int id)
    {

        // for each section, check connections and ID. if connected and the ID matches, add vertecies to the list. If not connected, add a different set to the list
        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                if (sectorGrid[x, y].id == id)
                {
                    VertexToggle enabledVertexes = new VertexToggle(true, true, true, true);

                    // Single Connectors
                    if (sectorGrid[x, y].connected.left && !sectorGrid[x, y].connected.up && !sectorGrid[x, y].connected.down)
                    {
                        enabledVertexes.topLeft = false;
                        enabledVertexes.bottomLeft = false;
                    }

                    if (sectorGrid[x, y].connected.right && !sectorGrid[x, y].connected.up && !sectorGrid[x, y].connected.down)
                    {
                        enabledVertexes.topRight = false;
                        enabledVertexes.bottomRight = false;
                    }

                    if (sectorGrid[x, y].connected.up && !sectorGrid[x, y].connected.left && !sectorGrid[x, y].connected.right)
                    {
                        enabledVertexes.topLeft = false;
                        enabledVertexes.topRight = false;
                    }

                    if (sectorGrid[x, y].connected.down && !sectorGrid[x, y].connected.left && !sectorGrid[x, y].connected.right)
                    {
                        enabledVertexes.bottomLeft = false;
                        enabledVertexes.bottomRight = false;
                    }

                    //Need to write cases for filled in areas


                    if (enabledVertexes.topLeft)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[2].x, 0, sectorGrid[x, y].vertexPosition[2].y));
                    }

                    if (enabledVertexes.topRight)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[3].x, 0, sectorGrid[x, y].vertexPosition[3].y));
                    }

                    if (enabledVertexes.bottomLeft)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[0].x, 0, sectorGrid[x, y].vertexPosition[0].y));
                    }

                    if (enabledVertexes.bottomRight)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[1].x, 0, sectorGrid[x, y].vertexPosition[1].y));
                    }

                    /*
                    for (int c = 0; c < sectorGrid[x, y].vertexPosition.Length; c++)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[c].x, 0, sectorGrid[x, y].vertexPosition[c].y));
                    }
                    */
                }
            }
        }

        //need to fix this to work with new vertecies...
        //Create triangles out of vertecies
        for (int a = 0; a < target.vertecies.Count; a += 4)
        {
            //   3       2 3
            // 0 1   +   0

            target.triangles.Add(a + 0);
            target.triangles.Add(a + 3);
            target.triangles.Add(a + 1);

            target.triangles.Add(a + 2);
            target.triangles.Add(a + 3);
            target.triangles.Add(a + 0);
        }

        string text = "ID: " + id + " | # of Vertecies: " + target.vertecies.Count + " | " + "Coordinates: ";
        for (int a = 0; a < target.vertecies.Count; a++)
        {
            text = text + target.vertecies[a] + ", ";
        }
        Debug.Log(text);
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