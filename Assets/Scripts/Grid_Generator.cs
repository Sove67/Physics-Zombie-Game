using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Generator : MonoBehaviour
{
    /*
        Note To Self:
        While the 2d arrays are constructed x first, then y
        the debug functions read y then x so that you can group rows together, allowing line by line console entries.

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
                text = text + "Sector: (" + x + ", " + y + ") Left: " + sectorGrid[x,y].connected.left + " Right: " + sectorGrid[x, y].connected.right + " Up: " + sectorGrid[x, y].connected.up + " Down: " + sectorGrid[x, y].connected.down + "\n\n";
            }
        }

        Debug.Log(text);
    }
    
    public void NumberGrid() // Creates a populated grid with random numbers within the "randomizerRange"
    {
        numGrid = new int[gridDimensions.x, gridDimensions.y];
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            { numGrid[x, y] = (Random.Range(0, (0 + randomizerRange))); }
        }


        // Debugging
        string text = "Numbers: \n\n";
        for (int y = 0; y < gridDimensions.y; y++)
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

        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                sectorGrid[x, y] = new Sector(null, new Vector2[4], new Connection(false, false, false, false));

                var a = x * (sectionSideLength + streetWidth);
                var b = y * (sectionSideLength + streetWidth);

                // [0][1]
                // [2][3]
                sectorGrid[x,y].vertexPosition[0] = (new Vector2(a + streetWidth / 2,                        b - streetWidth / 2                     ));
                sectorGrid[x,y].vertexPosition[1] = (new Vector2(a - streetWidth / 2 + sectionSideLength,    b - streetWidth / 2                     ));
                sectorGrid[x,y].vertexPosition[2] = (new Vector2(a + streetWidth / 2,                        b + streetWidth / 2 - sectionSideLength ));
                sectorGrid[x,y].vertexPosition[3] = (new Vector2(a - streetWidth / 2 + sectionSideLength,    b + streetWidth / 2 - sectionSideLength ));

            }
        }

        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
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
        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                text = text + sectorGrid[x,y].id + ", ";
            }
            text = text + "\n\n";
        }
        Debug.Log(text);
    }

    public void GridCrawler(int x, int y, int id) // Assign any cardinally connected numbers the same ID as the first, marks the connection, then repeats the check.
    {
        // Left
        if (x - 1 > 0                   && numGrid[x,y] == numGrid[x - 1,y]   && sectorGrid[x - 1,y].id == null)
        {
            sectorGrid[x - 1,y].id = id;
            GridCrawler(x - 1, y, id);
        }

        // Right
        if (x + 1 < gridDimensions.x    && numGrid[x,y] == numGrid[x + 1,y]   && sectorGrid[x + 1,y].id == null)
        {
            sectorGrid[x + 1,y].id = id;
            GridCrawler(x + 1, y, id);
        }

        // Up
        if (y + 1 < gridDimensions.y    && numGrid[x,y] == numGrid[x,y + 1]   && sectorGrid[x,y + 1].id == null)
        {
            sectorGrid[x,y + 1].id = id;
            GridCrawler(x, y + 1, id);
        }

        // Down
        if (y - 1 > 0                   && numGrid[x,y] == numGrid[x,y - 1]   && sectorGrid[x,y - 1].id == null)
        {
            sectorGrid[x,y - 1].id = id;
            GridCrawler(x, y - 1, id);
        }

        ConnectionMarker(x, y, id);
    }

    public void ConnectionMarker(int x, int y, int id)
    {
        // Left
        if (x - 1 > 0                   && numGrid[x,y] == numGrid[x - 1,y]   && sectorGrid[x - 1,y].connected.right == false)
        {
            sectorGrid[x,y].connected.left = true;
        }

        // Right
        if (x + 1 < gridDimensions.x    && numGrid[x,y] == numGrid[x + 1,y]   && sectorGrid[x + 1,y].connected.left == false )
        {
            sectorGrid[x,y].connected.right = true;
        }

        // Up
        if (y + 1 < gridDimensions.y    && numGrid[x,y] == numGrid[x,y + 1]   && sectorGrid[x,y + 1].connected.down == false )
        {
            sectorGrid[x,y].connected.up = true;
        }

        // Down
        if (y - 1 > 0                   && numGrid[x,y] == numGrid[x,y - 1]   && sectorGrid[x,y - 1].connected.up == false   )
        {
            sectorGrid[x,y].connected.down = true;
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

            for (int b = 0; b < target.vertecies.Count; b++)
            {
                GameObject newMarker = Instantiate(marker, new Vector3(target.vertecies[b].x, 5, target.vertecies[b].z), new Quaternion());
                newMarker.name = ("Mesh: " + a + " Marker: " + b);
            }
        }
    }
    
    public void AssignValues(int id)
    {
        VertexToggle enabledVertexes = new VertexToggle(true, true, true, true);

        // for each section, check connections and ID. if connected and the ID matches, add vertecies to the list. If not connected, add a different set to the list
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.x; y++)
            {

                if (sectorGrid[x, y].id == id)
                {

                    if (sectorGrid[x, y].connected.left == true)
                    {
                        enabledVertexes.topLeft = false;
                        enabledVertexes.bottomLeft = false;
                    }

                    if (sectorGrid[x, y].connected.right == true)
                    {
                        enabledVertexes.topRight = false;
                        enabledVertexes.bottomRight = false;
                    }

                    if (sectorGrid[x, y].connected.up == true)
                    {
                        enabledVertexes.topLeft = false;
                        enabledVertexes.topRight = false;
                    }

                    if (sectorGrid[x, y].connected.down == true)
                    {
                        enabledVertexes.bottomLeft = false;
                        enabledVertexes.bottomRight = false;
                    }

                    if (enabledVertexes.topLeft)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[0].y, 0, sectorGrid[x, y].vertexPosition[0].x));
                    }
                    if (enabledVertexes.topRight)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[1].y, 0, sectorGrid[x, y].vertexPosition[1].x));
                    }
                    if (enabledVertexes.bottomLeft)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[2].y, 0, sectorGrid[x, y].vertexPosition[2].x));
                    }
                    if (enabledVertexes.bottomRight)
                    {
                        target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[3].y, 0, sectorGrid[x, y].vertexPosition[3].x));
                    }

                    //for (int c = 0; c < sectorGrid[x, y].vertexPosition.Count; c++)
                    //{
                    //    target.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[c].y, 0, -sectorGrid[x, y].vertexPosition[c].x));
                    //}
                }
            }
        }

        //Create triangles out of vertecies
        for (int a = 0; a < target.vertecies.Count; a += 4)
        {
            // [0][1]
            // [2][3]

            target.triangles.Add(a);
            target.triangles.Add(a + 1);
            target.triangles.Add(a + 2);

            target.triangles.Add(a + 1);
            target.triangles.Add(a + 3);
            target.triangles.Add(a + 2);
        }


        string text = "ID: " + id + " | # of Vertecies: " + target.vertecies.Count + " | " + "Coordinates: ";
        for (int a = 0; a < target.vertecies.Count; a++)
        {
            text = text + target.vertecies[a] + ", ";
        }
        Debug.Log(text);

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