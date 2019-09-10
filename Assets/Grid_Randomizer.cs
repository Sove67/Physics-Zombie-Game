using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Randomizer : MonoBehaviour
{
    // Num Grid
    public int gridLength;
    public int randomizerRange;
    public List<List<int>> numGrid = new List<List<int>>();

    // Vertex Grid
    public int sectionSideLength;
    public List<List<Vector2[]>> vertexGrid = new List<List<Vector2[]>>();

    // ID Grid
    public List<List<int?>> sectGrid = new List<List<int?>>();

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
                var x = a * sectionSideLength;
                var y = b * sectionSideLength;

                Vector2[] square = new Vector2[4];

                /*  [0][1]
                    [2][3]  */
                square[0] = new Vector2(x,                      y                       );
                square[1] = new Vector2(x + sectionSideLength,  y                       );
                square[2] = new Vector2(x,                      y - sectionSideLength   );
                square[3] = new Vector2(x + sectionSideLength,  y - sectionSideLength   );

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

    // Creates a populated grid with groups of "numGrid"'s values using a unique ID for each. 
    // Groups are formed by cells that are neighbouring another cell of the same number in the cardinal directions.
    public void SectionedGrid()
    {
        // Create an empty grid
        for (int a = 0; a < gridLength; a++)
        {
            List<int?> sectGridCollumn = new List<int?>();
            for (int b = 0; b < gridLength; b++)
            {
                sectGridCollumn.Add(null);
            }
            sectGrid.Add(sectGridCollumn);
        }

        // Fill it with IDs
        var id = 0;
        for (int a = 0; a < gridLength; a++)
        {
            List<int?> sectGridCollumn = new List<int?>();
            for (int b = 0; b < gridLength; b++)
            {
                if (sectGrid[a][b] == null)
                {
                    sectGrid[a][b] = id;

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
                text = text + sectGrid[a][b] + ", ";

            }
        }

        sections.text = text;
    }

    // Assign any cardinally connected numbers the same ID as the first.
    public void GridCrawler(int a, int b, int id)
    {

        // Up
        if (b - 1 > 0 && numGrid[a][b] == numGrid[a][b - 1] && sectGrid[a][b - 1] == null)
        {
            sectGrid[a][b - 1] = id;
            GridCrawler(a, b - 1, id);
        }

        // Left
        if (a - 1 > 0 && numGrid[a][b] == numGrid[a - 1][b] && sectGrid[a - 1][b] == null)
        {
            sectGrid[a - 1][b] = id;
            GridCrawler(a - 1, b, id);
        }

        // Down
        if (b + 1 < numGrid[0].Count && numGrid[a][b] == numGrid[a][b + 1] && sectGrid[a][b + 1] == null)
        {
            sectGrid[a][b + 1] = id;
            GridCrawler(a, b + 1, id);
        }

        // Left
        if (a + 1 < numGrid.Count && numGrid[a][b] == numGrid[a + 1][b] && sectGrid[a + 1][b] == null)
        {
            sectGrid[a + 1][b] = id;
            GridCrawler(a + 1, b, id);
        }
    }
}