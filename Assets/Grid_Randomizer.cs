using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Randomizer : MonoBehaviour
{
    public int gridHeight;
    public int gridWidth;
    public int randomizerRange;
    public List<List<int>> row = new List<List<int>>();

    public Text container;

    public void Start()
    {
        Create();
    }

    public void Create()
    {
        for (int a = 0; a < gridWidth; a++)
        {
            List<int> collumn = new List<int>();

            for (int b = 0; b < gridHeight; b++)
            {
                collumn.Add(Random.Range(0, (0 + randomizerRange)));
            }

            foreach (var item in collumn)
            {
                Debug.Log(item);
            }

            row.Add(collumn);

            string text = "Grid:";
            
            foreach (var child in row)
            {
                text = text + "\n\n";
                foreach (var item in child)
                {
                    text = text + item + " ";
                }
            }

            container.text = text;
        }
    }
}
