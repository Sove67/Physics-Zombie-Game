using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Creator : MonoBehaviour
{
    // Variables
    // Script Link
    public Grid_Generator Grid_Generator;

    // Tower Randomization
    public Vector3Int randomizerRange;

    // Models
    public int sectionHeight;
    public Material towerMaterial;
    public Material roadMaterial;
    public GameObject sectionCap;
    public GameObject cornerCap;
    public GameObject[] sideNone = new GameObject[3];
    public GameObject[] sideLeft = new GameObject[3];
    public GameObject[] sideRight = new GameObject[3];
    public GameObject[] sideLeftRight = new GameObject[3];
    public GameObject[] sideCorner = new GameObject[3];

    public void CreateCityBlock(int sectionCount, Vector2Int gridDimensions, Grid_Generator.Sector[,] sectorGrid, int sectionSideLength, int streetWidth)
    {
        int[] height = SectorPrep(sectionCount, gridDimensions, sectorGrid);

        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                for (int i = 0; i < 4; i++)
                {
                    EdgeCreator(i, x, y, height, gridDimensions, sectorGrid, sectionSideLength, streetWidth);
                }
            }
        }

        // 2 3
        // 0 1

        Vector3[] baseCoords = new Vector3[]
        {
            new Vector3(-streetWidth/2,                                                       2, -streetWidth/2                                                      ),
            new Vector3(gridDimensions.x * (sectionSideLength + streetWidth) - streetWidth/2, 2, -streetWidth/2                                                      ),
            new Vector3(-streetWidth/2,                                                       2, gridDimensions.x * (sectionSideLength + streetWidth) - streetWidth/2),
            new Vector3(gridDimensions.x * (sectionSideLength + streetWidth) - streetWidth/2, 2, gridDimensions.x * (sectionSideLength + streetWidth) - streetWidth/2),
        };
        int[] triangles = new int[] {0, 3, 1,     2, 3, 0};

        GameObject baseplate = new GameObject("Baseplate");
        baseplate.AddComponent<MeshFilter>().mesh.vertices = baseCoords;
        baseplate.GetComponent<MeshFilter>().mesh.triangles = triangles;
        baseplate.AddComponent<MeshRenderer>().material = roadMaterial;
        baseplate.AddComponent<MeshCollider>();
    }

    public int[] SectorPrep(int sectionCount, Vector2Int gridDimensions, Grid_Generator.Sector[,] sectorGrid) // Prepare each sector for sorting
    {
        // For each ID
        int[] height = new int[sectionCount];
        for (int i = 0; i < sectionCount; i++)
        {
            // Make a container for the towers of that ID
            new GameObject("Tower (" + i + ")").transform.SetParent(this.transform);

            // Randomize heights for that ID
            height[i] = Random.Range(randomizerRange.x, randomizerRange.y);
        }

        return (height);
    }

    public void EdgeCreator(int i, int x, int y, int[] height, Vector2Int gridDimensions, Grid_Generator.Sector[,] sectorGrid, int sectionSideLength, int streetWidth)
    {
        GameObject newEdgePart;
        GameObject[] edgeStack = new GameObject[3];
        int id = sectorGrid[x, y].id ?? default(int); // Taken from https://stackoverflow.com/questions/5995317/how-to-convert-c-sharp-nullable-int-to-int/5995418

        int xMod = (x * (sectionSideLength + streetWidth)) + (sectionSideLength / 2);
        int yMod = (y * (sectionSideLength + streetWidth)) + (sectionSideLength / 2);

        int direction = (i * 90);
        float radianDirection = ((-direction + 180) * Mathf.Deg2Rad);
        int left = ((i + 3) % 4);
        int front = i;
        int right = ((i + 1) % 4);
        int back = ((i + 2) % 4);

        if (sectorGrid[x, y].connected[left] && !sectorGrid[x,y].connected[front] && sectorGrid[x, y].connected[right])
        { edgeStack = sideLeftRight; }
        else if (sectorGrid[x, y].connected[left] && !sectorGrid[x, y].connected[front] && !sectorGrid[x, y].connected[right])
        { edgeStack = sideLeft; }
        else if (!sectorGrid[x, y].connected[left] && !sectorGrid[x, y].connected[front] && sectorGrid[x, y].connected[right])
        { edgeStack = sideRight; }
        else if (!sectorGrid[x, y].connected[left] && !sectorGrid[x, y].connected[front] && !sectorGrid[x, y].connected[right])
        { edgeStack = sideNone; }
        else if (sectorGrid[x, y].connected[left] && sectorGrid[x, y].connected[front])
        {
            if (-1 < x + Mathf.RoundToInt(Mathf.Cos(radianDirection)) && x + Mathf.RoundToInt(Mathf.Cos(radianDirection)) < gridDimensions.x &&
                -1 < y + Mathf.RoundToInt(Mathf.Sin(radianDirection)) && y + Mathf.RoundToInt(Mathf.Sin(radianDirection)) < gridDimensions.y &&
                sectorGrid[x + Mathf.RoundToInt(Mathf.Cos(radianDirection)), y + Mathf.RoundToInt(Mathf.Sin(radianDirection))].connected[left] )
            {
                newEdgePart = Instantiate(cornerCap, new Vector3(xMod, (height[id] + 1) * sectionHeight, yMod), Quaternion.Euler(-90, 0, direction), GameObject.Find("Tower (" + id + ")").transform);
                newEdgePart.name = ("Top Section  | Corner Cap | Coords: " + x + ", " + y);
                newEdgePart.AddComponent<MeshCollider>();
                newEdgePart.GetComponent<MeshRenderer>().material = towerMaterial;
                return;
            }
            else
            { edgeStack = sideCorner; }
        }
        else { return; }


        newEdgePart = Instantiate(edgeStack[0], new Vector3(xMod, 0, yMod), Quaternion.Euler(-90, 0, direction), GameObject.Find("Tower (" + id + ")").transform);
        newEdgePart.name = ("Bottom Section | " + edgeStack[0].name + " | Coords: " + x + ", " + y);
        newEdgePart.AddComponent<MeshCollider>();
        newEdgePart.GetComponent<MeshRenderer>().material = towerMaterial;

        for (int b = 1; b <= height[id]; b++)
        {
            newEdgePart = Instantiate(edgeStack[1], new Vector3(xMod, b * sectionHeight, yMod), Quaternion.Euler(-90, 0, direction), GameObject.Find("Tower (" + id + ")").transform);
            newEdgePart.name = ("Middle Section (" + b + ")  | " + edgeStack[0].name + " | Coords: " + x + ", " + y);
            newEdgePart.AddComponent<MeshCollider>();
            newEdgePart.GetComponent<MeshRenderer>().material = towerMaterial;
        }

        newEdgePart = Instantiate(edgeStack[2], new Vector3(xMod, (height[id] + 1) * sectionHeight, yMod), Quaternion.Euler(-90, 0, direction), GameObject.Find("Tower (" + id + ")").transform);
        newEdgePart.name = ("Top Section  | " + edgeStack[0].name + " | Coords: " + x + ", " + y);
        newEdgePart.AddComponent<MeshCollider>();
        newEdgePart.GetComponent<MeshRenderer>().material = towerMaterial;
    }
}
