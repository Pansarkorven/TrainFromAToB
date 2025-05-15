using UnityEngine;

public class TileController : MonoBehaviour
{
    public Vector2Int gridPos; // Current position in the grid
    public SlidingPuzzleManager puzzleManager;

    private void OnMouseDown()
    {
        puzzleManager.TryMoveTile(this);
    }
}
