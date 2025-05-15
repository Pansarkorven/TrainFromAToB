using UnityEngine;
using System.Collections.Generic;

public class SlidingPuzzleManager : MonoBehaviour
{
    public int gridSize = 3;
    public GameObject tilePrefab;
    public float tileSpacing = 1.1f;

    private TileController[,] tiles;
    private Vector2Int emptyTilePos;

    void Start()
    {
        InitPuzzle();
    }

    void InitPuzzle()
    {
        tiles = new TileController[gridSize, gridSize];

        int num = 1;
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (x == gridSize - 1 && y == gridSize - 1)
                {
                    emptyTilePos = new Vector2Int(x, y);
                    continue;
                }

                Vector3 position = new Vector3(x * tileSpacing, 0, y * tileSpacing);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                TileController controller = tile.GetComponent<TileController>();
                controller.gridPos = new Vector2Int(x, y);
                controller.puzzleManager = this;
                tile.name = "Tile " + num;

                tiles[x, y] = controller;
                num++;
            }
        }
    }

    public void TryMoveTile(TileController tile)
    {
        Vector2Int pos = tile.gridPos;

        if (IsAdjacent(pos, emptyTilePos))
        {
            // Swap positions
            tiles[emptyTilePos.x, emptyTilePos.y] = tile;
            tiles[pos.x, pos.y] = null;

            Vector2Int oldEmpty = emptyTilePos;
            emptyTilePos = pos;
            tile.gridPos = oldEmpty;

            Vector3 newWorldPos = new Vector3(oldEmpty.x * tileSpacing, 0, oldEmpty.y * tileSpacing);
            tile.transform.position = newWorldPos;
        }
    }

    bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) == 1;
    }
}
