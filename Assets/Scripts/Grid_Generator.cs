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

        public bool crawled { get; set; }
        public Sector(int? id, Vector2[] vertexPosition, bool crawled)
        {
            this.id = id;
            this.vertexPosition = vertexPosition;
            this.crawled = crawled;
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
                sectorGrid[x, y] = new Sector(null, new Vector2[4], false);

                float xMod = x * (sectionSideLength + streetWidth);
                float yMod = y * (sectionSideLength + streetWidth);
                float shift2 = -streetWidth + sectionSideLength;

                // 2 3
                // 0 1

                sectorGrid[x, y].vertexPosition[0] = (new Vector2(xMod, yMod));
                sectorGrid[x, y].vertexPosition[1] = (new Vector2(xMod + shift2, yMod));
                sectorGrid[x, y].vertexPosition[2] = (new Vector2(xMod, yMod + shift2));
                sectorGrid[x, y].vertexPosition[3] = (new Vector2(xMod + shift2, yMod + shift2));

            }
        }

        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                if (sectorGrid[x, y].id == null)
                {
                    sectorGrid[x, y].id = id;
                    NumberCrawler(x, y, id);
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
                text = text + sectorGrid[x, y].id + ", ";
            }
            text = text + "\n\n";
        }
        Debug.Log(text);
    }

    public void NumberCrawler(int x, int y, int id) // Assign any cardinally connected numbers the same ID as the first, then repeats the check.
    {
        // Left
        if (x - 1 >= 0 && numGrid[x, y] == numGrid[x - 1, y] && sectorGrid[x - 1, y].id == null)
        {
            sectorGrid[x - 1, y].id = id;
            NumberCrawler(x - 1, y, id);
        }

        // Right
        if (x + 1 < gridDimensions.x && numGrid[x, y] == numGrid[x + 1, y] && sectorGrid[x + 1, y].id == null)
        {
            sectorGrid[x + 1, y].id = id;
            NumberCrawler(x + 1, y, id);
        }

        // Up
        if (y + 1 < gridDimensions.y && numGrid[x, y] == numGrid[x, y + 1] && sectorGrid[x, y + 1].id == null)
        {
            sectorGrid[x, y + 1].id = id;
            NumberCrawler(x, y + 1, id);
        }

        // Down
        if (y - 1 >= 0 && numGrid[x, y] == numGrid[x, y - 1] && sectorGrid[x, y - 1].id == null)
        {
            sectorGrid[x, y - 1].id = id;
            NumberCrawler(x, y - 1, id);
        }
    }

    public void CreateMeshObject()
    {
        for (int i = 0; i < sectionCount; i++)
        {
            // Create an empty mesh and assign it to "newMesh"
            MeshContainer newMesh;
            meshList.Add(new MeshContainer(Instantiate(meshPrefab, this.transform), new List<Vector3> { }, new List<int> { }));
            newMesh = meshList[meshList.Count - 1];
            newMesh.gameObject.name = ("Mesh (" + i + ")");

            //Fill the mesh
            newMesh = AssignValues(i, newMesh);
            SendMesh(newMesh);
        }
    }

    public MeshContainer AssignValues(int id, MeshContainer newMesh)
    {
        List<Vector3Int> startingSectors = new List<Vector3Int> { };

        // for each section, check connections and ID. if connected and the ID matches
        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                if (sectorGrid[x, y].id == id && !sectorGrid[x, y].crawled)
                {
                    int length = 0;
                    while (x + length < gridDimensions.x && sectorGrid[x + length, y].id == id)
                    {
                        sectorGrid[x + length, y].crawled = true;
                        length++;
                    }
                    length--;

                    startingSectors.Add(new Vector3Int(x, y, length));

                    // 2 3
                    // 0 1
                    newMesh.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[0].x, 0, sectorGrid[x, y].vertexPosition[0].y));
                    newMesh.vertecies.Add(new Vector3(sectorGrid[x + length, y].vertexPosition[1].x, 0, sectorGrid[x + length, y].vertexPosition[1].y));
                    newMesh.vertecies.Add(new Vector3(sectorGrid[x, y].vertexPosition[2].x, 0, sectorGrid[x, y].vertexPosition[2].y));
                    newMesh.vertecies.Add(new Vector3(sectorGrid[x + length, y].vertexPosition[3].x, 0, sectorGrid[x + length, y].vertexPosition[3].y));
                }
            }
        }

        for (int i = 0; i < startingSectors.Count; i++)
        {
            (int start, int end) = StreetCrawler(i, id, startingSectors);

            if (end >= start)
            {
                Debug.Log("ID: " + id + "  Start: " + start + "  End: " + end);
                newMesh.vertecies.Add(new Vector3(sectorGrid[start, startingSectors[i].y].vertexPosition[2].x, 0, sectorGrid[start, startingSectors[i].y].vertexPosition[2].y + 2 * streetWidth));
                newMesh.vertecies.Add(new Vector3(sectorGrid[end, startingSectors[i].y].vertexPosition[3].x, 0, sectorGrid[end, startingSectors[i].y].vertexPosition[3].y + 2 * streetWidth));
                newMesh.vertecies.Add(new Vector3(sectorGrid[start, startingSectors[i].y].vertexPosition[2].x, 0, sectorGrid[start, startingSectors[i].y].vertexPosition[2].y));
                newMesh.vertecies.Add(new Vector3(sectorGrid[end, startingSectors[i].y].vertexPosition[3].x, 0, sectorGrid[end, startingSectors[i].y].vertexPosition[3].y));
            }
        }

        for (int i = 0; i < newMesh.vertecies.Count; i += 4)
        {
            //   3       2 3
            // 0 1   +   0

            newMesh.triangles.Add(i + 0);
            newMesh.triangles.Add(i + 3);
            newMesh.triangles.Add(i + 1);

            newMesh.triangles.Add(i + 2);
            newMesh.triangles.Add(i + 3);
            newMesh.triangles.Add(i + 0);
        }

        return (newMesh);
    }

    public (int,int) StreetCrawler(int i, int id, List<Vector3Int> startingSectors)
    {
        bool started = false;
        int start = 0;
        int end = 0;
        for (int x = startingSectors[i].x; x < gridDimensions.x; x++)
        {
            if (startingSectors[i].y + 1 < gridDimensions.y)
            {
                if (sectorGrid[x, startingSectors[i].y].id == id && sectorGrid[x, startingSectors[i].y + 1].id == id && !started) 
                { 
                    start = x; 
                    started = true;
                }

                Debug.Log("ID: " + id + "  X: " + x);
                if (started && (sectorGrid[x, startingSectors[i].y].id != id || sectorGrid[x, startingSectors[i].y + 1].id != id))
                { return (start, end); }

                if (started)
                { end = x; }
            }
        }
        return (1, 0);
    }

    public void SendMesh(MeshContainer newMesh)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = newMesh.vertecies.ToArray();
        mesh.triangles = newMesh.triangles.ToArray();

        newMesh.gameObject.GetComponent<MeshFilter>().mesh = mesh;
        newMesh.gameObject.GetComponent<MeshRenderer>().material = baseMaterial;
        newMesh.gameObject.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0f, 1f, .5f, .5f, .25f, .75f, 1f, 1f);
    }
}