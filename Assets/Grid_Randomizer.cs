using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Randomizer : MonoBehaviour
{
    // Grid Population
    public int gridLength;
    public int randomizerRange;
    public List<List<int>> numGrid = new List<List<int>>();

    //Grid Rendering
    public int sectionSideLength;
    public List<List<Vector2[]>> vertexGrid = new List<List<Vector2[]>>();

    public void Start()
    {
        NumberGrid();
        VertexGrid();
    }

    // Creates a populated grid with random numbers within the "randomizerRange"
    public void NumberGrid()
    {
        List<int> numGridCollumn = new List<int>();

        for (int a = 0; a < gridLength; a++)
        {
            for (int b = 0; b < gridLength; b++)
            { numGridCollumn.Add(Random.Range(0, (0 + randomizerRange))); }

            numGrid.Add(numGridCollumn);
        }
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



        int rowIndex = 0;
        for(int a = 0; a < gridLength; a++)
        {
            List<Vector2[]> vertexGridCollumn = new List<Vector2[]>();
            int collumnIndex = 0;

            for (int b = 0; b < gridLength; b++)
            {
                var x = rowIndex * sectionSideLength;
                var y = collumnIndex * sectionSideLength;

                Vector2[] square = new Vector2[4];

                /*  [0][1]
                    [2][3]  */
                square[0] = new Vector2(x, y);
                square[1] = new Vector2(x + sectionSideLength, y);
                square[2] = new Vector2(x, y - sectionSideLength);
                square[3] = new Vector2(x + sectionSideLength, y - sectionSideLength);

                vertexGridCollumn.Add(square);
                

                collumnIndex++;
            }
            vertexGrid.Add(vertexGridCollumn);

            rowIndex++;
        }
    }
    
    // Creates a populated grid with groups of "numGrid"'s values using a unique ID for each. 
    // Groups are formed by cells that are neighbouring another cell of the same number in the cardinal directions.
    public void IDGrid()
    {

    }
}
