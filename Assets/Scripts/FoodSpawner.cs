using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;
    // private SnakeController snakeController; // We don't seem to use this reference, can remove if not needed

    private void Start()
    {
        Debug.Log("[FoodSpawner] Start called.");
        // snakeController = FindFirstObjectByType<SnakeController>(); // FindFirst is fine, but log if needed
        // Debug.Log($"[FoodSpawner] Found SnakeController: {snakeController != null}");
        SpawnFood();
    }

    public void SpawnFood()
    {
        Debug.Log("[FoodSpawner] SpawnFood called.");
        // Find a valid position that doesn't overlap with the snake
        Vector2Int spawnPosition;
        bool validPosition = false; // Initialize to false
        int maxAttempts = 100; // Prevent infinite loop
        int attempts = 0;

        do
        {
            spawnPosition = GridManager.Instance.GetRandomGridPosition();
            // Consider adding a log here if IsValidFoodPosition is complex/failing
            // Debug.Log($"[FoodSpawner] Attempt {attempts + 1}: Trying position {spawnPosition}");
            validPosition = IsValidFoodPosition(spawnPosition);
            attempts++;
        } while (!validPosition && attempts < maxAttempts);

        if (validPosition)
        {
            Vector2 worldPosition = GridManager.Instance.GridToWorldPosition(spawnPosition);
            Instantiate(foodPrefab, worldPosition, Quaternion.identity, transform);
            Debug.Log($"[FoodSpawner] Food instantiated at grid position: {spawnPosition}");
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
            // Debug.Log($"[FoodSpawner] Position {position} invalid: Out of bounds.");
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
                // Debug.Log($"[FoodSpawner] Position {position} invalid: Overlaps with {collider.tag} ({collider.gameObject.name}).");
                return false;
            }
        }

        // Debug.Log($"[FoodSpawner] Position {position} is valid.");
        return true;
    }
}
