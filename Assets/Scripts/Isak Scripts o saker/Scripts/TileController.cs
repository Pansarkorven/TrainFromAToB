using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2Int gridPos;
    public SlidingPuzzleManager puzzleManager;

    private void OnMouseDown()
    {
        puzzleManager.SelectTile(this);
    }
}
