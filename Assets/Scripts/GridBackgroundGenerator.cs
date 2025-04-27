using UnityEngine;

public class GridBackgroundGenerator : MonoBehaviour
{
	[Header("Sprite References")]
	[SerializeField] private Sprite lightCellSprite; // Assign light green sprite in Inspector
	[SerializeField] private Sprite darkCellSprite;  // Assign dark green sprite in Inspector

	[Header("Grid Settings (Match GridManager)")]
	[SerializeField] private int gridWidth = 32;   // Should match GridManager
	[SerializeField] private int gridHeight = 18;  // Should match GridManager
	[SerializeField] private float cellSize = 1f;  // Should match GridManager

	[Header("Rendering Settings")]
	[SerializeField] private float zPosition = 1f; // Position behind snake/food
	[SerializeField] private int sortingOrder = -10; // Ensure it draws behind

	void Start()
	{
		GenerateGrid();
	}

	void GenerateGrid()
	{
		if (lightCellSprite == null || darkCellSprite == null)
		{
			Debug.LogError("Assign Light and Dark Cell Sprites in the Inspector!");
			return;
		}

		// Optionally clear old cells if regenerating
		foreach (Transform child in transform)
		{
			if (Application.isPlaying) // Only destroy if running
				Destroy(child.gameObject);
			else // DestroyImmediate needed for editor regeneration
				DestroyImmediate(child.gameObject);
		}

		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				// Create a new GameObject for the cell
				GameObject cell = new GameObject($"Cell_{x}_{y}");
				cell.transform.SetParent(transform); // Parent to GridBackground

				// Calculate world position based *directly* on cell index and size
				Vector3 worldPos = new Vector3(
				   (x * cellSize),
				   (y * cellSize),
				   zPosition
			   );
				cell.transform.position = worldPos;

				// Add and configure SpriteRenderer
				SpriteRenderer sr = cell.AddComponent<SpriteRenderer>();

				// Choose sprite based on checkerboard pattern
				bool isLight = (x + y) % 2 == 0;
				sr.sprite = isLight ? lightCellSprite : darkCellSprite;

				// Set sorting order
				sr.sortingOrder = sortingOrder;

				// Scale the cell sprite if necessary (shouldn't be if PPU is set correctly)
				// sr.transform.localScale = Vector3.one * cellSize; // Only if PPU doesn't match cell size
			}
		}
	}

	// Optional: Button to regenerate in Editor
	[ContextMenu("Regenerate Grid")]
	void RegenerateGridEditor()
	{
		// Ensure GridManager values are up-to-date if changed in inspector
		// In a real scenario, you might fetch these from GridManager instance if available in editor
		GenerateGrid();
	}
}
