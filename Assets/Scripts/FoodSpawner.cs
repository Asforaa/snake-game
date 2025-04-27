using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;

    private void Start()
    {
        SpawnFood();
    }

    public void SpawnFood()
    {
        // Find a valid position that doesn't overlap with the snake
        Vector2Int spawnPosition;
        bool validPosition = false; // Initialize to false
        int maxAttempts = 100; // Prevent infinite loop
        int attempts = 0;

        do
        {
            spawnPosition = GridManager.Instance.GetRandomGridPosition();
            validPosition = IsValidFoodPosition(spawnPosition);
            attempts++;
        } while (!validPosition && attempts < maxAttempts);

        if (validPosition)
        {
            Vector2 worldPosition = GridManager.Instance.GridToWorldPosition(spawnPosition);
            Instantiate(foodPrefab, worldPosition, Quaternion.identity, transform);
        }
        else
        {
            Debug.LogWarning($"[FoodSpawner] Failed to find a valid position for food after {attempts} attempts.");
        }
    }

    private bool IsValidFoodPosition(Vector2Int position)
    {
        // Make sure it's within grid bounds
        if (!GridManager.Instance.IsValidGridPosition(position))
        {
            return false;
        }

        // Check if position overlaps with snake or its segments
        // This is simplified and may need to be expanded based on how you're tracking snake positions
        // Increased radius slightly to be safer with FindObjectOfType potentially grabbing colliders during movement frames
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            GridManager.Instance.GridToWorldPosition(position),
            0.5f // Increased radius slightly
        );

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Snake") || collider.CompareTag("SnakeSegment"))
            {
                return false;
            }
        }

        return true;
    }
}
