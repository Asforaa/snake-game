using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] public int width = 32;
    [SerializeField] public int height = 18;
    [SerializeField] private float cellSize = 1f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public Vector2 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector2(gridPosition.x * cellSize, gridPosition.y * cellSize);
    }

    public Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.y / cellSize)
        );
    }

    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < width &&
               gridPosition.y >= 0 && gridPosition.y < height;
    }

    public Vector2Int GetRandomGridPosition()
    {
        return new Vector2Int(
            Random.Range(0, width),
            Random.Range(0, height)
        );
    }

    // Draw grid in editor for visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        // Draw horizontal lines
        for (int y = 0; y <= height; y++)
        {
            Gizmos.DrawLine(
                new Vector3(0, y * cellSize, 0),
                new Vector3(width * cellSize, y * cellSize, 0)
            );
        }

        // Draw vertical lines
        for (int x = 0; x <= width; x++)
        {
            Gizmos.DrawLine(
                new Vector3(x * cellSize, 0, 0),
                new Vector3(x * cellSize, height * cellSize, 0)
            );
        }
    }
}
