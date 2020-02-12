using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Generator : MonoBehaviour
{
    // Num Grid
    public int gridLength;
    public int randomizerRange;
    private List<List<int>> numGrid = new List<List<int>>{};

    // Vertex Grid
    public int sectionSideLength;
    public int streetWidth;

    // Sector Grid
    public List<List<Sector>> sectorGrid = new List<List<Sector>>{};

    // Debugging
    public Text numbers;
    public Text vertecies;
    public Text sections;

    public void Start()
    {
        NumberGrid();
        SectorGrid();
        VertexFill();
        Section();
    }
    
    public void NumberGrid() // Creates a populated grid with random numbers within the "randomizerRange"
    {
        for (int a = 0; a < gridLength; a++)
        {
            List<int> numGridRow = new List<int>{};

            for (int b = 0; b < gridLength; b++)
            { numGridRow.Add(Random.Range(0, (0 + randomizerRange))); }

            numGrid.Add(numGridRow);
        }

        // Debugging
        var text = "Number Grid:";
        for (int a = 0; a < gridLength; a++)
        {
            text = text + "\n\n";
            for (int b = 0; b < gridLength; b++)
            {
                text = text + numGrid[a][b] + ", ";

            }
        }

        numbers.text = text;
    }
     
    public class Sector
    {
        public int? id { get; set; }
        public List<Vector2> vertexPosition { get; set; }
        public bool vertexToggle { get; set; }
        public Sector(int? id, List<Vector2> vertexPosition, bool vertexToggle)
        {
            this.id = id;
            this.vertexPosition = vertexPosition;
            this.vertexToggle = vertexToggle;
        }
    }

    public void SectorGrid() // Creates an empty "sectorGrid"
    {
        for (int a = 0; a < gridLength; a++)
        {
            List<Sector> sectorGridRow = new List<Sector>{};

            for (int b = 0; b < gridLength; b++)
            {
                sectorGridRow.Add(new Sector(null, new List<Vector2>{}, true));
            }
            sectorGrid.Add(sectorGridRow);
        }
    }
    
    public void VertexFill() // Fills the "sectorGrid" with four points on a 2D plane in the corners of each sector.
    {
        for (int a = 0; a < gridLength; a++)
        {
            for (int b = 0; b < gridLength; b++)
            {
                var x = a * (sectionSideLength + streetWidth);
                var y = b * (sectionSideLength + streetWidth);

                /*  [0][1]
                    [2][3]  */
                sectorGrid[a][b].vertexPosition.Add(new Vector2(x + streetWidth / 2,                        y - streetWidth / 2                     ));
                sectorGrid[a][b].vertexPosition.Add(new Vector2(x - streetWidth / 2 + sectionSideLength,    y - streetWidth / 2                     ));
                sectorGrid[a][b].vertexPosition.Add(new Vector2(x + streetWidth / 2,                        y + streetWidth / 2 - sectionSideLength ));
                sectorGrid[a][b].vertexPosition.Add(new Vector2(x - streetWidth / 2 + sectionSideLength,    y + streetWidth / 2 - sectionSideLength ));
            }
        }

        //Debug.Log("Vertex Grid (a,b)[c]:" + sectorGrid[a][b].vertexPosition[c]);
        // Debugging
        var text = "Vertex Grid:";
        for (int a = 0; a < gridLength; a++)
        {
            text = text + "\n\n";
            for (int b = 0; b < gridLength; b++)
            {
                text = text + sectorGrid[a][b].vertexPosition[0] + sectorGrid[a][b].vertexPosition[1] + sectorGrid[a][b].vertexPosition[2] + sectorGrid[a][b].vertexPosition[3] + ",  ";

            }
        }
        vertecies.text = text;
    }

    public void Section() // Sections "sectorGrid" based on "numGrid". Groups are formed according to "GridCrawler"
    {
        int id = 0;
        for (int a = 0; a < gridLength; a++)
        {
            List<int?> sectorGridCollumn = new List<int?> { };
            for (int b = 0; b < gridLength; b++)
            {
                if (sectorGrid[a][b].id == null)
                {
                    sectorGrid[a][b].id = id;
                    GridCrawler(a, b, id);
                    id++;
                }
            }
        }

        // Debugging
        var text = "Sections Grid:";
        for (int a = 0; a < gridLength; a++)
        {
            text = text + "\n\n";
            for (int b = 0; b < gridLength; b++)
            {
                text = text + sectorGrid[a][b].id + ", ";

            }
        }
        sections.text = text;
    }

    public void GridCrawler(int x, int y, int id) // Assign any cardinally connected numbers the same ID as the first, and call on each
    {
        // Up
        if (y - 1 > 0 && numGrid[x][y] == numGrid[x][y - 1] && sectorGrid[x][y - 1].id == null)
        {
            sectorGrid[x][y - 1].id = id;
            GridCrawler(x, y - 1, id);
        }

        // Left
        if (x - 1 > 0 && numGrid[x][y] == numGrid[x - 1][y] && sectorGrid[x - 1][y].id == null)
        {
            sectorGrid[x - 1][y].id = id;
            GridCrawler(x - 1, y, id);
        }

        // Down
        if (y + 1 < numGrid[0].Count && numGrid[x][y] == numGrid[x][y + 1] && sectorGrid[x][y + 1].id == null)
        {
            sectorGrid[x][y + 1].id = id;
            GridCrawler(x, y + 1, id);
        }

        // Right
        if (x + 1 < numGrid.Count && numGrid[x][y] == numGrid[x + 1][y] && sectorGrid[x + 1][y].id == null)
        {
            sectorGrid[x + 1][y].id = id;
            GridCrawler(x + 1, y, id);
        }
    }
}