using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public GameObject[] bubblePrefabs;
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float moveSpeed = 2.0f;

    private GameObject[,] grid;
    private Vector3[,] targetPositions;

    private void Start()
    {
        grid = new GameObject[gridWidth, gridHeight];
        targetPositions = new Vector3[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject bubble = Instantiate(bubblePrefabs[Random.Range(0, bubblePrefabs.Length)], new Vector3(x, y, 0), Quaternion.identity);
                grid[x, y] = bubble;
                bubble.GetComponent<Bubble>().gameManager = FindObjectOfType<GameManager>();
                targetPositions[x, y] = new Vector3(x, y, 0);
            }
        }
    }

    private void Update()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y].transform.position = Vector3.Lerp(grid[x, y].transform.position, targetPositions[x, y], Time.deltaTime * moveSpeed);
                }
            }
        }
    }

    public List<GameObject> GetMatchingBubbles(GameObject startBubble)
    {
        List<GameObject> matchingBubbles = new List<GameObject>();
        Stack<GameObject> stack = new Stack<GameObject>();
        List<GameObject> visited = new List<GameObject>();

        stack.Push(startBubble);

        while (stack.Count > 0)
        {
            GameObject currentBubble = stack.Pop();

            if (currentBubble != null && !visited.Contains(currentBubble))
            {
                visited.Add(currentBubble);
                matchingBubbles.Add(currentBubble);

                int currentX = (int)currentBubble.transform.position.x;
                int currentY = (int)currentBubble.transform.position.y;

                List<GameObject> neighbors = GetNeighbors(currentX, currentY);
                foreach (GameObject neighbor in neighbors)
                {
                    if (neighbor != null && neighbor.tag == currentBubble.tag && !visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }

        return matchingBubbles;
    }

    public List<GameObject> GetNeighbors(int x, int y)
    {
        List<GameObject> neighbors = new List<GameObject>();

        if (x > 0)
            neighbors.Add(grid[x - 1, y]);
        if (x < gridWidth - 1)
            neighbors.Add(grid[x + 1, y]);
        if (y > 0)
            neighbors.Add(grid[x, y - 1]);
        if (y < gridHeight - 1)
            neighbors.Add(grid[x, y + 1]);

        return neighbors;
    }

public void DropBubbles()
{
    for (int x = 0; x < gridWidth; x++)
    {
        for (int y = 0; y < gridHeight - 1; y++)
        {
            if (grid[x, y] == null)
            {
                for (int yAbove = y + 1; yAbove < gridHeight; yAbove++)
                {
                    if (grid[x, yAbove] != null)
                    {
                        // Update the logical position of the bubble.
                        int newY = y;
                        int newX = x;
                        targetPositions[newX, newY] = new Vector3(newX, newY, 0);

                        // move the bubble in the grid.
                        grid[newX, newY] = grid[x, yAbove];
                        grid[x, yAbove] = null;
                        break;
                    }
                }
            }
        }
    }
}


    public void CollapseColumns()
    {
        // while therre is an empty column that has filled columns to the right of it.
        while (true)
        {
            bool foundEmptyColumnWithFilledColumnsToRight = false;

            for (int x = 0; x < gridWidth - 1; x++)
            {
                if (IsColumnEmpty(x))
                {
                    // check if there are any filled columns to the right of this column.
                    bool foundFilledColumnToRight = false;
                    for (int xFilled = x + 1; xFilled < gridWidth; xFilled++)
                    {
                        if (!IsColumnEmpty(xFilled))
                        {
                            foundFilledColumnToRight = true;
                            break;
                        }
                    }

                    if (foundFilledColumnToRight)
                    {
                        foundEmptyColumnWithFilledColumnsToRight = true;

                        // shift all the columns to the right of the empty column to the left by one.
                        for (int xShift = x + 1; xShift < gridWidth; xShift++)
                        {
                            for (int y = 0; y < gridHeight; y++)
                            {
                                grid[xShift - 1, y] = grid[xShift, y];
                                grid[xShift, y] = null;
                                if (grid[xShift - 1, y] != null)
                                {
                                    targetPositions[xShift - 1, y] = new Vector3(xShift - 1, y, 0);
                                }
                            }
                        }
                    }
                }
            }

            if (!foundEmptyColumnWithFilledColumnsToRight)
            {
                break;
            }
        }
    }


    public void RemoveBubbleFromGrid(GameObject bubble)
    {
        int x = (int)bubble.transform.position.x;
        int y = (int)bubble.transform.position.y;
        if (grid[x, y] == bubble)
        {
            grid[x, y] = null;
        }
    }

    public bool CanBubbleBeClicked(GameObject bubble)
    {
        if (GetMatchingBubbles(bubble).Count > 1)
            return true;
        else
            return false;
    }

    public bool AreMatchingPairsLeft()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    if (CanBubbleBeClicked(grid[x, y]))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool IsColumnEmpty(int x)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (grid[x, y] != null)
            {
                return false;
            }
        }
        return true;
    }
}
