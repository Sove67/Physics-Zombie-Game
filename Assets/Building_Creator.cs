using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Creator : MonoBehaviour
{
    // Variables
    // Script Link
    public Grid_Generator Grid_Generator;

    // Tower Randomization
    public Vector2Int randomizerRange;
    private int[] height;

    // Models
    public int sectionHeight;
    public GameObject[] towerO = new GameObject[3];
    public GameObject[] towerQ = new GameObject[3];
    public GameObject[] towerI = new GameObject[3];
    public GameObject[] towerL = new GameObject[3];
    public GameObject[] towerT = new GameObject[3];
    public GameObject towerXTop;

    // Connection Cases
    private Grid_Generator.Connection all;
    private Grid_Generator.Connection none;

    private Grid_Generator.Connection left;
    private Grid_Generator.Connection right;
    private Grid_Generator.Connection up;
    private Grid_Generator.Connection down;

    private Grid_Generator.Connection leftRight;
    private Grid_Generator.Connection upDown;

    private Grid_Generator.Connection leftUp;
    private Grid_Generator.Connection leftDown;
    private Grid_Generator.Connection rightUp;
    private Grid_Generator.Connection rightDown;

    private Grid_Generator.Connection leftRightUp;
    private Grid_Generator.Connection leftRightDown;
    private Grid_Generator.Connection leftUpDown;
    private Grid_Generator.Connection rightUpDown;

    // Functions
    public void Start()
    {
        // Left, Right, Up, Down
        all =   new Grid_Generator.Connection(true,  true,  true,  true );
        none =  new Grid_Generator.Connection(false, false, false, false);

        left =  new Grid_Generator.Connection(true,  false, false, false);
        right = new Grid_Generator.Connection(false, true,  false, false);
        up =    new Grid_Generator.Connection(false, false, true,  false);
        down =  new Grid_Generator.Connection(false, false, false, true );

        leftRight = new Grid_Generator.Connection(true,  true,  false, false);
        upDown =    new Grid_Generator.Connection(false, false, true,  true );

        leftUp =    new Grid_Generator.Connection(true,  false, true,  false);
        leftDown =  new Grid_Generator.Connection(true,  false, false, true );
        rightUp =   new Grid_Generator.Connection(false, true,  true,  false);
        rightDown = new Grid_Generator.Connection(false, true,  false, true );

        leftRightUp =   new Grid_Generator.Connection(true,  true,  true,  false);
        leftRightDown = new Grid_Generator.Connection(true,  true,  false, true );
        leftUpDown =    new Grid_Generator.Connection(true,  false, true,  true );
        rightUpDown =   new Grid_Generator.Connection(false, true,  true,  true );
    }

    public void CreateBuildings(int sectionCount, Vector2Int gridDimensions, Grid_Generator.Sector[,] sectorGrid, int sectionSideLength, int streetWidth)
    {
        int[] height = SectorPrep(sectionCount, gridDimensions, sectorGrid);

        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                int ID = sectorGrid[x, y].id ?? default(int); // Taken from https://stackoverflow.com/questions/5995317/how-to-convert-c-sharp-nullable-int-to-int/5995418

                if (Grid_Generator.SameConnection(sectorGrid[x, y].connected, all)) // If it's the irregular X block dont do any checks
                {
                    float xMod = x * (sectionSideLength + streetWidth);
                    float yMod = y * (sectionSideLength + streetWidth);
                    GameObject towerSection = Instantiate(towerXTop, new Vector3(xMod + (sectionSideLength / 2), (height[ID] + 1) * sectionHeight, yMod + (sectionSideLength / 2)), Quaternion.Euler(new Vector3(-90, 0, 0)), GameObject.Find("Tower (" + sectorGrid[x, y].id + ")").transform);
                    towerSection.name = ("Tower Top");
                }
                else
                {
                    (GameObject[] stack, int rotation) = SectorSort(sectorGrid[x, y].connected);
                    InstantiateBuildings(x, y, stack, rotation, height[ID], sectorGrid, sectionSideLength, streetWidth);
                }
            }
        }
    }

    public int[] SectorPrep(int sectionCount, Vector2Int gridDimensions, Grid_Generator.Sector[,] sectorGrid) // Prepare each sector for sorting
    {
        // For each ID
        height = new int[sectionCount];
        for (int i = 0; i < sectionCount; i++)
        {
            // Make a container for the towers of that ID
            GameObject newBuilding = Instantiate(new GameObject(), this.transform);
            newBuilding.name = ("Tower (" + i + ")");

            // Randomize heights for that ID
            height[i] = Random.Range(randomizerRange.x, randomizerRange.y);
        }

        return (height);
    }

    public (GameObject[], int) SectorSort(Grid_Generator.Connection connections)
    {
        GameObject[] stack = null;
        int rotation = 0;

        if (Grid_Generator.SameConnection(connections, none))
        { stack = towerO; rotation = 0; }

        if (Grid_Generator.SameConnection(connections, left))
        { stack = towerQ; rotation = 0; }
        if (Grid_Generator.SameConnection(connections, right))
        { stack = towerQ; rotation = 180; }
        if (Grid_Generator.SameConnection(connections, up))
        { stack = towerQ; rotation = 90; }
        if (Grid_Generator.SameConnection(connections, down))
        { stack = towerQ; rotation = 270; }

        if (Grid_Generator.SameConnection(connections, leftRight))
        { stack = towerI; rotation = 90; }
        if (Grid_Generator.SameConnection(connections, upDown))
        { stack = towerI; rotation = 0; }

        if (Grid_Generator.SameConnection(connections, leftUp))
        { stack = towerL; rotation = 90; }
        if (Grid_Generator.SameConnection(connections, leftDown))
        { stack = towerL; rotation = 0; }
        if (Grid_Generator.SameConnection(connections, rightUp))
        { stack = towerL; rotation = 180; }
        if (Grid_Generator.SameConnection(connections, rightDown))
        { stack = towerL; rotation = 270; }

        if (Grid_Generator.SameConnection(connections, leftRightUp))
        { stack = towerT; rotation = 180; }
        if (Grid_Generator.SameConnection(connections, leftRightDown))
        { stack = towerT; rotation = 0; }
        if (Grid_Generator.SameConnection(connections, leftUpDown))
        { stack = towerT; rotation = 90; }
        if (Grid_Generator.SameConnection(connections, rightUpDown))
        { stack = towerT; rotation = 270; }

        return (stack, rotation);
    }

    public void InstantiateBuildings(int x, int y, GameObject[] towerBlock, int rotation, int height, Grid_Generator.Sector[,] sectorGrid, int sectionSideLength, int streetWidth)
    {
        float xMod = x * (sectionSideLength + streetWidth);
        float yMod = y * (sectionSideLength + streetWidth);
        GameObject towerSection;
        
        towerSection = Instantiate(towerBlock[0], new Vector3(xMod + (sectionSideLength / 2), 0, yMod + (sectionSideLength / 2)), Quaternion.Euler(new Vector3(-90, 0, rotation)), GameObject.Find("Tower (" + sectorGrid[x, y].id + ")").transform);
        towerSection.name = ("Tower Bottom");

        for (int i = 1; i < height + 1; i++)
        {
            towerSection = Instantiate(towerBlock[1], new Vector3(xMod + (sectionSideLength / 2), i * sectionHeight, yMod + (sectionSideLength / 2)), Quaternion.Euler(new Vector3(-90, 0, rotation)), GameObject.Find("Tower (" + sectorGrid[x, y].id + ")").transform);
            towerSection.name = ("Tower Middle (" + sectorGrid[x, y].id + ")");
        }

        towerSection = Instantiate(towerBlock[2], new Vector3(xMod + (sectionSideLength / 2), (height + 1) * sectionHeight, yMod + (sectionSideLength / 2)), Quaternion.Euler(new Vector3(-90, 0, rotation)), GameObject.Find("Tower (" + sectorGrid[x, y].id + ")").transform);
        towerSection.name = ("Tower Top");
    } // Create a tower stack
}
