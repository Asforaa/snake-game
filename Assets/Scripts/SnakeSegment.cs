using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // Ensure SpriteRenderer exists
public class SnakeSegment : MonoBehaviour
{
	public Vector2Int GridPosition { get; private set; }
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
		{
			Debug.LogError("SnakeSegment requires a SpriteRenderer component!");
		}
	}

	public void Initialize(Vector2Int startPosition)
	{
		GridPosition = startPosition;
		UpdateVisualPosition();
	}

	public void UpdatePosition(Vector2Int newPosition)
	{
		GridPosition = newPosition;
		UpdateVisualPosition();
	}

	// Method for the controller to update this segment's sprite
	public void UpdateSprite(Sprite newSprite)
	{
		if (spriteRenderer != null && newSprite != null)
		{
			spriteRenderer.sprite = newSprite;
		}
		else if (spriteRenderer == null)
		{
			Debug.LogWarning("Attempted to update sprite on segment with no SpriteRenderer.");
		}
	}

	private void UpdateVisualPosition()
	{
		transform.position = GridManager.Instance.GridToWorldPosition(GridPosition);
	}
}
