using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class SnakeController : MonoBehaviour
{
	[Header("Prefabs & Setup")]
	[SerializeField] private GameObject snakeSegmentPrefab;
	[SerializeField] private Transform segmentsParent;

	[Header("Movement")]
	[SerializeField] private float moveSpeed = 5f;

	[Header("Sprites")] // Add section for sprites
	[SerializeField] private Sprite headUpSprite;
	[SerializeField] private Sprite headDownSprite;
	[SerializeField] private Sprite headLeftSprite;
	[SerializeField] private Sprite headRightSprite;
	// --- Body Sprites ---
	[SerializeField] private Sprite bodyVerticalSprite;
	[SerializeField] private Sprite bodyHorizontalSprite;
	[SerializeField] private Sprite bodyTopLeftSprite; // Turn from Up to Left or Right to Down
	[SerializeField] private Sprite bodyTopRightSprite; // Turn from Up to Right or Left to Down
	[SerializeField] private Sprite bodyBottomLeftSprite; // Turn from Down to Left or Right to Up
	[SerializeField] private Sprite bodyBottomRightSprite; // Turn from Down to Right or Left to Up
														   // --- Tail Sprites ---
	[SerializeField] private Sprite tailUpSprite;
	[SerializeField] private Sprite tailDownSprite;
	[SerializeField] private Sprite tailLeftSprite;
	[SerializeField] private Sprite tailRightSprite;

	private SpriteRenderer spriteRenderer; // Reference to the head's renderer

	private Vector2Int gridPosition;
	private Vector2Int direction = Vector2Int.right;
	private Vector2Int nextDirection = Vector2Int.right;
	private float moveTimer;
	private float moveInterval => 1f / moveSpeed;

	private List<SnakeSegment> bodySegments = new List<SnakeSegment>();
	private List<Vector2Int> positionHistory = new List<Vector2Int>();

	private bool isAlive = true;
	private bool isEatingFood = false; // Flag to prevent multiple food collisions
	private HashSet<GameObject> processedFoodItems = new HashSet<GameObject>(); // Track food items being processed

	private void Awake() // Use Awake to get components
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
		{
			Debug.LogError("Snake head requires a SpriteRenderer component!");
		}
	}

	private void Start()
	{
		// Reset lists
		positionHistory.Clear();
		bodySegments.Clear(); // Clear in case of scene reload

		// 1. Calculate starting positions
		gridPosition = new Vector2Int(
			GridManager.Instance.width / 2,
			GridManager.Instance.height / 2
		);

		// Set up initial segments behind the head
		Vector2Int firstSegmentPos = gridPosition - direction;
		Vector2Int secondSegmentPos = firstSegmentPos - direction;

		// 2. Create segments and initialize them
		CreateSegmentAt(firstSegmentPos);
		CreateSegmentAt(secondSegmentPos);

		// 3. Set head sprite and position
		UpdateHeadSprite();
		transform.position = GridManager.Instance.GridToWorldPosition(gridPosition);

		// 4. Update segment sprites
		UpdateSegmentSprites();
	}

	// Helper method to create segment at specified position
	private void CreateSegmentAt(Vector2Int position)
	{
		GameObject segmentGO = Instantiate(
			snakeSegmentPrefab,
			GridManager.Instance.GridToWorldPosition(position),
			Quaternion.identity,
			segmentsParent
		);

		SnakeSegment segment = segmentGO.GetComponent<SnakeSegment>();
		if (segment != null)
		{
			segment.Initialize(position);
			bodySegments.Add(segment);
		}
	}

	private void Update()
	{
		if (!isAlive) return;

		// Handle input for direction changes
		HandleInput();

		// Move snake at regular intervals
		moveTimer += Time.deltaTime;
		if (moveTimer >= moveInterval)
		{
			moveTimer = 0f;
			Move();
		}
	}

	private void HandleInput()
	{
		// Get the current keyboard state
		Keyboard keyboard = Keyboard.current;

		if (keyboard == null) return; // Return if no keyboard is connected

		// Get input and prevent 180-degree turns
		if (keyboard.upArrowKey.wasPressedThisFrame && direction != Vector2Int.down)
		{
			nextDirection = Vector2Int.up;
		}
		else if (keyboard.downArrowKey.wasPressedThisFrame && direction != Vector2Int.up)
		{
			nextDirection = Vector2Int.down;
		}
		else if (keyboard.leftArrowKey.wasPressedThisFrame && direction != Vector2Int.right)
		{
			nextDirection = Vector2Int.left;
		}
		else if (keyboard.rightArrowKey.wasPressedThisFrame && direction != Vector2Int.left)
		{
			nextDirection = Vector2Int.right;
		}
	}

	private void Move()
	{
		// Store previous head position BEFORE updating
		Vector2Int previousHeadPosition = gridPosition;

		// Update direction
		direction = nextDirection;

		// Calculate new head position
		Vector2Int newHeadPosition = previousHeadPosition + direction;

		// Check for collisions before moving
		if (!GridManager.Instance.IsValidGridPosition(newHeadPosition) || IsSelfCollision(newHeadPosition))
		{
			GameOver();
			return;
		}

		// Update head sprite before moving
		UpdateHeadSprite();

		// IMPORTANT: Create a new array to hold all current positions BEFORE moving anything
		Vector2Int[] currentPositions = new Vector2Int[bodySegments.Count + 1];
		currentPositions[0] = previousHeadPosition; // Store head's current position
		for (int i = 0; i < bodySegments.Count; i++)
		{
			currentPositions[i + 1] = bodySegments[i].GridPosition;
		}

		// Move head to new position
		gridPosition = newHeadPosition;
		transform.position = GridManager.Instance.GridToWorldPosition(gridPosition);

		// Move each segment to the position of the segment in front of it
		for (int i = 0; i < bodySegments.Count; i++)
		{
			bodySegments[i].UpdatePosition(currentPositions[i]);
		}

		// Update all segment sprites to reflect their new positions and directions
		UpdateSegmentSprites();
	}

	private void UpdateHeadSprite()
	{
		if (spriteRenderer == null) return;

		if (direction == Vector2Int.up)
		{
			spriteRenderer.sprite = headUpSprite;
		}
		else if (direction == Vector2Int.down)
		{
			spriteRenderer.sprite = headDownSprite;
		}
		else if (direction == Vector2Int.left)
		{
			spriteRenderer.sprite = headLeftSprite;
		}
		else if (direction == Vector2Int.right)
		{
			spriteRenderer.sprite = headRightSprite;
		}
		// Optional: Add rotation logic if sprites aren't pre-rotated
		// transform.rotation = Quaternion.Euler(0, 0, GetAngleFromDirection(direction));
	}

	private void UpdateSegmentSprites()
	{
		for (int i = 0; i < bodySegments.Count; i++)
		{
			SnakeSegment currentSegment = bodySegments[i];
			Vector2Int segmentPosition = currentSegment.GridPosition;

			// Determine position of node ahead (towards head)
			Vector2Int positionAhead = (i == 0) ? gridPosition : bodySegments[i - 1].GridPosition;
			Vector2Int directionToAhead = positionAhead - segmentPosition;

			// Check if it's the tail segment
			if (i == bodySegments.Count - 1)
			{
				// --- Tail Logic (Reversed assignments) ---
				if (directionToAhead == Vector2Int.up) currentSegment.UpdateSprite(tailDownSprite); // Tail points Down
				else if (directionToAhead == Vector2Int.down) currentSegment.UpdateSprite(tailUpSprite); // Tail points Up
				else if (directionToAhead == Vector2Int.left) currentSegment.UpdateSprite(tailRightSprite); // Tail points Right
				else if (directionToAhead == Vector2Int.right) currentSegment.UpdateSprite(tailLeftSprite); // Tail points Left
				else { currentSegment.UpdateSprite(tailLeftSprite); } // Default fallback
			}
			else
			{
				// --- Body Segment Logic ---
				Vector2Int positionBehind = (i + 1 < bodySegments.Count) ? bodySegments[i + 1].GridPosition : segmentPosition;
				Vector2Int directionFromBehind = segmentPosition - positionBehind;

				// Check if the directions are valid before processing
				if (IsCardinalDirection(directionFromBehind) && IsCardinalDirection(directionToAhead))
				{
					if (directionFromBehind == directionToAhead)
					{
						// Straight piece
						if (directionToAhead.x != 0) // Horizontal
							currentSegment.UpdateSprite(bodyHorizontalSprite);
						else // Vertical
							currentSegment.UpdateSprite(bodyVerticalSprite);
					}
					else
					{
						// Corner piece - Assigning based on logical turn direction
						if ((directionFromBehind == Vector2Int.right && directionToAhead == Vector2Int.up) || (directionFromBehind == Vector2Int.down && directionToAhead == Vector2Int.left))
							currentSegment.UpdateSprite(bodyTopLeftSprite);
						else if ((directionFromBehind == Vector2Int.left && directionToAhead == Vector2Int.up) || (directionFromBehind == Vector2Int.down && directionToAhead == Vector2Int.right))
							currentSegment.UpdateSprite(bodyTopRightSprite);
						else if ((directionFromBehind == Vector2Int.right && directionToAhead == Vector2Int.down) || (directionFromBehind == Vector2Int.up && directionToAhead == Vector2Int.left))
							currentSegment.UpdateSprite(bodyBottomLeftSprite);
						else if ((directionFromBehind == Vector2Int.left && directionToAhead == Vector2Int.down) || (directionFromBehind == Vector2Int.up && directionToAhead == Vector2Int.right))
							currentSegment.UpdateSprite(bodyBottomRightSprite);
						else
						{
							// Default to horizontal if we have an unhandled case
							currentSegment.UpdateSprite(bodyHorizontalSprite);
						}
					}
				}
				else
				{
					// If we have invalid directions, default to a horizontal sprite
					currentSegment.UpdateSprite(bodyHorizontalSprite);
				}
			}
		}
	}

	// Helper to check if a direction is a valid cardinal direction (up, down, left, right)
	private bool IsCardinalDirection(Vector2Int dir)
	{
		return dir == Vector2Int.up || dir == Vector2Int.down || dir == Vector2Int.left || dir == Vector2Int.right;
	}

	private void UpdatePosition()
	{
		transform.position = GridManager.Instance.GridToWorldPosition(gridPosition);
	}

	private bool IsSelfCollision(Vector2Int position)
	{
		// Check if new position collides with any body segment
		for (int i = 0; i < bodySegments.Count; i++)
		{
			if (bodySegments[i].GridPosition == position)
			{
				return true;
			}
		}
		return false;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Food") && !isEatingFood)
		{
			// Check if food is already being processed or if we're not directly on top of it
			if (processedFoodItems.Contains(other.gameObject))
			{
				return; // Skip if already processing this food item
			}

			// Get grid position of the food
			Vector2 foodWorldPos = other.transform.position;
			Vector2Int foodGridPos = GridManager.Instance.WorldToGridPosition(foodWorldPos);

			// Only collect food if we're on the same grid cell
			if (gridPosition != foodGridPos)
			{
				return; // Skip if not on the same cell
			}

			// Set flag to prevent multiple collisions
			isEatingFood = true;
			processedFoodItems.Add(other.gameObject);

			// Collect food - destroy it first to prevent multiple collisions
			Destroy(other.gameObject);

			// Grow snake
			GrowSnake();

			// Try-catch to handle potential missing GameManager
			try
			{
				// Update score
				GameManager.Instance?.IncreaseScore(1);
			}
			catch (System.Exception)
			{
				Debug.LogWarning("GameManager not found. Score not updated.");
			}

			// Try-catch to handle potential missing FoodSpawner
			try
			{
				// Spawn new food
				FoodSpawner foodSpawner = FindFirstObjectByType<FoodSpawner>();
				if (foodSpawner != null)
				{
					foodSpawner.SpawnFood();
				}
				else
				{
					Debug.LogWarning("FoodSpawner not found. New food not spawned.");
				}
			}
			catch (System.Exception)
			{
				Debug.LogWarning("Error spawning new food.");
			}

			// Reset the eating flag after a short delay
			StartCoroutine(ResetEatingFlag());
		}
	}

	private System.Collections.IEnumerator ResetEatingFlag()
	{
		yield return new WaitForSeconds(0.1f); // Short delay to prevent multiple collisions
		isEatingFood = false;
		processedFoodItems.Clear();
	}

	public void GrowSnake()
	{
		if (bodySegments.Count == 0)
		{
			Debug.LogError("Cannot grow snake with no segments!");
			return;
		}

		// Get the last segment's position
		Vector2Int lastSegmentPos = bodySegments[bodySegments.Count - 1].GridPosition;

		// Calculate the direction from the second-to-last segment to the last segment
		Vector2Int growDirection;

		if (bodySegments.Count > 1)
		{
			// If we have more than one segment, use the direction from second-to-last to last
			Vector2Int secondToLastPos = bodySegments[bodySegments.Count - 2].GridPosition;
			growDirection = lastSegmentPos - secondToLastPos;
		}
		else
		{
			// If we only have one segment, use the opposite of the head's direction
			growDirection = lastSegmentPos - gridPosition;
		}

		// Create the new segment position (behind the last segment)
		Vector2Int newSegmentPos = lastSegmentPos + growDirection;

		// Ensure the new position is valid (within grid bounds)
		if (!GridManager.Instance.IsValidGridPosition(newSegmentPos))
		{
			// If not valid, just put it at the same position as the last segment (will fix visually on next move)
			newSegmentPos = lastSegmentPos;
		}

		// Create and initialize the new segment
		GameObject newSegment = Instantiate(
			snakeSegmentPrefab,
			GridManager.Instance.GridToWorldPosition(newSegmentPos),
			Quaternion.identity,
			segmentsParent);

		SnakeSegment segment = newSegment.GetComponent<SnakeSegment>();
		if (segment != null)
		{
			segment.Initialize(newSegmentPos);
			bodySegments.Add(segment);
		}

		// Update sprites to ensure the new segment has the correct appearance
		UpdateSegmentSprites();
	}

	private void GameOver()
	{
		isAlive = false;
		GameManager.Instance.GameOver();
	}
}
