using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Generator : MonoBehaviour
{
    // Num Grid
    public int gridLength;
    public int randomizerRange;
    public List<List<int>> numGrid = new List<List<int>>();

    // Vertex Grid
    public int sectionSideLength;
    public int streetWidth;
    public List<List<Vector2[]>> vertexGrid = new List<List<Vector2[]>>();

    // ID Grid
    public List<List<SectUnit>> sectGrid = new List<List<SectUnit>>();

    // Debugging
    public Text numbers;
    public Text vertecies;
    public Text sections;

    public void Start()
    {
        NumberGrid();
        VertexGrid();
        SectionedGrid();
    }
    // Creates a populated grid with random numbers within the "randomizerRange"
    public void NumberGrid()
    {

        for (int a = 0; a < gridLength; a++)
        {
            List<int> numGridCollumn = new List<int>();

            for (int b = 0; b < gridLength; b++)
            { numGridCollumn.Add(Random.Range(0, (0 + randomizerRange))); }

            numGrid.Add(numGridCollumn);
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

    // Creates a populated grid with four points on a 2D plane in the corners of each grid section.
    /*  This grid creates a layered series of lists like so:
     *  
     *      /               y0                         y1                         y2                         y3            \
     * x0  {|   [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)] |}
     *      \==============================================================================================================/
     *      /               y0                         y1                         y2                         y3            \
     * x2  {|   [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)] |}
     *      \==============================================================================================================/
     *      /               y0                         y1                         y2                         y3            \
     * x3  {|   [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)] |}
     *      \==============================================================================================================/
     *      /               y0                         y1                         y2                         y3            \
     * x4  {|   [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)]  [(x,y),(x,y),(x,y),(x,y)] |}
     *      \==============================================================================================================/
     */
    public void VertexGrid() 
    {
        for(int a = 0; a < gridLength; a++)
        {
            List<Vector2[]> vertexGridCollumn = new List<Vector2[]>();
            for (int b = 0; b < gridLength; b++)
            {
                var x = a * (sectionSideLength + streetWidth);
                var y = b * (sectionSideLength + streetWidth);

                Vector2[] square = new Vector2[4];

                /*  [0][1]
                    [2][3]  */
                square[0] = new Vector2(x + streetWidth/2,                      y - streetWidth/2     );
                square[1] = new Vector2(x - streetWidth/2 + sectionSideLength,  y - streetWidth/2     );
                square[2] = new Vector2(x + streetWidth/2,                      y + streetWidth / 2 - sectionSideLength   );
                square[3] = new Vector2(x - streetWidth/2 + sectionSideLength,  y + streetWidth / 2 - sectionSideLength   );

                vertexGridCollumn.Add(square);
            }

            vertexGrid.Add(vertexGridCollumn);
        }

        //Debug.Log("Vertex Grid (0,0)[0]:" + vertexGrid[0][0][0]);
        // Debugging
        var text = "Number Grid:";
        for (int a = 0; a < gridLength; a++)
        {
            text = text + "\n\n";
            for (int b = 0; b < gridLength; b++)
            {
                text = text + vertexGrid[a][b][0] + vertexGrid[a][b][1] + vertexGrid[a][b][2] + vertexGrid[a][b][3] + ",  ";

            }
        }

        vertecies.text = text;
    }

    public class SectUnit
    {
        public int? id { get; set; }
        public bool UL { get; set; }
        public bool UR { get; set; }
        public bool DL { get; set; }
        public bool DR { get; set; }

        public SectUnit(int? id, bool UL, bool UR, bool DL, bool DR)
        {
            this.id = id;
            this.UL = UL;
            this.UR = UR;
            this.DL = DL;
            this.DR = DR;
        }
    }

    // Creates a populated grid with groups of "numGrid"'s values using a unique ID for each. 
    // Groups are formed by cells that are neighbouring another cell of the same number in the cardinal directions.
    public void SectionedGrid()
    {
        // Create an empty grid
        for (int a = 0; a < gridLength; a++)
        {
            List<SectUnit> sectGridCollumn = new List<SectUnit>();
            for (int b = 0; b < gridLength; b++)
            {
                sectGridCollumn.Add(new SectUnit(null, true, true, true, true));
            }
            sectGrid.Add(sectGridCollumn);
        }
        // Fill the grid with IDs
        var id = 0;
        for (int a = 0; a < gridLength; a++)
        {
            List<int?> sectGridCollumn = new List<int?>();
            for (int b = 0; b < gridLength; b++)
            {
                if (sectGrid[a][b].id == null)
                {
                    sectGrid[a][b].id = id;

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
                text = text + sectGrid[a][b].id + ", ";

            }
        }

        sections.text = text;
    }


    // Assign any cardinally connected numbers the same ID as the first, and call on each
    public void GridCrawler(int x, int y, int id)
    {
        // Up
        if (y - 1 > 0 && numGrid[x][y] == numGrid[x][y - 1] && sectGrid[x][y - 1].id == null)
        {
            sectGrid[x][y - 1].id = id;
            GridCrawler(x, y - 1, id);
        }

        // Left
        if (x - 1 > 0 && numGrid[x][y] == numGrid[x - 1][y] && sectGrid[x - 1][y].id == null)
        {
            sectGrid[x - 1][y].id = id;
            GridCrawler(x - 1, y, id);
        }

        // Down
        if (y + 1 < numGrid[0].Count && numGrid[x][y] == numGrid[x][y + 1] && sectGrid[x][y + 1].id == null)
        {
            sectGrid[x][y + 1].id = id;
            GridCrawler(x, y + 1, id);
        }

        // Right
        if (x + 1 < numGrid.Count && numGrid[x][y] == numGrid[x + 1][y] && sectGrid[x + 1][y].id == null)
        {
            sectGrid[x + 1][y].id = id;
            GridCrawler(x + 1, y, id);
        }

        //CornerChecker(x, y, id);
    }

    // Decide which vertecies will be used from exch segment
    public void CornerChecker(int x, int y, int id)
    {
        // Up, Left
        if (x - 1 > 0               && y - 1 > 0                    && numGrid[x][y] == numGrid[x - 1][y - 1])
        {
            sectGrid[x][y].UL = false;
        }


        // Up, Right
        if (x + 1 < numGrid.Count   && y - 1 > 0                    && numGrid[x][y] == numGrid[x + 1][y - 1])
        {
            sectGrid[x][y].UL = false;
        }


        // Down, Left
        if (x - 1 > 0               && y + 1 < numGrid[0].Count     && numGrid[x][y] == numGrid[x - 1][y + 1])
        {
            sectGrid[x][y].UL = false;
        }


        // Down, Right
        if (x + 1 < numGrid.Count   && y + 1 < numGrid[0].Count     && numGrid[x][y] == numGrid[x + 1][y + 1])
        {
            sectGrid[x][y].UL = false;
        }
    }
}