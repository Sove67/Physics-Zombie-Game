using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Generator : MonoBehaviour
{
    // Variables
    // Script Link
    public Mesh_Creator Mesh_Creator;

    // Grid Parameters
    public Vector2Int gridDimensions;
    public int randomizerRange;
    public int sectionSideLength;
    public int streetWidth;
    public int defaultHeight;

    // Num Grid
    private int[,] numGrid;
    public Sector[,] sectorGrid;
    public int sectionCount;

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
        public bool crawled { get; set; }

        public Sector(int? id, Vector2[] vertexPosition, bool crawled, Connection connected)
        {
            this.id = id;
            this.vertexPosition = vertexPosition;
            this.crawled = crawled;
            this.connected = connected;
        }
    }

    // Functions
    public void Start()
    {
        NumberGrid();
        SectionGrid();

        Mesh_Creator.CreateMeshObject();
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
                sectorGrid[x, y] = new Sector(null, new Vector2[4], false, new Connection(false, false, false, false));

                float xMod = x * (sectionSideLength + streetWidth);
                float yMod = y * (sectionSideLength + streetWidth);

                // 2 3
                // 0 1

                sectorGrid[x, y].vertexPosition[0] = (new Vector2(xMod,                     yMod));
                sectorGrid[x, y].vertexPosition[1] = (new Vector2(xMod + sectionSideLength, yMod));
                sectorGrid[x, y].vertexPosition[2] = (new Vector2(xMod,                     yMod + sectionSideLength));
                sectorGrid[x, y].vertexPosition[3] = (new Vector2(xMod + sectionSideLength, yMod + sectionSideLength));

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

    public void ConnectionMarker(int x, int y, int id) // Marks the connection of any cardinally connected sectors with the same ID. UP/DOWN INVERTED
    {
        // Left
        if (x - 1 >= 0 && numGrid[x, y] == numGrid[x - 1, y])
        {
            sectorGrid[x, y].connected.left = true;
        }

        // Right
        if (x + 1 < gridDimensions.x && numGrid[x, y] == numGrid[x + 1, y])
        {
            sectorGrid[x, y].connected.right = true;
        }

        // Up
        if (y + 1 < gridDimensions.y && numGrid[x, y] == numGrid[x, y + 1])
        {
            sectorGrid[x, y].connected.up = true;
        }

        // Down
        if (y - 1 >= 0 && numGrid[x, y] == numGrid[x, y - 1])
        {
            sectorGrid[x, y].connected.down = true;
        }
    }
}